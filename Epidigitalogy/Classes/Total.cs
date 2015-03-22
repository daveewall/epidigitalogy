using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace epidigitalogy.Classes
{
	public class Total
	{
		public DateTime? date;
		public string group;
		public long total;

		public Total() {
		}

		public Total(DateTime? date, string group, long total) {
			this.date = date;
			this.group = group;
			this.total = total;
		}
	}
}