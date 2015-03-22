using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.Services;
using System.Web.Script.Services;
using System.Web.Script.Serialization;

using epidigitalogy;
using epidigitalogy.Classes;

namespace epidigitalogy {
	[ScriptService]
	public class LogData : System.Web.Services.WebService {
		[WebMethod]
		public Log GetLog(string feed, string groupName, DateTime startDate, DateTime endDate) {
			Log log = new Log();

			string sSQL;
			string sConn;
			SqlConnection conn = new SqlConnection();
			SqlCommand cmd = new SqlCommand();
			SqlDataReader rdr = null;
			DateTime mindt = DateTime.MaxValue;
			DateTime maxdt = DateTime.MinValue;

			switch (feed) {
				case "SymantecSEP":
					sConn = ConfigurationManager.ConnectionStrings["sqlSymantec"].ConnectionString;
					sSQL = @"
						CREATE TABLE #epiSEPResult (
							ComputerID char(32),
							TimeStamp datetime,
							EventType varchar(1)
						)

						-- Firewall
--						INSERT	INTO #epiSEPResult
--						SELECT	COMPUTER_ID,
--								DATEADD(second, [EVENT_TIME] / 1000, '1970-01-01'),
--								'F'
--						FROM	v_agent_packet_log
--						WHERE	DATEADD(second, [EVENT_TIME] / 1000, '1970-01-01') BETWEEN @StartDate AND @EndDate
--						UNION ALL
--						SELECT	COMPUTER_ID,
--								DATEADD(second, [EVENT_TIME] / 1000, '1970-01-01'),
--								'F'
--						FROM	v_agent_traffic_log
--						WHERE	DATEADD(second, [EVENT_TIME] / 1000, '1970-01-01') BETWEEN @StartDate AND @EndDate

						-- Virus definition updates
--						INSERT	INTO #epiSEPResult
--						SELECT	c.COMPUTER_ID,
--								dateadd(second, t.TIME_STAMP/1000, '1970-01-01'),
--								'U'
--						FROM	V_SEM_COMPUTER c
--								INNER JOIN SEM_AGENT a
--									INNER JOIN IDENTITY_MAP g ON a.GROUP_ID = g.ID
--								ON a.COMPUTER_ID = c.COMPUTER_ID
--								INNER JOIN SEM_CONTENT t ON t.AGENT_ID = a.AGENT_ID
--								INNER JOIN PATTERN p ON p.PATTERN_IDX = a.PATTERN_IDX
--						WHERE	p.PATTERN_TYPE='VIRUS_DEFS'
--								AND p.DELETED = '0'
--								AND t.DELETED = '0'
--								AND a.DELETED = '0'
--								AND c.DELETED = '0'
--								AND COALESCE(g.Name, '') = COALESCE(@GroupName, g.Name, '')
--								AND dateadd(second, t.TIME_STAMP/1000, '1970-01-01') BETWEEN @StartDate AND @EndDate
--						GROUP	BY
--								c.COMPUTER_ID,
--								c.COMPUTER_NAME,
--								t.TIME_STAMP,
--								p.PATTERN_TYPE;

						INSERT	INTO #epiSEPResult
						SELECT	c.COMPUTER_ID,
								DATEADD(SECOND, l.EVENT_TIME/1000, '1970-01-01'),
								'U'
						FROM	V_AGENT_SYSTEM_LOG l
								INNER JOIN SEM_COMPUTER c 
									INNER JOIN SEM_AGENT a
										INNER JOIN IDENTITY_MAP g ON a.GROUP_ID = g.ID
									ON c.COMPUTER_ID = a.COMPUTER_ID
								ON c.COMPUTER_ID = l.COMPUTER_ID
						WHERE	EVENT_SOURCE = 'SYLINK'
								AND EVENT_DESC LIKE 'Downloaded%'
								AND COALESCE(g.Name, '') = COALESCE(@GroupName, g.Name, '')
								AND dateadd(second, l.EVENT_TIME/1000, '1970-01-01') BETWEEN @StartDate AND @EndDate;

						-- Last Checkin time
						INSERT	INTO #epiSEPResult
						SELECT	a.COMPUTER_ID,
								dateadd(second, a.LAST_UPDATE_TIME/1000, '1970-01-01'),
								'X'
						FROM	SEM_AGENT a
								INNER JOIN IDENTITY_MAP g ON a.GROUP_ID = g.ID
						WHERE	COALESCE(g.Name, '') = COALESCE(@GroupName, g.Name, '') AND
								dateadd(second, a.LAST_UPDATE_TIME/1000, '1970-01-01') BETWEEN @StartDate AND @EndDate;

						-- Get Virus Alerts
						INSERT	INTO #epiSEPResult
						SELECT	DISTINCT c.COMPUTER_ID,
								a.ALERTDATETIME AS ts,
								'V' AS et
						FROM	V_ALERTS a
								INNER JOIN V_SEM_COMPUTER c
									INNER JOIN SEM_AGENT ag
										INNER JOIN IDENTITY_MAP g ON g.ID = ag.GROUP_ID
									ON ag.COMPUTER_ID = c.COMPUTER_ID
								ON a.COMPUTER_IDX = c.COMPUTER_ID
								LEFT JOIN V_VIRUS v ON a.VIRUSNAME_IDX = v.VIRUSNAME_IDX
						WHERE	v.TYPE IN (1, 2, 3, 4, 5, 6, 8, 9, 10, 14)
								AND COALESCE(g.Name, '') = COALESCE(@GroupName, g.Name, '')
								AND a.ALERTDATETIME BETWEEN @StartDate AND @EndDate;

						-- Downloads
						INSERT	INTO #epiSEPResult
						SELECT	c.COMPUTER_ID,
								dateadd(second, c.[LAST_ACCESS_TIME] / 100000000, '1970-01-01'),
								'D'
						FROM	[COMPUTER_APPLICATION] c
								INNER JOIN identity_map g ON c.[GROUP_ID] = g.[ID]
						WHERE	COALESCE(g.Name, '') = COALESCE(@GroupName, g.Name, '')
								AND dateadd(second, c.[LAST_ACCESS_TIME] / 100000000, '1970-01-01') BETWEEN @StartDate AND @EndDate;

						-- User Control
						INSERT	INTO #epiSEPResult
						SELECT	DISTINCT l.COMPUTER_ID AS id,
								DATEADD(second, l.[BEGIN_TIME] / 1000, '1970-01-01') as ts,
								'C' AS et
						FROM	[v_agent_behavior_log] l
								INNER JOIN identity_map g ON l.[GROUP_ID] = g.[ID]
						WHERE	caller_process_name != 'SysPlant' AND
								COALESCE(g.Name, '') = COALESCE(@GroupName, g.Name, '') AND
								DATEADD(second, l.[BEGIN_TIME] / 1000, '1970-01-01') BETWEEN @StartDate AND @EndDate;

						-- Firewall
						INSERT	INTO #epiSEPResult
						SELECT	l.COMPUTER_ID AS id,
								DATEADD(second, [EVENT_TIME] / 1000, '1970-01-01') AS ed,
								'F' AS et
						FROM	v_agent_packet_log l
								INNER JOIN IDENTITY_MAP g ON l.GROUP_ID = g.ID
								INNER JOIN V_SEM_COMPUTER c ON l.COMPUTER_ID = c.COMPUTER_ID
						WHERE	COALESCE(g.Name, '') = COALESCE(@GroupName, g.Name, '')
								AND DATEADD(second, l.[EVENT_TIME] / 1000, '1970-01-01') BETWEEN @StartDate AND @EndDate;

						-- Receive the data
						SELECT	t.ComputerID AS id,
								g.Name AS grp,
								c.COMPUTER_NAME AS name,
								t.TimeStamp AS ts,
								t.EventType AS et,
								t2.lci AS lci	-- Last Check In
						FROM	#epiSEPResult t
								INNER JOIN dbo.V_SEM_COMPUTER c ON c.COMPUTER_ID = t.ComputerID
								INNER JOIN
									(SEM_AGENT a INNER JOIN IDENTITY_MAP g ON g.ID = a.GROUP_ID)
								ON a.COMPUTER_ID = t.ComputerID
								LEFT JOIN(
									SELECT	ComputerID, MAX(TimeStamp) AS lci
									FROM	#epiSEPResult
									WHERE	EventType = 'U'
									GROUP	BY ComputerID
								) t2 ON t.ComputerID = t2.ComputerID
						WHERE	COALESCE(g.Name, '') = COALESCE(@GroupName, g.Name, '')
								AND t.TimeStamp BETWEEN @StartDate AND @EndDate
						ORDER	BY g.Name ASC, c.COMPUTER_NAME ASC, t.TimeStamp ASC;
						";
					break;
				default:
					return log;	// Don't do anything.  Need a feed.
			}

