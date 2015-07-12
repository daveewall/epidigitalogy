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
				case "Kaseya":
                    sql =   
                        @"	SELECT	groupName
                            FROM	dbo.machGroup
	                        ORDER	BY groupName ASC";
					strConn = ConfigurationManager.ConnectionStrings["Kaseya"].ConnectionString;
					break;
				case "Symantec":
                    sql =
                        @"  SELECT	DISTINCT [NAME] as groupName
	                        FROM	[dbo].[IDENTITY_MAP]
	                        WHERE	[TYPE] = 'SemClientGroup'
	                        ORDER	BY [name]";
					strConn = ConfigurationManager.ConnectionStrings["Symantec"].ConnectionString;
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

	}
}
