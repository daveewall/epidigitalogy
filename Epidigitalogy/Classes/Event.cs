using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace epidigitalogy
{
	public class Event
	{
		public string id;		// agentGuid (because of a JS issue I haven't yet figured out in d3)
		public DateTime ed;		// Event Date
		public string et;		// Event Type

		// Common
		public string description;
		public string user_name;
		public string domain_name;
		public string feed;
		public string db_version;

		// User Control
		public DateTime begin_time;
		public DateTime end_time;
		public int severity;
		public string severity_string;
		public string action;		// What the system (or user) is trying to do
		public string rule_name;	// Firewall, User, Virus
		public string caller_process;
		public string parameter;
		public bool alert;			// Whether the system created an alert
		public string user_action;	// What the user tried to do, not what the system did
		
		// Virus Detection Details
		public long ip;
		public string filepath;
		public string action_taken;	// What the system did with the issue
		public string source;
		public string app_name;
		public string app_company;
		public string app_version;
		public long filesize;
		public string app_md5;
		public string app_sha2;
        public string signer;

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

		// Firewall Logging
		public string traffic_type;
		public string protocol;
		public long local_ip;
		public long remote_ip;
		public string remote_hostname;
		public int local_port;
		public int remote_port;
		public string direction;
		public int repetition;
		public string location;
	
		// IPS
		public string intrusion_url;
		public string intrusion_payload_url;


		public Event() {
		}

		public Event(string ID, DateTime EventDate, string EventType) {
			id = ID;
			ed = EventDate;
			et = EventType;
		}
	}
}