			try {
				conn.ConnectionString = sConn;
				conn.Open();
				cmd.CommandText = sSQL;
				cmd.Connection = conn;

				if (String.IsNullOrEmpty(groupName)) {
					cmd.Parameters.Add(new SqlParameter("@GroupName", SqlDbType.VarChar, 100)).Value = DBNull.Value;
				} else {
					cmd.Parameters.Add(new SqlParameter("@GroupName", SqlDbType.VarChar, 100)).Value = groupName;
				}
				cmd.Parameters.Add(new SqlParameter("@StartDate", SqlDbType.DateTime)).Value = startDate;
				cmd.Parameters.Add(new SqlParameter("@EndDate", SqlDbType.DateTime)).Value = endDate;
				rdr = cmd.ExecuteReader();

				string prevAgentGuid = null;
				Host h = new Host();

				while (rdr.Read()) {
					if ((string)rdr.GetValue(0) != prevAgentGuid) {
						h = new Host();
						h.id = (string)rdr["id"];
						h.hn = (string)rdr["name"];
						h.gn = (string)rdr["grp"];
						h.lci = (DateTime)rdr["ts"];

						log.hosts.Add(h);

						prevAgentGuid = (string)rdr.GetValue(0);
					}

					h.events.Add(new Event((string)rdr["id"], (DateTime)rdr["ts"], (string)rdr["et"]));
					
					if ((DateTime)rdr["ts"] < mindt) {
						mindt = (DateTime)rdr["ts"];
					}

					if ((DateTime)rdr["ts"] > maxdt) {
						maxdt = (DateTime)rdr["ts"];
					}
				}
			} catch (Exception e) {
				throw e;
			} finally {
				rdr.Close();
				cmd.Dispose();
				conn.Close();
				conn.Dispose();
			}

			log.mindt = mindt;
			log.maxdt = maxdt;

			return log;
		}

