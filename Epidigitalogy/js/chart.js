var svgMain;
var svgDetails;

function plot() {
	var minDate = new Date(data["mindt"]);
	var maxDate = new Date(data["maxdt"]);
	var interval = maxDate - minDate;

	var margin = { top: 40, right: 40, bottom: 40, left: 40 },
		width = window.innerWidth - 100,
		height = Object.keys(indices).length * 5 + 75;

	var x = d3.time.scale()
		.domain([minDate, d3.time.day.offset(maxDate, 1)])
		.range([0, width - margin.left - margin.right]);

	var y = d3.scale.linear()
		.domain([d3.max(data.hosts, function (d) { return indices[d.id]; }), 0])
		.range([height - margin.top - margin.bottom, 0]);

	var xAxis = d3.svg.axis()
		.scale(x)
		.orient('top')
		.ticks((interval / 1000 / 60 / 60 / 24) <= 14 ? d3.time.day : 14)
		.tickFormat(d3.time.format('%_m/%e'))
		.tickSize(5)
		.tickPadding(8);

	var yAxis = d3.svg.axis()
		.scale(y)
		.orient('left')
		.ticks(Object.keys(indices).length / 10 <= 1 ? 1 : Object.keys(indices).length / 10)
//		.tickFormat(d3.time.format(',1d'))
		.tickPadding(4);

	svgMain = d3.select('#chart').append('svg')
		.attr('class', 'chart')
		.attr('width', width)
		.attr('height', height)
		.append('g')
		.attr('transform', 'translate(' + margin.left + ', ' + margin.top + ')');

	svgMain.on("mouseenter", function () { EnterMicroscopeX1(); });
	svgMain.on("mousemove", function () { MoveMicroscopeX1(); });
	svgMain.on("mouseleave", function () { RemoveMicroscopeX1(); });

	// Map the events
	svgMain.selectAll('.chart')
		.data($.map(data.hosts, function (el, i) {
			return el.events;
		}))
		.enter().append('rect')
		.attr('class', function (d) { return d.et; })
		.attr('x', function (d) {
			return x(d.ed);
		})
		.attr('y', function (d) {
			return y(indices[d.id]);		// BUG -- This really should be looking at the parent node, but I'm too lazy to figure that out now.
		})
		.attr('width', 3)
		.attr('height', 3)
		.attr('onclick', 'javascript:showMachineDetails(this)');

	svgMain.append('g')
		.attr('class', 'x axis')
		.attr('transform', 'translate(0)')
		.call(xAxis);

	svgMain.append('g')
		.attr('class', 'y axis')
		.call(yAxis);

	svgMain.selectAll('rect')
		.style("opacity", 1);
}


