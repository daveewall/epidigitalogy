using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace epidigitalogy.Classes.Events
{
	public class FirewallEvent : Event
	{
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
		public string app_name;
		public string rule_name;
		public bool alert;			// Whether the system created an alert
		public string action_taken;
	}
}