		[WebMethod]
		public Log GetDetails(string system, string id, DateTime startDate, DateTime endDate) {
			Log log = new Log();
			Feed feed = null;
			SqlConnection conn = null;
			SqlCommand cmd = null;
			SqlDataReader rdr = null;
			string sConn;
			string sql;
			DataSet result = new DataSet();
			DateTime mindt = DateTime.MaxValue;
			DateTime maxdt = DateTime.MinValue;

			switch (system) {
				case "Symantec":
					sql = @"
						SELECT	a.AGENT_ID,
								a.COMPUTER_ID,
								a.AGENT_VERSION,
								dateadd(second,[LAST_UPDATE_TIME]/1000, '1970-01-01') AS LAST_UPDATE_TIME,
								a.FULL_NAME,
								a.EMAIL,
								a.JOB_TITLE,
								a.DEPARTMENT,
								a.OFFICE_PHONE,
								a.MOBILE_PHONE,
								CASE WHEN a.INFECTED = 1 THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS INFECTED,
								dateadd(second,[LAST_SCAN_TIME]/1000, '1970-01-01') AS LAST_SCAN_TIME,
								CASE WHEN LAST_VIRUS_TIME = 0 THEN NULL ELSE dateadd(second,[LAST_VIRUS_TIME]/1000, '1970-01-01') END AS LAST_VIRUS_TIME,
								dateadd(second,[LAST_DOWNLOAD_TIME]/1000, '1970-01-01') AS LAST_DOWNLOAD_TIME,
								a.LAST_DOWNLOAD_TIME,
								CASE WHEN a.AVENGINE_ONOFF = 1 THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS AVENGINE_ONOFF,
								CASE WHEN a.TAMPER_ONOFF = 1 THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS TAMPER_ONOFF,
								CASE WHEN a.FIREWALL_ONOFF = 1 THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS FIREWALL_ONOFF,
								CASE WHEN a.REBOOT_REQUIRED = 1 THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS REBOOT_REQUIRED,
								a.REBOOT_REASON,
								c.COMPUTER_NAME,
								c.COMPUTER_DOMAIN_NAME,
								c.OPERATION_SYSTEM AS operating_system,
								c.SERVICE_PACK,
								c.CURRENT_LOGIN_USER,
								c.CURRENT_LOGIN_DOMAIN,
								c.DNS_SERVER1,
								c.DNS_SERVER2,
								c.DHCP_SERVER,
								c.MAC_ADDR1,
								c.IP_ADDR1,
								c.GATEWAY1,
								c.SUBNET_MASK1
--								c.MAC_ADDR2,
--								c.IP_ADDR2,
--								c.GATEWAY2,
--								c.SUBNET_MASK2,
--								c.MAC_ADDR3,
--								c.IP_ADDR3,
--								c.GATEWAY3,
--								c.SUBNET_MASK3,
--								c.MAC_ADDR4,
--								c.IP_ADDR4,
--								c.GATEWAY4,
--								c.SUBNET_MASK4
						FROM	SEM_AGENT a
								INNER JOIN V_SEM_COMPUTER c ON a.[COMPUTER_ID] = c.[COMPUTER_ID]
						WHERE	c.COMPUTER_ID = @Id";

					sConn = ConfigurationManager.ConnectionStrings["sqlSymantec"].ConnectionString;
					break;
				default:
					return null;	// Return empty data
			}

			conn = new SqlConnection(sConn);
			conn.Open();

			Host host = new Host();

			/////////////////////////////////////////////////////////////////////////////
			// Host Information
			/////////////////////////////////////////////////////////////////////////////
			try {
				cmd = new SqlCommand(sql, conn);
				cmd.CommandType = System.Data.CommandType.Text;

				cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.VarChar, 32)).Value = id;
				rdr = cmd.ExecuteReader();

