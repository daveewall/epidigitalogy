using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using epidigitalogy.Classes;
using epidigitalogy.Classes.Events;
using Newtonsoft.Json;

namespace epidigitalogy.Classes.Systems
{
	public class Symantec : ISystem
	{
		private string sqlConnectionString = ConfigurationManager.ConnectionStrings["Symantec"].ConnectionString;
		private DateTime mindt = DateTime.MaxValue;
		private DateTime maxdt = DateTime.MinValue;

		public Host getHostInfo(string id) {
			SqlConnection conn = null;
			SqlCommand cmd = null;
			SqlDataReader rdr = null;

			string sql = @"
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
--						c.MAC_ADDR2,
--						c.IP_ADDR2,
--						c.GATEWAY2,
--						c.SUBNET_MASK2,
--						c.MAC_ADDR3,
--						c.IP_ADDR3,
--						c.GATEWAY3,
--						c.SUBNET_MASK3,
--						c.MAC_ADDR4,
--						c.IP_ADDR4,
--						c.GATEWAY4,
--						c.SUBNET_MASK4
				FROM	SEM_AGENT a
						INNER JOIN V_SEM_COMPUTER c ON a.[COMPUTER_ID] = c.[COMPUTER_ID]
				WHERE	c.COMPUTER_ID = @Id";

			conn = new SqlConnection(sqlConnectionString);
			conn.Open();

			Host host = new Host();

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
				rdr.Close();
				rdr.Dispose();
				cmd.Dispose();
			}
			return host;
		}

		public string getSQLMain(string feeds) {
			Hashtable feed = new Hashtable();
			foreach (string f in feeds.Split(',')) {
				feed[f] = true;
			}

			string sSQL = @"
				CREATE TABLE #epiSEPResult (
					ComputerID char(32),
					TimeStamp datetime,
					EventType varchar(1)
				)";


			if (feed.ContainsKey("fw")) {
				sSQL += getSQLMainFirewall();
			}
			if (feed.ContainsKey("ips")) {
				sSQL += getSQLMainIPS();
			}
			if (feed.ContainsKey("dl")) {
				sSQL += getSQLMainDownloads();
			}
			if (feed.ContainsKey("av")) {
				sSQL += getSQLMainAntiVirus();
			}
			if (feed.ContainsKey("uc")) {
				sSQL += getSQLMainUserControl();
			}
			if (feed.ContainsKey("up")) {
				sSQL += getSQLMainUpdates();
			}

			sSQL += @"
				-- Receive the data
				SELECT	c.COMPUTER_ID AS id,
						g.Name AS grp,
						c.COMPUTER_NAME AS name,
						t.TimeStamp AS ts,
						t.EventType AS et,
						t2.lci AS lci	-- Last Check In
				FROM	dbo.V_SEM_COMPUTER c
						LEFT JOIN #epiSEPResult t ON c.COMPUTER_ID = t.ComputerID AND t.TimeStamp BETWEEN @StartDate AND @EndDate
						LEFT JOIN
							(SEM_AGENT a LEFT JOIN IDENTITY_MAP g ON g.ID = a.GROUP_ID)
						ON a.COMPUTER_ID = c.COMPUTER_ID
						LEFT JOIN(
							SELECT	ComputerID, MAX(TimeStamp) AS lci
							FROM	#epiSEPResult
							WHERE	EventType = 'U'
							GROUP	BY ComputerID
						) t2 ON t.ComputerID = t2.ComputerID
				WHERE	COALESCE(g.Name, '') = COALESCE(@GroupName, g.Name, '')
						AND c.DELETED = 0
				ORDER	BY g.Name ASC, c.COMPUTER_NAME ASC, t.TimeStamp ASC;
				";

