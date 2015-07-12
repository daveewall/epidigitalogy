
function populateGroups() {
	showWaitDialog("Populating Groups...");
	$.ajax({
		type: "POST",
		contentType: "application/json; charset=utf-8",
		url: "ws/Population.asmx/PopulateGroups",
		data: '{ "system": "' + $("#system").val() + '" }',
		dataType: "json",
		success: function (d) {
			$("#groupName").find("option").remove();
			$("#groupName").append($("<option />").val("").text("(All Groups)"));
			$.each(d.d, function (i, name) {
				if (name != "My Company") {
					$("#groupName").append($("<option />").val(name.replace(/\\/g, "\\\\")).text(name.replace("My Company\\", "")));
				}
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

	$("#feedsKaseya").hide();
	$("#feedsKaseya input").removeAttr("checked");

	$("#feedsSymantec").hide();
	$("#feedsSymantec input").removeAttr("checked");

	if (system == "Kaseya") {
		$("#feedsKaseya").show();
		$("#feedsKaseya input[value='kes']").prop("checked", true);
		$("#feedsKaseya input[value='kam']").prop("checked", true);
		$("#feedsKaseya input[value='kav']").prop("checked", true);
	} else if (system == "Symantec") {
		$("#feedsSymantec").show();
		$("#feedsSymantec input[value='fw']").prop("checked", false);
		$("#feedsSymantec input[value='ips']").prop("checked", true);
		$("#feedsSymantec input[value='dl']").prop("checked", false);
		$("#feedsSymantec input[value='av']").prop("checked", true);
		$("#feedsSymantec input[value='uc']").prop("checked", false);
		$("#feedsSymantec input[value='up']").prop("checked", true);
	}
}
