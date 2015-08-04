var reload = true;
var mainData = null;
var filter = [];

$(document).ready(function () {
	systemChanged();
	$("#startDate").datepicker();
	$("#endDate").datepicker();

	// Set defaults for dates
	var startDate = new Date();
	startDate.setDate(startDate.getDate() - 7);
	var endDate = new Date();
	endDate.setDate(endDate.getDate() + 1);
	endDate.setHours(0);
	endDate.setMinutes(0);
	endDate.setSeconds(0);
	endDate.setMilliseconds(0);
	$("#startDate").datepicker("setDate", startDate);
	$("#endDate").datepicker("setDate", endDate);

	$("#system").change(updateReload);
	$("#feed").change(updateReload);
	$("#groupName").change(updateReload);
	$("#startDate").change(updateReload);
	$("#endDate").change(updateReload);
});

function updateReload() {
	reload = true;
}

function resetFilter() {
	reload = true;
	filter = [];
}

function systemChanged() {
	populateFeeds();
	populateGroups();
	feedChanged();
}

function presetSelected(preset) {
	var endDate = new Date();
	endDate.setDate(endDate.getDate() + 1);
	endDate.setHours(0);
	endDate.setMinutes(0);
	endDate.setSeconds(0);
	endDate.setMilliseconds(0);

	var numDays1 = new Date(endDate.getTime());
	var numDays7 = new Date(endDate.getTime());
	numDays1.setDate(numDays1.getDate() - 1);
	numDays7.setDate(numDays7.getDate() - 7);

	switch (preset) {
		case "av-action-virus-computer":
			$("#feed").val("av");
			$("#endDate").datepicker("setDate", endDate);
			$("#startDate").datepicker("setDate", numDays7);
			$("#cat1").val("action_taken");
			$("#cat2").val("virus_name");
			$("#cat3").val("computer_name");
			break;
		case "av-infected-virus-computer":
			$("#feed").val("av");
			$("#endDate").datepicker("setDate", endDate);
			$("#startDate").datepicker("setDate", numDays7);
			$("#cat1").val("infected");
			$("#cat2").val("virus_name");
			$("#cat3").val("computer_name");
			break;
		case "av-action-computer-virus":
			$("#feed").val("av");
			$("#endDate").datepicker("setDate", endDate);
			$("#startDate").datepicker("setDate", numDays7);
			$("#cat1").val("action_taken");
			$("#cat2").val("computer_name");
			$("#cat3").val("virus_name");
			break;
		case "av-computer-virus-action":
			$("#feed").val("av");
			$("#endDate").datepicker("setDate", endDate);
			$("#startDate").datepicker("setDate", numDays7);
			$("#cat1").val("computer_name");
			$("#cat2").val("virus_name");
			$("#cat3").val("action_taken");
			break;
		case "av-infected-os-virus":
			$("#feed").val("av");
			$("#endDate").datepicker("setDate", endDate);
			$("#startDate").datepicker("setDate", numDays7);
			$("#cat1").val("infected");
			$("#cat2").val("operating_system");
			$("#cat3").val("virus_name");
			break;
		case "av-virus-app-computer":
			$("#feed").val("av");
			$("#endDate").datepicker("setDate", endDate);
			$("#startDate").datepicker("setDate", numDays7);
			$("#cat1").val("virus_name");
			$("#cat2").val("app_name");
			$("#cat3").val("computer_name");
			break;
		case "av-virus-os-computer":
			$("#feed").val("av");
			$("#endDate").datepicker("setDate", endDate);
			$("#startDate").datepicker("setDate", numDays7);
			$("#cat1").val("virus_name");
			$("#cat2").val("operating_system");
			$("#cat3").val("computer_name");
			break;
		case "av-group-virus-computer":
			$("#feed").val("av");
			$("#endDate").datepicker("setDate", endDate);
			$("#startDate").datepicker("setDate", numDays7);
			$("#cat1").val("gn");
			$("#cat2").val("virus_name");
			$("#cat3").val("computer_name");
			break;
		case "downloads-appcompany-appname-computer":
			$("#feed").val("dl");
			$("#endDate").datepicker("setDate", endDate);
			$("#startDate").datepicker("setDate", numDays1);
			$("#cat1").val("app_company");
			$("#cat2").val("app_name");
			$("#cat3").val("computer_name");
			break;
		case "downloads-app-computer-user":
			$("#feed").val("dl");
			$("#endDate").datepicker("setDate", endDate);
			$("#startDate").datepicker("setDate", numDays1);
			$("#cat1").val("app_company");
			$("#cat2").val("computer_name");
			$("#cat3").val("current_login_user");
			break;
		case "downloads-infected-app-computer":
			$("#feed").val("dl");
			$("#endDate").datepicker("setDate", endDate);
			$("#startDate").datepicker("setDate", numDays1);
			$("#cat1").val("infected");
			$("#cat2").val("app_name");
			$("#cat3").val("computer_name");
			break;
		case "downloads-os-appcompany-appname":
			$("#feed").val("dl");
			$("#endDate").datepicker("setDate", endDate);
			$("#startDate").datepicker("setDate", numDays1);
			$("#cat1").val("operating_system");
			$("#cat2").val("app_company");
			$("#cat3").val("app_name");
			break;
		case "downloads-signer-appname-computer":
			$("#feed").val("dl");
			$("#endDate").datepicker("setDate", endDate);
			$("#startDate").datepicker("setDate", numDays1);
			$("#cat1").val("signer");
			$("#cat2").val("app_name");
			$("#cat3").val("computer_name");
			break;
		case "downloads-user-appname-desc":
			$("#feed").val("dl");
			$("#endDate").datepicker("setDate", endDate);
			$("#startDate").datepicker("setDate", numDays1);
			$("#cat1").val("current_login_user");
			$("#cat2").val("app_name");
			$("#cat3").val("description");
			break;
		case "downloads-computer-appname-description":
			$("#feed").val("dl");
			$("#endDate").datepicker("setDate", endDate);
			$("#startDate").datepicker("setDate", numDays1);
			$("#cat1").val("computer_name");
			$("#cat2").val("app_name");
			$("#cat3").val("current_login_user");
			break;
		case "downloads-gateway-appname-computer":
			$("#feed").val("dl");
			$("#endDate").datepicker("setDate", endDate);
			$("#startDate").datepicker("setDate", numDays1);
			$("#cat1").val("gateway1");
			$("#cat2").val("app_name");
			$("#cat3").val("computer_name");
			break;
		case "downloads-group-computer-appname":
			$("#feed").val("dl");
			$("#endDate").datepicker("setDate", endDate);
			$("#startDate").datepicker("setDate", numDays1);
			$("#cat1").val("gn");
			$("#cat2").val("computer_name");
			$("#cat3").val("app_name");
			break;
		case "ips-desc-computer-user":
			$("#feed").val("ips");
			$("#endDate").datepicker("setDate", endDate);
			$("#startDate").datepicker("setDate", numDays7);
			$("#cat1").val("description");
			$("#cat2").val("computer_name");
			$("#cat3").val("current_login_user");
			break;
		case "ips-computer-desc-user":
			$("#feed").val("ips");
			$("#endDate").datepicker("setDate", endDate);
			$("#startDate").datepicker("setDate", numDays7);
			$("#cat1").val("computer_name");
			$("#cat2").val("description");
			$("#cat3").val("current_login_user");
			break;
		case "ips-group-computer-desc":
			$("#feed").val("ips");
			$("#endDate").datepicker("setDate", endDate);
			$("#startDate").datepicker("setDate", numDays7);
			$("#cat1").val("gn");
			$("#cat2").val("computer_name");
			$("#cat3").val("description");
			break;
		case "ips-direction-computer-desc":
			$("#feed").val("ips");
			$("#endDate").datepicker("setDate", endDate);
			$("#startDate").datepicker("setDate", numDays7);
			$("#cat1").val("direction");
			$("#cat2").val("computer_name");
			$("#cat3").val("description");
			break;
		case "ips-os-computer-desc":
			$("#feed").val("ips");
			$("#endDate").datepicker("setDate", endDate);
			$("#startDate").datepicker("setDate", numDays7);
			$("#cat1").val("operating_system");
			$("#cat2").val("computer_name");
			$("#cat3").val("description");
			break;
		case "ips-protocol-computer-desc":
			$("#feed").val("ips");
			$("#endDate").datepicker("setDate", endDate);
			$("#startDate").datepicker("setDate", numDays7);
			$("#cat1").val("protocol");
			$("#cat2").val("computer_name");
			$("#cat3").val("description");
			break;
		case "ips-repetition-computer-desc":
			$("#feed").val("ips");
			$("#endDate").datepicker("setDate", endDate);
			$("#startDate").datepicker("setDate", numDays7);
			$("#cat1").val("repetition");
			$("#cat2").val("computer_name");
			$("#cat3").val("description");
			break;
		case "user-rule-computer-process":
			$("#feed").val("uc");
			$("#endDate").datepicker("setDate", endDate);
			$("#startDate").datepicker("setDate", numDays1);
			$("#cat1").val("rule_name");
			$("#cat2").val("computer_name");
			$("#cat3").val("caller_process_name");
			break;
		case "user-process-rule-computer":
			$("#feed").val("uc");
			$("#endDate").datepicker("setDate", endDate);
			$("#startDate").datepicker("setDate", numDays1);
			$("#cat1").val("caller_process_name");
			$("#cat2").val("rule_name");
			$("#cat3").val("computer_name");
			break;
		case "user-computer-rule-process":
			$("#feed").val("uc");
			$("#endDate").datepicker("setDate", endDate);
			$("#startDate").datepicker("setDate", numDays1);
			$("#cat1").val("computer_name");
			$("#cat2").val("rule_name");
			$("#cat3").val("caller_process_name");
			break;
		case "updates-lastupdate-computername":
			$("#feed").val("up");
			$("#endDate").datepicker("setDate", endDate);
			$("#startDate").datepicker("setDate", numDays7);
			$("#cat1").val("last_update_time");
			$("#cat2").val("computer_name");
			$("#cat3").val("");
			break;
		case "updates-group-computer-lastupdate":
			$("#feed").val("up");
			$("#endDate").datepicker("setDate", endDate);
			$("#startDate").datepicker("setDate", numDays7);
			$("#cat1").val("gn");
			$("#cat2").val("computer_name");
			$("#cat3").val("last_update_time");
			break;
		case "fw-rule-computername-remoteip":
			$("#feed").val("fw");
			$("#endDate").datepicker("setDate", endDate);
			$("#startDate").datepicker("setDate", numDays1);
			$("#cat1").val("rule_name");
			$("#cat2").val("computer_name");
			$("#cat3").val("remote_ip");
			break;
		case "fw-infected-localip-remoteip":
			$("#feed").val("fw");
			$("#endDate").datepicker("setDate", endDate);
			$("#startDate").datepicker("setDate", numDays1);
			$("#cat1").val("infected");
			$("#cat2").val("local_ip");
			$("#cat3").val("remote_ip");
			break;
		case "fw-traffictype-computername-remoteip":
			$("#feed").val("fw");
			$("#endDate").datepicker("setDate", endDate);
			$("#startDate").datepicker("setDate", numDays1);
			$("#cat1").val("traffic_type");
			$("#cat2").val("computer_name");
			$("#cat3").val("remote_ip");
			break;
		case "fw-remoteport-computername-localport":
			$("#feed").val("fw");
			$("#endDate").datepicker("setDate", endDate);
			$("#startDate").datepicker("setDate", numDays1);
			$("#cat1").val("remote_port");
			$("#cat2").val("computer_name");
			$("#cat3").val("local_port");
			break;
		case "fw-direction-protocol-remoteport":
			$("#feed").val("fw");
			$("#endDate").datepicker("setDate", endDate);
			$("#startDate").datepicker("setDate", numDays1);
			$("#cat1").val("direction");
			$("#cat2").val("protocol");
			$("#cat3").val("remote_port");
			break;
		case "fw-computername-remoteip-remoteport":
			$("#feed").val("fw");
			$("#endDate").datepicker("setDate", endDate);
			$("#startDate").datepicker("setDate", numDays1);
			$("#cat1").val("computer_name");
			$("#cat2").val("remote_ip");
			$("#cat3").val("remote_port");
			break;
	}

	getFlare();
}

