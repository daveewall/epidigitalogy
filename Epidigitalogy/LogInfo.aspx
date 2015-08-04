<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LogInfo.aspx.cs" Inherits="epidigitalogy.LogInfo" %>
<!DOCTYPE html>
<html lang="en">
	<head>
		<meta charset="utf-8">
		<meta http-equiv="X-UA-Compatible" content="IE=edge">
		<meta name="viewport" content="width=device-width, initial-scale=1">
		<link rel="icon" href="">

		<title>Digital Disease Tracking Chart</title>

		<!-- Latest compiled and minified CSS -->
		<link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.4/css/bootstrap.min.css">

		<!-- Optional theme -->
		<link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.4/css/bootstrap-theme.min.css">

		<!-- HTML5 shim and Respond.js for IE8 support of HTML5 elements and media queries -->
		<!-- WARNING: Respond.js doesn't work if you view the page via file:// -->
		<!--[if lt IE 9]>
		  <script src="https://oss.maxcdn.com/html5shiv/3.7.2/html5shiv.min.js"></script>
		  <script src="https://oss.maxcdn.com/respond/1.4.2/respond.min.js"></script>
		<![endif]-->
	
		<link rel="stylesheet" type="text/css" href="/js/jquery/jquery-ui.structure.min.css" />
		<link rel="stylesheet" type="text/css" href="/js/jquery/jquery-ui.theme.min.css" />
		<link rel="stylesheet" type="text/css" href="/css/dashboard.css" />
		<link rel="stylesheet" type="text/css" href="/css/epi.css" />
	</head>
	<body>

	<nav class="navbar navbar-inverse navbar-fixed-top">
		<div class="container-fluid">
			<div class="navbar-header">
				<button type="button" class="navbar-toggle collapsed" data-toggle="collapse" data-target="#navbar" aria-expanded="false" aria-controls="navbar">
					<span class="sr-only">Toggle navigation</span>
					<span class="icon-bar"></span>
					<span class="icon-bar"></span>
					<span class="icon-bar"></span>
				</button>
				<a class="navbar-brand" href="#">Digital Disease Tracking Chart</a>
			</div>
			<div id="navbar" class="navbar-collapse collapse">
				<ul class="nav navbar-nav navbar-right">
					<li><a href="Flare.aspx">Flare</a></li>
<!--					<li><a href="#">Help</a></li> -->
				</ul>
<!--				<form class="navbar-form navbar-right">
					<input type="text" class="form-control" placeholder="Search...">
				</form>
-->			</div>
		</div>
	</nav>

	<div class="container-fluid">
		<div class="row">
			<div class="col-sm-3 col-md-2 sidebar">
				<ul class="nav nav-sidebar">
					<li>
						<h3>Search</h3>
					</li>
					<li class="searchEntry">System:</li>
					<li>
						<select id="system" onchange="javascript:systemChanged();">
							<option value="Symantec">Symantec</option>
							<option value="Kaseya">Kaseya</option>
						</select>
					</li>

					<li class="searchEntry">Feeds:</li>
					<li id="feedsKaseya" style="display:none;">
						<table width="100%">
							<tr>
								<td><input type="checkbox" name="feeds" value="kes" checked="checked"> KES</input></td>
								<td width="50" class="U"></td>
								<td width="50" class="V"></td>
							</tr>
							<tr>
								<td><input type="checkbox" name="feeds" value="kam" checked="checked"> KAM</input></td>
								<td width="50" class="U"></td>
								<td width="50" class="V"></td>
							</tr>
							<tr>
								<td><input type="checkbox" name="feeds" value="kav" checked="checked"> KAV</input></td>
								<td width="50" class="U"></td>
								<td width="50" class="V"></td>
							</tr>
						</table>
					</li>
					<li id="feedsSymantec" style="display:none;">
						<table width="100%">
							<tr>
								<td><input type="checkbox" name="feeds" value="fw"> Firewall</input></td>
								<td width="50" class="F"></td>
							</tr>
							<tr>
								<td><input type="checkbox" name="feeds" value="ips" checked="checked"> IPS</input></td>
								<td width="50" class="I"></td>
							</tr>
							<tr>
								<td><input type="checkbox" name="feeds" value="dl"> Downloads</input></td>
								<td width="50" class="D"></td>
							</tr>
							<tr>
								<td><input type="checkbox" name="feeds" value="av" checked="checked"> Anti-Virus</input></td>
								<td width="50" class="V"></td>
							</tr>
							<tr>
								<td><input type="checkbox" name="feeds" value="uc"> User Control</input></td>
								<td width="50" class="C"></td>
							</tr>
							<tr>
								<td><input type="checkbox" name="feeds" value="up" checked="checked"> Updates</input></td>
								<td width="50" class="U"></td>
							</tr>
						</table>
					</li>

					<li class="searchEntry">Group:</li>
					<li>
						<select id="groupName">
							<option value="">(All Groups)</option>
						</select>
					</li>
					
					<li class="searchEntry">Start Date:</li>
					<li>
						<input type="text" id="startDate" />
					</li>
					
					<li class="searchEntry">End Date:</li>
					<li>
						<input type="text" id="endDate" />
					</li>
					
					<li class="searchEntry"></li>
					<li>
						<input type="button" id="search" value="Search" onclick="getLog();" />
					</li>
				</ul>
			</div>
			<div class="col-sm-9 col-sm-offset-3 col-md-10 col-md-offset-2 main">

				<div class="row placeholders">
					<div class="col-xs-4 col-sm-4 placeholder">
						<div id="groupTotals" class="widget"></div>
						<h4>Alert Count by Client Group</h4>
					</div>
					<div class="col-xs-4 col-sm-4 placeholder">
						<div id="dateTotals" class="widget"></div>
						<h4>Alert Count by Day</h4>
					</div>
				</div>

				<div class="row">
					<span id="spanTotal" style="float:right;display:none;"></span>
					<h3 id="searchResults" class="page-header" style="display:none;">Search Results</h3>
					<div id="chart" class="col-xs-12 col-sm-12"></div>

					<div id="error"></div>
					<div id="wait"></div>
					<div id="message"></div>
					<div id="details" style="display: none;">
						<table style="width: 100%;">
							<tr>
								<th>User Name:</th>
								<td id="userName"></td>
								<th>Name:</th>
								<td id="name" width="100"></td>
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
			</div>

		</div>

		<script type="text/javascript" src="/js/d3/d3.js"></script>
		<script type="text/javascript" src="/js/d3/d3.tip.js"></script>
		<script type="text/javascript" src="/js/jquery/jquery-2.1.3.js"></script>
		<script type="text/javascript" src="/js/jquery/jquery-ui.min.js"></script>

		<!-- Latest compiled and minified JavaScript -->
		<script type="text/javascript" src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.4/js/bootstrap.min.js"></script>

		<script type="text/javascript" src="/js/dialog.js"></script>
		<script type="text/javascript" src="/js/chart.js"></script>
		<script type="text/javascript" src="/js/graphs.js"></script>
		<script type="text/javascript" src="/js/util.js"></script>
		<script type="text/javascript" src="/js/parseJSON.js"></script>
		<script type="text/javascript" src="/js/population.js"></script>
		<script type="text/javascript" src="/js/loginfo.js"></script>
	
	</body>
</html>
