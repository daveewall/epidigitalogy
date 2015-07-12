using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace epidigitalogy.Classes.Events
{
	public class IPSEvent : Event
	{
		public int severity;

		public string traffic_type;
		public long local_ip;
		public long remote_ip;
		public string remote_hostname;
		public int local_port;
		public int remote_port;
		public string direction;
		public int repetition;
		public string protocol;
		public string app_name;
		public string location;
		public bool alert;			// Whether the system created an alert
		public string intrusion_url;
		public string intrusion_payload_url;

	}
}