function updatePresets() {
	$("#preset").find("option").remove();
	$("#preset").append($("<option />").val(null).text("(Select One)"));
	switch ($("#feed").val()) {
		case "av":
			$("#preset").append($("<option />").val("av-infected-virus-computer").text("AV - Which computers are infected?"));
			$("#preset").append($("<option />").val("av-action-virus-computer").text("AV - Which computers downloaded viruses and what happened to them?"));
			$("#preset").append($("<option />").val("av-action-computer-virus").text("AV - Which machine had the most quarantined viruses?"));
			$("#preset").append($("<option />").val("av-computer-virus-action").text("AV - Which machine keeps getting hit by viruses?"));
			$("#preset").append($("<option />").val("av-infected-os-virus").text("AV - Which operating systems are getting infected the most?"));
			$("#preset").append($("<option />").val("av-virus-app-computer").text("AV - Which viruses are getting to computers the most?"));
			$("#preset").append($("<option />").val("av-virus-os-computer").text("AV - Which operating systems are getting hit by them?"));
			$("#preset").append($("<option />").val("av-group-virus-computer").text("AV - Which departments are getting hit the most?"));
			break;
		case "uc":
			$("#preset").append($("<option />").val("user-rule-computer-process").text("User Control - Which rules are being broken the most, and by whom?"));
			$("#preset").append($("<option />").val("user-process-rule-computer").text("User Control - Which processes are breaking the most rules?"));
			$("#preset").append($("<option />").val("user-computer-rule-process").text("User Control - Which computers are breaking the most rules?"));
			break;
		case "ips":
			$("#preset").append($("<option />").val("ips-desc-computer-user").text("IPS - Which computers/users violated specific IPS rules?"));
			$("#preset").append($("<option />").val("ips-computer-desc-user").text("IPS - Which computers violated the most rules?"));
			$("#preset").append($("<option />").val("ips-group-computer-desc").text("IPS - Which departments violate the most rules as a whole?"));
			$("#preset").append($("<option />").val("ips-direction-computer-desc").text("IPS - Which computers are sending (not receiving) infections?"));
			$("#preset").append($("<option />").val("ips-os-computer-desc").text("IPS - Which operating systems are causing the most pain?"));
			$("#preset").append($("<option />").val("ips-protocol-computer-desc").text("IPS - Which protocols are causing havoc?"));
			$("#preset").append($("<option />").val("ips-repetition-computer-desc").text("IPS - Which computers are getting it multiple times by the same infection?"));
			break;
		case "dl":
			$("#preset").append($("<option />").val("downloads-appcompany-appname-computer").text("Downloads - Which programs get downloaded the most?"));
			$("#preset").append($("<option />").val("downloads-app-computer-user").text("Downloads - Which users/computers also downloaded a specific infected file?"));
			$("#preset").append($("<option />").val("downloads-infected-app-computer").text("Downloads - Which computers downloaded infected files?"));
			$("#preset").append($("<option />").val("downloads-os-appcompany-appname").text("Downloads - Which files did different OSes download?"));
			$("#preset").append($("<option />").val("downloads-signer-appname-computer").text("Downloads - Which computers downloaded unsigned files?"));
			$("#preset").append($("<option />").val("downloads-user-appname-desc").text("Downloads - Which user downloaded the most?"));
			$("#preset").append($("<option />").val("downloads-computer-appname-description").text("Downloads - Which computers downloaded the most?"));
			$("#preset").append($("<option />").val("downloads-gateway-appname-computer").text("Downloads - Which subnets downloaded which files?"));
			$("#preset").append($("<option />").val("downloads-group-computer-appname").text("Downloads - Which departments download the most?"));
			break;
		case "fw":
			$("#preset").append($("<option />").val("fw-rule-computername-remoteip").text("Firewall - Which rules are getting violated the most?"));
			$("#preset").append($("<option />").val("fw-infected-localip-remoteip").text("Firewall - Where are infected machines attempting to communicate?"));
			$("#preset").append($("<option />").val("fw-traffictype-computername-remoteip").text("Firewall - Which type of traffic gets blocked the most?"));
			$("#preset").append($("<option />").val("fw-remoteport-computername-localport").text("Firewall - Which remote port gets blocked most often?"));
			$("#preset").append($("<option />").val("fw-direction-protocol-remoteport").text("Firewall - Which direction is the traffic going that gets blocked?"));
			$("#preset").append($("<option />").val("fw-computername-remoteip-remoteport").text("Firewall - Which machines are transmitting the most blocked packets?"));
			break;
		case "up":
			$("#preset").append($("<option />").val("updates-lastupdate-computername").text("Updates - Which computers aren't getting updated?"));
			$("#preset").append($("<option />").val("updates-group-computer-lastupdate").text("Updates - Which groups aren't using update nodes (GUPs) and are downloading directly from the server?"));
			break;
	}
}

