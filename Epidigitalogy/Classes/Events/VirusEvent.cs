using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace epidigitalogy.Classes.Events
{
	public class VirusEvent : Event
	{
		// Virus Detection Details
		public long ip;
		public string filepath;
		public string action_taken;	// What the system did with the issue
		public string source;
		public string app_name;
		public string app_company;
		public string app_version;
		public string app_md5;
		public string app_sha2;
		public string signer;
		public long filesize;

		// -1 = 'default'
		// 0 = 'Trojan worm'
		// 1 = 'Trojan worm'
		// 2 = 'Key logger'
		// 100 = 'Remote control'
		public int app_type;
		public string virus_name;
		public int sensitivity;
		public int detection_score;
		public string truscan_engine_version;
		public int submission_recommendation;

		// 0 = 'Not on the permitted application list' 
		// 100 = 'Symantec permitted application list' 
		// 101 = 'Administrator permitted application list' 
		// 102 = 'User permitted application list'
		public int whitelist_reason;
		public int disposition;	// 127 if there's no data to compare to

		// >= 100 THEN 'Extremely High'
		// >= 65 THEN 'High'
		// >= 25 THEN 'Medium'
		// >= 10 THEN 'Low'
		// >= 1 THEN 'Symantec knows very little about the file/unknown.'
		public int confidence;
		
		// >= 201 THEN 'Very High'
		// >= 151 THEN 'High'
		// >= 101 THEN 'Medium'
		// >= 51 THEN 'Low'
		// >= 1 THEN 'Very Low'
		public int prevalence;
		public string url;
		public string web_domain;
		public string downloader;
		public int cids_onoff;

		// 0 = 'unknown'
		// 1 = 'Low'
		// 2 = 'Low'
		// 3 = 'Medium'
		// 4 = 'High'
		public int risk_level;

	}
}