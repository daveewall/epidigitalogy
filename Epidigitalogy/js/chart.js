var svgMain;
var svgDetails;
var blockHeight = 12, blockWidth = 3;
var hostHeight = blockHeight + 2;

function plot() {
	var minDate = new Date($("#startDate").val());
	var maxDate = new Date($("#endDate").val());

	var interval = maxDate - minDate;
	var xAxisHeight = 75;

	var margin = { top: 40, right: 40, bottom: 40, left: 110 },
		width = $("#chart").width(),
		height = Object.keys(indices).length * hostHeight + xAxisHeight;

	var x = d3.time.scale()
		.domain([minDate, maxDate])
		.range([0, width - margin.left - margin.right]);

	var map = data.hosts.map(function (d) {
		return d.id;
	});
	map.reverse();
	
	var y = d3.scale.ordinal()
		.domain(map)
		.rangeBands([height - margin.top - 20, 2, 0]);

	var xAxis = d3.svg.axis()
		.scale(x)
		.orient('top')
		.ticks((interval / 1000 / 60 / 60 / 24) <= 14 ? d3.time.day : 14)
		.tickFormat(d3.time.format('%_m/%e'))
		.tickSize(4)
		.tickPadding(2);

	var yAxis = d3.svg.axis()
		.scale(y)
		.orient('left')
		.ticks(Object.keys(hostNames).length)
		.tickFormat(function (d, i) {
			return hostNames[d].replace(/^.*[\\\/]/, '');
		})
		.tickSize(6, 100)
		.tickPadding(2);

	var tip = d3.tip()
		.attr('class', 'd3-tip')
		.offset([-15, 0])
		.html(function (d) {
			return "Host: <span style='color:red'>" + hostNames[d.id] + "</span>";
		});

	// create the zoom listener
	var zoom = d3.behavior.zoom()
		.x(x)
		.scaleExtent([1, 10])
		.on("zoom", zoomed);

	svgMain = d3.select('#chart').append('svg')
		.attr('class', 'chart')
		.attr('width', width)
		.attr('height', height)
		.append('g')
		.attr("id", "container")
		.attr('transform', 'translate(' + margin.left + ', ' + margin.top + ')')
		.call(zoom);

	var rect = svgMain.append("rect")
		.attr("width", width)
		.attr("height", height)
		.style("fill", "none")
		.style("pointer-events", "all");

	var container = svgMain.append("g");

	svgMain.call(tip);

//	var tickPadding = 2;
/*	svgMain.append("rect")
		.attr("class", "overlay")
		.attr("transform", "translate(" + -tickPadding + ",0)")
		.attr("width", width + (tickPadding * 2))
		.attr("height", height)
		.attr("style", "fill-opacity:0.1;opacity:0;z-index:99;")
//		.on("mouseover", mouseover)
////		.on("mouseout", mouseout)
//		.on("mousemove", mousemove);
*/
//	svgMain.on("mouseenter", function () { EnterMicroscopeX1(this); });
//	svgMain.on("mousemove", function () { MoveMicroscopeX1(this); });
//	svgMain.on("mouseleave", function () { RemoveMicroscopeX1(this); });

	// Map the events
	container
		.attr("class", "epiinfo")
		.selectAll('rect')
		.data($.map(data.hosts, function (h, i) {
			return h.events;
		}))
		.enter()
			.append('rect')
			.attr({
				'class': function (d) { return d.et; },
				'width': blockWidth,
				'height': blockHeight,
				'x': function (d) {
					return x(d.ed);
				},
				'y': function (d) {
					return y(d.id);		// BUG -- This really should be looking at the parent node, so we don't need the ID in each event, but I'm too lazy to figure that out now.
				},
				'onclick': "javascript:clearHostDetails();showMachineDetails(this);"
			})
			.on("mouseover", tip.show)
			.on("mouseout", tip.hide);

	// Draw X-axis grid lines
	var grid = svgMain.selectAll("line.x")
		.data(x.ticks(10))
		.enter()
			.append("line")
			.attr({
				"class": "gridline",
				"x1": function (d) {
					return x(d);
				},
				"x2": function (d) {
					return x(d);
				},
				"y1": 0,
				"y2": height - margin.top - margin.bottom / 2
			});

	var xaxis = svgMain.append('g')
		.attr('class', 'x axis')
		.call(xAxis);

	var yaxis = svgMain.append('g')
		.attr('class', 'y axis')
		.call(yAxis);

	svgMain.selectAll('rect')
		.style("opacity", 1);
/*
	var rect = svgMain.append("rect")
		.attr("width", width - $("g.y.axis").width())
		.attr("height", height)
		.style("fill", "none")
		.style("pointer-events", "all");
*/
	zoom(svgMain);

	// function for handling zoom event
	function zoomed() {
		svgMain.select("g.x.axis").call(xAxis);

		// zoom the rectangles, but don't zoom y-axis... also, fix the width of the rectangles so they remain the same between zooms.
		d3.selectAll(".epiinfo rect")
			.attr("transform", "translate(" + d3.event.translate[0] + ",0)scale(" + d3.event.scale + ", 1)")
			.attr("width", blockWidth / d3.event.scale);
	
		// Remove gridlines and recreate them.
		// BUG -- this is probably automatically calculated by d3, but couldn't figure it out easily.  Quick hack.
		svgMain.selectAll("#container .gridline").remove();

		grid = svgMain.selectAll("line.x")
			.data(x.ticks(10))
			.enter()
				.append("line")
				.attr({
					"class": "gridline",
					"x1": function (d) {
						return x(d);
					},
					"x2": function (d) {
						return x(d);
					},
					"y1": 0,
					"y2": height - margin.top - margin.bottom / 2
				});
	}

	// Adjust tick names to center between ticks
//	svgMain.selectAll(".axis text")
//		.attr("dy", blockHeight - 2);

/*
	function EnterMicroscopeX1(d) {
		var position = d3.mouse(svgMain.node());
		var chart = svgMain.append("rect")
			.attr("class", "pathogenAlert")
			.attr("x", 0)
			.attr("y", position[1] - 2)
			.attr('width', window.innerWidth)
			.attr('height', blockHeight)
			.style("stroke", "grey")
			.style("fill", "none");
	}

	function MoveMicroscopeX1(d) {
		var position = d3.mouse(svgMain.node());
		var highlightTarget = d3.selectAll('.pathogenAlert');

		highlightTarget.attr("y", position[1] - 2)
			.style("stroke", 2)
			.style("fill", 0);
	}

	function RemoveMicroscopeX1() {
		d3.selectAll('.pathogenAlert').remove();
	}

	function mouseout() {
		d3.selectAll('.highlight').remove();
	}

	function mousemove() {
		var computerNumber = Math.floor(y.invert(d3.mouse(this)[1]));
		var highlightTarget = d3.selectAll('.highlight');
		console.log(computerNumber);
//		console.log(y(computerNumber));
		highlightTarget.attr("y", y(computerNumber))
			.style("stroke", 2)
			.style("fill", 0);
	}

	function mouseover() {
		var computerNumber = Math.floor(y.invert(d3.mouse(this)[1]));
		var chart = svgMain.append("rect")
			.attr("class", "highlight")
			.attr("x", 0)
			.attr("y", y(computerNumber))
			.attr('width', this.innerWidth)
			.attr('height', blockHeight)
			.style("stroke", "grey")
			.style("fill", "none");
	}
*/
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
//		width = $("#details").dialog("option", "width") - 30,
		width = $("#chartDetails").innerWidth(),
		height = dataDetails.d.feeds.length * 30 + 50;

	var x = d3.time.scale()
		.domain([minDate, maxDate])
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
//		.tickFormat(d3.time.format('%_m/%e'))
		.tickSize(5)
		.tickPadding(8);

	var yAxis = d3.svg.axis()
		.scale(y)
		.tickValues(feeds)
		.orient('left')
		.tickPadding(4);

	var tipDetail = d3.tip()
		.attr('class', 'd3-tip')
		.offset([-15, 0])
		.html(getToolTipContent);

	// create the zoom listener
	var zoom = d3.behavior.zoom()
		.x(x)
		.scaleExtent([1, 50])
		.on("zoom", zoomed);

	var svgDetails = d3.select('#chartDetails').append('svg')
		.attr('class', 'chart')
		.attr('width', width)
		.attr('height', height)
		.append('g')
		.attr('transform', 'translate(' + margin.left + ', ' + margin.top + ')')
		.call(zoom);

	var rect = svgDetails.append("rect")
		.attr("width", width)
		.attr("height", height)
		.style("fill", "none")
		.style("pointer-events", "all");

	var container = svgDetails.append("g");

	svgMain.call(tipDetail);

	svgDetails.append("text")      // text label for the y axis
		.attr("x", -79)
		.attr("y", 120)
		.style("font-size", 12)
		.append("a")
		.attr("xlink:href", '/Alerts_legend.htm')
		.attr("target", "_blank");

	container
		.attr("class", "epiDetails")
		.selectAll('rect')
		.data($.map(dataDetails.d.feeds, function (el, i) {
			return el.events;
		}))
		.enter()
			.append('rect')
			.attr({
				'class': function (d) {
					return d.action_taken;
				},
				'x': function (d) {
					return x(d.ed);
				},
				'y': function (d) {
					return y(d.feed);
				},
				'width': 2,
				'height': 30,
				'class': getDetailClass
			})
			.on("mouseover", tipDetail.show)
			.on("mouseout", tipDetail.hide);
	
	svgDetails.append('g')
	.attr('class', 'x axis')
	.attr('transform', 'translate(0)')
	.call(xAxis);

	svgDetails.append('g')
		.attr('class', 'y axis')
		.call(yAxis);
	
	// Draw X-axis grid lines
	var grid = svgDetails.selectAll("line.x")
		.data(x.ticks(10))
		.enter()
			.append("line")
			.attr({
				"class": "gridline",
				"x1": function (d) {
					return x(d);
				},
				"x2": function (d) {
					return x(d);
				},
				"y1": 0,
				"y2": height - margin.top - margin.bottom / 2
			});

	svgDetails.selectAll('rect')
		.style("opacity", 1);


	zoom(svgDetails);

	// function for handling zoom event
	function zoomed() {
		svgDetails.select("g.x.axis").call(xAxis);

		// zoom the rectangles, but don't zoom y-axis... also, fix the width of the rectangles so they remain the same between zooms.
		d3.selectAll(".epiDetails rect")
			.attr("transform", "translate(" + d3.event.translate[0] + ",0)scale(" + d3.event.scale + ", 1)")
			.attr("width", blockWidth / d3.event.scale);

		// Remove gridlines and recreate them.
		// BUG -- this is probably automatically calculated by d3, but couldn't figure it out easily.  Quick hack.
		svgDetails.selectAll(".gridline").remove();

		grid = svgDetails.selectAll("line.x")
			.data(x.ticks(10))
			.enter()
				.append("line")
				.attr({
					"class": "gridline",
					"x1": function (d) {
						return x(d);
					},
					"x2": function (d) {
						return x(d);
					},
					"y1": 0,
					"y2": height - margin.top - margin.bottom / 2
				});
	}

	function getDetailClass(d) {
		switch (d.feed) {
			case "Firewall":
				if (d.action_taken == "blocked") {
					return "F";
				} else {
					return "f";
				}
				break;
			case "IPS":
				if (d.severity < 8) {
					return "I";
				} else {
					return "i";
				}
				break;
			case "Downloads":
				if (d.signer == null) {
					return "D";
				} else {
					return "d";
				}
				break;
			case "AV Engine":
				if (d.action_taken == "Deleted") {
					return "v";
				}
				if (d.action_taken == "Quarantined") {
					return "v";
				}
				if (d.action_taken == "AVDel") {
					return "v";
				}
				if (d.action_taken == "Left_alone") {
					return "V";
				}
				if (d.action_taken == "Cleaned") {
					return "v";
				}
				if (d.action_taken == "Cleaned or macros deleted") {
					return "v";
				}
				if (d.action_taken == "Saved") {
					return "v";
				}
				if (d.action_taken == "Moved Back") {
					return "V";
				}
				if (d.action_taken == "Renamed back") {
					return "V";
				}
				if (d.action_taken == "Undone") {
					return "V";
				}
				if (d.action_taken == "Bad") {
					return "V";
				}
				if (d.action_taken == "Backed up") {
					return "v";
				}
				if (d.action_taken == "Cleaned or macros deleted") {
					return "v";
				}
				if (d.action_taken == "Pending repair") {
					return "V";
				}
				if (d.action_taken == "Partially repaired") {
					return "V";
				}
				if (d.action_taken == "Process termination pending restart") {
					return "v";
				}
				if (d.action_taken == "Excluded") {
					return "v";
				}
				if (d.action_taken == "Restart processing") {
					return "v";
				}
				if (d.action_taken == "Cleaned_by_deletion") {
					return "v";
				}
				if (d.action_taken == "Access denied") {
					return "V";
				}
				if (d.action_taken == "Process terminated") {
					return "v";
				}
				if (d.action_taken == "No repair available") {
					return "V";
				}
				if (d.action_taken == "All actions failed") {
					return "V";
				}
				if (d.action_taken == "Suspicious") {
					return "V";
				}
				if (d.action_taken == "Details pending") {
					return "V";
				}
				if (d.action_taken == "Detected by using the commercial application list") {
					return "V";
				}
				if (d.action_taken == "Forced detection by using the file name") {
					return "V";
				}
				if (d.action_taken == "Forced detection by using the file hash") {
					return "V";
				}
				if (d.action_taken == "Not applicable") {
					return "V";
				}
				break;
			case "Virus Updates":
				return "U";
				break;
			case "User Control":
				if (d.action_taken == "continue") {
					return "C";
				}
				if (d.action_taken == "allow") {
					return "c";
				}
				if (d.action_taken == "block") {
					return "C";
				}
				if (d.action_taken == "ask") {
					return "C";
				}
				if (d.action_taken == "terminate") {
					return "C";
				}
				break;
			default:
		}
	}
}

