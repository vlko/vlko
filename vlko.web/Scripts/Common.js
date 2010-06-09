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
function createContentDialog(title, data, buttons, dialogName) {
	if (!buttons) {
		buttons = { "Ok": function () { $(this).dialog("close") } };
	}
	if (!dialogName) {
		dialogName = "content_dialog";
	}
	var contentDialog = $('<div class="' + dialogName + '"></div>')

						.dialog({
							autoOpen: false,
							modal: true,
							width: $("#content").width(),
							draggable: true,
							resizable: true,
							title: title,
							close: function () { $("." + dialogName).empty(); },
							buttons: buttons
						});
	fillContentWithData(contentDialog, data);
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