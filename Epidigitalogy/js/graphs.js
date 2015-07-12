function updateAlertCountByGroup() {
	var margin = { top: 0, right: 0, bottom: 0, left: 0 };
	var width = $("#groupTotals").width() - margin.left - margin.right;
	var height = 70 - margin.top - margin.bottom;

	var x = d3.scale.ordinal()
		.rangeRoundBands([0, width], .1);

	var y = d3.scale.linear()
		.range([height, 0]);

	var xAxis = d3.svg.axis()
		.scale(x)
		.orient("bottom");

	var yAxis = d3.svg.axis()
		.scale(y)
		.orient("left")
		.ticks(30, "%");

	var svgGroupSum = d3.select("#groupTotals").append("svg")
		.attr("width", width + margin.left + margin.right)
		.attr("height", height + margin.top + margin.bottom)
		.append("g")
		.attr("transform", "translate(" + margin.left + "," + margin.top + ")");

	$.ajax({
		type: "POST",
		contentType: "application/json; charset=utf-8",
		url: "ws/Graphs.asmx/GetAlertCountsByGroup",
		data: '{ "system": "' + $("#system").val() + '" }',
		dataType: "json",
		converters: {
			"text json": function (data) {
				return $.parseJSON(data, true);
			}
		},
		success: function (groupdata) {
			var totals = groupdata.d.map(function (d) {
				return d.total;
			});

			y.domain([0, Math.max.apply(null, totals)]);

			var barWidth = width / totals.length;
//			if (barWidth > 5) { barWidth = 5; }

			svgGroupSum.selectAll(".bar")
				.data(groupdata.d)
				.enter().append("rect")
				.attr("class", "bar")
		        .attr("x", function (d, i) { return i * barWidth; }) //x(d.group); })
				.attr("width", barWidth)
				.attr("y", function (d) { return y(d.total); })
				.attr("height", function (d) { return height - y(d.total); })
				.on('mouseover', function (d) {
					d3.select(this)
					.append('svg:title')
					.text(function (d) {
						return 'Group: ' + d.group + ', Count:' + d.total;
					})
				})
				.on('click', function (d) {
					$("#groupName").val(d.group.replace(/\\/g, "\\\\"));
				})
			;
		}
	});
}

function updateAlertCountByDate() {
	var margin = { top: 0, right: 0, bottom: 0, left: 0 },
			width = $("#dateTotals").width() - margin.left - margin.right,
			height = 70 - margin.top - margin.bottom;

	var x = d3.scale.ordinal()
		.rangeRoundBands([0, width], .1);

	var y = d3.scale.linear()
		.range([height, 0]);

	var xAxis = d3.svg.axis()
		.scale(x)
		.orient("bottom");

	var yAxis = d3.svg.axis()
		.scale(y)
		.orient("left")
		.ticks(30, "%");

	var svgDateSum = d3.select("#dateTotals").append("svg")
		.attr("width", width + margin.left + margin.right)
		.attr("height", height + margin.top + margin.bottom)
		.append("g")
		.attr("transform", "translate(" + margin.left + "," + margin.top + ")");

	$.ajax({
		type: "POST",
		contentType: "application/json; charset=utf-8",
		url: "ws/Graphs.asmx/GetAlertCountsByDay",
		data: '{ "system": "' + $("#system").val() + '" }',
		dataType: "json",
		converters: {
			"text json": function (data) {
				return $.parseJSON(data, true);
			}
		},
		success: function (groupdata) {
			var totals = groupdata.d.map(function (d) {
				return d.total;
			});

			y.domain([0, Math.max.apply(null, totals)]);

			var barWidth = width / totals.length;
			if (barWidth > 5) {
				barWidth = 5;
			} else if (barWidth < 2) {
				barWidth = 2;
			}

			svgDateSum.selectAll(".bar")
				.data(groupdata.d)
				.enter().append("rect")
				.attr("class", "bar")
				.attr("x", function (d, i) { return i * barWidth; }) //x(d.group); })
				.attr("width", barWidth)
				.attr("y", function (d) { return y(d.total); })
				.attr("height", function (d) { return height - y(d.total); })
				.on('mouseover', function (d) {
					d3.select(this)
					.append('svg:title')
					.text(function (d) {
						var cur_date = d.date.getDate();
						var cur_month = d.date.getMonth() + 1; //Months are zero based
						var cur_year = d.date.getFullYear();
						var dt = cur_month + "/" + cur_date + "/" + cur_year;
						return 'Date: ' + dt + ', Count:' + d.total;
					})
				})
				.on('click', function (bar) {
					var d = new Date();
					d.setTime(bar.date.getTime());
					d.setDate(d.getDate() + 1);	// Add a day
					var cur_date = d.getDate();
					var cur_month = d.getMonth() + 1; //Months are zero based
					var cur_year = d.getFullYear();
					var endDate = cur_month + "/" + cur_date + "/" + cur_year;
					$("#endDate").val(endDate);
					
					d.setDate(d.getDate() - 8);	// Add a day
					cur_date = d.getDate();
					cur_month = d.getMonth() + 1; //Months are zero based
					cur_year = d.getFullYear();
					var startDate = cur_month + "/" + cur_date + "/" + cur_year;
					$("#startDate").val(startDate);
				})
			;
		}
	});
}