function showHostDetails(h) {
	if (h.current_login_domain != null) {
		$("#userName").html(h.current_login_domain + "\\" + h.current_login_user);
	} else {
		$("#userName").html(h.current_login_user);
	}
	$("#name").html(h.full_name || "n/a");
	$("#lastScanTime").html(dateToString(h.last_scan_time));
	$("#title").html(h.job_title || "n/a");
	$("#lastDownloadTime").html(dateToString(h.last_download_time));
	$("#department").html(h.department || "n/a");
	$("#os").html(h.os + " " + h.service_pack);
	$("#email").html(h.email || "n/a");
	$("#agentVersion").html(h.agent_version);
	$("#officePhone").html(h.office_phone || "n/a");
	$("#lastVirusTime").html(dateToString(h.last_virus_time));
	$("#mobilePhone").html(h.mobile_phone || "n/a");
	$("#ipAddr1").html(long2ip(h.ip_addr1));
	$("#subnetMask1").html(long2ip(h.subnet_mask1));
	$("#gateway1").html(long2ip(h.gateway1));
	$("#dnsServer1").html(long2ip(h.dns_server1));
	$("#dnsServer2").html(long2ip(h.dns_server2));
	$("#dhcpServer").html(long2ip(h.dhcp_server));
}

function clearHostDetails(h) {
	$("#userName").html("");
	$("#name").html("");
	$("#lastScanTime").html("");
	$("#title").html("");
	$("#lastDownloadTime").html("");
	$("#department").html("");
	$("#os").html("");
	$("#email").html("");
	$("#agentVersion").html("");
	$("#officePhone").html("");
	$("#lastVirusTime").html("");
	$("#mobilePhone").html("");
	$("#ipAddr1").html("");
	$("#subnetMask1").html("");
	$("#gateway1").html("");
	$("#dnsServer1").html("");
	$("#dnsServer2").html("");
	$("#dhcpServer").html("");
}

//function showEventDetails(event) {
//	$(event).tooltip({
////		"content":	getToolTipContent(event),
//		"option":	"show"
//	});
//}

//function hideEventDetails(event) {
//	$(event).tooltip( "option", "hide" );
//}

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
