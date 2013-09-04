// script cache handling
(function (scriptCache, $, undefined) {
	var loadedScripts = [];
	var syncScriptsQueue = [];

	$.ajaxPrefilter(function (options, originalOptions, jqXHR) {
		if (options.dataType == 'script' || originalOptions.dataType == 'script') {
			options.cache = true;
		}
	});

	scriptCache.loadSync = function (scriptUrl, fallbackFile) {
		syncScriptsQueue.push({ scriptUrl: scriptUrl, fallbackFile: fallbackFile });
		loadSync();
	};

	function loadSync(completeCall) {
		// if not complete call and not just one item in queue, then stop
		if (!completeCall && syncScriptsQueue.length != 1) {
			return;
		}
		// if complete call we can remove script
		if (completeCall) {
			syncScriptsQueue.splice(0, 1);
		}
		if (syncScriptsQueue.length == 0) {
			return;
		}
		scriptCache.load(syncScriptsQueue[0].scriptUrl, syncScriptsQueue[0].fallbackFile, loadSync);
	};

	scriptCache.load = function (scriptUrl, fallbackFile, loaded) {
		// if script is not yet loaded
		if (!loadedScripts[scriptUrl]) {
			$.ajax({
				url: scriptUrl,
				dataType: "script",
				cache: true,
				async: false,
				complete: function (jqXHR, textStatus) {
					// if empty response 
					if (!jqXHR.responseText && fallbackFile) {
						$.ajax({
							url: fallbackFile,
							dataType: "script",
							cache: true,
							async: false,
							complete: loaded
						});
					}
					else {
						if (loaded) {
							loaded(true);
						}
					}
				}
			});
			loadedScripts[scriptUrl] = true;
		}
		else {
			if (loaded) {
				loaded(true);
			}
		}
	};
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
	var popup = $('<div><iframe class="dialogIFrame" frameborder="0" marginheight="0" marginwidth="0" width="100%" height="100%"></iframe></div>');
	popup.dialog({
			autoOpen: true,
			width: $("#content").width(),
			height: 500,
			modal: true,
			title: "Error:" + thrownError
		});
	setTimeout(function() {
		var doc = $("iframe", popup)[0].contentWindow.document;
		var $body = $('body', doc);
		$body.html(xhr.responseText);
	}, 10);
	//alert(thrownError);
}

// create content dialog
function createContentDialog(settings) {
	var config = {
		dialogName: "content_dialog",
		contentId: "content"
	};
	$.extend(config, settings);

	var dialog = (function (dialog, config, $, undefined) {

		var content = $("#" + config.contentId);

		// crate back panel
		var createBackPanel = function (buttons) {
			var backPanel = $('<div></div>');

			$.each(buttons, function (name, props) {
				var self = dialog;

				props = $.isFunction(props) ? { click: props, text: name} : props;
				var button = $('<a>')
					.attr(props, true)
					.unbind('click')
					.click(function () {
						props.click.apply(self, arguments);
					})
					.appendTo(backPanel);
			});

			return backPanel;
		};

		// load all current visible items and hide them
		var visibleItems = $(">:visible", content).hide().toArray();

		// inner content
		dialog.inner = $('<div class="inner"></div>').css("overflow", "hidden").height("0px"); ;

		// create dialog wrapper
		var contentDialog = $('<div class="' + config.dialogName + '"></div>');
		content.append(contentDialog);
		contentDialog.append(createBackPanel(
			{ "Back": {
				"class": "action back",
				html: '<span class="icon"></span><span class="caption">Back</span>',
				title: "Back",
				click: function () { this.close(); }
			}
			}));
		contentDialog.append(dialog.inner);

		// fill content
		fillContentWithData(dialog.inner, config.data);
		$("form :submit", dialog.inner).click(function(event) {
			event.preventDefault();
			settings.submit($("form", dialog.inner));
		});

		dialog.inner.hide().css("overflow", "none").height("auto");

		dialog.inner.show("slide", { "direction": "right" }, function () {
		    $(":focusable:first", dialog.inner)
			    .not(':input[type=button], :input[type=submit], :input[type=reset]')
		        .click().focus();
		});
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
	$(":focusable:first", form).click().focus();
}

function updateEffect(content, callback) {
	content.effect("slide", { direction: "up" }, 300,
		function() {
			$('html, body').animate({
					scrollTop: $(content).offset().top
				}, 300);
			callback && callback();
		});
}



var current_url = undefined;

// ajax history function
function addToHistory(url) {
	current_url = url;
	$.bbq.pushState({ url: url }); 
}
// get current ajax url
function getCurrentHistoryUrl() {
	var url = $.bbq.getState("url"); 
	if (!url) {
		url = window.location.pathname;
	}
	return url;
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

// jquery append workaround for support of html5 elements in ie less that 9
$(function () {
	if ($.browser.msie && $.browser.version < 9) {
		$.fn.append = function () {

			this.domManip(arguments, true, function (E) {
				if (this.nodeType == 1) {

					if (E.nodeType == 11) E = innerShiv(E, false);
					if (E.nodeType) {
						this.appendChild(E);
					}
					else {
						while (E.length) {
							this.appendChild(E[0]);
						}
					}
				}
			});
			return this;
		};
	}
});
// http://jdbartlett.github.com/innershiv | WTFPL License
window.innerShiv = (function () {
	var d, r;
	return function (h, u) {
		if (typeof h != "string") {
			//convert h to string (w/o jQuery)
			var t = document.createElement('div');
			/*@cc_on document.body.appendChild(t); @*/
			t.appendChild(h);
			h = t.innerHTML;
			/*@cc_on document.body.removeChild(t); @*/
		}
		if (!d) {
			d = document.createElement('div');
			r = document.createDocumentFragment();
			/*@cc_on d.style.display = 'none'; @*/
		}
		var e = d.cloneNode(true);
		/*@cc_on document.body.appendChild(e); @*/
		e.innerHTML = h.replace(/^\s\s*/, '').replace(/\s\s*$/, '');
		/*@cc_on document.body.removeChild(e); @*/

		if (u === false) return e.childNodes;

		var f = r.cloneNode(true), i = e.childNodes.length;
		while (i--) f.appendChild(e.firstChild);
		return f;
	};
} ());

(function ($) {
	$.fn.ajaxClick = function(settings) {

		if (typeof content === 'undefined') {
			content = "#content";
		}

		var config = $.extend({ }, $.fn.ajaxClick.defaults, settings);

		return this.each(function() {
			var $this = $(this);

			$this.click(function() {
				createLoading();
				var nextUrl = $(this).attr("href");
				$.ajax({
					type: "POST",
					url: nextUrl + (nextUrl.indexOf("?") > 0 ? "&" : "?") + "ajaxTime=" + new Date().getTime(),
					success: function(data) {
						var content = $(config.content);
						content.html(data);
						closeLoading();
						updateEffect(content);
					},
					error: ajaxException
				});
				return false;
			});
		});
	};

	$.fn.ajaxClick.defaults = {
		content: '#content'
	};
	
	$.fn.ajaxRequest = function(link, settings) {

		var config = $.extend({ }, $.fn.ajaxRequest.defaults, settings);

		return this.each(function() {
			var $this = $(this);

			var nextUrl = link;

			createLoading();
			$.ajax({
				type: "POST",
				url: nextUrl + (nextUrl.indexOf("?") > 0 ? "&" : "?") + "ajaxTime=" + new Date().getTime(),
				success: function(data) {
					$this.html(data);
					closeLoading();
					updateEffect($this);
				},
				error: ajaxException
			});
			return false;
		});
	};

	$.fn.ajaxRequest.defaults = {
	};
})(jQuery);