var data = null;
var indices = {};
var reverseIndices = {};
var hostTotals = {};
var hostNames = {};

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
});

function systemChanged() {
	$("#groupTotals").empty();
	$("#dateTotals").empty();
	populateFeeds();
	populateGroups();
	updateAlertCountByGroup();
	updateAlertCountByDate();
}

function getLog() {
	showWaitDialog("Retrieving Logs...");

	$("#chart").empty();
	indices = {};

	var feedValues = [];
	$('input[type="checkbox"]:checked').each(function (index, elem) {
		feedValues.push($(elem).val());
	});
	var feeds = feedValues.join(',');

	$.ajax({
		type: "POST",
		contentType: "application/json; charset=utf-8",
		url: "ws/LogData.asmx/GetLog",
		data: '{ "system": "' + $("#system").val() + '", "feeds": "' + feeds + '", "groupName": "' + $("#groupName").val() + '", "startDate": "' + $("#startDate").val() + '", "endDate": "' + $("#endDate").val() + '" }',
		dataType: "json",
		converters: {
			"text json": function (data) {
				return $.parseJSON(data, true);
			}
		},
		success: function (d) {
			data = d.d;
			if (data.hosts.length == 0) {
				showMessage("There is no data to display.");
			}
			sortMachines(data);
			plot(data);

			$("#spanTotal").show();
			$("#spanTotal").text("Total: " + data.hosts.length);
			$("#searchResults").show();
			$("#chart").show();
		},
		error: function (e) {
			handleError(e);
		},
		complete: function () {
			hideWaitDialog();
		}
	});
};

function sortMachines(data) {
	indices = {};
	reverseIndices = {};
	hostTotals = {};
	hostNames = {};

	data.hosts.sort(function (a, b) {
		return (a.gn.toLowerCase() + "." + a.hn.toLowerCase() > b.gn.toLowerCase() + "." + b.hn.toLowerCase()
			? 1
			: (a.gn.toLowerCase() + "." + a.hn.toLowerCase() < b.gn.toLowerCase() + "." + b.hn.toLowerCase()
				? -1
				: 0)
			);
	});

	$.each(data.hosts, function (i, d) {
		hostNames[d.id] = (d.gn + "\\" + d.hn);
	});

	$.each(data.hosts, function (i, d) {
		indices[d.id] = i;
		reverseIndices[i] = d.id;
	});

	$.each(data.hosts, function (i, d) {
		hostTotals[d.id] = d.events.length;
	});
}

function showMachineDetails(d) {
	var groupname = data.hosts[indices[d.__data__.id]].gn;
	var hostname = data.hosts[indices[d.__data__.id]].hn;
	var computername;
	var system = $("#system").val();

	if (system == "Kaseya") {
		computername = groupname + "." + hostname;
	} else if (system == "Symantec") {
		computername = groupname + "\\" + hostname;
	}

	$.ajax({
		type: "POST",
		contentType: "application/json; charset=utf-8",
		url: "ws/LogData.asmx/GetDetails",
		data: '{ "system": "' + $("#system").val() + '", "id": "' + $.trim(d.__data__.id) + '", "startDate": "' + $("#startDate").val() + '", "endDate": "' + $("#endDate").val() + '" }',
		dataType: "json",
		converters: {
			"text json": function (data) {
				return $.parseJSON(data, true);
			}
		},
		success: function (d) {
			plotDetails(d);
			//					alert(d);
		}
	});

	$("#chartDetails").empty();
	$("#details").show();

	$("#details").dialog({
		title: computername,
		dialogClass: "no-close",
		width: 1000,
		height: 450,
	});
}