function plotDetails(dataDetails) {
	showHostDetails(dataDetails.d.hosts[0]);

	var minDate = new Date(dataDetails.d.mindt);
	var maxDate = new Date(dataDetails.d.maxdt);
	var interval = maxDate - minDate;

	// Sort IPS by severity
	dataDetails.d.feeds[1].events.sort(function (a, b) {
		return a.severity < b.severity;
	});

	var margin = { top: 40, right: 40, bottom: 40, left: 120 },
		width = $("#details").dialog("option", "width") - 30,
		height = dataDetails.d.feeds.length * 30 + 50;

	var x = d3.time.scale()
		.domain([minDate, d3.time.day.offset(maxDate, 1)])
		.rangeRound([0, width - margin.left - margin.right]);

	var feeds = [];
	dataDetails.d.feeds.forEach(function (f) {
		feeds.push(f.name);
	});

	var y = d3.scale.ordinal()
		.domain(feeds)
		.rangeRoundBands([0, height - 50]);

	var xAxis = d3.svg.axis()
		.scale(x)
		.orient('top')
		.ticks((interval / 1000 / 60 / 60 / 24) <= 14 ? d3.time.day : 14)
		.tickFormat(d3.time.format('%_m/%e'))
		.tickSize(5)
		.tickPadding(8);

	var yAxis = d3.svg.axis()
		.scale(y)
		.tickValues(feeds)
		.orient('left')
		.tickPadding(4);

	var svgDetails = d3.select('#chartDetails').append('svg')
		.attr('class', 'chart')
		.attr('width', width)
		.attr('height', height)
		.append('g')
		.attr('transform', 'translate(' + margin.left + ', ' + margin.top + ')');

	svgDetails.append('g')
		.attr('class', 'x axis')
		.attr('transform', 'translate(0)')
		.call(xAxis);

	svgDetails.append('g')
		.attr('class', 'y axis')
		.call(yAxis);

	svgDetails.append("text")      // text label for the y axis
		.attr("x", -79)
		.attr("y", 120)
		.style("font-size", 12)
		.append("a")
		.attr("xlink:href", '/Alerts_legend.htm')
		.attr("target", "_blank");

	svgDetails.selectAll('.chart')
		.data($.map(dataDetails.d.feeds, function (el, i) {
			return el.events;
		}))
		.enter().append('rect')
		.attr('class', function (d) { return d.action_taken; })
		.attr('x', function (d) {
			return x(d.ed);
		})
		.attr('y', function (d) {
			return y(d.feed);
		})
		.attr('width', 2)
		.attr('height', 30)
//		.attr('onclick', 'javascript:showMachineDetails(this)');
//		.attr('onmouseover', 'javascript:setOpacity(this)')
//		.attr('onmouseleave', 'javascript:setOpacityNormal()');
		.attr('class', function (d) {
		    switch (d.feed) {
		        case "Firewall":
		            if (d.action_taken == "blocked") {
		                return "FWBlocked";
		            }
		            if (d.direction == "Outbound") {
		                return "FWOutbound";
		            }
		            if (d.direction == "Inbound") {
		                return "FWInbound";
		            }
					break;
		        case "IPS":
		            if (d.direction == "Inbound") {
		                return "IPSIN";
		            }
		            if (d.direction == "Outbound") {
		                return "IPSOUT";
		            }
		            if (d.direction == "Unknown") {
		                return "IPSunknown";
		            }
					if (d.severity < 4) {
						return "severity-critical";
					} else if (d.severity < 8) {
						return "severity-high";
					} else if (d.severity < 12) {
						return "severity-medium";
					} else if (d.severity <= 15) {
						return "severity-low";
					}
					break;
		        case "Downloads":
		            if (d.signer != null) {
		                return "DSign1";
		            }
		            else {
		                return "DSign0";
		            }

					break;
			    case "AV Engine":
			        if (d.action_taken == "Quarantined") {
			            return "Quarantined";
			        }
			        if (d.action_taken == "AVDel") {
			            return "Deleted";
			        }
			        if (d.action_taken == "Left_alone") {
                        return "AVLeft_alone";
			        }
			        if (d.action_taken == "Cleaned") {
			            return "AVClean";
			        }
			        if (d.action_taken == "Cleaned or macros deleted") {
			            return "AVCleanDel";
			        }
			        if (d.action_taken == "Saved") {
			            return "Saved";
			        }
			        if (d.action_taken == "Moved Back") {
			            return "AVMove";
			        }
			        if (d.action_taken == "Renamed back") {
			            return "AVRename";
			        }
			        if (d.action_taken == "Undone") {
			            return "AVUndone";
			        }
			        if (d.action_taken == "Bad") {
			            return "AVBad";
			        }
			        if (d.action_taken == "Backed up") {
			            return "AVBackedUp";
			        }
			        if (d.action_taken == "Cleaned or macros deleted") {
			            return "AVCleanMac";
			        }
			        if (d.action_taken == "Pending repair") {
			            return "AVPending";
			        }
			        if (d.action_taken == "Partially repaired") {
			            return "AVPartRepair";
			        }
			        if (d.action_taken == "Process termination pending restart") {
			            return "AVProcTermRestart";
			        }
			        if (d.action_taken == "Excluded") {
			            return "AVExcluded";
			        }
			        if (d.action_taken == "Restart processing") {
			            return "AVRestart";
			        }
			        if (d.action_taken == "Cleaned_by_deletion") {
			            return "AVCleanedDeletion";
			        }
			        if (d.action_taken == "Access denied") {
			            return "Accessdenied";
			        }
			        if (d.action_taken == "Process terminated") {
			            return "Processterminated";
			        }
			        if (d.action_taken == "No repair available") {
			            return "No repair available";
			        }
			        if (d.action_taken == "All actions failed") {
			            return "AllActionsFailed";
			        }
			        if (d.action_taken == "Suspicious") {
			            return "Suspicious";
			        }
			        if (d.action_taken == "Details pending") {
			            return "DetailsPending";
			        }
			        if (d.action_taken == "Detected by using the commercial application list") {
			            return "OnCommercialAppList";
			        }
			        if (d.action_taken == "Forced detection by using the file name") {
			            return "ForcedDetectionbyFileName";
			        }
			        if (d.action_taken == "Forced detection by using the file hash") {
			            return "ForcedDetectionbyFilehash";
			        }
			        if (d.action_taken == "Not applicable") {
			            return "NotApplicable";
			        }
					break;
		        case "Virus Updates":
					break;
		        case "User Control":
		            if (d.action_taken == "continue") {
		                return "UCcontinue";
		            }
		            if (d.action_taken == "allow") {
		                return "UCallow";
		            }
		            if (d.action_taken == "block") {
		                return "UCblock";
		            }
		            if (d.action_taken == "ask") {
		                return "UCask";
		            }
		            if (d.action_taken == "terminate") {
		                return "UCterminate";
		            }
					break;
				default:
			}
		})
		.append("svg:title")
		.html(getToolTipContent)
		.style("fill", "#FF00FF");

//	$(".severity-medium").toFront();
//	$(".severity-high").toFront();
//	$(".severity-critical").toFront();

	svgDetails.selectAll('rect')
		.style("opacity", 1);
}

