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

namespace epidigitalogy
{
	[ScriptService]
	public class Population : System.Web.Services.WebService
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="system">Which system to query from.  Kaseya, Symantec, others (in the future).</param>
		/// <returns></returns>
		[WebMethod]
		public List<string> PopulateGroups(string system) {
            string sql = null;
			string strConn = "";
			List<string> groups = new List<string>();

			switch (system) {
				case "Symantec":
                    sql =
                        @"  SELECT	DISTINCT [NAME] as groupName
	                        FROM	[dbo].[IDENTITY_MAP]
	                        WHERE	[TYPE] = 'SemClientGroup'
	                        ORDER	BY [name]";
					strConn = ConfigurationManager.ConnectionStrings["sqlSymantec"].ConnectionString;
					break;
			}

			try {
				SqlConnection conn = new SqlConnection(strConn);
				conn.Open();
				SqlCommand cmd = new SqlCommand(sql, conn);
				cmd.CommandType = System.Data.CommandType.Text;
				SqlDataReader rdr = cmd.ExecuteReader();

				groups = (from IDataRecord r in rdr
						  select (string)r["groupName"]
						  ).ToList();

				rdr.Close();
				cmd.Dispose();

				return groups;
			} catch (Exception e) {
				throw e;
			}
		}

		[WebMethod]
		public List<Total> GetAlertCountsByGroup(string system) {
			string sql = null;
			string strConn = "";
			List<Total> result = new List<Total>();
			
			switch (system) {
				case "Symantec":
					sql =
						@"  SELECT	name AS [group], count(*) AS group_count
							FROM	V_ALERTS
									INNER JOIN IDENTITY_MAP ON ID = CLIENTGROUP_IDX
							GROUP	BY name
							ORDER	BY name ASC";
					strConn = ConfigurationManager.ConnectionStrings["sqlSymantec"].ConnectionString;
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
				case "Symantec":
					sql =
						@"  SELECT	DATEADD(dd, DATEDIFF(dd, 0, ALERTDATETIME), 0) AS date,
									count(*) AS date_count
							FROM	V_ALERTS
									INNER JOIN IDENTITY_MAP ON ID = CLIENTGROUP_IDX
							GROUP	BY DATEADD(dd, DATEDIFF(dd, 0, ALERTDATETIME), 0)
							ORDER	BY date DESC";
					strConn = ConfigurationManager.ConnectionStrings["sqlSymantec"].ConnectionString;
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
						while (((DateTime)rdr[0]).Date < dt.AddDays(-1).Date) {
							dt = dt.AddDays(-1);
							t = new Total(dt, null, 0);
							result.Add(t);
						}
						t = new Total((DateTime)rdr[0], null, (int)rdr[1]);
						result.Add(t);
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
