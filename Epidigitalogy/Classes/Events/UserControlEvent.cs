using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace epidigitalogy.Classes.Events
{
	public class UserControlEvent : Event
	{
		// User Control
		public DateTime begin_time;
//		public DateTime end_time;	-- Not used?
		public int severity;
//		public string severity_string;
		public string action;		// What the system (or user) is trying to do
		public string rule_name;	// Firewall, User, Virus
		public string caller_process;
		public string parameter;
		public bool alert;			// Whether the system created an alert
		public string user_action;	// What the user tried to do, not what the system did
		public string user_name;	// UC, et al.
		public string domain_name;	// UC, et al.
		public string action_taken;

	}
}