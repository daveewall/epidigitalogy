<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GetLogInfo.aspx.cs" Inherits="epidigitalogy.GetLogInfo" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Digital Disease Tracking Chart</title>
	
	<script src="/js/d3/d3.v3.min.js"></script>
	<script src="/js/jquery/jquery-2.1.3.js"></script>
	<script src="/js/jquery/jquery-ui.min.js"></script>

	<script src="/js/dialog.js"></script>
	<script src="/js/chart.js"></script>
    <script src="/js/totals.js"></script>
    <script src="/js/util.js"></script>
	<script src="/js/parseJSON.js"></script>
  
	<link rel="stylesheet" href="/js/jquery/jquery-ui.structure.min.css"/>
	<link rel="stylesheet" href="/js/jquery/jquery-ui.theme.min.css"/>
    <link rel="stylesheet" href="/css/epi.css"/>
    
	<script>

		var data = null;
		var indices = {};
		var hostTotals = {};

		$(document).ready(function () {
			systemChanged();
			$("#startDate").datepicker();
			$("#endDate").datepicker();

			// Set defaults for dates
			var startDate = new Date();
			var endDate = new Date();
			startDate.setDate(1);
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

		function populateGroups() {
			showWaitDialog("Populating Groups...");
			$.ajax({
				type: "POST",
				contentType: "application/json; charset=utf-8",
				url: "Population.asmx/PopulateGroups",
				data: '{ "system": "' + $("#system").val() + '" }',
				dataType: "json",
				success: function (d) {
					$("#groupName").find("option").remove();
					$("#groupName").append($("<option />").val("").text("(All Groups)"));
					$.each(d.d, function (i, name) {
						$("#groupName").append($("<option />").val(name.replace(/\\/g, "\\\\")).text(name));
					});
				},
				error: function (e) {
					handleError(e);
				},
				complete: function () {
					hideWaitDialog();
				}
			});
		}

		function populateFeeds() {
			var system = $("#system").val();
			$("#feed").find("option").remove();
			if (system == "Symantec") {
				$("#feed").append($("<option />").val("SymantecSEP").text("SymantecSEP"));
			}
		}

		function getLog() {
			showWaitDialog("Retrieving Logs...");

			$("#chart").empty();
			indices = {};

			$.ajax({
				type: "POST",
				contentType: "application/json; charset=utf-8",
				url: "LogData.asmx/GetLog",
				data: '{ "feed": "' + $("#feed").val() + '", "groupName": "' + $("#groupName").val() + '", "startDate": "' + $("#startDate").val() + '", "endDate": "' + $("#endDate").val() + '" }',
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
			hostTotals = {};

			data.hosts.sort(function (a, b) {
				return (a.gn + "." + a.hn > b.gn + "." + b.hn ? 1 : (a.gn + "." + a.hn < b.gn + "." + b.hn ? -1 : 0));
			});

			$.each(data.hosts, function (i, d) {
				indices[d.id] = i;
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

			if (system == "Symantec") {
				computername = groupname + "\\" + hostname;
			}

			$.ajax({
				type: "POST",
				contentType: "application/json; charset=utf-8",
				url: "LogData.asmx/GetDetails",
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
	</script>
</head>
<body>
    <table class="headerTable">
		<tr>
			<td class="header"><h1>Digital Disease Tracking Chart</h1></td>
			<td class="sumheader">Alert Count by Client Group<br /><div id="groupTotals"></div></td>
			<td class="sumheader">Alert Count by Day<br /><div id="dateTotals"></div></td>
		</tr>
	</table>
    
    <form id="form1" runat="server">
		<div>
            <div style="width:100%; display:table;">
		        <span style="display:table-cell;">
		            System:
		            <select id="system" onchange="javascript:systemChanged();">
			            <option value="Symantec">Symantec</option>
		            </select>
		        </span>

		        <span style="display:table-cell;">
		            Feed:
		            <select id="feed">
		            </select>
		        </span>

		        <span style="display:table-cell;">
		            Group:
		            <select id="groupName">
			            <option value="">(All Groups)</option>
		            </select>
		        </span>
		
		        <span style="display:table-cell;">
                    Start Date: <input type="text" id="startDate"/>
		        </span>
		
		        <span style="display:table-cell;">
                    End Date: <input type="text" id="endDate"/>
		        </span>

		        <span style="display:table-cell;">
		            <input type="button" id="search" value="Search" onclick="getLog();" />
		        </span>
            </div>
            
		    <div id="chart"></div>

		    <div id="error"></div>
		    <div id="wait"></div>
		    <div id="message"></div>
		    <div id="details" style="display:none;">
				<table style="width:100%;">
					<tr>
						<th>User Name:</th>
						<td id="userName"></td>
						<th>Name:</th>
						<td id="name"></td>
						<th>IP Address:</th>
						<td id="ipAddr1"></td>
					</tr>
					<tr>
						<th>OS:</th>
						<td id="os"></td>
						<th>Title:</th>
						<td id="title"></td>
						<th>Subnet Mask:</th>
						<td id="subnetMask1"></td>
					</tr>
					<tr>
						<th>Agent Version:</th>
						<td id="agentVersion"></td>
						<th>Department:</th>
						<td id="department"></td>
						<th>Gateway:</th>
						<td id="gateway1"></td>
					</tr>
					<tr>
						<th>Last Scan:</th>
						<td id="lastScanTime"></td>
						<th>Email:</th>
						<td id="email"></td>
						<th>DNS 1:</th>
						<td id="dnsServer1"></td>
					</tr>
					<tr>
						<th>Last Download:</th>
						<td id="lastDownloadTime"></td>
						<th>Office Phone:</th>
						<td id="officePhone"></td>
						<th>DNS 2:</th>
						<td id="dnsServer2"></td>
					</tr>
					<tr>
						<th>Last Virus Time:</th>
						<td id="lastVirusTime"></td>
						<th>Mobile Phone:</th>
						<td id="mobilePhone"></td>
						<th>DHCP Server:</th>
						<td id="dhcpServer"></td>
					</tr>
				</table>
				<div id="chartDetails"></div>
		    </div>
            
        </div>
    </form>
</body>
</html>
