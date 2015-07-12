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
	$("#group").change(updateReload);
	$("#startDate").change(updateReload);
	$("#endDate").change(updateReload);

});

function updateReload() {
	reload = true;
}

function resetFilter() {
	filter = [];
}

function systemChanged() {
	populateFeeds();
	populateGroups();
	feedChanged();
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
					$("#cat" + catNumber).append($("<option />").val(name).text(fields[name]));
				});
			});
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

function plotSunburst(data) {
	var diameter = 960,
		format = d3.format(",d"),
		width = 800,
		height = 800,
	    radius = Math.min(width, height) / 2 - 100,
		color = d3.scale.category20c();

	var x = d3.scale.linear()
		.range([0, 2 * Math.PI]);

	var y = d3.scale.sqrt()
		.range([0, radius]);

	var svg = d3.select("#chart").append("svg")
		.attr("width", diameter)
		.attr("height", diameter)
		.append("g")
		.attr("transform", "translate(" + (width / 2 + 10) + "," + (height / 2 + 10) + ")");	// BUG -- Make this the size of the surrounding div

	var partition = d3.layout.partition()
		.sort(null)
		.value(function (d) {
			return d.values;
		})
		.children(function (d) { return d.values ? d.values : null });

	var arc = d3.svg.arc()
		.startAngle(function (d) { return Math.max(0, Math.min(2 * Math.PI, x(d.x))); })
		.endAngle(function (d) { return Math.max(0, Math.min(2 * Math.PI, x(d.x + d.dx))); })
		.innerRadius(function (d) {
			return Math.max(0, y(d.y));
		})
//		.outerRadius(function (d) { return Math.max(0, y(d.y + d.dy)); });
		.outerRadius(radius);


	var g = svg.selectAll("g")
		.data(partition.nodes(data))
		.enter()
			.append("svg:g")
			.on("click", on_click);

	g.append("svg:path")
		.attr({
			"d": arc,
			"display": function (d) { return d.depth ? null : "none"; }, // hide inner ring
			"class": function (d) { return d.values ? "node" : "leaf node"; },
//			"onclick": "javascript:on_click(this);"
		})
		.style({
			"stroke": function (d) { return color(d.key); },
			"fill": function (d) { return color(d.key); },
			"fill-rule": "evenodd"
		})
		.append("svg:title").text(function (d) {
			return (d.key != "" ? d.key : "(unknown)") + ", Count: " + (typeof d.values == 'object' && d.values != null ? d.values.length : d.values)
		})


	var nodes = partition.nodes(data);
	var text = svg.selectAll("text").data(nodes);

	var textEnter = text.enter().append("svg:text")
		.attr("dy", ".2em")
		.style("opacity", 1)
		.attr("text-anchor", function (d) {
			return x(d.x + d.dx / 2) > Math.PI ? "end" : "start";
		})
		.attr("transform", function (d) {
			var multiline = (d.name || "").split(" ").length > 1,
                angle = x(d.x + d.dx / 2) * 180 / Math.PI - 90,
                rotate = angle + (multiline ? -.5 : 0);
			return "rotate(" + rotate + ")translate(" + (y(d.y) + 6) + ")rotate(" + (angle > 90 ? -180 : 0) + ")";
		})

	textEnter.append("tspan")
		.attr("color", "#FFF")
		.attr("x", 0)
		.text(function (d) {
			if ($("#showCat" + d.depth + "Text").is(':checked')) {
				return d.key.replace("My Company\\", "");
			} else {
				return null;
			}
		});

	/*
	svg.datum(data).selectAll("path")
		.data(partition.nodes)
		.enter()
			.append("path")
			.attr({
				"d": arc,
				"display": function (d) { return d.depth ? null : "none"; }, // hide inner ring
				"class": function (d) { return d.values ? "node" : "leaf node"; },
				"onclick": "javascript:on_click(this);"
			})
			.style({
				"stroke": function (d) { return color(d.key); },
				"fill": function (d) { return color(d.key); },
				"fill-rule": "evenodd"
			})
			.on("click", on_click)
			.append("svg:title").text(function (d) {
				return (d.key != "" ? d.key : "(unknown)") + ", Count: " + (typeof d.values == 'object' ? d.values.length : d.values)
			})


	// Put text on the graph
	var nodes = partition.nodes(data);
	var text = svg.selectAll("text").data(nodes);

	var textEnter = text.enter().append("text")
		 .attr("class", function (d, i) {
		 	return "ring_" + d.depth;
		 })
		.style("opacity", 1)
		.attr("text-anchor", function (d) {
			return x(d.x + d.dx / 2) > Math.PI ? "end" : "start";
		})
		.attr("dy", ".2em")
		.attr("transform", function (d) {
			var multiline = (d.name || "").split(" ").length > 1,
                angle = x(d.x + d.dx / 2) * 180 / Math.PI - 90,
                rotate = angle + (multiline ? -.5 : 0);
			return "rotate(" + rotate + ")translate(" + (y(d.y) + 6) + ")rotate(" + (angle > 90 ? -180 : 0) + ")";
		})
	;

	textEnter.append("tspan")
		.attr("color", "#FFF")
		.attr("x", 0)
		.text(function (d) {
			if ($("#showCat" + d.depth + "Text").is(':checked')) {
				return d.key.replace("My Company\\", "");
			} else {
				return null;
			}
		});
*/

//	node.append("title")
//		.text(function (d) { return d.key + (d.values ? d.values.count : "0"); });

//	node.append("circle")
//		.attr("r", function (d) { return d.r; });

//	node.filter(function (d) { return !d.values; }).append("text")
//		.attr("dy", ".3em")
//		.style("text-anchor", "middle");
////		.text(function (d) { return d.key.substring(0, d.r / 3); });

	d3.select(self.frameElement).style("height", diameter + "px");
/*
	// Interpolate the arcs in data space.
	function arcTween(a) {
		var i = d3.interpolate({ x: a.x0, dx: a.dx0 }, a);
		return function (t) {
			var b = i(t);
			a.x0 = b.x;
			a.dx0 = b.dx;
			return arc(b);
		};
	}
*/
}

function plotIcicle(data) {
	var width = 800,
		height = 800;

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


	var g = svg.selectAll("g")
		.data(partition.nodes(data))
		.enter()
			.append("svg:g")
			.attr("transform", function (d) { return "translate(" + x(d.y) + "," + y(d.x) + ")"; })
			.on("click", click);

	var kx = width / data.dx,
		ky = height / 1;

	g.append("svg:rect")
		.attr("width", data.dy * kx)
		.attr("height", function (d) { return d.dx * ky; })
		.attr("fill", function (d) { return color(d.key); })
//		.attr("class", function (d) { return d.children ? "parent" : "child"; })

	g.append("svg:text")
		.attr("transform", transform)
		.attr("dy", ".35em")
		.style("opacity", function (d) { return d.dx * ky > 12 ? 1 : 0; })
		.text(function (d) {
			if ($("#showCat" + d.depth + "Text").is(':checked')) {
				return d.key.replace("My Company\\", "");
			} else {
				return null;
			}
		})

	d3.select(window)
		.on("click", function () { click(data); })

	function click(d) {
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


function on_click(e) {
	// Remove the data
	filter.push(e.key);
	pruneAll(nestedData.values, e.key);
	//	prune(nestedData.values, e.key);
	plot();
//	zoom(e);
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