function feedChanged() {
	showWaitDialog("Retrieving Categories...");

	$.ajax({
		type: "POST",
		contentType: "application/json; charset=utf-8",
		url: "ws/FlareCreator.asmx/GetFlareFieldNames",
		data: '{ "system": "' + $("#system").val() + '", "feed": "' + $("#feed").val() + '" }',
		dataType: "json",
		converters: {
			"text json": function (data) {
				return $.parseJSON(data, true);
			}
		},
		success: function (d) {
			var fields = JSON.parse(d.d);

			if (fields === null) {
				showMessage("There are no categories to select from.");
				fields = {};
			}

			$.each([1, 2, 3], function (j, catNumber) {
				$("#cat" + catNumber).find("option").remove();
				$("#cat" + catNumber).append($("<option />").val(null).text("(Select One)"));
				$.each(Object.keys(fields).sort(), function (i, name) {
					$("#cat" + catNumber).append($("<option />").val(fields[name]).text(name));
				});
			});

			updatePresets();
		},
		error: function (e) {
			handleError(e);
			updateReload();
		},
		complete: function () {
			hideWaitDialog();
		}
	});
}

function getFlare() {
	$("#chart").empty();

	if (reload) {
		showWaitDialog("Retrieving Logs...");

		$.ajax({
			type: "POST",
			contentType: "application/json; charset=utf-8",
			url: "ws/FlareCreator.asmx/GetFlare",
			data: '{ "system": "' + $("#system").val() + '", "feed": "' + $("#feed").val() + '", "groupName": "' + $("#groupName").val() + '", "startDate": "' + $("#startDate").val() + '", "endDate": "' + $("#endDate").val() + '" }',
			dataType: "json",
			converters: {
				"text json": function (data) {
					return $.parseJSON(data, true);
				}
			},
			success: function (d) {
				mainData = JSON.parse(d.d);

				if (mainData.length == 0) {
					showMessage("There is no data to display.");
				}

				nest(mainData);		// Create a nested data structure based on the categories selected.
				plot();

				$("#spanTotal").show();
				$("#spanTotal").text("Total Records: " + mainData.length);
				$("#searchResults").show();
				$("#chart").show();
			},
			error: function (e) {
				handleError(e);
				updateReload();
			},
			complete: function () {
				hideWaitDialog();
				reload = false;
			}
		});
	} else {
		nest(mainData);		// Create a nested data structure based on the categories selected, but keeping the existing removed nodes.
		plot();

		$("#spanTotal").show();
		$("#spanTotal").text("Total Records: " + nestedData.value);
		$("#searchResults").show();
		$("#chart").show();
	}
};

