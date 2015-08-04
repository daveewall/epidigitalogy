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

		List<Dictionary<string, string>> getFlareUserControlDetail(string groupName, DateTime startDate, DateTime endDate);
		Dictionary<string, string> getFlareUserControlFieldNames();

		List<Dictionary<string, string>> getFlareUpdatesDetail(string groupName, DateTime startDate, DateTime endDate);
		Dictionary<string, string> getFlareUpdatesFieldNames();

		List<Dictionary<string, string>> getFlareFirewallDetail(string groupName, DateTime startDate, DateTime endDate);
		Dictionary<string, string> getFlareFirewallFieldNames();
	}
}
