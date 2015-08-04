function long2ip(ip) {
	//  discuss at: http://phpjs.org/functions/long2ip/
	// original by: Waldo Malqui Silva
	//   example 1: long2ip( 3221234342 );
	//   returns 1: '192.0.34.166'

	if (!isFinite(ip))
		return false;

	return [ip >>> 24, ip >>> 16 & 0xFF, ip >>> 8 & 0xFF, ip & 0xFF].join('.');
}

function dateToString(date) {
	if (date) {
		var ms = date.getMilliseconds();
		if (ms <= 9)
			ms = "00" + ms;
		else if (ms <= 99)
			ms = "0" + ms;

		var s = date.getSeconds();
		if (s <= 9) s = "0" + s;

		var m = date.getMinutes();
		if (m <= 9) m = "0" + m;

		var h = date.getHours();
		if (h <= 9) h = "0" + h;

		var d = date.getDate();

		var mo = date.getMonth() + 1;

		var y = date.getFullYear();

		return mo + "/" + d + "/" + y + " " + h + ":" + m + ":" + s + "." + ms;
	} else {
		return "";
	}
}


function getToolTipContent(event) {
	if (event.hasOwnProperty("key")) return event.key;

	var text = "<table cellpadding='2'><tr><td>Event Date:</td><td> <span style='color:red'>" + dateToString(event.ed) + "</span></td></tr>";

	if (event.et) {
		text += "<tr><td>Event Type:</td><td><span style='color:red'>" + event.et + "</span></td></tr>";
	}
	if (event.source) {
		text += "<tr><td>Source:</td><td><span style='color:red'>" + event.source + "</span></td></tr>";
	}
	if (event.action_taken) {
		text += "<tr><td>Action Taken:</td><td><span style='color:red'>" + event.action_taken + "</span></td></tr>";
	}
	if (event.user_name) {
		var user = event.user_name;
		if (event.domain_name) {
			user = event.domain_name + '\\' + user;
		}
		text += "<tr><td>User:</td><td><span style='color:red'>" + user + "</span></td></tr>";
	}
	if (event.app_name) {
		text += "<tr><td>Application:</td><td><span style='color:red'>" + event.app_name + "</span></td></tr>";
	}
	if (event.app_company) {
		text += "<tr><td>App Company:</td><td><span style='color:red'>" + event.app_company + "</span></td></tr>";
	}
	if (event.app_version) {
		text += "<tr><td>App Version:</td><td><span style='color:red'>" + event.app_version + "</span></td></tr>";
	}
	if (event.description) {
		text += "<tr><td>Description:</td><td><span style='color:red'>" + event.description + "</span></td></tr>";
	}
	if (event.app_md5) {
		text += "<tr><td>App MD5:</td><td><span style='color:red'>" + event.app_md5 + "</span></td></tr>";
	}
	if (event.app_sha2 && event.app_sha2 != 0) {
		text += "<tr><td>App SHA2:</td><td><span style='color:red'>" + event.app_sha2 + "</span></td></tr>";
	}
	if (event.signer) {
		text += "<tr><td>Signer:</td><td><span style='color:red'>" + event.signer + "</span></td></tr>";
	}
	if (event.last_modify_time) {
		text += "<tr><td>Last Modify Time:</td><td><span style='color:red'>" + dateToString(new Date(event.last_modify_time)) + "</span></td></tr>";
	}
	if (event.confidence) {
		text += "<tr><td>Confidence:</td><td><span style='color:red'>" + event.confidence + "</span></td></tr>";
	}
	if (event.detection_score) {
		text += "<tr><td>Detect Score:</td><td><span style='color:red'>" + event.detection_score + "</span></td></tr>";
	}
	if (event.virus_name) {
		text += "<tr><td>Virus Name:</td><td><span style='color:red'>" + event.virus_name + "</span></td></tr>";
	}
	if (event.user_action) {
		text += "<tr><td>User Action:</td><td><span style='color:red'>" + event.user_action + "</span></td></tr>";
	}
	if (event.filepath && event.filepath != "Unavailable") {
		text += "<tr><td>File Path:</td><td><span style='color:red'>" + event.filepath + "</span></td></tr>";
	}
	if (event.filesize) {
		text += "<tr><td>File Size:</td><td><span style='color:red'>" + event.filesize + "</span></td></tr>";
	}
	if (event.caller_process) {
		text += "<tr><td nowrap='nowrap'>Caller Process:</td><td><span style='color:red'>" + event.caller_process + "</span></td></tr>";
	}
	if (event.parameter) {
		text += "<tr><td>Parameter:</td><td><span style='color:red'>" + event.parameter + "</span></td></tr>";
	}
	if (event.rule_name) {
		text += "<tr><td>Rule Name:</td><td><span style='color:red'>" + event.rule_name + "</span></td></tr>";
	}
	if (event.protocol) {
		text += "<tr><td>Protocol:</td><td><span style='color:red'>" + event.protocol + "</span></td></tr>";
	}
	if (event.traffic_type) {
		text += "<tr><td>Traffic Type:</td><td><span style='color:red'>" + event.traffic_type + "</span></td></tr>";
	}
	if (event.direction) {
		text += "<tr><td>Direction:</td><td><span style='color:red'>" + event.direction + "</span></td></tr>";
	}
	if (event.local_ip != null) {
		text += "<tr><td>Local IP/port:</td><td><span style='color:red'>" + long2ip(event.local_ip);
		if (event.local_port) {
			text += ":" + event.local_port;
		}
		text += "</span></td></tr>";
	}
	if (event.remote_ip != null) {
		text += "<tr><td>Remote IP/port:</td><td><span style='color:red'>" + long2ip(event.remote_ip);
		if (event.remote_port) {
			text += ":" + event.remote_port;
		}
		text += "</span></td></tr>";
	}
	if (event.remote_hostname) {
		text += "<tr><td>Remote Hostname:</td><td><span style='color:red'>" + event.remote_hostname + "</span></td></tr>";
	}
	if (event.intrusion_url) {
		text += "<tr><td>Intrusion URL:</td><td><span style='color:red'>" + event.intrusion_url + "</span></td></tr>";
	}
	if (event.intrusion_payload_url) {
		text += "<tr><td>Intrusion Payload:</td><td><span style='color:red'>" + event.intrusion_payload_url + "</span></td></tr>";
	}
	if (event.db_version) {
		text += "<tr><td>Version:</td><td><span style='color:red'>" + event.db_version + "</span></td></tr>";
	}
	if (event.whitelist_reason) {
		text += "<tr><td>Whitelist:</td><td><span style='color:red'>" + event.whitelist_reason + "</span></td></tr>";
	}
	text += "</table>";

	return text;

}
