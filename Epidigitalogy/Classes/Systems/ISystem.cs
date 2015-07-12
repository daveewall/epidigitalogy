using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using epidigitalogy.Classes.Events;

namespace epidigitalogy.Classes.Systems
{
	interface ISystem
	{
		Host getHostInfo(string id);
		string getSQLMain(string feeds);
		Log getDetailedLog(string id, DateTime startDate, DateTime endDate);
		
		List<Dictionary<string, string>> getFlareAntiVirusDetail(string groupName, DateTime startDate, DateTime endDate);
		Dictionary<string, string> getFlareAntiVirusFieldNames();

		List<Dictionary<string, string>> getFlareIPSDetail(string groupName, DateTime startDate, DateTime endDate);
		Dictionary<string, string> getFlareIPSFieldNames();

		List<Dictionary<string, string>> getFlareDownloadDetail(string groupName, DateTime startDate, DateTime endDate);
		Dictionary<string, string> getFlareDownloadFieldNames();
	}
}