				if (rdr.Read()) {
					host.agent_id = (string)rdr["agent_id"];
					host.computer_id = (string)rdr["computer_id"];
					host.agent_version = (string)rdr["agent_version"];
					host.last_update_time = (DateTime)rdr["last_update_time"];
					host.full_name = rdr["full_name"] == DBNull.Value ? "n/a" : (string)rdr["full_name"];
					host.email = rdr["email"] == DBNull.Value ? "n/a" : (string)rdr["email"];
					host.job_title = rdr["job_title"] == DBNull.Value ? "n/a" : (string)rdr["job_title"];
					host.department = rdr["department"] == DBNull.Value ? "n/a" : (string)rdr["department"];
					host.office_phone = rdr["office_phone"] == DBNull.Value ? "n/a" : (string)rdr["office_phone"];
					host.mobile_phone = rdr["mobile_phone"] == DBNull.Value ? "n/a" : (string)rdr["mobile_phone"];
					if (rdr["infected"] != DBNull.Value) {
						host.infected = (bool)rdr["infected"];
					}
					if (rdr["last_scan_time"] != DBNull.Value) {
						host.last_scan_time = (DateTime)rdr["last_scan_time"];
					}
					if (rdr["last_virus_time"] != DBNull.Value) {
						host.last_virus_time = (DateTime)rdr["last_virus_time"];
					}
					if (rdr["last_download_time"] != DBNull.Value) {
						host.last_download_time = (DateTime)rdr["last_download_time"];
					}
					if (rdr["avengine_onoff"] != DBNull.Value) {
						host.avengine_onoff = (bool)rdr["avengine_onoff"];
					}
					if (rdr["tamper_onoff"] != DBNull.Value) {
						host.tamper_onoff = (bool)rdr["tamper_onoff"];
					}
					if (rdr["firewall_onoff"] != DBNull.Value) {
						host.firewall_onoff = (bool)rdr["firewall_onoff"];
					}
					if (rdr["reboot_required"] != DBNull.Value) {
						host.reboot_required = (bool)rdr["reboot_required"];
					}
					host.reboot_reason = rdr["reboot_reason"] == DBNull.Value ? null : (string)rdr["reboot_reason"];
					host.computer_name = rdr["computer_name"] == DBNull.Value ? null : (string)rdr["computer_name"];
					host.computer_domain_name = rdr["computer_domain_name"] == DBNull.Value ? null : (string)rdr["computer_domain_name"];
					host.os = rdr["operating_system"] == DBNull.Value ? null : (string)rdr["operating_system"];
					host.service_pack = rdr["service_pack"] == DBNull.Value ? null : (string)rdr["service_pack"];
					host.current_login_user = rdr["current_login_user"] == DBNull.Value ? null : (string)rdr["current_login_user"];
					host.current_login_domain = rdr["current_login_domain"] == DBNull.Value ? null : (string)rdr["current_login_domain"];
					if (rdr["dns_server1"] != DBNull.Value) {
						if (Type.GetTypeCode(rdr.GetFieldType(rdr.GetOrdinal("dns_server1"))) == TypeCode.String) {
							host.dns_server1 = Util.IP2Long((string)rdr["dns_server1"]);
						} else {
							host.dns_server1 = (long)rdr["dns_server1"];
						}
					}
					if (rdr["dns_server2"] != DBNull.Value) {
						if (Type.GetTypeCode(rdr.GetFieldType(rdr.GetOrdinal("dns_server2"))) == TypeCode.String) {
							host.dns_server2 = Util.IP2Long((string)rdr["dns_server2"]);
						} else {
							host.dns_server2 = (long)rdr["dns_server2"];
						}
					}
					if (rdr["dhcp_server"] != DBNull.Value) {
						if (Type.GetTypeCode(rdr.GetFieldType(rdr.GetOrdinal("dhcp_server"))) == TypeCode.String) {
							host.dhcp_server = Util.IP2Long((string)rdr["dhcp_server"]);
						} else {
							host.dhcp_server = (long)rdr["dhcp_server"];
						}
					}
					host.mac_addr1 = rdr["mac_addr1"] == DBNull.Value ? null : (string)rdr["mac_addr1"];

					if (rdr["ip_addr1"] != DBNull.Value) {
						if (Type.GetTypeCode(rdr.GetFieldType(rdr.GetOrdinal("ip_addr1"))) == TypeCode.String) {
							host.ip_addr1 = Util.IP2Long((string)rdr["ip_addr1"]);
						} else {
							host.ip_addr1 = (long)rdr["ip_addr1"];
						}
					}

					if (rdr["gateway1"] != DBNull.Value) {
						if (Type.GetTypeCode(rdr.GetFieldType(rdr.GetOrdinal("gateway1"))) == TypeCode.String) {
							host.gateway1 = Util.IP2Long((string)rdr["gateway1"]);
						} else {
							host.gateway1 = (long)rdr["gateway1"];
						}
					}

					if (rdr["subnet_mask1"] != DBNull.Value) {
						if (Type.GetTypeCode(rdr.GetFieldType(rdr.GetOrdinal("subnet_mask1"))) == TypeCode.String) {
							host.subnet_mask1 = Util.IP2Long((string)rdr["subnet_mask1"]);
						} else {
							host.subnet_mask1 = (long)rdr["subnet_mask1"];
						}
					}

					if ((DateTime)rdr["ed"] < mindt) {
						mindt = (DateTime)rdr["ed"];
					}

					if ((DateTime)rdr["ed"] > maxdt) {
						maxdt = (DateTime)rdr["ed"];
					}
//					host.mac_addr2 = rdr["mac_addr2"] == DBNull.Value ? null : (string)rdr["mac_addr2"];
//					host.ip_addr2 = (long)rdr["ip_addr2"];
//					host.gateway2 = (long)rdr["gateway2"];
//					host.subnet_mask2 = (long)rdr["subnet_mask2"];
//					host.mac_addr3 = rdr["mac_addr3"] == DBNull.Value ? null : (string)rdr["mac_addr3"];
//					host.ip_addr3 = (long)rdr["ip_addr3"];
//					host.gateway3 = (long)rdr["gateway3"];
//					host.subnet_mask3 = (long)rdr["subnet_mask3"];
//					host.mac_addr4 = rdr["mac_addr4"] == DBNull.Value ? null : (string)rdr["mac_addr4"];
//					host.ip_addr4 = (long)rdr["ip_addr4"];
//					host.gateway4 = (long)rdr["gateway4"];
//					host.subnet_mask4 = (long)rdr["subnet_mask4"];
				}
			} catch (Exception e) {
				host.error = e.Message;
			} finally {
				log.hosts.Add(host);

				rdr.Close();
				rdr.Dispose();
				cmd.Dispose();
			}

			switch (system) {
				case "Symantec":
					/////////////////////////////////////////////////////////////////////////////
					// Firewall Feed
					/////////////////////////////////////////////////////////////////////////////

					feed = new Feed();
					feed.name = "Firewall";

					try {
						if (system == "Symantec") {
							sql = @"
							SELECT	DATEADD(second, [EVENT_TIME] / 1000, '1970-01-01') AS ed,
									NULL AS protocol,
									(SELECT CASE [EVENT_ID]
										WHEN 301 THEN 'TCP Initiated'
										WHEN 302 THEN 'UDP datagram'
										WHEN 303 THEN 'Ping request'
										WHEN 304 THEN 'TCP Completed'
										WHEN 305 THEN 'Traffic (other)'
										WHEN 306 THEN 'ICMP packet'
										WHEN 307 THEN 'Ethernet packet'
										WHEN 308 THEN 'IP Packet' END) AS traffic_type,
									LOCAL_HOST_IP AS local_ip,
									REMOTE_HOST_IP AS remote_ip,
									REMOTE_HOST_NAME AS remote_hostname,
									LOCAL_PORT AS local_port,
									REMOTE_PORT AS remote_port,
									(SELECT CASE TRAFFIC_DIRECTION
										WHEN 0 THEN 'Unknown'
										WHEN 1 THEN 'Inbound'
										WHEN 2 THEN 'Outbound'
									END) AS direction,
									NULL AS severity,
									NULL AS severity_string,
									BLOCKED AS blocked,
									APP_NAME AS app_name,
									ALERT AS Alert,
									RULE_NAME AS rule_name,
									NULL AS location,
									null AS repetition
							FROM	v_agent_packet_log l
							WHERE	l.COMPUTER_ID = @Id
									AND l.BLOCKED = 1
									AND DATEADD(second, l.[EVENT_TIME] / 1000, '1970-01-01') BETWEEN @StartDate AND @EndDate

							UNION

							SELECT	DATEADD(second, [EVENT_TIME] / 1000, '1970-01-01') AS ed,
									(SELECT CASE [NETWORK_PROTOCOL]
										WHEN 1 THEN 'Others'
										WHEN 2 THEN 'TCP'
										WHEN 3 THEN 'UDP' END) AS protocol,
									(SELECT CASE [EVENT_ID]
										WHEN 301 THEN 'TCP Initiated'
										WHEN 302 THEN 'UDP datagram'
										WHEN 303 THEN 'Ping request'
										WHEN 304 THEN 'TCP Completed'
										WHEN 305 THEN 'Traffic (other)'
										WHEN 306 THEN 'ICMP packet'
										WHEN 307 THEN 'Ethernet packet'
										WHEN 308 THEN 'IP Packet' END) AS traffic_type,
									LOCAL_HOST_IP AS local_ip,
									REMOTE_HOST_IP AS remote_ip,
									[REMOTE_HOST_NAME] AS remote_hostname,
									[LOCAL_PORT] AS local_port,
									[REMOTE_PORT] AS remote_port,
									(SELECT CASE TRAFFIC_DIRECTION
										WHEN 0 THEN 'Unknown'
										WHEN 1 THEN 'Inbound'
										WHEN 2 THEN 'Outbound'
									END) AS direction,
									[SEVERITY],	-- 0 to 15, lower is worse
									(SELECT CASE [SEVERITY] 
										WHEN 0 THEN 'Critical' 
										WHEN 1 THEN 'Critical' 
										WHEN 2 THEN 'Critical' 
										WHEN 3 THEN 'Critical' 
										WHEN 4 THEN 'Major' 
										WHEN 5 THEN 'Major' 
										WHEN 6 THEN 'Major' 
										WHEN 7 THEN 'Major' 
										WHEN 8 THEN 'Minor' 
										WHEN 9 THEN 'Minor' 
										WHEN 10 THEN 'Minor' 
										WHEN 11 THEN 'Minor' 
										WHEN 12 THEN 'Info' 
										WHEN 13 THEN 'Info' 
										WHEN 14 THEN 'Info'
										WHEN 15 THEN 'Info'
										END) AS Severity_String,
									[BLOCKED],
									[APP_NAME],
									[ALERT],
									[RULE_NAME],
									[LOCATION_NAME],
									[REPETITION]
							FROM	v_agent_traffic_log l
							WHERE	l.COMPUTER_ID = @Id
									AND l.BLOCKED = 1
									AND DATEADD(second, l.[EVENT_TIME] / 1000, '1970-01-01') BETWEEN @StartDate AND @EndDate
						";
						}

						cmd = new SqlCommand(sql, conn);
						cmd.CommandType = System.Data.CommandType.Text;

						cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.VarChar, 32)).Value = id;
						cmd.Parameters.Add(new SqlParameter("@StartDate", SqlDbType.DateTime)).Value = startDate;
						cmd.Parameters.Add(new SqlParameter("@EndDate", SqlDbType.DateTime)).Value = endDate;
						rdr = cmd.ExecuteReader();

						while (rdr.Read()) {
							Event e = new Event();
							e.feed = feed.name;
							e.ed = (DateTime)rdr["ed"];
							if (rdr["local_ip"] != DBNull.Value) {
								if (Type.GetTypeCode(rdr.GetFieldType(rdr.GetOrdinal("local_ip"))) == TypeCode.String) {
									e.local_ip = Util.IP2Long((string)rdr["local_ip"]);
								} else {
									e.local_ip = (long)rdr["local_ip"];
								}
							}
							if (rdr["remote_ip"] != DBNull.Value) {
								if (Type.GetTypeCode(rdr.GetFieldType(rdr.GetOrdinal("remote_ip"))) == TypeCode.String) {
									e.remote_ip = Util.IP2Long((string)rdr["remote_ip"]);
								} else {
									e.remote_ip = (long)rdr["remote_ip"];
								}
							}
							e.protocol = rdr["protocol"] == DBNull.Value ? null : (string)rdr["protocol"];
							e.traffic_type = rdr["traffic_type"] == DBNull.Value ? null : (string)rdr["traffic_type"];
							e.remote_hostname = rdr["remote_hostname"] == DBNull.Value ? null : (string)rdr["remote_hostname"];
							e.local_port = (int)rdr["local_port"];
							e.remote_port = (int)rdr["remote_port"];
							e.direction = rdr["direction"] == DBNull.Value ? null : (string)rdr["direction"];
							e.repetition = (int)rdr["repetition"];
							if (rdr["blocked"] != DBNull.Value && (byte)rdr["blocked"] == 1) {
								e.action_taken = "Blocked";
							}
							e.app_name = rdr["app_name"] == DBNull.Value ? null : (string)rdr["app_name"];
							if (rdr["alert"] != DBNull.Value && (int)rdr["alert"] == 1) {
								e.alert = true;
							}
							e.rule_name = rdr["rule_name"] == DBNull.Value ? null : (string)rdr["rule_name"];
							e.location = rdr["location"] == DBNull.Value ? null : (string)rdr["location"];

							feed.events.Add(e);

							if ((DateTime)rdr["ed"] < mindt) {
								mindt = (DateTime)rdr["ed"];
							}

							if ((DateTime)rdr["ed"] > maxdt) {
								maxdt = (DateTime)rdr["ed"];
							}
						}
					} catch (Exception e) {
						feed.error = e.Message;
					} finally {
						log.feeds.Add(feed);

						rdr.Close();
						rdr.Dispose();
						cmd.Dispose();
					}

					/////////////////////////////////////////////////////////////////////////////
					// IPS Feed
					/////////////////////////////////////////////////////////////////////////////
					feed = new Feed();
					feed.name = "IPS";

					try {
						if (system == "Symantec") {
							sql = @"
							SELECT	DATEADD(second, EVENT_TIME / 1000, '1970-01-01') AS ed,
									(SELECT CASE [EVENT_ID]
										WHEN 301 THEN 'TCP Initiated'
										WHEN 302 THEN 'UDP datagram'
										WHEN 303 THEN 'Ping request'
										WHEN 304 THEN 'TCP Completed'
										WHEN 305 THEN 'Traffic (other)'
										WHEN 306 THEN 'ICMP packet'
										WHEN 307 THEN 'Ethernet packet'
										WHEN 308 THEN 'IP Packet' END) AS traffic_type,
									SEVERITY,
									LOCAL_HOST_IP AS local_ip,
									REMOTE_HOST_IP AS remote_ip,
									REMOTE_HOST_NAME AS remote_hostname,
									LOCAL_PORT,
									REMOTE_PORT,
									(SELECT CASE TRAFFIC_DIRECTION
										WHEN 0 THEN 'Unknown'
										WHEN 1 THEN 'Inbound'
										WHEN 2 THEN 'Outbound'
									END) AS direction,
									REPETITION,
									(SELECT CASE NETWORK_PROTOCOL
										WHEN 1 THEN 'Others'
										WHEN 2 THEN 'TCP'
										WHEN 3 THEN 'UDP' END) AS protocol,
									HACK_TYPE,
									BEGIN_TIME,
									EVENT_DESC AS description,
									APP_NAME,
									ALERT,
									LOCATION_NAME AS location,
									INTRUSION_URL AS intrusion_url,
									INTRUSION_PAYLOAD_URL AS intrusion_payload_url
							FROM	V_AGENT_SECURITY_LOG
							WHERE	COMPUTER_ID = @Id
									AND DATEADD(second, EVENT_TIME / 1000, '1970-01-01') BETWEEN @StartDate AND @EndDate
						";
						}

						cmd = new SqlCommand(sql, conn);
						cmd.CommandType = System.Data.CommandType.Text;

						cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.VarChar, 32)).Value = id;
						cmd.Parameters.Add(new SqlParameter("@StartDate", SqlDbType.DateTime)).Value = startDate;
						cmd.Parameters.Add(new SqlParameter("@EndDate", SqlDbType.DateTime)).Value = endDate;
						rdr = cmd.ExecuteReader();

						while (rdr.Read()) {
							Event e = new Event();
							e.feed = feed.name;
							e.id = id;
							e.ed = (DateTime)rdr["ed"];
							e.traffic_type = rdr["traffic_type"] == DBNull.Value ? null : (string)rdr["traffic_type"];
							e.severity = (int)rdr["severity"];
							if (rdr["local_ip"] != DBNull.Value) {
								if (Type.GetTypeCode(rdr.GetFieldType(rdr.GetOrdinal("local_ip"))) == TypeCode.String) {
									e.local_ip = Util.IP2Long((string)rdr["local_ip"]);
								} else {
									e.local_ip = (long)rdr["local_ip"];
								}
							}
							if (rdr["remote_ip"] != DBNull.Value) {
								if (Type.GetTypeCode(rdr.GetFieldType(rdr.GetOrdinal("remote_ip"))) == TypeCode.String) {
									e.remote_ip = Util.IP2Long((string)rdr["remote_ip"]);
								} else {
									e.remote_ip = (long)rdr["remote_ip"];
								}
							}
							e.remote_hostname = rdr["remote_hostname"] == DBNull.Value ? null : (string)rdr["remote_hostname"];
							e.local_port = (int)rdr["local_port"];
							e.remote_port = (int)rdr["remote_port"];
							e.direction = rdr["direction"] == DBNull.Value ? null : (string)rdr["direction"];
							e.repetition = (int)rdr["repetition"];
							e.protocol = rdr["protocol"] == DBNull.Value ? null : (string)rdr["protocol"];
							e.app_name = rdr["app_name"] == DBNull.Value ? null : (string)rdr["app_name"];

							if (rdr["alert"] != DBNull.Value && (byte)rdr["alert"] == 1) {
								e.alert = true;
							}
							e.location = rdr["location"] == DBNull.Value ? null : (string)rdr["location"];
							e.intrusion_url = rdr["intrusion_url"] == DBNull.Value ? null : (string)rdr["intrusion_url"];
							e.intrusion_payload_url = rdr["intrusion_payload_url"] == DBNull.Value ? null : (string)rdr["intrusion_payload_url"];
							e.description = rdr["description"] == DBNull.Value ? null : (string)rdr["description"];

							feed.events.Add(e);

							if ((DateTime)rdr["ed"] < mindt) {
								mindt = (DateTime)rdr["ed"];
							}

							if ((DateTime)rdr["ed"] > maxdt) {
								maxdt = (DateTime)rdr["ed"];
							}
						}
					} catch (Exception e) {
						feed.error = e.Message;
					} finally {
						log.feeds.Add(feed);

						rdr.Close();
						rdr.Dispose();
						cmd.Dispose();
					}

					/////////////////////////////////////////////////////////////////////////////
					// Downloads Feed
					/////////////////////////////////////////////////////////////////////////////
					feed = new Feed();
					feed.name = "Downloads";

					try {
						if (system == "Symantec") {
							sql = @"
							SELECT	dateadd(second, c.TIME_STAMP / 1000, '1970-01-01') AS ed,
									a.[APPLICATION_NAME] AS app_name,
									a.[COMPANY_NAME] AS app_company,
									a.[VERSION] AS app_version,
									a.[FILE_SIZE] AS filesize,
									a.[APP_DESCRIPTION] AS description,
									c.[APP_HASH] AS app_md5,
									c.[CREATOR_SHA2] AS app_sha2,
									c.[DOWNLOAD_URL] AS url,
                                    a.[SIGNER_NAME] AS signer
							FROM	COMPUTER_APPLICATION c
									LEFT JOIN SEM_APPLICATION a on a.APP_HASH = c.APP_HASH
							WHERE	c.COMPUTER_ID = @Id
									AND dateadd(second, c.TIME_STAMP / 1000, '1970-01-01') BETWEEN @StartDate AND @EndDate
						";
						}

						cmd = new SqlCommand(sql, conn);
						cmd.CommandType = System.Data.CommandType.Text;

						cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.VarChar, 32)).Value = id;
						cmd.Parameters.Add(new SqlParameter("@StartDate", SqlDbType.DateTime)).Value = startDate;
						cmd.Parameters.Add(new SqlParameter("@EndDate", SqlDbType.DateTime)).Value = endDate;
						rdr = cmd.ExecuteReader();

						while (rdr.Read()) {
							Event e = new Event();
							e.feed = feed.name;
							e.id = id;
							e.ed = (DateTime)rdr["ed"];
							e.app_name = rdr["app_name"] == DBNull.Value ? null : (string)rdr["app_name"];
							e.app_company = rdr["app_company"] == DBNull.Value ? null : (string)rdr["app_company"];
							e.app_version = rdr["app_version"] == DBNull.Value ? null : (string)rdr["app_version"];
							e.filesize = (long)rdr["filesize"];
							e.description = rdr["description"] == DBNull.Value ? null : (string)rdr["description"];
							e.app_md5 = rdr["app_md5"] == DBNull.Value ? null : (string)rdr["app_md5"];
							e.app_sha2 = rdr["app_sha2"] == DBNull.Value ? null : (string)rdr["app_sha2"];
							e.url = rdr["url"] == DBNull.Value ? null : (string)rdr["url"];
							e.signer = rdr["signer"] == DBNull.Value ? null : (string)rdr["signer"];

                            

							feed.events.Add(e);

							if ((DateTime)rdr["ed"] < mindt) {
								mindt = (DateTime)rdr["ed"];
							}

							if ((DateTime)rdr["ed"] > maxdt) {
								maxdt = (DateTime)rdr["ed"];
							}
						}
					} catch (Exception e) {
						feed.error = e.Message;
					} finally {
						log.feeds.Add(feed);

						rdr.Close();
						rdr.Dispose();
						cmd.Dispose();
					}
						
					/////////////////////////////////////////////////////////////////////////////
					// AV Engine Feed
					/////////////////////////////////////////////////////////////////////////////
					feed = new Feed();
					feed.name = "AV Engine";

					try {
						if (system == "Symantec") {
							sql = @"
									SELECT
										a.[ALERTDATETIME] AS ed, 
										a.[SOURCE] AS source, 
										a.filepath,
										a.[DESCRIPTION] AS description, 
										(replace([ACTUALACTION].[ACTUALACTION],' ', '_')) AS action_taken,
										app.APP_NAME AS app_name, 
										app.[COMPANY_NAME] AS app_company, 
										app.[APP_VERSION] AS app_version, 
										app.[FILE_SIZE] AS filesize, 
										app.APP_TYPE AS app_type,
										v.[VIRUSNAME] AS virus_name,
										a.[USER_NAME], 
										COALESCE(alerts.[SENSITIVITY], 0) AS sensitivity,
										COALESCE(alerts.[DETECTION_SCORE], 0) AS detection_score,
										COALESCE(alerts.[COH_ENGINE_VERSION], '') AS 'truscan_engine_version',
										COALESCE(alerts.[DIS_SUBMIT], 0) AS submission_recommendation,
										COALESCE(alerts.[WHITELIST_REASON], 0) AS whitelist_reason,
										COALESCE(alerts.[DISPOSITION], 0) AS disposition,
										COALESCE(alerts.[Confidence], 0) AS confidence,
										COALESCE(alerts.[PREVALENCE], 0) AS prevalence,
										COALESCE(alerts.[URL], '') AS url,
										COALESCE(alerts.[WEB_DOMAIN], '') AS web_domain,
										COALESCE(alerts.[DOWNLOADER], '') AS downloader,
										COALESCE(alerts.[CIDS_ONOFF], 0) AS cids_onoff,
										COALESCE(alerts.[RISK_LEVEL], 0) AS risk_level
								FROM	V_ALERTS a
										LEFT JOIN HPP_APPLICATION app ON a.[HPP_APP_IDX] = app.[APP_IDX]
										LEFT JOIN IDENTITY_MAP g ON a.[CLIENTGROUP_IDX] = g.[ID]
										LEFT JOIN [V_SEM_COMPUTER] ON a.[COMPUTER_IDX]= [V_SEM_COMPUTER].[COMPUTER_ID]
										LEFT JOIN V_VIRUS v ON v.[VIRUSNAME_IDX] = a.VIRUSNAME_IDX
										LEFT JOIN [ACTUALACTION] on a.[ACTUALACTION_IDX] = [ACTUALACTION].[ACTUALACTION_IDX]
										LEFT JOIN [HPP_ALERTS] alerts on a.[IDX] = alerts.[IDX]
								WHERE	[V_SEM_COMPUTER].COMPUTER_ID = @Id
										AND app_type != -1	 -- Seems to return junk data.
										AND a.[ALERTDATETIME] BETWEEN @StartDate AND @EndDate
								ORDER	BY g.[NAME],
										a.[ALERTDATETIME],
										v.[VIRUSNAME]";

							cmd = new SqlCommand(sql, conn);
							cmd.CommandType = System.Data.CommandType.Text;

							cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.VarChar, 32)).Value = id;
							cmd.Parameters.Add(new SqlParameter("@StartDate", SqlDbType.DateTime)).Value = startDate;
							cmd.Parameters.Add(new SqlParameter("@EndDate", SqlDbType.DateTime)).Value = endDate;
							rdr = cmd.ExecuteReader();

							while (rdr.Read()) {
								Event e = new Event();
								e.feed = feed.name;
								e.id = id;
								e.ed = (DateTime)rdr["ed"];
								e.filepath = rdr["filepath"] == DBNull.Value ? null : (string)rdr["filepath"];
								e.action_taken = rdr["action_taken"] == DBNull.Value ? null : (string)rdr["action_taken"];
								e.source = rdr["source"] == DBNull.Value ? null : (string)rdr["source"];
								e.app_name = rdr["app_name"] == DBNull.Value ? null : (string)rdr["app_name"];
								e.app_company = rdr["app_company"] == DBNull.Value ? null : (string)rdr["app_company"];
								e.app_version = rdr["app_version"] == DBNull.Value ? null : (string)rdr["app_version"];
								e.filesize = (long)rdr["filesize"];
								e.app_type = (int)rdr["app_type"];
								e.virus_name = rdr["virus_name"] == DBNull.Value ? null : (string)rdr["virus_name"];
								e.sensitivity = (int)rdr["sensitivity"];
								e.detection_score = (int)rdr["detection_score"];
								e.truscan_engine_version = rdr["truscan_engine_version"] == DBNull.Value ? null : (string)rdr["truscan_engine_version"];
								e.submission_recommendation = (int)rdr["submission_recommendation"];
								e.whitelist_reason = (int)rdr["whitelist_reason"];
								e.disposition = (int)rdr["disposition"];	// 127 if there's no data to compare to
								e.confidence = (int)rdr["confidence"];
								e.prevalence = (int)rdr["prevalence"];
								e.url = rdr["url"] == DBNull.Value ? null : (string)rdr["url"].ToString();
								e.web_domain = rdr["web_domain"] == DBNull.Value ? null : (string)rdr["web_domain"];
								e.downloader = rdr["downloader"] == DBNull.Value ? null : (string)rdr["downloader"];
								e.cids_onoff = (int)rdr["cids_onoff"];
								e.risk_level = (int)rdr["risk_level"];

								feed.events.Add(e);

								if ((DateTime)rdr["ed"] < mindt) {
									mindt = (DateTime)rdr["ed"];
								}

								if ((DateTime)rdr["ed"] > maxdt) {
									maxdt = (DateTime)rdr["ed"];
								}
							}
						}
					} catch (Exception e) {
						feed.error = e.Message;
					} finally {
						log.feeds.Add(feed);

						rdr.Close();
						rdr.Dispose();
						cmd.Dispose();
					}

					/////////////////////////////////////////////////////////////////////////////
					// Virus Definition Updates
					/////////////////////////////////////////////////////////////////////////////
					feed = new Feed();
					feed.name = "Virus Updates";

					try {
						sql = @"
								SELECT	DATEADD(SECOND, l.EVENT_TIME/1000, '1970-01-01') AS ed,
										l.EVENT_DESC AS description
								FROM	V_AGENT_SYSTEM_LOG l
										INNER JOIN SEM_COMPUTER c ON c.COMPUTER_ID = l.COMPUTER_ID
								WHERE	EVENT_SOURCE = 'SYLINK'
										AND EVENT_DESC LIKE 'Downloaded%'
										AND c.COMPUTER_ID = @Id
										AND dateadd(second, l.EVENT_TIME/1000, '1970-01-01') BETWEEN @StartDate AND @EndDate";

/*						sql = @"
								SELECT	DISTINCT
										dateadd(second, t.TIME_STAMP/1000, '1970-01-01') AS ed,
										p.version AS db_version
								FROM	V_SEM_COMPUTER c
										INNER JOIN SEM_AGENT a
											INNER JOIN IDENTITY_MAP g ON a.GROUP_ID = g.ID
										ON a.COMPUTER_ID = c.COMPUTER_ID
										INNER JOIN SEM_CONTENT t ON t.AGENT_ID = a.AGENT_ID
										INNER JOIN PATTERN p ON p.PATTERN_IDX = a.PATTERN_IDX
								WHERE	p.PATTERN_TYPE='VIRUS_DEFS'
										AND c.COMPUTER_ID = @Id
										AND dateadd(second, t.TIME_STAMP/1000, '1970-01-01') BETWEEN @StartDate AND @EndDate";
*/
						cmd = new SqlCommand(sql, conn);
						cmd.CommandType = System.Data.CommandType.Text;

						cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.VarChar, 32)).Value = id;
						cmd.Parameters.Add(new SqlParameter("@StartDate", SqlDbType.DateTime)).Value = startDate;
						cmd.Parameters.Add(new SqlParameter("@EndDate", SqlDbType.DateTime)).Value = endDate;
						rdr = cmd.ExecuteReader();

						while (rdr.Read()) {
							Event e = new Event();
							e.feed = feed.name;
							e.ed = (DateTime)rdr["ed"];
							e.description = rdr["description"] == DBNull.Value ? null : (string)rdr["description"];
//							e.db_version = rdr["db_version"] == DBNull.Value ? null : (string)rdr["db_version"];
							e.et = "Virus Definition Update";

							feed.events.Add(e);

							if ((DateTime)rdr["ed"] < mindt) {
								mindt = (DateTime)rdr["ed"];
							}

							if ((DateTime)rdr["ed"] > maxdt) {
								maxdt = (DateTime)rdr["ed"];
							}
						}
					} catch (Exception e) {
						feed.error = e.Message;
					} finally {
						log.feeds.Add(feed);

						rdr.Close();
						rdr.Dispose();
						cmd.Dispose();
					}

					/////////////////////////////////////////////////////////////////////////////
					// User Control Feed
					/////////////////////////////////////////////////////////////////////////////
					feed = new Feed();
					feed.name = "User Control";

					try {
						if (system == "Symantec") {
							sql = @"
								SELECT	dateadd(second,[EVENT_TIME]/1000, '1970-01-01') AS ed,
										dateadd(second,[BEGIN_TIME]/1000, '1970-01-01') AS begin_time,
										dateadd(second,[END_TIME]/1000, '1970-01-01') AS end_time,
										(SELECT
											CASE [EVENT_ID]
												WHEN 501 THEN 'Application Control Driver'
												WHEN 502 THEN 'Application Control Rules'
												WHEN 999 THEN 'Tamper Protection'
											END) AS event_type,
										severity,
										(SELECT
											CASE [ACTION]
												WHEN 0 THEN 'allow'
												WHEN 1 THEN 'block' 
												WHEN 2 THEN 'ask'
												WHEN 3 THEN 'continue'
												WHEN 4 THEN 'terminate'
											END) AS action,
										description,
										rule_name,
										caller_process_name,
										parameter,
										alert,
										user_name,
										domain_name
								FROM	v_agent_behavior_log l
											INNER JOIN identity_map m ON l.[GROUP_ID] = m.[ID]
								WHERE	caller_process_name != 'SysPlant' AND
										COMPUTER_ID = @Id AND
										dateadd(second,[BEGIN_TIME]/1000, '1970-01-01')  BETWEEN @StartDate AND @EndDate
								ORDER	BY l.EVENT_TIME";
						}

						cmd = new SqlCommand(sql, conn);
						cmd.CommandType = System.Data.CommandType.Text;

						cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.VarChar, 32)).Value = id;
						cmd.Parameters.Add(new SqlParameter("@StartDate", SqlDbType.DateTime)).Value = startDate;
						cmd.Parameters.Add(new SqlParameter("@EndDate", SqlDbType.DateTime)).Value = endDate;
						rdr = cmd.ExecuteReader();

						while (rdr.Read()) {
							Event e = new Event();
							e.feed = feed.name;
							e.ed = (DateTime)rdr["begin_time"];
							e.et = rdr["event_type"] == DBNull.Value ? null : (string)rdr["event_type"];
							e.severity = (int)rdr["severity"];
							e.action_taken = rdr["action"] == DBNull.Value ? null : (string)rdr["action"];
							e.description = rdr["description"] == DBNull.Value ? null : (string)rdr["description"];
							e.rule_name = rdr["rule_name"] == DBNull.Value ? null : (string)rdr["rule_name"];
							e.caller_process = rdr["caller_process_name"] == DBNull.Value ? null : (string)rdr["caller_process_name"];
							e.parameter = rdr["parameter"] == DBNull.Value ? null : (string)rdr["parameter"];
							if (rdr["alert"] != DBNull.Value && (int)rdr["alert"] == 1) {
								e.alert = true;
							}
							e.user_name = rdr["user_name"] == DBNull.Value ? null : (string)rdr["user_name"];
							e.domain_name = rdr["domain_name"] == DBNull.Value ? null : (string)rdr["domain_name"];

							feed.events.Add(e);

							if ((DateTime)rdr["ed"] < mindt) {
								mindt = (DateTime)rdr["ed"];
							}

							if ((DateTime)rdr["ed"] > maxdt) {
								maxdt = (DateTime)rdr["ed"];
							}
						}
					} catch (Exception e) {
						feed.error = e.Message;
					} finally {
						log.feeds.Add(feed);

						rdr.Close();
						rdr.Dispose();
						cmd.Dispose();
					}
					break;
			}

			conn.Close();
			conn.Dispose();

			log.mindt = mindt;
			log.maxdt = maxdt;

			return log;
		}
	
	}
}
