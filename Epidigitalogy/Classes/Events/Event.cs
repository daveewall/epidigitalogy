using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace epidigitalogy.Classes.Events
{
	public class Event
	{
		// Common Event Fields
		public string id;		// agentGuid
		public DateTime ed;		// Event Date
		public string et;		// Event Type
		public string feed;
		public string description;

		public Event() {
		}

		public Event(string ID, DateTime EventDate, string EventType) {
			id = ID;
			ed = EventDate;
			et = EventType;
		}
	}
}