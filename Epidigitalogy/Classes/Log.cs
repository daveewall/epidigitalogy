using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace epidigitalogy.Classes
{
	public class Log
	{
		public List<Host> hosts;
		public List<Feed> feeds;
		public DateTime mindt;
		public DateTime maxdt;

		public Log() {
			hosts = new List<Host>();
			feeds = new List<Feed>();
		}

	}
}