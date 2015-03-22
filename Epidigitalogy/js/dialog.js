
function handleError(e) {
	if (e.responseText != null) {
		var error = JSON.parse(e.responseText);
		$("#error").text(error.Message);
		$("#error").dialog({
			//dialogClass: "no-close",
			title: error.ExceptionType,
			buttons: [{
				text: "OK",
				click: function () {
					$(this).dialog("close");
				}
			}
			]
		});

		return true;
	} else {
		return false;
	}
}

function showWaitDialog(message, title) {
	if (message) {
		$("#wait").text(message);
	} else {
		$("#wait").text("Please wait while we retrieve data...");
	}
	$("#wait").dialog({
		dialogClass: "no-close",
		title: title == null ? "Please Wait" : title
	});
}

function hideWaitDialog() {
	$("#wait").dialog("close");
}

function showMessage(message, title) {
	$("#message").text(message);
	$("#message").dialog({
		title: title == null ? "Message" : title,
		modal: true,
		buttons: [{
			text: "OK",
			click: function () {
				$(this).dialog("close");
			}
		}
		]
	});
}