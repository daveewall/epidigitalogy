using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;
using epidigitalogy.Classes;

namespace epidigitalogy.ws
{
	/// <summary>
	/// Summary description for MiniGraphs
	/// </summary>
	[ScriptService]
	[System.ComponentModel.ToolboxItem(false)]
	// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
	// [System.Web.Script.Services.ScriptService]
	public class Graphs : System.Web.Services.WebService
	{

		[WebMethod]
		public List<Total> GetAlertCountsByGroup(string system) {
			string sql = null;
			string strConn = "";
			List<Total> result = new List<Total>();

			switch (system) {
				case "Kaseya":
					// BUG -- Doesn't work yet
					sql =
						@"  SELECT	a.groupName AS [group], count(*) AS group_count
							FROM	(
									SELECT	eventTime AS et, a.reverseName AS groupName
									FROM	[dbo].[AVLogHistory] h
											INNER JOIN vAgentsAll a ON h.agentGUID = a.AgentGuid	-- Filters out non-existant machines
									WHERE	EventType = 1000	-- CodeText = ThreatDetected

									UNION ALL

									SELECT	t.[TimeStamp] AS et, a.reverseName AS groupName
									FROM	kam.KamThreatIncidents t
											INNER JOIN vAgentsAll a ON t.agentGUID = a.AgentGuid	-- Filters out non-existant machines

									UNION ALL

									SELECT	DetectedTime AS et, a.reverseName AS groupName
									FROM	[kav].[vThreats] t
											INNER JOIN vAgentsAll a ON t.agentGUID = a.AgentGuid	-- Filters out non-existant machines
									) a
							GROUP	BY groupName
							ORDER	BY groupName ASC;";
					strConn = ConfigurationManager.ConnectionStrings["Kaseya"].ConnectionString;
					break;
				case "Symantec":
					sql =
						@"  SELECT	name AS [group], count(*) AS group_count
							FROM	V_ALERTS
									INNER JOIN IDENTITY_MAP ON ID = CLIENTGROUP_IDX
							GROUP	BY name
							ORDER	BY name ASC";
					strConn = ConfigurationManager.ConnectionStrings["Symantec"].ConnectionString;
					break;
			}

			SqlConnection conn = null;
			SqlCommand cmd = null;
			SqlDataReader rdr = null;

			try {
				conn = new SqlConnection(strConn);
				conn.Open();
				cmd = new SqlCommand(sql, conn);
				cmd.CommandType = System.Data.CommandType.Text;
				rdr = cmd.ExecuteReader();

				while (rdr.Read()) {
					Total t = new Total(null, (string)rdr[0], (int)rdr[1]);
					result.Add(t);
				}

			} catch (Exception e) {
				throw e;
			} finally {
				rdr.Close();
				cmd.Dispose();
			}

			return result;
		}

		[WebMethod]
		public List<Total> GetAlertCountsByDay(string system) {
			string sql = null;
			string strConn = "";
			List<Total> result = new List<Total>();

			switch (system) {
				case "Kaseya":
					sql =
						@"  SELECT	DATEADD(dd, DATEDIFF(dd, 0, a.et), 0) AS date, count(*) AS date_count
							FROM	(
									SELECT	eventTime AS et, a.reverseName AS groupName
									FROM	[dbo].[AVLogHistory] h
											INNER JOIN vAgentsAll a ON h.agentGUID = a.AgentGuid	-- Filters out non-existant machines
									WHERE	EventType = 1000	-- CodeText = ThreatDetected

									UNION ALL

									SELECT	t.[TimeStamp] AS et, a.reverseName AS groupName
									FROM	kam.KamThreatIncidents t
											INNER JOIN vAgentsAll a ON t.agentGUID = a.AgentGuid	-- Filters out non-existant machines

									UNION ALL

									SELECT	DetectedTime AS et, a.reverseName AS groupName
									FROM	[kav].[vThreats] t
											INNER JOIN vAgentsAll a ON t.agentGUID = a.AgentGuid	-- Filters out non-existant machines
									) a
							GROUP	BY DATEADD(dd, DATEDIFF(dd, 0, a.et), 0)
							ORDER	BY date DESC";
					strConn = ConfigurationManager.ConnectionStrings["Kaseya"].ConnectionString;
					break;
				case "Symantec":
					sql =
						@"  SELECT	DATEADD(dd, DATEDIFF(dd, 0, ALERTDATETIME), 0) AS date,
									count(*) AS date_count
							FROM	V_ALERTS
									INNER JOIN IDENTITY_MAP ON ID = CLIENTGROUP_IDX
							GROUP	BY DATEADD(dd, DATEDIFF(dd, 0, ALERTDATETIME), 0)
							ORDER	BY date DESC";
					strConn = ConfigurationManager.ConnectionStrings["Symantec"].ConnectionString;
					break;
			}

			SqlConnection conn = null;
			SqlCommand cmd = null;
			SqlDataReader rdr = null;

			try {
				conn = new SqlConnection(strConn);
				conn.Open();
				cmd = new SqlCommand(sql, conn);
				cmd.CommandType = System.Data.CommandType.Text;
				rdr = cmd.ExecuteReader();

				DateTime dt = DateTime.MaxValue;
				while (rdr.Read()) {
					Total t;
					if (dt != DateTime.MaxValue || (int)rdr[1] != 0) {
						if (dt == DateTime.MaxValue) {
							dt = (DateTime)rdr[0];
						}
						while (((DateTime)rdr[0]).Date < (dt = dt.AddDays(-1).Date)) {
							t = new Total(dt, null, 0);
							result.Add(t);
						}
						t = new Total((DateTime)rdr[0], null, (int)rdr[1]);
						result.Add(t);
						dt = dt.AddDays(-1);
					}
					if (result.Count > 365) {
						break;
					}
				}

				result.Reverse();

			} catch (Exception e) {
				throw e;
			} finally {
				rdr.Close();
				cmd.Dispose();
			}

			return result;
		}

	}
}