function nest(data) {
	var cat1 = $("#cat1").val();
	var cat2 = $("#cat2").val();
	var cat3 = $("#cat3").val();

	var nest = d3.nest();

	[cat1, cat2, cat3].forEach(function (key) {
		if (key != "") {
			nest.key(function (d) {
				return d[key];
			}).sortKeys(d3.ascending);
		}
	});
	var nested = nest
		.rollup(function (leaves) { return leaves.length; })
		.sortValues()
		.entries(data);

	$.each(filter, function (i, f) {
		pruneAll(nested, f);
	});

	nestedData = {
		key: "root",
		values: nested
	};

	return nestedData;
}

function plot() {
	$("#chart").empty();

	var chartType = $("#chartType").val();
	if (chartType == "sunburst") {
		plotSunburst(nestedData);
	} else if (chartType == "icicle") {
		plotIcicle(nestedData);
	}
}

function showHideText(checked, level) {
	if (checked) {
		if (chartType.value == "icicle") {
			d3.selectAll(".text" + level).style("opacity", function (d) {
				if ($("#showCat" + d.depth + "Text").is(':checked')) {
					return d.dx * $("#chart").height() > 12 ? 1 : 0;
				} else {
					return 0;
				}
			})
		} else {
			d3.selectAll(".text" + level).style({ "opacity": 1 });
		}
	} else {
		d3.selectAll(".text" + level).style({ "opacity": 0 });
	}
}

