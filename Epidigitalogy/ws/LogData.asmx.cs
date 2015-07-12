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
using epidigitalogy.Classes.Events;
using epidigitalogy.Classes.Systems;

namespace epidigitalogy.ws
{
	[ScriptService]
	public class LogData : System.Web.Services.WebService {
		[WebMethod]
		public Log GetLog(string system, string feeds, string groupName, DateTime startDate, DateTime endDate) {
			Log log = new Log();

			string sSQL;
			string sConn;
			SqlConnection conn = new SqlConnection();
			SqlCommand cmd = new SqlCommand();
			SqlDataReader rdr = null;
			DateTime mindt = DateTime.MaxValue;
			DateTime maxdt = DateTime.MinValue;

			ISystem sys = (ISystem)Activator.CreateInstance(Type.GetType("epidigitalogy.Classes.Systems." + system));
			sSQL = sys.getSQLMain(feeds);
			sConn = ConfigurationManager.ConnectionStrings[system].ConnectionString;

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
					if (rdr.GetValue(0) == DBNull.Value) {
						continue;
					}
					if ((string)rdr.GetValue(0) != prevAgentGuid) {
						h = new Host();
						h.id = (string)rdr["id"];
						h.hn = (string)rdr["name"];
						h.gn = (string)rdr["grp"];
						if (rdr["ts"] != DBNull.Value) {
							h.lci = (DateTime)rdr["ts"];
						}

						log.hosts.Add(h);

						prevAgentGuid = (string)rdr.GetValue(0);
					}

					if (rdr["ts"] != DBNull.Value && rdr["et"] != DBNull.Value) {
						h.events.Add(new Event((string)rdr["id"], (DateTime)rdr["ts"], (string)rdr["et"]));
					}

					if (rdr["ts"] != DBNull.Value) {
						if ((DateTime)rdr["ts"] < mindt) {
							mindt = (DateTime)rdr["ts"];
						}

						if ((DateTime)rdr["ts"] > maxdt) {
							maxdt = (DateTime)rdr["ts"];
						}
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
			DateTime mindt = DateTime.MaxValue;
			DateTime maxdt = DateTime.MinValue;
			
			ISystem sys = (ISystem)Activator.CreateInstance(Type.GetType("epidigitalogy.Classes.Systems." + system));
			Log log = sys.getDetailedLog(id, startDate, endDate);
			
			log.hosts.Add(sys.getHostInfo(id));

			return log;
		}
	
	}
}