			return sSQL;
		}

		public string getSQLMainFirewall() {
			return @"
				-- Firewall
				INSERT	INTO #epiSEPResult
				SELECT	l.COMPUTER_ID,
						DATEADD(second, l.[EVENT_TIME] / 1000, '1970-01-01'),
						'F'
				FROM	v_agent_packet_log l
						INNER JOIN IDENTITY_MAP g ON l.GROUP_ID = g.ID
				WHERE	l.BLOCKED = 1
						AND DATEADD(second, l.[EVENT_TIME] / 1000, '1970-01-01') BETWEEN @StartDate AND @EndDate

				UNION

				SELECT	l.COMPUTER_ID,
						DATEADD(second, l.[EVENT_TIME] / 1000, '1970-01-01'),
						'F'
				FROM	v_agent_traffic_log l
						INNER JOIN IDENTITY_MAP g ON l.GROUP_ID = g.ID
				WHERE	l.BLOCKED = 1
						AND DATEADD(second, l.[EVENT_TIME] / 1000, '1970-01-01') BETWEEN @StartDate AND @EndDate
			";
		}

		public string getSQLMainIPS() {
			return @"
				INSERT	INTO #epiSEPResult
				SELECT	s.COMPUTER_ID,
						DATEADD(second, s.EVENT_TIME / 1000, '1970-01-01') AS ed,
						'I'
				FROM	V_AGENT_SECURITY_LOG s
						INNER JOIN identity_map g ON s.[GROUP_ID] = g.[ID]	
				WHERE	COALESCE(g.Name, '') = COALESCE(@GroupName, g.Name, '')
						AND DATEADD(second, s.EVENT_TIME / 1000, '1970-01-01') BETWEEN @StartDate AND @EndDate
			";
		}

		public string getSQLMainDownloads() {
			return @"
				INSERT	INTO #epiSEPResult
				SELECT	c.COMPUTER_ID,
						dateadd(second, c.TIME_STAMP / 1000, '1970-01-01'),
						'D'
				FROM	[COMPUTER_APPLICATION] c
						INNER JOIN identity_map g ON c.[GROUP_ID] = g.[ID]	
				WHERE	COALESCE(g.Name, '') = COALESCE(@GroupName, g.Name, '')
						AND dateadd(second, c.TIME_STAMP / 1000, '1970-01-01') BETWEEN @StartDate AND @EndDate;
			";
		}

		public string getSQLMainAntiVirus() {
			return @"
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
				WHERE	--v.TYPE IN (1, 2, 3, 4, 5, 6, 8, 9, 10, 14)  -- Causes issues in new SEP versions, where SONAR detects issues behaviorally
						COALESCE(g.Name, '') = COALESCE(@GroupName, g.Name, '')
						AND a.ALERTDATETIME BETWEEN @StartDate AND @EndDate;
			";
		}

		public string getSQLMainUserControl() {
			return @"
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
			";
		}

		public string getSQLMainUpdates() {
			return @"						
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
			";
		}

		public Log getDetailedLog(string id, DateTime startDate, DateTime endDate) {
			Log log = new Log();

			log.feeds.Add(getHostFirewallDetail(id, startDate, endDate));
			log.feeds.Add(getHostIPSDetail(id, startDate, endDate));
			log.feeds.Add(getHostDownloadDetail(id, startDate, endDate));
			log.feeds.Add(getHostAntiVirusDetail(id, startDate, endDate));
			log.feeds.Add(getHostUserControlDetail(id, startDate, endDate));
			log.feeds.Add(getUpdateDetail(id, startDate, endDate));

			log.mindt = mindt;
			log.maxdt = maxdt;

			return log;
		}

		public Feed getHostFirewallDetail(string id, DateTime startDate, DateTime endDate) {
			SqlConnection conn = new SqlConnection();
			SqlCommand cmd = new SqlCommand();
			SqlDataReader rdr = null;

			Feed feed = new Feed();
			feed.name = "Firewall";

			string sql = @"
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
						AND DATEADD(second, l.[EVENT_TIME] / 1000, '1970-01-01') BETWEEN @StartDate AND @EndDate";

			try {
				conn.ConnectionString = this.sqlConnectionString;
				conn.Open();

				cmd = new SqlCommand(sql, conn);
				cmd.CommandType = System.Data.CommandType.Text;

				cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.VarChar, 32)).Value = id;
				cmd.Parameters.Add(new SqlParameter("@StartDate", SqlDbType.DateTime)).Value = startDate;
				cmd.Parameters.Add(new SqlParameter("@EndDate", SqlDbType.DateTime)).Value = endDate;
				rdr = cmd.ExecuteReader();

				while (rdr.Read()) {
					FirewallEvent e = new FirewallEvent();
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
				rdr.Close();
				rdr.Dispose();
				cmd.Dispose();
				conn.Close();
				conn.Dispose();
			}

			return feed;
		}

		public Feed getHostIPSDetail(string id, DateTime startDate, DateTime endDate) {
			SqlConnection conn = new SqlConnection();
			SqlCommand cmd = new SqlCommand();
			SqlDataReader rdr = null;


			Feed feed = new Feed();
			feed.name = "IPS";

			string sql = @"
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

			try {
				conn.ConnectionString = this.sqlConnectionString;
				conn.Open();

				cmd = new SqlCommand(sql, conn);
				cmd.CommandType = System.Data.CommandType.Text;

				cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.VarChar, 32)).Value = id;
				cmd.Parameters.Add(new SqlParameter("@StartDate", SqlDbType.DateTime)).Value = startDate;
				cmd.Parameters.Add(new SqlParameter("@EndDate", SqlDbType.DateTime)).Value = endDate;
				rdr = cmd.ExecuteReader();

				while (rdr.Read()) {
					IPSEvent e = new IPSEvent();
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
				rdr.Close();
				rdr.Dispose();
				cmd.Dispose();
				conn.Close();
				conn.Dispose();
			}

			return feed;
		}

		public Feed getHostDownloadDetail(string id, DateTime startDate, DateTime endDate) {
			SqlConnection conn = new SqlConnection();
			SqlCommand cmd = new SqlCommand();
			SqlDataReader rdr = null;


			Feed feed = new Feed();
			feed.name = "Downloads";

			string sql = @"
				SELECT	dateadd(second, c.TIME_STAMP / 1000, '1970-01-01') AS ed,
						a.[APPLICATION_NAME] AS app_name,
						a.[COMPANY_NAME] AS app_company,
						a.[VERSION] AS app_version,
						a.[FILE_SIZE] AS filesize,
						a.[APP_DESCRIPTION] AS description,
						c.[APP_HASH] AS app_md5,
						c.[CREATOR_SHA2] AS app_sha2,
						c.[DOWNLOAD_URL] AS url,
                        a.[SIGNER_NAME] AS signer,
						a.[LAST_MODIFY_TIME] AS last_modify_time
				FROM	COMPUTER_APPLICATION c
						LEFT JOIN SEM_APPLICATION a on a.APP_HASH = c.APP_HASH
				WHERE	c.COMPUTER_ID = @Id
						AND dateadd(second, c.TIME_STAMP / 1000, '1970-01-01') BETWEEN @StartDate AND @EndDate
			";
			try {
				conn.ConnectionString = this.sqlConnectionString;
				conn.Open();

				cmd = new SqlCommand(sql, conn);
				cmd.CommandType = System.Data.CommandType.Text;

				cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.VarChar, 32)).Value = id;
				cmd.Parameters.Add(new SqlParameter("@StartDate", SqlDbType.DateTime)).Value = startDate;
				cmd.Parameters.Add(new SqlParameter("@EndDate", SqlDbType.DateTime)).Value = endDate;
				rdr = cmd.ExecuteReader();

				while (rdr.Read()) {
					DownloadEvent e = new DownloadEvent();
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
					e.last_modify_time = (long)rdr["last_modify_time"];

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
				rdr.Close();
				rdr.Dispose();
				cmd.Dispose();
				conn.Close();
				conn.Dispose();
			}

			return feed;
		}

		public Feed getHostAntiVirusDetail(string id, DateTime startDate, DateTime endDate) {
			SqlConnection conn = new SqlConnection();
			SqlCommand cmd = new SqlCommand();
			SqlDataReader rdr = null;

			Feed feed = new Feed();
			feed.name = "AV Engine";

			string sql = @"
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
--						AND app_type != -1	 -- Seems to return junk data.
						AND a.[ALERTDATETIME] BETWEEN @StartDate AND @EndDate
				ORDER	BY g.[NAME],
						a.[ALERTDATETIME],
						v.[VIRUSNAME]";

			try {
				conn.ConnectionString = this.sqlConnectionString;
				conn.Open();

				cmd = new SqlCommand(sql, conn);
				cmd.CommandType = System.Data.CommandType.Text;

				cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.VarChar, 32)).Value = id;
				cmd.Parameters.Add(new SqlParameter("@StartDate", SqlDbType.DateTime)).Value = startDate;
				cmd.Parameters.Add(new SqlParameter("@EndDate", SqlDbType.DateTime)).Value = endDate;
				rdr = cmd.ExecuteReader();

				while (rdr.Read()) {
					VirusEvent e = new VirusEvent();
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
			} catch (Exception e) {
				feed.error = e.Message;
			} finally {
				rdr.Close();
				rdr.Dispose();
				cmd.Dispose();
				conn.Close();
				conn.Dispose();
			}

			return feed;
		}

		public Feed getHostUserControlDetail(string id, DateTime startDate, DateTime endDate) {
			SqlConnection conn = new SqlConnection();
			SqlCommand cmd = new SqlCommand();
			SqlDataReader rdr = null;

			Feed feed = new Feed();
			feed.name = "User Control";

			string sql = @"
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

			try {
				conn.ConnectionString = this.sqlConnectionString;
				conn.Open();

				cmd = new SqlCommand(sql, conn);
				cmd.CommandType = System.Data.CommandType.Text;

				cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.VarChar, 32)).Value = id;
				cmd.Parameters.Add(new SqlParameter("@StartDate", SqlDbType.DateTime)).Value = startDate;
				cmd.Parameters.Add(new SqlParameter("@EndDate", SqlDbType.DateTime)).Value = endDate;
				rdr = cmd.ExecuteReader();

				while (rdr.Read()) {
					UserControlEvent e = new UserControlEvent();
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
				rdr.Close();
				rdr.Dispose();
				cmd.Dispose();
				conn.Close();
				conn.Dispose();
			}

			return feed;
		}

		public Feed getUpdateDetail(string id, DateTime startDate, DateTime endDate) {
			SqlConnection conn = new SqlConnection();
			SqlCommand cmd = new SqlCommand();
			SqlDataReader rdr = null;

			Feed feed = new Feed();
			feed.name = "Virus Updates";

			string sql = @"
				SELECT	DATEADD(SECOND, l.EVENT_TIME/1000, '1970-01-01') AS ed,
						l.EVENT_DESC AS description
				FROM	V_AGENT_SYSTEM_LOG l
						INNER JOIN SEM_COMPUTER c ON c.COMPUTER_ID = l.COMPUTER_ID
				WHERE	EVENT_SOURCE = 'SYLINK'
						AND EVENT_DESC LIKE 'Downloaded%'
						AND c.COMPUTER_ID = @Id
						AND dateadd(second, l.EVENT_TIME/1000, '1970-01-01') BETWEEN @StartDate AND @EndDate";

			try {
				conn.ConnectionString = this.sqlConnectionString;
				conn.Open();

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
				rdr.Close();
				rdr.Dispose();
				cmd.Dispose();
				conn.Close();
				conn.Dispose();
			}

			return feed;
		}

		public List<Dictionary<string, string>> getFlareAntiVirusDetail(string groupName, DateTime startDate, DateTime endDate) {
			SqlConnection conn = new SqlConnection();
			SqlCommand cmd = new SqlCommand();
			SqlDataReader rdr = null;

			List<Dictionary<string, string>> events = new List<Dictionary<string, string>>();

			string sql = @"
				SELECT	ag.AGENT_ID AS agent_id,
						c.COMPUTER_ID AS id,
						g.NAME as gn,
						ag.AGENT_VERSION AS agent_version,
						dateadd(second,[LAST_UPDATE_TIME]/1000, '1970-01-01') AS last_update_time,
						ag.FULL_NAME AS full_name,
						ag.EMAIL AS email,
						ag.JOB_TITLE AS job_title, 
						ag.DEPARTMENT AS department,
						ag.OFFICE_PHONE AS office_phone,
						ag.MOBILE_PHONE AS mobile_phone,
						CASE WHEN ag.INFECTED = 1 THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS infected,
						dateadd(second,[LAST_SCAN_TIME]/1000, '1970-01-01') AS last_scan_time,
						CASE WHEN LAST_VIRUS_TIME = 0 THEN NULL ELSE dateadd(second,[LAST_VIRUS_TIME]/1000, '1970-01-01') END AS last_virus_time,
						dateadd(second,[LAST_DOWNLOAD_TIME]/1000, '1970-01-01') AS last_download_time,
						CASE WHEN ag.AVENGINE_ONOFF = 1 THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS avengine_onoff,
						CASE WHEN ag.TAMPER_ONOFF = 1 THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS tamper_onoff,
						CASE WHEN ag.FIREWALL_ONOFF = 1 THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS firewall_onoff,
						CASE WHEN ag.REBOOT_REQUIRED = 1 THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS reboot_required,
						ag.REBOOT_REASON AS reboot_reason,
						c.COMPUTER_NAME AS computer_name,
						c.COMPUTER_DOMAIN_NAME AS computer_domain_name,
						c.OPERATION_SYSTEM AS operating_system,
						c.SERVICE_PACK AS service_pack,
						c.CURRENT_LOGIN_USER AS current_login_user,
						c.CURRENT_LOGIN_DOMAIN AS current_login_domain,
						c.DNS_SERVER1 AS dns_server1,
						c.DNS_SERVER2 AS dns_server2,
						c.DHCP_SERVER AS dhcp_server,
						c.MAC_ADDR1 AS mac_addr1,
						c.IP_ADDR1 AS ip_addr1,
						c.GATEWAY1 AS gateway1,
						c.SUBNET_MASK1 AS subnet_mask1,

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
						LEFT JOIN
							([V_SEM_COMPUTER] c INNER JOIN SEM_AGENT ag ON ag.[COMPUTER_ID] = c.[COMPUTER_ID])
						ON a.[COMPUTER_IDX]= c.[COMPUTER_ID]
						LEFT JOIN V_VIRUS v ON v.[VIRUSNAME_IDX] = a.VIRUSNAME_IDX
						LEFT JOIN [ACTUALACTION] on a.[ACTUALACTION_IDX] = [ACTUALACTION].[ACTUALACTION_IDX]
						LEFT JOIN [HPP_ALERTS] alerts on a.[IDX] = alerts.[IDX]
				WHERE	COALESCE(g.Name, '') = COALESCE(@GroupName, g.Name, '')
						AND a.[ALERTDATETIME] BETWEEN @StartDate AND @EndDate
				ORDER	BY g.[NAME],
						c.COMPUTER_NAME,
						a.[ALERTDATETIME],
						v.[VIRUSNAME]";

			try {
				conn.ConnectionString = this.sqlConnectionString;
				conn.Open();

				cmd = new SqlCommand(sql, conn);
				cmd.CommandType = System.Data.CommandType.Text;

				if (String.IsNullOrEmpty(groupName)) {
					cmd.Parameters.Add(new SqlParameter("@GroupName", SqlDbType.VarChar, 100)).Value = DBNull.Value;
				} else {
					cmd.Parameters.Add(new SqlParameter("@GroupName", SqlDbType.VarChar, 100)).Value = groupName;
				}
				cmd.Parameters.Add(new SqlParameter("@StartDate", SqlDbType.DateTime)).Value = startDate;
				cmd.Parameters.Add(new SqlParameter("@EndDate", SqlDbType.DateTime)).Value = endDate;
				rdr = cmd.ExecuteReader();

				while (rdr.Read()) {
					Dictionary<string, string> e = new Dictionary<string, string>();
					e["id"] = rdr["id"] == DBNull.Value ? null : (string)rdr["id"];
					e["ed"] = ((DateTime)rdr["ed"]).ToString();

					e["gn"] = rdr["gn"] == DBNull.Value ? null : (string)rdr["gn"];
					e["agent_id"] = rdr["agent_id"] == DBNull.Value ? null : (string)rdr["agent_id"];
					e["agent_version"] = rdr["agent_version"] == DBNull.Value ? null : (string)rdr["agent_version"];
					e["last_update_time"] = ((DateTime)rdr["last_update_time"]).ToString();
					e["full_name"] = rdr["full_name"] == DBNull.Value ? null : (string)rdr["full_name"];
					e["email"] = rdr["email"] == DBNull.Value ? null : (string)rdr["email"];
					e["job_title"] = rdr["job_title"] == DBNull.Value ? null : (string)rdr["job_title"];
					e["department"] = rdr["department"] == DBNull.Value ? null : (string)rdr["department"];
					e["office_phone"] = rdr["office_phone"] == DBNull.Value ? null : (string)rdr["office_phone"];
					e["mobile_phone"] = rdr["mobile_phone"] == DBNull.Value ? null : (string)rdr["mobile_phone"];
					e["infected"] = ((Boolean)rdr["infected"]).ToString();
					e["last_scan_time"] = ((DateTime)rdr["last_scan_time"]).ToString();
					e["last_virus_time"] = rdr["last_virus_time"] == DBNull.Value ? null : ((DateTime)rdr["last_virus_time"]).ToString();
					e["last_download_time"] = ((DateTime)rdr["last_download_time"]).ToString();

					e["avengine_onoff"] = ((Boolean)rdr["avengine_onoff"]).ToString();
					e["tamper_onoff"] = ((Boolean)rdr["tamper_onoff"]).ToString();
					e["firewall_onoff"] = ((Boolean)rdr["firewall_onoff"]).ToString();
					e["reboot_required"] = ((Boolean)rdr["reboot_required"]).ToString();
					e["reboot_reason"] = rdr["reboot_reason"] == DBNull.Value ? null : (string)rdr["reboot_reason"];
					e["computer_name"] = rdr["computer_name"] == DBNull.Value ? null : (string)rdr["computer_name"];
					e["computer_domain_name"] = rdr["computer_domain_name"] == DBNull.Value ? null : (string)rdr["computer_domain_name"];
					e["operating_system"] = rdr["operating_system"] == DBNull.Value ? null : (string)rdr["operating_system"];
					e["service_pack"] = rdr["service_pack"] == DBNull.Value ? null : (string)rdr["service_pack"];
					e["current_login_user"] = rdr["current_login_user"] == DBNull.Value ? null : (string)rdr["current_login_user"];
					e["current_login_domain"] = rdr["current_login_domain"] == DBNull.Value ? null : (string)rdr["current_login_domain"];
					e["dns_server1"] = rdr["dns_server1"] == DBNull.Value ? null : Util.LongToIP((long)rdr["dns_server1"]);
					e["dns_server2"] = rdr["dns_server2"] == DBNull.Value ? null : Util.LongToIP((long)rdr["dns_server2"]);
					e["dhcp_server"] = rdr["dhcp_server"] == DBNull.Value ? null : Util.LongToIP((long)rdr["dhcp_server"]);
					e["mac_addr1"] = rdr["mac_addr1"] == DBNull.Value ? null : (string)rdr["mac_addr1"];
					e["ip_addr1"] = rdr["ip_addr1"] == DBNull.Value ? null : Util.LongToIP((long)rdr["ip_addr1"]);
					e["gateway1"] = rdr["gateway1"] == DBNull.Value ? null : Util.LongToIP((long)rdr["gateway1"]);
					e["subnet_mask1"] = rdr["subnet_mask1"] == DBNull.Value ? null : Util.LongToIP((long)rdr["subnet_mask1"]);

					e["filepath"] = rdr["filepath"] == DBNull.Value ? null : (string)rdr["filepath"];
					e["action_taken"] = rdr["action_taken"] == DBNull.Value ? null : (string)rdr["action_taken"];
					e["source"] = rdr["source"] == DBNull.Value ? null : (string)rdr["source"];
					e["app_name"] = rdr["app_name"] == DBNull.Value ? null : (string)rdr["app_name"];
					e["app_company"] = rdr["app_company"] == DBNull.Value ? null : (string)rdr["app_company"];
					e["app_version"] = rdr["app_version"] == DBNull.Value ? null : (string)rdr["app_version"];
					e["filesize"] = ((long)rdr["filesize"]).ToString();
					e["app_type"] = ((int)rdr["app_type"]).ToString();
					e["virus_name"] = rdr["virus_name"] == DBNull.Value ? null : (string)rdr["virus_name"];
					e["sensitivity"] = ((int)rdr["sensitivity"]).ToString();
					e["detection_score"] = ((int)rdr["detection_score"]).ToString();
					e["truscan_engine_version"] = rdr["truscan_engine_version"] == DBNull.Value ? null : (string)rdr["truscan_engine_version"];
					e["submission_recommendation"] = ((int)rdr["submission_recommendation"]).ToString();
					e["whitelist_reason"] = ((int)rdr["whitelist_reason"]).ToString();
					e["disposition"] = ((int)rdr["disposition"]).ToString();	// 127 if there's no data to compare to
					e["confidence"] = ((int)rdr["confidence"]).ToString();
					e["prevalence"] = ((int)rdr["prevalence"]).ToString();
					e["url"] = rdr["url"] == DBNull.Value ? null : (string)rdr["url"].ToString();
					e["web_domain"] = rdr["web_domain"] == DBNull.Value ? null : (string)rdr["web_domain"];
					e["downloader"] = rdr["downloader"] == DBNull.Value ? null : (string)rdr["downloader"];
					e["cids_onoff"] = ((int)rdr["cids_onoff"]).ToString();
					e["risk_level"] = ((int)rdr["risk_level"]).ToString();

					events.Add(e);
				}
			} catch {
				events = new List<Dictionary<string, string>>(); ;
			} finally {
				rdr.Close();
				rdr.Dispose();
				cmd.Dispose();
				conn.Close();
				conn.Dispose();
			}

			return events;
		}

		public Dictionary<string, string> getFlareAntiVirusFieldNames() {
			Dictionary<string, string> e = new Dictionary<string, string>();

			e["Group Name"] = "gn";
			e["Agent ID"] = "agent_id";
			e["Agent Version"] = "agent_version";
			e["Last Update Time"] = "last_update_time";
			e["Full Name"] = "full_name";
			e["Email"] = "email";
			e["Job Title"] = "job_title";
			e["Department"] = "department";
			e["Office Phone"] = "office_phone";
			e["Mobile Phone"] = "mobile_phone";
			e["Infected"] = "infected";
			e["Last Scan Time"] = "last_scan_time";
			e["Last Virus Time"] = "last_virus_time";
			e["Last Download Time"] = "last_download_time";

			e["AV Engine on/off"] = "avengine_onoff";
			e["Tamper on/off"] = "tamper_onoff";
			e["Firewall on/off"] = "firewall_onoff";
			e["Reboot Required"] = "reboot_required";
			e["Reboot Reason"] = "reboot_reason";
			e["Computer Name"] = "computer_name";
			e["Computer Domain Name"] = "computer_domain_name";
			e["Operating System"] = "operating_system";
			e["Service Pack"] = "service_pack";
			e["Current Login User"] = "current_login_user";
			e["Current Login Domain"] = "current_login_domain";
			e["DNS Server 1"] = "dns_server1";
			e["DNS Server 2"] = "dns_server2";
			e["DHCP Server"] = "dhcp_server";
			e["MAC Address 1"] = "mac_addr1";
			e["IP Address 1"] = "ip_addr1";
			e["Gateway 1"] = "gateway1";
			e["Subnet Mask 1"] = "subnet_mask1";

			e["File Path"] = "filepath";
			e["Action Taken"] = "action_taken";
			e["Source"] = "source";
			e["App Name"] = "app_name";
			e["App Company"] = "app_company";
			e["App Version"] = "app_version";
			e["File Size"] = "filesize";
			e["App Type"] = "app_type";
			e["Virus Name"] = "virus_name";
			e["Sensitivity"] = "sensitivity";
			e["Detection Score"] = "detection_score";
			e["Truscan Engine Version"] = "truscan_engine_version";
			e["Submission Recommendation"] = "submission_recommendation";
			e["Whitelist Reason"] = "whitelist_reason";
			e["Disposition"] = "disposition";
			e["Confidence"] = "confidence";
			e["Prevalence"] = "prevalence";
			e["URL"] = "url";
			e["Web Domain"] = "web_domain";
			e["Downloader"] = "downloader";
			e["CIDS on/off"] = "cids_onoff";
			e["Risk Level"] = "risk_level";

			return e;
		}

		public List<Dictionary<string, string>> getFlareIPSDetail(string groupName, DateTime startDate, DateTime endDate) {
			SqlConnection conn = new SqlConnection();
			SqlCommand cmd = new SqlCommand();
			SqlDataReader rdr = null;

			List<Dictionary<string, string>> events = new List<Dictionary<string, string>>();

			string sql = @"
				SELECT	ag.AGENT_ID AS agent_id,
						c.COMPUTER_ID AS id,
						g.NAME as gn,
						ag.AGENT_VERSION AS agent_version,
						dateadd(second,[LAST_UPDATE_TIME]/1000, '1970-01-01') AS last_update_time,
						ag.FULL_NAME AS full_name,
						ag.EMAIL AS email,
						ag.JOB_TITLE AS job_title, 
						ag.DEPARTMENT AS department,
						ag.OFFICE_PHONE AS office_phone,
						ag.MOBILE_PHONE AS mobile_phone,
						CASE WHEN ag.INFECTED = 1 THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS infected,
						dateadd(second,[LAST_SCAN_TIME]/1000, '1970-01-01') AS last_scan_time,
						CASE WHEN LAST_VIRUS_TIME = 0 THEN NULL ELSE dateadd(second,[LAST_VIRUS_TIME]/1000, '1970-01-01') END AS last_virus_time,
						dateadd(second,[LAST_DOWNLOAD_TIME]/1000, '1970-01-01') AS last_download_time,
						CASE WHEN ag.AVENGINE_ONOFF = 1 THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS avengine_onoff,
						CASE WHEN ag.TAMPER_ONOFF = 1 THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS tamper_onoff,
						CASE WHEN ag.FIREWALL_ONOFF = 1 THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS firewall_onoff,
						CASE WHEN ag.REBOOT_REQUIRED = 1 THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS reboot_required,
						ag.REBOOT_REASON AS reboot_reason,
						c.COMPUTER_NAME AS computer_name,
						c.COMPUTER_DOMAIN_NAME AS computer_domain_name,
						c.OPERATION_SYSTEM AS operating_system,
						c.SERVICE_PACK AS service_pack,
						c.CURRENT_LOGIN_USER AS current_login_user,
						c.CURRENT_LOGIN_DOMAIN AS current_login_domain,
						c.DNS_SERVER1 AS dns_server1,
						c.DNS_SERVER2 AS dns_server2,
						c.DHCP_SERVER AS dhcp_server,
						c.MAC_ADDR1 AS mac_addr1,
						c.IP_ADDR1 AS ip_addr1,
						c.GATEWAY1 AS gateway1,
						c.SUBNET_MASK1 AS subnet_mask1,

						DATEADD(second, l.EVENT_TIME / 1000, '1970-01-01') AS ed,
						(SELECT CASE l.[EVENT_ID]
							WHEN 301 THEN 'TCP Initiated'
							WHEN 302 THEN 'UDP datagram'
							WHEN 303 THEN 'Ping request'
							WHEN 304 THEN 'TCP Completed'
							WHEN 305 THEN 'Traffic (other)'
							WHEN 306 THEN 'ICMP packet'
							WHEN 307 THEN 'Ethernet packet'
							WHEN 308 THEN 'IP Packet' END) AS traffic_type,
						l.SEVERITY,
						l.LOCAL_HOST_IP AS local_ip,
						l.REMOTE_HOST_IP AS remote_ip,
						l.REMOTE_HOST_NAME AS remote_hostname,
						l.LOCAL_PORT,
						l.REMOTE_PORT,
						(SELECT CASE l.TRAFFIC_DIRECTION
							WHEN 0 THEN 'Unknown'
							WHEN 1 THEN 'Inbound'
							WHEN 2 THEN 'Outbound'
						END) AS direction,
						l.REPETITION,
						(SELECT CASE l.NETWORK_PROTOCOL
							WHEN 1 THEN 'Others'
							WHEN 2 THEN 'TCP'
							WHEN 3 THEN 'UDP' END) AS protocol,
						l.HACK_TYPE,
						l.BEGIN_TIME,
						l.EVENT_DESC AS description,
						l.APP_NAME,
						l.ALERT,
						l.LOCATION_NAME AS location,
						l.INTRUSION_URL AS intrusion_url,
						l.INTRUSION_PAYLOAD_URL AS intrusion_payload_url
				FROM	V_AGENT_SECURITY_LOG l
						LEFT JOIN IDENTITY_MAP g ON l.GROUP_ID = g.[ID]
						LEFT JOIN
							([V_SEM_COMPUTER] c INNER JOIN SEM_AGENT ag ON ag.[COMPUTER_ID] = c.[COMPUTER_ID])
						ON l.[COMPUTER_ID]= c.[COMPUTER_ID]
				WHERE	COALESCE(g.Name, '') = COALESCE(@GroupName, g.Name, '')
						AND DATEADD(second, l.EVENT_TIME / 1000, '1970-01-01') BETWEEN @StartDate AND @EndDate
				ORDER	BY g.[NAME],
						c.COMPUTER_NAME,
						l.EVENT_TIME";

			try {
				conn.ConnectionString = this.sqlConnectionString;
				conn.Open();

				cmd = new SqlCommand(sql, conn);
				cmd.CommandType = System.Data.CommandType.Text;

				if (String.IsNullOrEmpty(groupName)) {
					cmd.Parameters.Add(new SqlParameter("@GroupName", SqlDbType.VarChar, 100)).Value = DBNull.Value;
				} else {
					cmd.Parameters.Add(new SqlParameter("@GroupName", SqlDbType.VarChar, 100)).Value = groupName;
				}
				cmd.Parameters.Add(new SqlParameter("@StartDate", SqlDbType.DateTime)).Value = startDate;
				cmd.Parameters.Add(new SqlParameter("@EndDate", SqlDbType.DateTime)).Value = endDate;
				rdr = cmd.ExecuteReader();

				while (rdr.Read()) {
					Dictionary<string, string> e = new Dictionary<string, string>();
					e["id"] = rdr["id"] == DBNull.Value ? null : (string)rdr["id"];
					e["ed"] = ((DateTime)rdr["ed"]).ToString();

					e["gn"] = rdr["gn"] == DBNull.Value ? null : (string)rdr["gn"];
					e["agent_id"] = rdr["agent_id"] == DBNull.Value ? null : (string)rdr["agent_id"];
					e["agent_version"] = rdr["agent_version"] == DBNull.Value ? null : (string)rdr["agent_version"];
					e["last_update_time"] = ((DateTime)rdr["last_update_time"]).ToString();
					e["full_name"] = rdr["full_name"] == DBNull.Value ? null : (string)rdr["full_name"];
					e["email"] = rdr["email"] == DBNull.Value ? null : (string)rdr["email"];
					e["job_title"] = rdr["job_title"] == DBNull.Value ? null : (string)rdr["job_title"];
					e["department"] = rdr["department"] == DBNull.Value ? null : (string)rdr["department"];
					e["office_phone"] = rdr["office_phone"] == DBNull.Value ? null : (string)rdr["office_phone"];
					e["mobile_phone"] = rdr["mobile_phone"] == DBNull.Value ? null : (string)rdr["mobile_phone"];
					e["infected"] = ((Boolean)rdr["infected"]).ToString();
					e["last_scan_time"] = ((DateTime)rdr["last_scan_time"]).ToString();
					e["last_virus_time"] = rdr["last_virus_time"] == DBNull.Value ? null : ((DateTime)rdr["last_virus_time"]).ToString();
					e["last_download_time"] = ((DateTime)rdr["last_download_time"]).ToString();

					e["avengine_onoff"] = ((Boolean)rdr["avengine_onoff"]).ToString();
					e["tamper_onoff"] = ((Boolean)rdr["tamper_onoff"]).ToString();
					e["firewall_onoff"] = ((Boolean)rdr["firewall_onoff"]).ToString();
					e["reboot_required"] = ((Boolean)rdr["reboot_required"]).ToString();
					e["reboot_reason"] = rdr["reboot_reason"] == DBNull.Value ? null : (string)rdr["reboot_reason"];
					e["computer_name"] = rdr["computer_name"] == DBNull.Value ? null : (string)rdr["computer_name"];
					e["computer_domain_name"] = rdr["computer_domain_name"] == DBNull.Value ? null : (string)rdr["computer_domain_name"];
					e["operating_system"] = rdr["operating_system"] == DBNull.Value ? null : (string)rdr["operating_system"];
					e["service_pack"] = rdr["service_pack"] == DBNull.Value ? null : (string)rdr["service_pack"];
					e["current_login_user"] = rdr["current_login_user"] == DBNull.Value ? null : (string)rdr["current_login_user"];
					e["current_login_domain"] = rdr["current_login_domain"] == DBNull.Value ? null : (string)rdr["current_login_domain"];
					e["dns_server1"] = rdr["dns_server1"] == DBNull.Value ? null : Util.LongToIP((long)rdr["dns_server1"]);
					e["dns_server2"] = rdr["dns_server2"] == DBNull.Value ? null : Util.LongToIP((long)rdr["dns_server2"]);
					e["dhcp_server"] = rdr["dhcp_server"] == DBNull.Value ? null : Util.LongToIP((long)rdr["dhcp_server"]);
					e["mac_addr1"] = rdr["mac_addr1"] == DBNull.Value ? null : (string)rdr["mac_addr1"];
					e["ip_addr1"] = rdr["ip_addr1"] == DBNull.Value ? null : Util.LongToIP((long)rdr["ip_addr1"]);
					e["gateway1"] = rdr["gateway1"] == DBNull.Value ? null : Util.LongToIP((long)rdr["gateway1"]);
					e["subnet_mask1"] = rdr["subnet_mask1"] == DBNull.Value ? null : Util.LongToIP((long)rdr["subnet_mask1"]);

					e["traffic_type"] = rdr["traffic_type"] == DBNull.Value ? null : (string)rdr["traffic_type"];
					e["severity"] = ((int)rdr["severity"]).ToString();
					if (rdr["local_ip"] != DBNull.Value) {
						if (Type.GetTypeCode(rdr.GetFieldType(rdr.GetOrdinal("local_ip"))) == TypeCode.String) {
							e["local_ip"] = Util.IP2Long((string)rdr["local_ip"]).ToString();
						} else {
							e["local_ip"] = Util.LongToIP((long)rdr["local_ip"]);
						}
					}
					if (rdr["remote_ip"] != DBNull.Value) {
						if (Type.GetTypeCode(rdr.GetFieldType(rdr.GetOrdinal("remote_ip"))) == TypeCode.String) {
							e["remote_ip"] = Util.IP2Long((string)rdr["remote_ip"]).ToString();
						} else {
							e["remote_ip"] = Util.LongToIP((long)rdr["remote_ip"]);
						}
					}
					e["remote_hostname"] = rdr["remote_hostname"] == DBNull.Value ? null : (string)rdr["remote_hostname"];
					e["local_port"] = ((int)rdr["local_port"]).ToString();
					e["remote_port"] = ((int)rdr["remote_port"]).ToString();
					e["direction"] = rdr["direction"] == DBNull.Value ? null : (string)rdr["direction"];
					e["repetition"] = ((int)rdr["repetition"]).ToString();
					e["protocol"] = rdr["protocol"] == DBNull.Value ? null : (string)rdr["protocol"];
					e["app_name"] = rdr["app_name"] == DBNull.Value ? null : (string)rdr["app_name"];

					if (rdr["alert"] != DBNull.Value && (byte)rdr["alert"] == 1) {
						e["alert"] = "true";
					} else {
						e["alert"] = "false";
					}
					e["location"] = rdr["location"] == DBNull.Value ? null : (string)rdr["location"];
					e["intrusion_url"] = rdr["intrusion_url"] == DBNull.Value ? null : (string)rdr["intrusion_url"];
					e["intrusion_payload_url"] = rdr["intrusion_payload_url"] == DBNull.Value ? null : (string)rdr["intrusion_payload_url"];
					e["description"] = rdr["description"] == DBNull.Value ? null : (string)rdr["description"];

					events.Add(e);
				}
			} catch {
				events = new List<Dictionary<string, string>>();
			} finally {
				rdr.Close();
				rdr.Dispose();
				cmd.Dispose();
				conn.Close();
				conn.Dispose();
			}

			return events;
		}

		public Dictionary<string, string> getFlareIPSFieldNames() {
			Dictionary<string, string> e = new Dictionary<string, string>();

			e["Group Name"] = "gn";
			e["Agent ID"] = "agent_id";
			e["Agent Version"] = "agent_version";
			e["Last Update Time"] = "last_update_time";
			e["Full Name"] = "full_name";
			e["Email"] = "email";
			e["Job Title"] = "job_title";
			e["Department"] = "department";
			e["Office Phone"] = "office_phone";
			e["Mobile Phone"] = "mobile_phone";
			e["Infected"] = "infected";
			e["Last Scan Time"] = "last_scan_time";
			e["Last Virus Time"] = "last_virus_time";
			e["Last Download Time"] = "last_download_time";

			e["AV Engine on/off"] = "avengine_onoff";
			e["Tamper on/off"] = "tamper_onoff";
			e["Firewall on/off"] = "firewall_onoff";
			e["Reboot Required"] = "reboot_required";
			e["Reboot Reason"] = "reboot_reason";
			e["Computer Name"] = "computer_name";
			e["Computer Domain Name"] = "computer_domain_name";
			e["Operating System"] = "operating_system";
			e["Service Pack"] = "service_pack";
			e["Current Login User"] = "current_login_user";
			e["Current Login Domain"] = "current_login_domain";
			e["DNS Server 1"] = "dns_server1";
			e["DNS Server 2"] = "dns_server2";
			e["DHCP Server"] = "dhcp_server";
			e["MAC Address 1"] = "mac_addr1";
			e["IP Address 1"] = "ip_addr1";
			e["Gateway 1"] = "gateway1";
			e["Subnet Mask 1"] = "subnet_mask1";

			e["Traffic Type"] = "traffic_type";
			e["Severity"] = "severity";
			e["Local IP"] = "local_ip";
			e["Remote IP"] = "remote_ip";
			e["Remote Hostname"] = "remote_hostname";
			e["Local Port"] = "local_port";
			e["Remote Port"] = "remote_port";
			e["Direction"] = "direction";
			e["Repetition"] = "repetition";
			e["Protocol"] = "protocol";
			e["App Name"] = "app_name";
			e["Alert"] = "alert";
			e["Location"] = "location";
			e["Intrusion URL"] = "intrusion_url";
			e["Intrusion Payload URL"] = "intrusion_payload_url";
			e["Description"] = "description";

			return e;
		}

		public List<Dictionary<string, string>> getFlareDownloadDetail(string groupName, DateTime startDate, DateTime endDate) {
			SqlConnection conn = new SqlConnection();
			SqlCommand cmd = new SqlCommand();
			SqlDataReader rdr = null;

			List<Dictionary<string, string>> events = new List<Dictionary<string, string>>();

			string sql = @"
				SELECT	ag.AGENT_ID AS agent_id,
						c.COMPUTER_ID AS id,
						g.NAME as gn,
						ag.AGENT_VERSION AS agent_version,
						dateadd(second,[LAST_UPDATE_TIME]/1000, '1970-01-01') AS last_update_time,
						ag.FULL_NAME AS full_name,
						ag.EMAIL AS email,
						ag.JOB_TITLE AS job_title, 
						ag.DEPARTMENT AS department,
						ag.OFFICE_PHONE AS office_phone,
						ag.MOBILE_PHONE AS mobile_phone,
						CASE WHEN ag.INFECTED = 1 THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS infected,
						dateadd(second,[LAST_SCAN_TIME]/1000, '1970-01-01') AS last_scan_time,
						CASE WHEN LAST_VIRUS_TIME = 0 THEN NULL ELSE dateadd(second,[LAST_VIRUS_TIME]/1000, '1970-01-01') END AS last_virus_time,
						dateadd(second,[LAST_DOWNLOAD_TIME]/1000, '1970-01-01') AS last_download_time,
						CASE WHEN ag.AVENGINE_ONOFF = 1 THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS avengine_onoff,
						CASE WHEN ag.TAMPER_ONOFF = 1 THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS tamper_onoff,
						CASE WHEN ag.FIREWALL_ONOFF = 1 THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS firewall_onoff,
						CASE WHEN ag.REBOOT_REQUIRED = 1 THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS reboot_required,
						ag.REBOOT_REASON AS reboot_reason,
						c.COMPUTER_NAME AS computer_name,
						c.COMPUTER_DOMAIN_NAME AS computer_domain_name,
						c.OPERATION_SYSTEM AS operating_system,
						c.SERVICE_PACK AS service_pack,
						c.CURRENT_LOGIN_USER AS current_login_user,
						c.CURRENT_LOGIN_DOMAIN AS current_login_domain,
						c.DNS_SERVER1 AS dns_server1,
						c.DNS_SERVER2 AS dns_server2,
						c.DHCP_SERVER AS dhcp_server,
						c.MAC_ADDR1 AS mac_addr1,
						c.IP_ADDR1 AS ip_addr1,
						c.GATEWAY1 AS gateway1,
						c.SUBNET_MASK1 AS subnet_mask1,

						dateadd(second, app.TIME_STAMP / 1000, '1970-01-01') AS ed,
						a.[APPLICATION_NAME] AS app_name,
						a.[COMPANY_NAME] AS app_company,
						a.[VERSION] AS app_version,
						a.[FILE_SIZE] AS filesize,
						a.[APP_DESCRIPTION] AS description,
						app.[APP_HASH] AS app_md5,
						app.[CREATOR_SHA2] AS app_sha2,
						app.[DOWNLOAD_URL] AS url,
                        a.[SIGNER_NAME] AS signer,
						a.[LAST_MODIFY_TIME] AS last_modify_time
				FROM	COMPUTER_APPLICATION app
						INNER JOIN SEM_APPLICATION a on a.APP_HASH = app.APP_HASH
						INNER JOIN SEM_AGENT ag
							INNER JOIN V_SEM_COMPUTER c ON ag.[COMPUTER_ID] = c.[COMPUTER_ID]
							INNER JOIN IDENTITY_MAP g ON ag.GROUP_ID = g.[ID]
						on ag.AGENT_ID = app.AGENT_ID
				WHERE	COALESCE(g.Name, '') = COALESCE(@GroupName, g.Name, '')
						AND DATEADD(second, app.TIME_STAMP / 1000, '1970-01-01') BETWEEN @StartDate AND @EndDate
				ORDER	BY g.[NAME],
						c.COMPUTER_NAME,
						app.TIME_STAMP";

			try {
				conn.ConnectionString = this.sqlConnectionString;
				conn.Open();

				cmd = new SqlCommand(sql, conn);
				cmd.CommandType = System.Data.CommandType.Text;

				if (String.IsNullOrEmpty(groupName)) {
					cmd.Parameters.Add(new SqlParameter("@GroupName", SqlDbType.VarChar, 100)).Value = DBNull.Value;
				} else {
					cmd.Parameters.Add(new SqlParameter("@GroupName", SqlDbType.VarChar, 100)).Value = groupName;
				}
				cmd.Parameters.Add(new SqlParameter("@StartDate", SqlDbType.DateTime)).Value = startDate;
				cmd.Parameters.Add(new SqlParameter("@EndDate", SqlDbType.DateTime)).Value = endDate;
				rdr = cmd.ExecuteReader();

				while (rdr.Read()) {
					Dictionary<string, string> e = new Dictionary<string, string>();
					e["id"] = rdr["id"] == DBNull.Value ? null : (string)rdr["id"];
					e["ed"] = ((DateTime)rdr["ed"]).ToString();

					e["gn"] = rdr["gn"] == DBNull.Value ? null : (string)rdr["gn"];
					e["agent_id"] = rdr["agent_id"] == DBNull.Value ? null : (string)rdr["agent_id"];
					e["agent_version"] = rdr["agent_version"] == DBNull.Value ? null : (string)rdr["agent_version"];
					e["last_update_time"] = ((DateTime)rdr["last_update_time"]).ToString();
					e["full_name"] = rdr["full_name"] == DBNull.Value ? null : (string)rdr["full_name"];
					e["email"] = rdr["email"] == DBNull.Value ? null : (string)rdr["email"];
					e["job_title"] = rdr["job_title"] == DBNull.Value ? null : (string)rdr["job_title"];
					e["department"] = rdr["department"] == DBNull.Value ? null : (string)rdr["department"];
					e["office_phone"] = rdr["office_phone"] == DBNull.Value ? null : (string)rdr["office_phone"];
					e["mobile_phone"] = rdr["mobile_phone"] == DBNull.Value ? null : (string)rdr["mobile_phone"];
					e["infected"] = ((Boolean)rdr["infected"]).ToString();
					e["last_scan_time"] = ((DateTime)rdr["last_scan_time"]).ToString();
					e["last_virus_time"] = rdr["last_virus_time"] == DBNull.Value ? null : ((DateTime)rdr["last_virus_time"]).ToString();
					e["last_download_time"] = ((DateTime)rdr["last_download_time"]).ToString();

					e["avengine_onoff"] = ((Boolean)rdr["avengine_onoff"]).ToString();
					e["tamper_onoff"] = ((Boolean)rdr["tamper_onoff"]).ToString();
					e["firewall_onoff"] = ((Boolean)rdr["firewall_onoff"]).ToString();
					e["reboot_required"] = ((Boolean)rdr["reboot_required"]).ToString();
					e["reboot_reason"] = rdr["reboot_reason"] == DBNull.Value ? null : (string)rdr["reboot_reason"];
					e["computer_name"] = rdr["computer_name"] == DBNull.Value ? null : (string)rdr["computer_name"];
					e["computer_domain_name"] = rdr["computer_domain_name"] == DBNull.Value ? null : (string)rdr["computer_domain_name"];
					e["operating_system"] = rdr["operating_system"] == DBNull.Value ? null : (string)rdr["operating_system"];
					e["service_pack"] = rdr["service_pack"] == DBNull.Value ? null : (string)rdr["service_pack"];
					e["current_login_user"] = rdr["current_login_user"] == DBNull.Value ? null : (string)rdr["current_login_user"];
					e["current_login_domain"] = rdr["current_login_domain"] == DBNull.Value ? null : (string)rdr["current_login_domain"];
					e["dns_server1"] = rdr["dns_server1"] == DBNull.Value ? null : Util.LongToIP((long)rdr["dns_server1"]);
					e["dns_server2"] = rdr["dns_server2"] == DBNull.Value ? null : Util.LongToIP((long)rdr["dns_server2"]);
					e["dhcp_server"] = rdr["dhcp_server"] == DBNull.Value ? null : Util.LongToIP((long)rdr["dhcp_server"]);
					e["mac_addr1"] = rdr["mac_addr1"] == DBNull.Value ? null : (string)rdr["mac_addr1"];
					e["ip_addr1"] = rdr["ip_addr1"] == DBNull.Value ? null : Util.LongToIP((long)rdr["ip_addr1"]);
					e["gateway1"] = rdr["gateway1"] == DBNull.Value ? null : Util.LongToIP((long)rdr["gateway1"]);
					e["subnet_mask1"] = rdr["subnet_mask1"] == DBNull.Value ? null : Util.LongToIP((long)rdr["subnet_mask1"]);

					e["app_name"] = rdr["app_name"] == DBNull.Value ? null : (string)rdr["app_name"];
					e["app_company"] = rdr["app_company"] == DBNull.Value ? null : (string)rdr["app_company"];
					e["app_version"] = rdr["app_version"] == DBNull.Value ? null : (string)rdr["app_version"];
					e["filesize"] = ((long)rdr["filesize"]).ToString();
					e["description"] = rdr["description"] == DBNull.Value ? null : (string)rdr["description"];
					e["app_md5"] = rdr["app_md5"] == DBNull.Value ? null : (string)rdr["app_md5"];
					e["app_sha2"] = rdr["app_sha2"] == DBNull.Value ? null : (string)rdr["app_sha2"];
					e["url"] = rdr["url"] == DBNull.Value ? null : (string)rdr["url"];
					e["signer"] = rdr["signer"] == DBNull.Value ? null : (string)rdr["signer"];
					e["last_modify_time"] = ((long)rdr["last_modify_time"]).ToString();

					events.Add(e);
				}
			} catch (Exception e) {
				throw e;
//				events = new List<Dictionary<string, string>>();
			} finally {
				if (rdr != null) {
					rdr.Close();
					rdr.Dispose();
				}
				cmd.Dispose();
				conn.Close();
				conn.Dispose();
			}

			return events;
		}

		public Dictionary<string, string> getFlareDownloadFieldNames() {
			Dictionary<string, string> e = new Dictionary<string, string>();

			e["Group Name"] = "gn";
			e["Agent ID"] = "agent_id";
			e["Agent Version"] = "agent_version";
			e["Last Update Time"] = "last_update_time";
			e["Full Name"] = "full_name";
			e["Email"] = "email";
			e["Job Title"] = "job_title";
			e["Department"] = "department";
			e["Office Phone"] = "office_phone";
			e["Mobile Phone"] = "mobile_phone";
			e["Infected"] = "infected";
			e["Last Scan Time"] = "last_scan_time";
			e["Last Virus Time"] = "last_virus_time";
			e["Last Download Time"] = "last_download_time";

			e["AV Engine on/off"] = "avengine_onoff";
			e["Tamper on/off"] = "tamper_onoff";
			e["Firewall on/off"] = "firewall_onoff";
			e["Reboot Required"] = "reboot_required";
			e["Reboot Reason"] = "reboot_reason";
			e["Computer Name"] = "computer_name";
			e["Computer Domain Name"] = "computer_domain_name";
			e["Operating System"] = "operating_system";
			e["Service Pack"] = "service_pack";
			e["Current Login User"] = "current_login_user";
			e["Current Login Domain"] = "current_login_domain";
			e["DNS Server 1"] = "dns_server1";
			e["DNS Server 2"] = "dns_server2";
			e["DHCP Server"] = "dhcp_server";
			e["MAC Address 1"] = "mac_addr1";
			e["IP Address 1"] = "ip_addr1";
			e["Gateway 1"] = "gateway1";
			e["Subnet Mask 1"] = "subnet_mask1";

			e["App Name"] = "app_name";
			e["App Company"] = "app_company";
			e["App Version"] = "app_version";
			e["Filesize"] = "filesize";
			e["Description"] = "description";
			e["App MD5"] = "app_md5";
			e["App SHA2"] = "app_sha2";
			e["URL"] = "url";
			e["Signer"] = "signer";
			e["Last Modify Time"] = "last_modify_time";

			return e;
		}

		public List<Dictionary<string, string>> getFlareUserControlDetail(string groupName, DateTime startDate, DateTime endDate) {
			SqlConnection conn = new SqlConnection();
			SqlCommand cmd = new SqlCommand();
			SqlDataReader rdr = null;

			List<Dictionary<string, string>> events = new List<Dictionary<string, string>>();

			string sql = @"
				SELECT	ag.AGENT_ID AS agent_id,
						c.COMPUTER_ID AS id,
						g.NAME as gn,
						ag.AGENT_VERSION AS agent_version,
						dateadd(second,[LAST_UPDATE_TIME]/1000, '1970-01-01') AS last_update_time,
						ag.FULL_NAME AS full_name,
						ag.EMAIL AS email,
						ag.JOB_TITLE AS job_title, 
						ag.DEPARTMENT AS department,
						ag.OFFICE_PHONE AS office_phone,
						ag.MOBILE_PHONE AS mobile_phone,
						CASE WHEN ag.INFECTED = 1 THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS infected,
						dateadd(second,[LAST_SCAN_TIME]/1000, '1970-01-01') AS last_scan_time,
						CASE WHEN LAST_VIRUS_TIME = 0 THEN NULL ELSE dateadd(second,[LAST_VIRUS_TIME]/1000, '1970-01-01') END AS last_virus_time,
						dateadd(second,[LAST_DOWNLOAD_TIME]/1000, '1970-01-01') AS last_download_time,
						CASE WHEN ag.AVENGINE_ONOFF = 1 THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS avengine_onoff,
						CASE WHEN ag.TAMPER_ONOFF = 1 THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS tamper_onoff,
						CASE WHEN ag.FIREWALL_ONOFF = 1 THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS firewall_onoff,
						CASE WHEN ag.REBOOT_REQUIRED = 1 THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS reboot_required,
						ag.REBOOT_REASON AS reboot_reason,
						c.COMPUTER_NAME AS computer_name,
						c.COMPUTER_DOMAIN_NAME AS computer_domain_name,
						c.OPERATION_SYSTEM AS operating_system,
						c.SERVICE_PACK AS service_pack,
						c.CURRENT_LOGIN_USER AS current_login_user,
						c.CURRENT_LOGIN_DOMAIN AS current_login_domain,
						c.DNS_SERVER1 AS dns_server1,
						c.DNS_SERVER2 AS dns_server2,
						c.DHCP_SERVER AS dhcp_server,
						c.MAC_ADDR1 AS mac_addr1,
						c.IP_ADDR1 AS ip_addr1,
						c.GATEWAY1 AS gateway1,
						c.SUBNET_MASK1 AS subnet_mask1,

--						dateadd(second, l.EVENT_TIME / 1000, '1970-01-01') AS ed,
--						dateadd(second, l.BEGIN_TIME / 1000, '1970-01-01') AS begin_time,
--						dateadd(second, l.END_TIME / 1000, '1970-01-01') AS end_time,
						(SELECT
							CASE l.EVENT_ID
								WHEN 501 THEN 'Application Control Driver'
								WHEN 502 THEN 'Application Control Rules'
								WHEN 999 THEN 'Tamper Protection'
							END) AS event_type,
						l.severity,
						(SELECT
							CASE l.ACTION
								WHEN 0 THEN 'allow'
								WHEN 1 THEN 'block' 
								WHEN 2 THEN 'ask'
								WHEN 3 THEN 'continue'
								WHEN 4 THEN 'terminate'
							END) AS action,
						l.description,
						l.rule_name,
						l.caller_process_name,
--						l.parameter,
						l.alert,
						l.user_name,
						l.domain_name,
--						l.vapi_name,
						l.ip_addr,
--						l.file_size,
						l.param_device_id,
						COUNT(1) AS count
				FROM	v_agent_behavior_log l
						INNER JOIN SEM_AGENT ag
							INNER JOIN V_SEM_COMPUTER c ON ag.[COMPUTER_ID] = c.[COMPUTER_ID]
							INNER JOIN IDENTITY_MAP g ON ag.GROUP_ID = g.[ID]
						on ag.AGENT_ID = l.AGENT_ID
				WHERE	l.caller_process_name != 'SysPlant'
						AND COALESCE(g.Name, '') = COALESCE(@GroupName, g.Name, '')
						AND DATEADD(second, l.EVENT_TIME / 1000, '1970-01-01') BETWEEN @StartDate AND @EndDate
				GROUP	BY
						ag.AGENT_ID,
						c.COMPUTER_ID,
						g.NAME,
						ag.AGENT_VERSION,
						dateadd(second,[LAST_UPDATE_TIME]/1000, '1970-01-01'),
						ag.FULL_NAME,
						ag.EMAIL,
						ag.JOB_TITLE,
						ag.DEPARTMENT,
						ag.OFFICE_PHONE,
						ag.MOBILE_PHONE,
						CASE WHEN ag.INFECTED = 1 THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END,
						dateadd(second,[LAST_SCAN_TIME]/1000, '1970-01-01'),
						CASE WHEN LAST_VIRUS_TIME = 0 THEN NULL ELSE dateadd(second,[LAST_VIRUS_TIME]/1000, '1970-01-01') END,
						dateadd(second,[LAST_DOWNLOAD_TIME]/1000, '1970-01-01'),
						CASE WHEN ag.AVENGINE_ONOFF = 1 THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END,
						CASE WHEN ag.TAMPER_ONOFF = 1 THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END,
						CASE WHEN ag.FIREWALL_ONOFF = 1 THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END,
						CASE WHEN ag.REBOOT_REQUIRED = 1 THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END,
						ag.REBOOT_REASON,
						c.COMPUTER_NAME,
						c.COMPUTER_DOMAIN_NAME,
						c.OPERATION_SYSTEM,
						c.SERVICE_PACK,
						c.CURRENT_LOGIN_USER,
						c.CURRENT_LOGIN_DOMAIN,
						c.DNS_SERVER1,
						c.DNS_SERVER2,
						c.DHCP_SERVER,
						c.MAC_ADDR1,
						c.IP_ADDR1,
						c.GATEWAY1,
						c.SUBNET_MASK1,

--						dateadd(second, l.EVENT_TIME / 1000, '1970-01-01'),
--						dateadd(second, l.BEGIN_TIME / 1000, '1970-01-01'),
--						dateadd(second, l.END_TIME / 1000, '1970-01-01'),
						l.event_id,
						l.severity,
						l.action,
						l.description,
						l.rule_name,
						l.caller_process_name,
--						l.parameter,
						l.alert,
						l.user_name,
						l.domain_name,
--						l.vapi_name,
						l.ip_addr,
--						l.file_size,
						l.param_device_id
				ORDER	BY g.[NAME],
						c.COMPUTER_NAME";

			try {
				conn.ConnectionString = this.sqlConnectionString;
				conn.Open();

				cmd = new SqlCommand(sql, conn);
				cmd.CommandType = System.Data.CommandType.Text;

				if (String.IsNullOrEmpty(groupName)) {
					cmd.Parameters.Add(new SqlParameter("@GroupName", SqlDbType.VarChar, 100)).Value = DBNull.Value;
				} else {
					cmd.Parameters.Add(new SqlParameter("@GroupName", SqlDbType.VarChar, 100)).Value = groupName;
				}
				cmd.Parameters.Add(new SqlParameter("@StartDate", SqlDbType.DateTime)).Value = startDate;
				cmd.Parameters.Add(new SqlParameter("@EndDate", SqlDbType.DateTime)).Value = endDate;
				rdr = cmd.ExecuteReader();

				while (rdr.Read()) {
					Dictionary<string, string> e = new Dictionary<string, string>();
					e["id"] = rdr["id"] == DBNull.Value ? null : (string)rdr["id"];
//					e["ed"] = ((DateTime)rdr["ed"]).ToString();

					e["gn"] = rdr["gn"] == DBNull.Value ? null : (string)rdr["gn"];
					e["agent_id"] = rdr["agent_id"] == DBNull.Value ? null : (string)rdr["agent_id"];
					e["agent_version"] = rdr["agent_version"] == DBNull.Value ? null : (string)rdr["agent_version"];
					e["last_update_time"] = ((DateTime)rdr["last_update_time"]).ToString();
					e["full_name"] = rdr["full_name"] == DBNull.Value ? null : (string)rdr["full_name"];
					e["email"] = rdr["email"] == DBNull.Value ? null : (string)rdr["email"];
					e["job_title"] = rdr["job_title"] == DBNull.Value ? null : (string)rdr["job_title"];
					e["department"] = rdr["department"] == DBNull.Value ? null : (string)rdr["department"];
					e["office_phone"] = rdr["office_phone"] == DBNull.Value ? null : (string)rdr["office_phone"];
					e["mobile_phone"] = rdr["mobile_phone"] == DBNull.Value ? null : (string)rdr["mobile_phone"];
					e["infected"] = ((Boolean)rdr["infected"]).ToString();
					e["last_scan_time"] = ((DateTime)rdr["last_scan_time"]).ToString();
					e["last_virus_time"] = rdr["last_virus_time"] == DBNull.Value ? null : ((DateTime)rdr["last_virus_time"]).ToString();
					e["last_download_time"] = ((DateTime)rdr["last_download_time"]).ToString();

					e["avengine_onoff"] = ((Boolean)rdr["avengine_onoff"]).ToString();
					e["tamper_onoff"] = ((Boolean)rdr["tamper_onoff"]).ToString();
					e["firewall_onoff"] = ((Boolean)rdr["firewall_onoff"]).ToString();
					e["reboot_required"] = ((Boolean)rdr["reboot_required"]).ToString();
					e["reboot_reason"] = rdr["reboot_reason"] == DBNull.Value ? null : (string)rdr["reboot_reason"];
					e["computer_name"] = rdr["computer_name"] == DBNull.Value ? null : (string)rdr["computer_name"];
					e["computer_domain_name"] = rdr["computer_domain_name"] == DBNull.Value ? null : (string)rdr["computer_domain_name"];
					e["operating_system"] = rdr["operating_system"] == DBNull.Value ? null : (string)rdr["operating_system"];
					e["service_pack"] = rdr["service_pack"] == DBNull.Value ? null : (string)rdr["service_pack"];
					e["current_login_user"] = rdr["current_login_user"] == DBNull.Value ? null : (string)rdr["current_login_user"];
					e["current_login_domain"] = rdr["current_login_domain"] == DBNull.Value ? null : (string)rdr["current_login_domain"];
					e["dns_server1"] = rdr["dns_server1"] == DBNull.Value ? null : Util.LongToIP((long)rdr["dns_server1"]);
					e["dns_server2"] = rdr["dns_server2"] == DBNull.Value ? null : Util.LongToIP((long)rdr["dns_server2"]);
					e["dhcp_server"] = rdr["dhcp_server"] == DBNull.Value ? null : Util.LongToIP((long)rdr["dhcp_server"]);
					e["mac_addr1"] = rdr["mac_addr1"] == DBNull.Value ? null : (string)rdr["mac_addr1"];
					e["ip_addr1"] = rdr["ip_addr1"] == DBNull.Value ? null : Util.LongToIP((long)rdr["ip_addr1"]);
					e["gateway1"] = rdr["gateway1"] == DBNull.Value ? null : Util.LongToIP((long)rdr["gateway1"]);
					e["subnet_mask1"] = rdr["subnet_mask1"] == DBNull.Value ? null : Util.LongToIP((long)rdr["subnet_mask1"]);


//					e["begin_time"] = ((DateTime)rdr["begin_time"]).ToString();
//					e["end_time"] = ((DateTime)rdr["end_time"]).ToString();
//					e["event_type"] = ((int)rdr["event_type"]).ToString();
					e["severity"] = rdr["severity"] == DBNull.Value ? null : ((int)rdr["severity"]).ToString();
					e["action"] = rdr["action"] == DBNull.Value ? null : (string)rdr["action"];
					e["description"] = rdr["description"] == DBNull.Value ? null : (string)rdr["description"];
					e["rule_name"] = rdr["rule_name"] == DBNull.Value ? null : (string)rdr["rule_name"];
					e["caller_process_name"] = rdr["caller_process_name"] == DBNull.Value ? null : (string)rdr["caller_process_name"];
//					e["parameter"] = rdr["parameter"] == DBNull.Value ? null : (string)rdr["parameter"];
					e["alert"] = ((int)rdr["alert"]).ToString();
					e["user_name"] = rdr["user_name"] == DBNull.Value ? null : (string)rdr["user_name"];
					e["domain_name"] = rdr["domain_name"] == DBNull.Value ? null : (string)rdr["domain_name"];
//					e["vapi_name"] = rdr["vapi_name"] == DBNull.Value ? null : (string)rdr["vapi_name"];
					e["ip_addr"] = rdr["ip_addr"] == DBNull.Value ? null : Util.LongToIP((long)rdr["ip_addr"]);
//					e["file_size"] = ((long)rdr["file_size"]).ToString();
					e["param_device_id"] = rdr["param_device_id"] == DBNull.Value ? null : (string)rdr["param_device_id"];

					e["count"] = ((int)rdr["count"]).ToString();
					
					events.Add(e);
				}
			} catch (Exception e) {
				throw e;
				//				events = new List<Dictionary<string, string>>();
			} finally {
				if (rdr != null) {
					rdr.Close();
					rdr.Dispose();
				}
				cmd.Dispose();
				conn.Close();
				conn.Dispose();
			}

			return events;
		}

		public Dictionary<string, string> getFlareUserControlFieldNames() {
			Dictionary<string, string> e = new Dictionary<string, string>();

			e["Count"] = "count";
			e["Group Name"] = "gn";
			e["Agent ID"] = "agent_id";
			e["Agent Version"] = "agent_version";
			e["Last Update Time"] = "last_update_time";
			e["Full Name"] = "full_name";
			e["Email"] = "email";
			e["Job Title"] = "job_title";
			e["Department"] = "department";
			e["Office Phone"] = "office_phone";
			e["Mobile Phone"] = "mobile_phone";
			e["Infected"] = "infected";
			e["Last Scan Time"] = "last_scan_time";
			e["Last Virus Time"] = "last_virus_time";
			e["Last Download Time"] = "last_download_time";

			e["AV Engine on/off"] = "avengine_onoff";
			e["Tamper on/off"] = "tamper_onoff";
			e["Firewall on/off"] = "firewall_onoff";
			e["Reboot Required"] = "reboot_required";
			e["Reboot Reason"] = "reboot_reason";
			e["Computer Name"] = "computer_name";
			e["Computer Domain Name"] = "computer_domain_name";
			e["Operating System"] = "operating_system";
			e["Service Pack"] = "service_pack";
			e["Current Login User"] = "current_login_user";
			e["Current Login Domain"] = "current_login_domain";
			e["DNS Server 1"] = "dns_server1";
			e["DNS Server 2"] = "dns_server2";
			e["DHCP Server"] = "dhcp_server";
			e["MAC Address 1"] = "mac_addr1";
			e["IP Address 1"] = "ip_addr1";
			e["Gateway 1"] = "gateway1";
			e["Subnet Mask 1"] = "subnet_mask1";

			//			e["Event Date/Time"] = "ed";
			//			e["Begin Time"] = "begin_time";
			//			e["End Time"] = "end_time";
			e["Event Type"] = "event_type";
			e["Severity"] = "severity";
			e["Action"] = "action";
			e["Description"] = "description";
			e["Rule Name"] = "rule_name";
			e["Process Name"] = "caller_process_name";
			//			e["Parameter"] = "parameter";
			e["Alert"] = "alert";
			e["User Name"] = "user_name";
			e["Domain Name"] = "domain_name";
			//			e["API Type"] = "vapi_name";
			e["IP Address"] = "ip_addr";
			//			e["File Size"] = "file_size";
			e["Device ID"] = "param_device_id";

			return e;
		}

		public List<Dictionary<string, string>> getFlareUpdatesDetail(string groupName, DateTime startDate, DateTime endDate) {
			SqlConnection conn = new SqlConnection();
			SqlCommand cmd = new SqlCommand();
			SqlDataReader rdr = null;

			List<Dictionary<string, string>> events = new List<Dictionary<string, string>>();

			string sql = @"
				SELECT	ag.AGENT_ID AS agent_id,
						c.COMPUTER_ID AS id,
						g.NAME as gn,
						ag.AGENT_VERSION AS agent_version,
						dateadd(second,[LAST_UPDATE_TIME]/1000, '1970-01-01') AS last_update_time,
						ag.FULL_NAME AS full_name,
						ag.EMAIL AS email,
						ag.JOB_TITLE AS job_title, 
						ag.DEPARTMENT AS department,
						ag.OFFICE_PHONE AS office_phone,
						ag.MOBILE_PHONE AS mobile_phone,
						CASE WHEN ag.INFECTED = 1 THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS infected,
						dateadd(second,[LAST_SCAN_TIME]/1000, '1970-01-01') AS last_scan_time,
						CASE WHEN LAST_VIRUS_TIME = 0 THEN NULL ELSE dateadd(second,[LAST_VIRUS_TIME]/1000, '1970-01-01') END AS last_virus_time,
						dateadd(second,[LAST_DOWNLOAD_TIME]/1000, '1970-01-01') AS last_download_time,
						CASE WHEN ag.AVENGINE_ONOFF = 1 THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS avengine_onoff,
						CASE WHEN ag.TAMPER_ONOFF = 1 THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS tamper_onoff,
						CASE WHEN ag.FIREWALL_ONOFF = 1 THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS firewall_onoff,
						CASE WHEN ag.REBOOT_REQUIRED = 1 THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS reboot_required,
						ag.REBOOT_REASON AS reboot_reason,
						c.COMPUTER_NAME AS computer_name,
						c.COMPUTER_DOMAIN_NAME AS computer_domain_name,
						c.OPERATION_SYSTEM AS operating_system,
						c.SERVICE_PACK AS service_pack,
						c.CURRENT_LOGIN_USER AS current_login_user,
						c.CURRENT_LOGIN_DOMAIN AS current_login_domain,
						c.DNS_SERVER1 AS dns_server1,
						c.DNS_SERVER2 AS dns_server2,
						c.DHCP_SERVER AS dhcp_server,
						c.MAC_ADDR1 AS mac_addr1,
						c.IP_ADDR1 AS ip_addr1,
						c.GATEWAY1 AS gateway1,
						c.SUBNET_MASK1 AS subnet_mask1,

						DATEADD(SECOND, l.EVENT_TIME/1000, '1970-01-01') AS ed,
						l.EVENT_DESC AS description

				FROM	V_AGENT_SYSTEM_LOG l
						INNER JOIN SEM_AGENT ag
							INNER JOIN V_SEM_COMPUTER c ON ag.[COMPUTER_ID] = c.[COMPUTER_ID]
							INNER JOIN IDENTITY_MAP g ON ag.GROUP_ID = g.[ID]
						on ag.AGENT_ID = l.AGENT_ID
				WHERE	EVENT_SOURCE = 'SYLINK'
						AND EVENT_DESC LIKE 'Downloaded%'
						AND COALESCE(g.Name, '') = COALESCE(@GroupName, g.Name, '')
						AND DATEADD(second, l.TIME_STAMP / 1000, '1970-01-01') BETWEEN @StartDate AND @EndDate
				ORDER	BY g.[NAME],
						c.COMPUTER_NAME,
						l.TIME_STAMP";

			try {
				conn.ConnectionString = this.sqlConnectionString;
				conn.Open();

				cmd = new SqlCommand(sql, conn);
				cmd.CommandType = System.Data.CommandType.Text;

				if (String.IsNullOrEmpty(groupName)) {
					cmd.Parameters.Add(new SqlParameter("@GroupName", SqlDbType.VarChar, 100)).Value = DBNull.Value;
				} else {
					cmd.Parameters.Add(new SqlParameter("@GroupName", SqlDbType.VarChar, 100)).Value = groupName;
				}
				cmd.Parameters.Add(new SqlParameter("@StartDate", SqlDbType.DateTime)).Value = startDate;
				cmd.Parameters.Add(new SqlParameter("@EndDate", SqlDbType.DateTime)).Value = endDate;
				rdr = cmd.ExecuteReader();

				while (rdr.Read()) {
					Dictionary<string, string> e = new Dictionary<string, string>();
					e["id"] = rdr["id"] == DBNull.Value ? null : (string)rdr["id"];
					e["ed"] = ((DateTime)rdr["ed"]).ToString();

					e["gn"] = rdr["gn"] == DBNull.Value ? null : (string)rdr["gn"];
					e["agent_id"] = rdr["agent_id"] == DBNull.Value ? null : (string)rdr["agent_id"];
					e["agent_version"] = rdr["agent_version"] == DBNull.Value ? null : (string)rdr["agent_version"];
					e["last_update_time"] = ((DateTime)rdr["last_update_time"]).ToString();
					e["full_name"] = rdr["full_name"] == DBNull.Value ? null : (string)rdr["full_name"];
					e["email"] = rdr["email"] == DBNull.Value ? null : (string)rdr["email"];
					e["job_title"] = rdr["job_title"] == DBNull.Value ? null : (string)rdr["job_title"];
					e["department"] = rdr["department"] == DBNull.Value ? null : (string)rdr["department"];
					e["office_phone"] = rdr["office_phone"] == DBNull.Value ? null : (string)rdr["office_phone"];
					e["mobile_phone"] = rdr["mobile_phone"] == DBNull.Value ? null : (string)rdr["mobile_phone"];
					e["infected"] = ((Boolean)rdr["infected"]).ToString();
					e["last_scan_time"] = ((DateTime)rdr["last_scan_time"]).ToString();
					e["last_virus_time"] = rdr["last_virus_time"] == DBNull.Value ? null : ((DateTime)rdr["last_virus_time"]).ToString();
					e["last_download_time"] = ((DateTime)rdr["last_download_time"]).ToString();

					e["avengine_onoff"] = ((Boolean)rdr["avengine_onoff"]).ToString();
					e["tamper_onoff"] = ((Boolean)rdr["tamper_onoff"]).ToString();
					e["firewall_onoff"] = ((Boolean)rdr["firewall_onoff"]).ToString();
					e["reboot_required"] = ((Boolean)rdr["reboot_required"]).ToString();
					e["reboot_reason"] = rdr["reboot_reason"] == DBNull.Value ? null : (string)rdr["reboot_reason"];
					e["computer_name"] = rdr["computer_name"] == DBNull.Value ? null : (string)rdr["computer_name"];
					e["computer_domain_name"] = rdr["computer_domain_name"] == DBNull.Value ? null : (string)rdr["computer_domain_name"];
					e["operating_system"] = rdr["operating_system"] == DBNull.Value ? null : (string)rdr["operating_system"];
					e["service_pack"] = rdr["service_pack"] == DBNull.Value ? null : (string)rdr["service_pack"];
					e["current_login_user"] = rdr["current_login_user"] == DBNull.Value ? null : (string)rdr["current_login_user"];
					e["current_login_domain"] = rdr["current_login_domain"] == DBNull.Value ? null : (string)rdr["current_login_domain"];
					e["dns_server1"] = rdr["dns_server1"] == DBNull.Value ? null : Util.LongToIP((long)rdr["dns_server1"]);
					e["dns_server2"] = rdr["dns_server2"] == DBNull.Value ? null : Util.LongToIP((long)rdr["dns_server2"]);
					e["dhcp_server"] = rdr["dhcp_server"] == DBNull.Value ? null : Util.LongToIP((long)rdr["dhcp_server"]);
					e["mac_addr1"] = rdr["mac_addr1"] == DBNull.Value ? null : (string)rdr["mac_addr1"];
					e["ip_addr1"] = rdr["ip_addr1"] == DBNull.Value ? null : Util.LongToIP((long)rdr["ip_addr1"]);
					e["gateway1"] = rdr["gateway1"] == DBNull.Value ? null : Util.LongToIP((long)rdr["gateway1"]);
					e["subnet_mask1"] = rdr["subnet_mask1"] == DBNull.Value ? null : Util.LongToIP((long)rdr["subnet_mask1"]);

					e["description"] = rdr["description"] == DBNull.Value ? null : (string)rdr["description"];

					events.Add(e);
				}
			} catch (Exception e) {
				throw e;
			} finally {
				if (rdr != null) {
					rdr.Close();
					rdr.Dispose();
				}
				cmd.Dispose();
				conn.Close();
				conn.Dispose();
			}

			return events;
		}

		public Dictionary<string, string> getFlareUpdatesFieldNames() {
			Dictionary<string, string> e = new Dictionary<string, string>();

			e["Group Name"] = "gn";
			e["Agent ID"] = "agent_id";
			e["Agent Version"] = "agent_version";
			e["Last Update Time"] = "last_update_time";
			e["Full Name"] = "full_name";
			e["Email"] = "email";
			e["Job Title"] = "job_title";
			e["Department"] = "department";
			e["Office Phone"] = "office_phone";
			e["Mobile Phone"] = "mobile_phone";
			e["Infected"] = "infected";
			e["Last Scan Time"] = "last_scan_time";
			e["Last Virus Time"] = "last_virus_time";
			e["Last Download Time"] = "last_download_time";

			e["AV Engine on/off"] = "avengine_onoff";
			e["Tamper on/off"] = "tamper_onoff";
			e["Firewall on/off"] = "firewall_onoff";
			e["Reboot Required"] = "reboot_required";
			e["Reboot Reason"] = "reboot_reason";
			e["Computer Name"] = "computer_name";
			e["Computer Domain Name"] = "computer_domain_name";
			e["Operating System"] = "operating_system";
			e["Service Pack"] = "service_pack";
			e["Current Login User"] = "current_login_user";
			e["Current Login Domain"] = "current_login_domain";
			e["DNS Server 1"] = "dns_server1";
			e["DNS Server 2"] = "dns_server2";
			e["DHCP Server"] = "dhcp_server";
			e["MAC Address 1"] = "mac_addr1";
			e["IP Address 1"] = "ip_addr1";
			e["Gateway 1"] = "gateway1";
			e["Subnet Mask 1"] = "subnet_mask1";

			e["Description"] = "description";

			return e;
		}

		public List<Dictionary<string, string>> getFlareFirewallDetail(string groupName, DateTime startDate, DateTime endDate) {
			SqlConnection conn = new SqlConnection();
			SqlCommand cmd = new SqlCommand();
			SqlDataReader rdr = null;

			List<Dictionary<string, string>> events = new List<Dictionary<string, string>>();

			string sql = @"
				SELECT	ag.AGENT_ID AS agent_id,
						c.COMPUTER_ID AS id,
						g.NAME as gn,
						ag.AGENT_VERSION AS agent_version,
						dateadd(second,[LAST_UPDATE_TIME]/1000, '1970-01-01') AS last_update_time,
						ag.FULL_NAME AS full_name,
						ag.EMAIL AS email,
						ag.JOB_TITLE AS job_title, 
						ag.DEPARTMENT AS department,
						ag.OFFICE_PHONE AS office_phone,
						ag.MOBILE_PHONE AS mobile_phone,
						CASE WHEN ag.INFECTED = 1 THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS infected,
						dateadd(second,[LAST_SCAN_TIME]/1000, '1970-01-01') AS last_scan_time,
						CASE WHEN LAST_VIRUS_TIME = 0 THEN NULL ELSE dateadd(second,[LAST_VIRUS_TIME]/1000, '1970-01-01') END AS last_virus_time,
						dateadd(second,[LAST_DOWNLOAD_TIME]/1000, '1970-01-01') AS last_download_time,
						CASE WHEN ag.AVENGINE_ONOFF = 1 THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS avengine_onoff,
						CASE WHEN ag.TAMPER_ONOFF = 1 THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS tamper_onoff,
						CASE WHEN ag.FIREWALL_ONOFF = 1 THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS firewall_onoff,
						CASE WHEN ag.REBOOT_REQUIRED = 1 THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS reboot_required,
						ag.REBOOT_REASON AS reboot_reason,
						c.COMPUTER_NAME AS computer_name,
						c.COMPUTER_DOMAIN_NAME AS computer_domain_name,
						c.OPERATION_SYSTEM AS operating_system,
						c.SERVICE_PACK AS service_pack,
						c.CURRENT_LOGIN_USER AS current_login_user,
						c.CURRENT_LOGIN_DOMAIN AS current_login_domain,
						c.DNS_SERVER1 AS dns_server1,
						c.DNS_SERVER2 AS dns_server2,
						c.DHCP_SERVER AS dhcp_server,
						c.MAC_ADDR1 AS mac_addr1,
						c.IP_ADDR1 AS ip_addr1,
						c.GATEWAY1 AS gateway1,
						c.SUBNET_MASK1 AS subnet_mask1,

						DATEADD(SECOND, l.EVENT_TIME/1000, '1970-01-01') AS ed,

						NETWORK_PROTOCOL AS protocol,
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
						SEVERITY AS severity,
						NULL AS severity_string,
						BLOCKED AS blocked,
						APP_NAME AS app_name,
						ALERT AS Alert,
						RULE_NAME AS rule_name,
						LOCATION_NAME AS location,
						REPETITION AS repetition
				FROM	V_AGENT_TRAFFIC_LOG l
						INNER JOIN SEM_AGENT ag
							INNER JOIN V_SEM_COMPUTER c ON ag.[COMPUTER_ID] = c.[COMPUTER_ID]
							INNER JOIN IDENTITY_MAP g ON ag.GROUP_ID = g.[ID]
						on ag.AGENT_ID = l.AGENT_ID
				WHERE	l.BLOCKED = 1
						AND COALESCE(g.Name, '') = COALESCE(@GroupName, g.Name, '')
						AND DATEADD(second, l.TIME_STAMP / 1000, '1970-01-01') BETWEEN @StartDate AND @EndDate
				ORDER	BY g.[NAME],
						c.COMPUTER_NAME";

			try {
				conn.ConnectionString = this.sqlConnectionString;
				conn.Open();

				cmd = new SqlCommand(sql, conn);
				cmd.CommandType = System.Data.CommandType.Text;

				if (String.IsNullOrEmpty(groupName)) {
					cmd.Parameters.Add(new SqlParameter("@GroupName", SqlDbType.VarChar, 100)).Value = DBNull.Value;
				} else {
					cmd.Parameters.Add(new SqlParameter("@GroupName", SqlDbType.VarChar, 100)).Value = groupName;
				}
				cmd.Parameters.Add(new SqlParameter("@StartDate", SqlDbType.DateTime)).Value = startDate;
				cmd.Parameters.Add(new SqlParameter("@EndDate", SqlDbType.DateTime)).Value = endDate;
				rdr = cmd.ExecuteReader();

				while (rdr.Read()) {
					Dictionary<string, string> e = new Dictionary<string, string>();
					e["id"] = rdr["id"] == DBNull.Value ? null : (string)rdr["id"];
					e["ed"] = ((DateTime)rdr["ed"]).ToString();

					e["gn"] = rdr["gn"] == DBNull.Value ? null : (string)rdr["gn"];
					e["agent_id"] = rdr["agent_id"] == DBNull.Value ? null : (string)rdr["agent_id"];
					e["agent_version"] = rdr["agent_version"] == DBNull.Value ? null : (string)rdr["agent_version"];
					e["last_update_time"] = ((DateTime)rdr["last_update_time"]).ToString();
					e["full_name"] = rdr["full_name"] == DBNull.Value ? null : (string)rdr["full_name"];
					e["email"] = rdr["email"] == DBNull.Value ? null : (string)rdr["email"];
					e["job_title"] = rdr["job_title"] == DBNull.Value ? null : (string)rdr["job_title"];
					e["department"] = rdr["department"] == DBNull.Value ? null : (string)rdr["department"];
					e["office_phone"] = rdr["office_phone"] == DBNull.Value ? null : (string)rdr["office_phone"];
					e["mobile_phone"] = rdr["mobile_phone"] == DBNull.Value ? null : (string)rdr["mobile_phone"];
					e["infected"] = ((Boolean)rdr["infected"]).ToString();
					e["last_scan_time"] = ((DateTime)rdr["last_scan_time"]).ToString();
					e["last_virus_time"] = rdr["last_virus_time"] == DBNull.Value ? null : ((DateTime)rdr["last_virus_time"]).ToString();
					e["last_download_time"] = ((DateTime)rdr["last_download_time"]).ToString();

					e["avengine_onoff"] = ((Boolean)rdr["avengine_onoff"]).ToString();
					e["tamper_onoff"] = ((Boolean)rdr["tamper_onoff"]).ToString();
					e["firewall_onoff"] = ((Boolean)rdr["firewall_onoff"]).ToString();
					e["reboot_required"] = ((Boolean)rdr["reboot_required"]).ToString();
					e["reboot_reason"] = rdr["reboot_reason"] == DBNull.Value ? null : (string)rdr["reboot_reason"];
					e["computer_name"] = rdr["computer_name"] == DBNull.Value ? null : (string)rdr["computer_name"];
					e["computer_domain_name"] = rdr["computer_domain_name"] == DBNull.Value ? null : (string)rdr["computer_domain_name"];
					e["operating_system"] = rdr["operating_system"] == DBNull.Value ? null : (string)rdr["operating_system"];
					e["service_pack"] = rdr["service_pack"] == DBNull.Value ? null : (string)rdr["service_pack"];
					e["current_login_user"] = rdr["current_login_user"] == DBNull.Value ? null : (string)rdr["current_login_user"];
					e["current_login_domain"] = rdr["current_login_domain"] == DBNull.Value ? null : (string)rdr["current_login_domain"];
					e["dns_server1"] = rdr["dns_server1"] == DBNull.Value ? null : Util.LongToIP((long)rdr["dns_server1"]);
					e["dns_server2"] = rdr["dns_server2"] == DBNull.Value ? null : Util.LongToIP((long)rdr["dns_server2"]);
					e["dhcp_server"] = rdr["dhcp_server"] == DBNull.Value ? null : Util.LongToIP((long)rdr["dhcp_server"]);
					e["mac_addr1"] = rdr["mac_addr1"] == DBNull.Value ? null : (string)rdr["mac_addr1"];
					e["ip_addr1"] = rdr["ip_addr1"] == DBNull.Value ? null : Util.LongToIP((long)rdr["ip_addr1"]);
					e["gateway1"] = rdr["gateway1"] == DBNull.Value ? null : Util.LongToIP((long)rdr["gateway1"]);
					e["subnet_mask1"] = rdr["subnet_mask1"] == DBNull.Value ? null : Util.LongToIP((long)rdr["subnet_mask1"]);

					if (rdr["local_ip"] != DBNull.Value) {
						if (Type.GetTypeCode(rdr.GetFieldType(rdr.GetOrdinal("local_ip"))) == TypeCode.String) {
							e["local_ip"] = Util.IP2Long((string)rdr["local_ip"]).ToString();
						} else {
							e["local_ip"] = Util.LongToIP((long)rdr["local_ip"]);
						}
					}
					if (rdr["remote_ip"] != DBNull.Value) {
						if (Type.GetTypeCode(rdr.GetFieldType(rdr.GetOrdinal("remote_ip"))) == TypeCode.String) {
							e["remote_ip"] = Util.IP2Long((string)rdr["remote_ip"]).ToString();
						} else {
							e["remote_ip"] = Util.LongToIP((long)rdr["remote_ip"]);
						}
					}
/*					if (rdr["protocol"] != DBNull.Value) {
						switch ((byte)rdr["protocol"]) {
							case 1:
								e["protocol"] = "Others";
								break;
							case 2:
								e["protocol"] = "TCP";
								break;
							case 3:
								e["protocol"] = "UDP";
								break;
							case 4:
								e["protocol"] = "ICMP";
								break;
						}
					}
*/					e["protocol"] = rdr["protocol"] == DBNull.Value ? null : ((byte)rdr["protocol"]).ToString();
					e["traffic_type"] = rdr["traffic_type"] == DBNull.Value ? null : (string)rdr["traffic_type"];
					e["remote_hostname"] = rdr["remote_hostname"] == DBNull.Value ? null : (string)rdr["remote_hostname"];
					e["local_port"] = ((int)rdr["local_port"]).ToString();
					e["remote_port"] = ((int)rdr["remote_port"]).ToString();
					e["direction"] = rdr["direction"] == DBNull.Value ? null : (string)rdr["direction"];
					e["repetition"] = ((int)rdr["repetition"]).ToString();
					if (rdr["blocked"] != DBNull.Value && (byte)rdr["blocked"] == 1) {
						e["action_taken"] = "Blocked";
					}
					e["app_name"] = rdr["app_name"] == DBNull.Value ? null : (string)rdr["app_name"];
					if (rdr["alert"] != DBNull.Value && (byte)rdr["alert"] == 1) {
						e["alert"] = "true";
					}
					e["rule_name"] = rdr["rule_name"] == DBNull.Value ? null : (string)rdr["rule_name"];
					e["location"] = rdr["location"] == DBNull.Value ? null : (string)rdr["location"];

					events.Add(e);
				}
			} catch (Exception e) {
				throw e;
			} finally {
				if (rdr != null) {
					rdr.Close();
					rdr.Dispose();
				}
				cmd.Dispose();
				conn.Close();
				conn.Dispose();
			}

			return events;
		}

		public Dictionary<string, string> getFlareFirewallFieldNames() {
			Dictionary<string, string> e = new Dictionary<string, string>();

			e["Group Name"] = "gn";
			e["Agent ID"] = "agent_id";
			e["Agent Version"] = "agent_version";
			e["Last Update Time"] = "last_update_time";
			e["Full Name"] = "full_name";
			e["Email"] = "email";
			e["Job Title"] = "job_title";
			e["Department"] = "department";
			e["Office Phone"] = "office_phone";
			e["Mobile Phone"] = "mobile_phone";
			e["Infected"] = "infected";
			e["Last Scan Time"] = "last_scan_time";
			e["Last Virus Time"] = "last_virus_time";
			e["Last Download Time"] = "last_download_time";

			e["AV Engine on/off"] = "avengine_onoff";
			e["Tamper on/off"] = "tamper_onoff";
			e["Firewall on/off"] = "firewall_onoff";
			e["Reboot Required"] = "reboot_required";
			e["Reboot Reason"] = "reboot_reason";
			e["Computer Name"] = "computer_name";
			e["Computer Domain Name"] = "computer_domain_name";
			e["Operating System"] = "operating_system";
			e["Service Pack"] = "service_pack";
			e["Current Login User"] = "current_login_user";
			e["Current Login Domain"] = "current_login_domain";
			e["DNS Server 1"] = "dns_server1";
			e["DNS Server 2"] = "dns_server2";
			e["DHCP Server"] = "dhcp_server";
			e["MAC Address 1"] = "mac_addr1";
			e["IP Address 1"] = "ip_addr1";
			e["Gateway 1"] = "gateway1";
			e["Subnet Mask 1"] = "subnet_mask1";

			e["Local IP"] = "local_ip";
			e["Remote IP"] = "remote_ip";
			e["Protocol"] = "protocol";
			e["Traffic Type"] = "traffic_type";
			e["Remote Hostname"] = "remote_hostname";
			e["Local Port"] = "local_port";
			e["Remote Port"] = "remote_port";
			e["Direction"] = "direction";
			e["Repetition"] = "repetition";
			e["Action Taken"] = "action_taken";
			e["App Name"] = "app_name";
			e["Alert"] = "alert";
			e["Rule Name"] = "rule_name";
			e["Location"] = "location";

			return e;
		}
	}


}