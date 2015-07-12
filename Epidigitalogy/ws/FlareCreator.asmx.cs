using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
using epidigitalogy.Classes;
using epidigitalogy.Classes.Events;
using epidigitalogy.Classes.Systems;
using Newtonsoft.Json;

namespace epidigitalogy.ws
{
	/// <summary>
	/// Summary description for Flare
	/// </summary>
	[XmlInclude(typeof(Feed))]
	[XmlInclude(typeof(VirusEvent))]
	[XmlInclude(typeof(IPSEvent))]
	[XmlInclude(typeof(UserControlEvent))]
	[XmlInclude(typeof(DownloadEvent))]
	[XmlInclude(typeof(FirewallEvent))]
	[XmlInclude(typeof(Event))]
	[ScriptService]
	[System.ComponentModel.ToolboxItem(false)]
	// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
	// [System.Web.Script.Services.ScriptService]
	public class FlareCreator : System.Web.Services.WebService
	{

		[WebMethod]
		public string GetFlare(string system, string feed, string groupName, DateTime startDate, DateTime endDate) {
			ISystem sys = (ISystem)Activator.CreateInstance(Type.GetType("epidigitalogy.Classes.Systems." + system));
			switch (feed) {
				case "av":
					return JsonConvert.SerializeObject(sys.getFlareAntiVirusDetail(groupName, startDate, endDate));
				case "ips":
					return JsonConvert.SerializeObject(sys.getFlareIPSDetail(groupName, startDate, endDate));
				case "uc":
					break;
				case "dl":
					return JsonConvert.SerializeObject(sys.getFlareDownloadDetail(groupName, startDate, endDate));
				case "fw":
					break;
				case "up":
					break;
				default:
					break;
			}
			return null;
		}

		[WebMethod]
		public string GetFlareFieldNames(string system, string feed) {
			ISystem sys = (ISystem)Activator.CreateInstance(Type.GetType("epidigitalogy.Classes.Systems." + system));
			switch (feed) {
				case "av":
					return JsonConvert.SerializeObject(sys.getFlareAntiVirusFieldNames());
				case "ips":
					return JsonConvert.SerializeObject(sys.getFlareIPSFieldNames());
				case "uc":
					break;
				case "dl":
					return JsonConvert.SerializeObject(sys.getFlareDownloadFieldNames());
				case "fw":
					break;
				case "up":
					break;
				default:
					break;
			}
			return null;
		}
	}
}
