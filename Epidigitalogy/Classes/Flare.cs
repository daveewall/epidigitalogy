using System;
using System.Collections;
using System.Linq;
using System.Web;

namespace epidigitalogy.Classes
{
	public class Flare
	{
		public string name = null;
		public string parent = null;
		public int val = 0;
		public int depth = 0;
		public ArrayList children;

		public Flare() {
		}

		public Flare(string name, string parent, int val, int depth) {
			this.name = name;
			this.parent = parent;
			this.val = val;
			this.depth = depth;
			this.children = new ArrayList();
		}
	}
}