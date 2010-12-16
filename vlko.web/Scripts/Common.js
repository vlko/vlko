// script cache handling
(function (scriptCache, $, undefined) {
	var loadedScripts = [];
	scriptCache.load = function (scriptUrl) {
		// if script is not yet loaded
		if (!loadedScripts[scriptUrl]) {
			$("body").append('<script src="' + scriptUrl + '" type="text/javascript"></script>');
			loadedScripts[scriptUrl] = true;
		}
	}
} (window.scriptCache = window.scriptCache || {}, jQuery)); 

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
		buttons: { "Back": function () { this.close(); } },
		dialogName: "content_dialog",
		contentId: "content"
	}
	$.extend(config, settings);

	var dialog = (function (dialog, config, $, undefined) {

		var content = $("#" + config.contentId);

		// create buttons
		var createButtons = function (buttons) {
			var buttonPanel = $('<div class="buttons"></div>');

			$.each(buttons, function (name, props) {
				var self = dialog;

				props = $.isFunction(props) ? { click: props, text: name} : props;
				var button = $('<button type="button"></button>')
					.attr(props, true)
					.unbind('click')
					.click(function () {
						props.click.apply(self, arguments);
					})
					.appendTo(buttonPanel);
				if ($.fn.button) {
					button.button();
				}
			});

			return buttonPanel;
		}

		// load all current visible items and hide them
		var visibleItems = $(">:visible", content).hide().toArray();

		// inner content
		dialog.inner = $('<div class="inner"></div>').css("overflow", "hidden").height("0px"); ;

		// create dialog wrapper
		var contentDialog = $('<div class="' + config.dialogName + '"></div>');
		content.append(contentDialog);
		contentDialog.append(createButtons(config.buttons));
		contentDialog.append(dialog.inner);
		contentDialog.append(createButtons(config.buttons));


		// fill content
		fillContentWithData(dialog.inner, config.data);

		dialog.inner.hide().css("overflow", "none").height("auto");

		dialog.inner.show("slide", { "direction": "right" })

		// close dialog
		dialog.close = function (action) {
			$(contentDialog).remove();
			$(visibleItems).show("slide");
			if (config.prevUrl !== null) {
				addToHistory(config.prevUrl);
			}
		};

		return dialog;
	} ({}, config, $));

	return dialog;
}

function fillContentWithData(content, data) {
	content.html(data);
	$(".ajax_ignore", content).hide();
	var form = $("form", content);
	if (form.length
		&& jQuery.validator && jQuery.validator.unobtrusive) {
		jQuery.validator.unobtrusive.parse(content);
	}
}

function updateEffect(content, callback) {
	content.effect("slide", { direction: "up" }, 300, callback);
}

var current_url = undefined;

// ajax history function
function addToHistory(url) {
	current_url = url;
	$.bbq.pushState({ url: url }); 
}
// get current ajax url
function getCurrentHistoryUrl() {
	$.bbq.getState("url"); 
}

// initialize ajax history plugin
$(function () {
	// Bind a callback that executes when document.location.hash changes.
	$(window).bind("hashchange", function (e) {
		// In jQuery 1.4, use e.getState( "url" );
		var url = $.bbq.getState("url");
		if (url != current_url) {
			if (!url) {
				window.location = window.location.href;
			}
			else {
				window.location = url;
			}
		}
	});

	// Since the event is only triggered when the hash changes, we need
	// to trigger the event now, to handle the hash the page may have
	// loaded with.
	$(window).trigger("hashchange");

});