function select_colour(d) {
	if (d.depth == 0) { return "#ddd" };
	if (d.depth == 1) {
		d.color = color(d.key);
		return d.color;
	} else {
		if (d.parent) {
			var c = d3.hsl(d.parent.color),
				a = d3.hsl(c.h * 1.25, c.s, c.l),
				b = d3.hsl(c.h * .75, c.s, c.l);

			d.color = d3.hsl((a.h + b.h) / 2, a.s * 1.2, a.l / 1.2);
			return d.color;
		}
	}
}

function plotSunburst(data) {
	var diameter = 960,
		format = d3.format(",d"),
		width = $("#chart").innerWidth(),
		height = Math.min($("#chart").width(), $(window).height() - 135),
	    radius = Math.min(width, height) / 2 - 25;
		color = d3.scale.category20c();

	var x = d3.scale.linear()
		.range([0, 2 * Math.PI]);

	var y = d3.scale.linear()
		.range([0, radius]);

	var svg = d3.select("#chart").append("svg")
		.attr("width", width)
		.attr("height", height)
		.append("g")
		.attr("transform", "translate(" + (width / 2 + 10) + "," + (height / 2 + 10) + ")");	// BUG -- Make this the size of the surrounding div

	var partition = d3.layout.partition()
		.sort(null)
		.value(function (d) {
			return d.values ? d.values : 1;
		})
		.children(function (d) {
			return d.values ? d.values : null;
		});

	var arc = d3.svg.arc()
		.startAngle(function (d) { return Math.max(0, Math.min(2 * Math.PI, x(d.x))); })
		.endAngle(function (d) { return Math.max(0, Math.min(2 * Math.PI, x(d.x + d.dx))); })
		.innerRadius(function (d) { return Math.max(0, y(d.y)); })
		.outerRadius(function (d) { return Math.max(0, y(d.y + d.dy)); });

	var tipDetail = d3.tip()
		.attr('class', 'd3-tip')
		.offset([-15, 0])
		.html(getToolTipContent);

	var nodes = partition.nodes(data);

	var g = svg.selectAll("g")
		.data(nodes)
		.enter()
			.append("svg:g");

	svg.call(tipDetail);

	var nodeNumber = 0;

	var path = g.append("svg:path")
		.attr({
			"nodeNumber": function (d) {
				d.nodeNumber = nodeNumber;
				return ++nodeNumber;
			},
			"d": arc,
			"class": function (d) { return d.values ? "node" : "leaf node"; }
		})
		.style({
			"fill": select_colour,
			"fill-rule": "evenodd"
		})
		.on("click", sunburst_click)
		.on("mouseover", function (e) {
			var t = d3.select("#text-" + e.nodeNumber);
			if (t.style("opacity") != 0) {
				tipDetail.show(e);
			}
		})
		.on("mouseout", tipDetail.hide);

	var text = g.append("svg:text")
		.style({
			"opacity": function (d) {
					return 1;
			}
		})
		.attr({
			"transform": computeTextRotation,
			"dy": ".2em",
			"text-anchor": function (d) {
				return x(d.x + d.dx / 2) > Math.PI ? "end" : "start";
			},
			"class": function (e) {
				return "text" + e.depth;
			},
			"id": function (d) {
				return "text-" + d.nodeNumber;
			}
		})
		.text(function (d) {
			if (d.key) {
				return d.key.replace("My Company\\", "");
			}
		})
		.on("mouseover", function (e) {
			var t = d3.select("#text-" + e.nodeNumber);
			if (t.style("opacity") != 0) {
				tipDetail.show(e);
			}
		})
		.on("mouseout", tipDetail.hide);

	function sunburst_click(e) {
		var action = $("input[name=click_action]:checked").val();

		switch (action) {
			case "zoom":
				sunburst_zoom(e);
				break;
			case "purge":
				// Remove the data
				filter.push(e.key);
				prune(nestedData.values, e.key);
				plot();
				break;
			case "purge_all":
				// Remove the data
				filter.push(e.key);
				pruneAll(nestedData.values, e.key);
				plot();
				break;
		}
	}

	function sunburst_zoom(d) {
		var textNodes = d3.selectAll("text");

		// fade out all text elements
		textNodes.style({ "opacity": 0 });

		path.transition()
			.duration(750)
			.attrTween("d", arcTween(d))
			.each("end", function (e, i) {
				// check if the animated element's data e lies within the visible angle span given in d
				if (e.x >= d.x && e.x < (d.x + d.dx)) {
					// get a selection of the associated text element
					var arcText = d3.select("#text-" + e.nodeNumber);

					// fade in the text element and recalculate positions
					arcText.transition().duration(750)
						.style({
							"opacity": function (d) {
									return 1;
							}
						})
						.attr({
							"text-anchor": function (d) {
								return x(d.x + d.dx / 2) > Math.PI ? "end" : "start";
							},
							"transform": computeTextRotation(e),
						});
				}
			});
	}

//	d3.select(self.frameElement).style("height", diameter + "px");

	// Interpolate the arcs in data space.
	function arcTween(d) {
		var xd = d3.interpolate(x.domain(), [d.x, d.x + d.dx]),
			yd = d3.interpolate(y.domain(), [d.y, 1]),
			yr = d3.interpolate(y.range(), [d.y ? 20 : 0, radius]);
		return function (d, i) {
			return i
				? function (t) {
					d.arc = arc(d);
					return arc(d);
				}
				: function (t) { x.domain(xd(t)); y.domain(yd(t)).range(yr(t)); return arc(d); };
		};
	}

	function computeTextRotation(d) {
		var multiline = (d.name || "").split(" ").length > 1,
			angle = x(d.x + d.dx / 2) * 180 / Math.PI - 90,
			rotate = angle + (multiline ? -.5 : 0);
		return result = "rotate(" + rotate + ")translate(" + (y(d.y) + 6) + ")rotate(" + (angle > 90 ? -180 : 0) + ")";
	}

}