function showHostDetails(h) {
	if (h.current_login_domain != null) {
		$("#userName").html(h.current_login_domain + "\\" + h.current_login_user);
	} else {
		$("#userName").html(h.current_login_user);
	}
	$("#name").html(h.full_name);
	$("#lastScanTime").html(dateToString(h.last_scan_time));
	$("#title").html(h.job_title);
	$("#lastDownloadTime").html(dateToString(h.last_download_time));
	$("#department").html(h.department);
	$("#os").html(h.os + " " + h.service_pack);
	$("#email").html(h.email);
	$("#agentVersion").html(h.agent_version);
	$("#officePhone").html(h.office_phone);
	$("#lastVirusTime").html(dateToString(h.last_virus_time));
	$("#mobilePhone").html(h.mobile_phone);
	$("#ipAddr1").html(long2ip(h.ip_addr1));
	$("#subnetMask1").html(long2ip(h.subnet_mask1));
	$("#gateway1").html(long2ip(h.gateway1));
	$("#dnsServer1").html(long2ip(h.dns_server1));
	$("#dnsServer2").html(long2ip(h.dns_server2));
	$("#dhcpServer").html(long2ip(h.dhcp_server));
}

function showEventDetails(event) {
	$(event).tooltip({
//		"content":	getToolTipContent(event),
		"option":	"show"
	});
}

function hideEventDetails(event) {
	$(event).tooltip( "option", "hide" );
}

function getToolTipContent(event) {
	var text = "Event Date: " + dateToString(event.ed);

	if (event.et) {
		text += "\nEvent Type: " + event.et;
	}
	if (event.source) {
		text += "\nSource: " + event.source;
	}
	if (event.action_taken) {
		text += "\nAction Taken: " + event.action_taken;
	}
	if (event.user_name) {
		var user = event.user_name;
		if (event.domain_name) {
			user = event.domain_name + '\\' + user;
		}
		text += "\nUser: " + user;
	}
	if (event.app_name) {
		text += "\nApplication: " + event.app_name;
	}
	if (event.app_company) {
		text += "\nApp Company: " + event.app_company;
	}
	if (event.app_version) {
		text += "\nApp Version: " + event.app_version;
	}
	if (event.description) {
		text += "\nDescription: " + event.description;
	}
	if (event.app_md5) {
		text += "\nApp MD5: " + event.app_md5;
	}
	if (event.app_sha2 && event.app_sha2 != 0) {
		text += "\nApp SHA2: " + event.app_sha2;
	}
	if (event.signer) {
	    text = "\nSigner: " + event.signer;
	}
	if (event.confidence) {
		text += "\nConfidence: " + event.confidence;
	}
	if (event.detection_score) {
		text += "\nDetect Score: " + event.detection_score;
	}
	if (event.virus_name) {
		text += "\nVirus Name: " + event.virus_name;
	}
	if (event.user_action) {
		text += "\nUser Action: " + event.user_action;
	}
	if (event.filepath && event.filepath != "Unavailable") {
		text += "\nFile Path: " + event.filepath;
	}
	if (event.filesize) {
		text += "\nFile Size: " + event.filesize;
	}
	if (event.caller_process) {
		text += "\nCaller Process: " + event.caller_process;
	}
	if (event.parameter) {
		text += "\nParameter: " + event.parameter;
	}
	if (event.rule_name) {
		text += "\nRule Name: " + event.rule_name;
	}
	if (event.protocol) {
		text += "\nProtocol: " + event.protocol;
	}
	if (event.traffic_type) {
		text += "\nTraffic Type: " + event.traffic_type;
	}
	if (event.direction) {
		text += "\nDirection: " + event.direction;
	}
	if (event.local_ip) {
		text += "\nLocal IP/port: " + long2ip(event.local_ip);
		if (event.local_port) {
			text += ":" + event.local_port;
		}
	}
	if (event.remote_ip) {
		text += "\nRemote IP/port: " + long2ip(event.remote_ip);
		if (event.remote_port) {
			text += ":" + event.remote_port;
		}
	}
	if (event.remote_hostname) {
		text += "\nRemote Hostname: " + event.remote_hostname;
	}
	if (event.intrusion_url) {
		text += "\nIntrusion URL: " + event.intrusion_url;
	}
	if (event.intrusion_payload_url) {
		text += "\nIntrusion Payload: " + event.intrusion_payload_url;
	}
	if (event.db_version) {
		text += "\nVersion: " + event.db_version;
	}

	return text;

}

function setOpacityNormal() {
	svgMain.selectAll('rect')
		.style("opacity", 1);
}

function setOpacity(obj) {
	svgMain.selectAll('rect')
		.style("opacity", function (o) {
			return o.id == obj.__data__.id ? 1 : .2;
		});
}

function EnterMicroscopeX1() {
	var position = d3.mouse(svgMain.node());
	var chart = svgMain.append("rect")
		.attr("class", "pathogenAlert")
		.attr("x", 0)
		.attr("y", position[1] - 2)
		.attr('width', window.innerWidth)
		.attr('height', 5)
		.style("stroke", "grey")
        .style("fill", "none");
}

function MoveMicroscopeX1() {
	var position = d3.mouse(svgMain.node());
    var highlightTarget = d3.selectAll('.pathogenAlert');

	highlightTarget.attr("y", position[1] - 2)
		.style("stroke", 2)
		.style("fill", 0);
}

function RemoveMicroscopeX1() {
    d3.selectAll('.pathogenAlert').remove();
}
