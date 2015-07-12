using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using epidigitalogy.Classes.Events;

namespace epidigitalogy.Classes
{
	public class Feed
	{
		public string name;
		public List<Event> events;
		public string error;

		public Feed() {
			events = new List<Event>();
		}
	}
}