function plotIcicle(data) {
	var width = $("#chart").innerWidth(),
	height = Math.min($("#chart").width(), $(window).height() - 135);
//	var width = Math.min($("#chart").width(), $(window).height() - 150),
//	height = Math.min($("#chart").width(), $(window).height() - 150);

	var x = d3.scale.linear()
		.range([0, width]);

	var y = d3.scale.linear()
		.range([0, height]);

	var color = d3.scale.category20c();

	var partition = d3.layout.partition()
		.sort(null)
		.value(function (d) { return d.values; })
		.children(function (d) { return Array.isArray(d.values) ? d.values : null });

	var svg = d3.select("#chart")
		.append("svg:svg")
		.attr("width", width)
		.attr("height", height);

	var tipDetail = d3.tip()
		.attr('class', 'd3-tip')
		.offset([-15, 0])
		.html(getToolTipContent);

	var g = svg.selectAll("g")
		.data(partition.nodes(data))
		.enter()
			.append("svg:g")
			.attr("transform", function (d) { return "translate(" + x(d.y) + "," + y(d.x) + ")"; })
			.on("click", icicle_click);

	svg.call(tipDetail);

	var kx = width / data.dx,
		ky = height / 1;

	g.append("svg:rect")
		.attr({
			"width": data.dy * kx,
			"height": function (d) {
				return d.dx * ky;
			},
			"fill": select_colour
		})
		.on("mouseover", tipDetail.show)
		.on("mouseout", tipDetail.hide);

	g.append("svg:text")
		.attr("transform", transform)
		.attr("class", function (e) {
			return "text" + e.depth;
		})
		.attr("dy", ".35em")
		.style("opacity", function (d) {
			if ($("#showCat" + d.depth + "Text").is(':checked')) {
				return d.dx * ky > 12 ? 1 : 0;
			} else {
				return 0;
			}
		})
		.text(function (d) {
			if (d.key)
				return d.key.replace("My Company\\", "");
		})
		.on("mouseover", tipDetail.show)
		.on("mouseout", tipDetail.hide);

	d3.select("#chart").on("click", icicle_click);

	function icicle_click(e) {
		var action = $("input[name=click_action]:checked").val();

		switch (action) {
			case "zoom":
				icicle_zoom(e);
				break;
			case "purge":
				// Remove the data
				filter.push(e.key);
				prune(nestedData.values, e.key);
				plot();
				break;
			case "purge_all":
				// Remove the data
				filter.push(e.key);
				pruneAll(nestedData.values, e.key);
				plot();
				break;
		}
	}

	function icicle_zoom(d) {
		if (!d) return;
		if (!d.children) return;

		kx = (d.y ? width - 40 : width) / (1 - d.y);
		ky = height / d.dx;
		x.domain([d.y, 1]).range([d.y ? 40 : 0, width]);
		y.domain([d.x, d.x + d.dx]);

		var t = g.transition()
			.duration(d3.event.altKey ? 7500 : 750)
			.attr("transform", function (d) { return "translate(" + x(d.y) + "," + y(d.x) + ")"; });

		t.select("rect")
			.attr("width", d.dy * kx)
			.attr("height", function (d) { return d.dx * ky; });

		t.select("text")
			.attr("transform", transform)
			.style("opacity", function (d) { return d.dx * ky > 12 ? 1 : 0; });

		d3.event.stopPropagation();
	}

	function transform(d) {
		return "translate(8," + d.dx * ky / 2 + ")";
	}

}

function prune(array, nodeToDelete) {
	for (var i = 0; i < array.length; i++) {
		var node = array[i];
		if (node.x == nodeToDelete.x && node.y == nodeToDelete.y) {
			array.splice(i, 1);
			return true;
		}
		if (node.values) {
			if (prune(node.values, nodeToDelete)) {
				if (node.values.length === 0) {
					node.values = null;
				}
				return true;
			}
		}
	}
}

function pruneAll(array, key) {
	for (var i = 0; i < array.length; i++) {
		var node = array[i];
		if (node.key == key) {
			array.splice(i, 1);
		}
		if (node.values) {
			pruneAll(node.values, key);
			if (node.values.length === 0) {
				node.values = null;
			}
		}
	}
}