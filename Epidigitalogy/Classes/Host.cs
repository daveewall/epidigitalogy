using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using epidigitalogy.Classes.Events;

namespace epidigitalogy.Classes
{
	public class Host
	{
		public string id;		// agentGuid
		public string gn;		// GroupName
		public string hn;		// Hostname
		public string os;		// Operating System
		public DateTime lci;	// Last Check In DateTime
		public string error;
		public List<Feed> feeds;
		public List<Event> events;

		public string agent_id;
		public string agent_version;
		public string computer_id;
		public DateTime? last_update_time;
		public DateTime? last_scan_time;
		public DateTime? last_virus_time;
		public DateTime? last_download_time;

		public string full_name;
		public string email;
		public string job_title;
		public string department;
		public string office_phone;
		public string mobile_phone;

		public bool infected;
		public bool avengine_onoff;
		public bool tamper_onoff;
		public bool firewall_onoff;
		public bool reboot_required;
		public string reboot_reason;

		public string computer_name;
		public string computer_domain_name;
		public string service_pack;
		public string current_login_user;
		public string current_login_domain;
		public long dns_server1;
		public long dns_server2;
		public long dhcp_server;
		public string mac_addr1;
		public long ip_addr1;
		public long gateway1;
		public long subnet_mask1;
		public string mac_addr2;
		public long ip_addr2;
		public long gateway2;
		public long subnet_mask2;
		public string mac_addr3;
		public long ip_addr3;
		public long gateway3;
		public long subnet_mask3;
		public string mac_addr4;
		public long ip_addr4;
		public long gateway4;
		public long subnet_mask4;

		public Host() {
			events = new List<Event>();
			feeds = new List<Feed>();
		}
	}
}