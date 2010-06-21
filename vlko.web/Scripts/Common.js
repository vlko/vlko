var $loadingDialog;

// create loading dialog
function createLoading() {
	$loadingDialog = $('<div class="loading_dialog"></div>')
		.html('Loading...')
		.dialog({
			autoOpen: true,
			modal: true,
			close: function () { $(".loading_dialog").empty(); },
			title: 'Loading...'
		});
}

// close loading dialog
function closeLoading() {
	if ($loadingDialog) {
		$loadingDialog.dialog("close");
	}
	$loadingDialog = null;
}

// handle ajax exception
function ajaxException(xhr, ajaxOptions, thrownError) {
	closeLoading();
	$('<div></div>')
		.html(xhr.responseText)
		.dialog({
			autoOpen: true,
			width: $("#content").width(),
			modal: true,
			title: "Error:" + thrownError
		});
	alert(thrownError);
}

// create content dialog
function createContentDialog(settings) {
	var config = {
		buttons: { "Ok": function () { $(this).dialog("close") } },
		dialogName: "content_dialog"
	}
	$.extend(config, settings);

	var contentDialog = $('<div class="' + config.dialogName + '"></div>')

						.dialog({
							autoOpen: false,
							modal: true,
							width: $("#content").width(),
							draggable: true,
							resizable: true,
							title: config.title,
							close: function () {
								$("." + config.dialogName).empty(); 
								if (config.prevUrl !== null){
									addToHistory(config.prevUrl);
								}
							},
							buttons: config.buttons
						});
	fillContentWithData(contentDialog, config.data);
	return contentDialog;
}

function fillContentWithData(form, data) {
	form.html(data);
	form.children(":not(form, .ajax_content)").hide();
	form.children("form").children(":not(.ajax_content)").hide();
}

function updateEffect(content) {
	content.effect("pulsate", { times: 1 }, 500);
}

// ajax history function
function addToHistory(url) {
	$.history.load(url);
}
// get current ajax url
function getCurrentHistoryUrl() {
	return $.history.appState;
}

// initialize ajax history plugin
$.history.init(function (url, phase) {
	if (phase == "check") {
		if (!url) {
			window.location = window.location.href;
		}
		else {
			//var newUrl = window.location.protocol + "//" + window.location.host + url
			window.location = url;
		}
	}
});