<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Flare.aspx.cs" Inherits="epidigitalogy.Flare" %>
<!DOCTYPE html>
<html lang="en">
	<head>
		<meta charset="utf-8">
		<meta http-equiv="X-UA-Compatible" content="IE=edge">
		<meta name="viewport" content="width=device-width, initial-scale=1">
		<link rel="icon" href="">

		<title>Digital Disease Flare Comparator</title>

		<!-- Latest compiled and minified CSS -->
		<link rel="stylesheet" href="/css/bootstrap/bootstrap.min.css">

		<!-- Optional theme -->
		<link rel="stylesheet" href="/css/bootstrap/bootstrap-theme.min.css">

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
				<a class="navbar-brand" href="#">Digital Disease Flare Comparator</a>
			</div>
			<div id="navbar" class="navbar-collapse collapse">
				<ul class="nav navbar-nav navbar-right">
					<li><a href="LogInfo.aspx">Tracking Chart</a></li>
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

					<li class="searchEntry">Feed:</li>
					<li id="feedsKaseya" style="display:none;" onchange="javascript:feedChanged();">
						<select id="feeds">
							<option value="kes">KES</option>
							<option value="kam">KAM</option>
							<option value="kav">KAV</option>
						</select>
					</li>
					<li id="feedsSymantec" style="display:none;" onchange="javascript:feedChanged();">
						<select id="feed">
							<option value="av">Anti-Virus</option>
							<option value="uc">User Control</option>
							<option value="ips">IPS</option>
							<option value="dl">Downloads</option>
							<option value="fw">Firewall</option>
							<option value="up">Updates</option>
						</select>
					</li>
					
					<li class="searchEntry">Preset:</li>
					<li>
						<select id="preset" onchange="javascript:presetSelected(this.value);">
						</select>
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
					
					<li class="searchEntry">Category 1:<span style="float:right;display:none"><input type="checkbox" id="showCat1Text" value="1" checked="" onchange="javascript:showHideText(this.checked, 1);" />&nbsp;Show Text</span></li>
					<li>
						<select id="cat1"></select>
					</li>
					
					<li class="searchEntry">Category 2:<span style="float:right;display:none"><input type="checkbox" id="showCat2Text" value="1" checked="" onchange="javascript:showHideText(this.checked, 2);" />&nbsp;Show Text</span></li>
					<li>
						<select id="cat2"></select>
					</li>
					
					<li class="searchEntry">Category 3:<span style="float:right;display:none"><input type="checkbox" id="showCat3Text" value="1" checked="" onchange="javascript:showHideText(this.checked, 3);" />&nbsp;Show Text</span></li>
					<li>
						<select id="cat3"></select>
					</li>
					
					<li class="searchEntry">Chart Type:</li>
					<li>
						<select id="chartType">
							<option value="sunburst">Sunburst</option>
							<option value="icicle">Icicle</option>
						</select>
					</li>

					<li class="searchEntry"></li>
					<li>
						<input type="button" id="search" value="Search" onclick="getFlare();" />
						<input type="button" id="reset_filter" value="Reset Filter" onclick="resetFilter();" />
					</li>
					<br />
					<br />
					<li class="searchEntry">Click Action:</li>
					<li>
						<input type="radio" name="click_action" value="zoom" checked="true">Zoom</input><br />
						<input type="radio" name="click_action" value="purge_all">Purge All Similar Nodes</input><br />
						<input type="radio" name="click_action" value="purge">Purge That Clicked Node</input>
					</li>
				</ul>
			</div>
			<div id="divResults" class="col-sm-9 col-sm-offset-3 col-md-10 col-md-offset-2 main" style="overflow:hidden;">
<!--
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
-->
				<div class="row">
					<span id="spanTotal" style="float:right;display:none;"></span>
					<h3 id="searchResults" class="page-header" style="margin:0px;display:none;">Search Results</h3>
					<div id="chart" align="center" class="col-xs-12 col-sm-12"></div>

					<div id="error"></div>
					<div id="wait"></div>
					<div id="message"></div>

				</div>
			</div>

		</div>
		
		<script type="text/javascript" src="/js/d3/d3.js"></script>
		<script type="text/javascript" src="/js/d3/d3.tip.js"></script>
		<script type="text/javascript" src="/js/jquery/jquery-2.1.3.js"></script>
		<script type="text/javascript" src="/js/jquery/jquery-ui.min.js"></script>

		<!-- Latest compiled and minified JavaScript -->
		<script type="text/javascript" src="/js/bootstrap/bootstrap.min.js"></script>

		<script type="text/javascript" src="/js/dialog.js"></script>
		<script type="text/javascript" src="/js/graphs.js"></script>
		<script type="text/javascript" src="/js/util.js"></script>
		<script type="text/javascript" src="/js/parseJSON.js"></script>
		<script type="text/javascript" src="/js/population.js"></script>
		<script type="text/javascript" src="/js/flare.js"></script>
	
	</body>
</html>
