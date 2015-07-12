using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace epidigitalogy.Classes.Events
{
	public class DownloadEvent : Event
	{
		public string app_name;
		public string app_company;
		public string app_version;
		public string app_md5;
		public string app_sha2;
		public string signer;
		public long filesize;
		public string url;
		public long last_modify_time;
	}
}