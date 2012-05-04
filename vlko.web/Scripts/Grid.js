(function ($) {

	$.fn.ajaxGrid = function (settings) {
		var config = {
			content: this, 
			prevUrl: getCurrentHistoryUrl()
		};

		if (settings) $.extend(config, settings);

		var showForm = function (dialog, form) {
			createLoading();
			$.ajax({
				type: "POST",
				url: form.attr("action"),
				data: form.serialize(),
				success: function (data) {
					if (data.actionName) {
						dialog.close();
						var nextUrl = data.actionName;
						$.ajax({
							url: nextUrl + (nextUrl.indexOf("?") > 0 ? "&" : "?") + "ajaxTime=" + new Date().getTime(),
							success: function (data) {
								var content = $(config.content);
								content.html(data);
								closeLoading();
								updateEffect(content);
							},
							error: ajaxException
						});
					}
					else {
						fillContentWithData(dialog.inner, data);
					}
					closeLoading();
				},
				error: ajaxException
			});
		}

		this.each(function () {
			// edit buttons
			$(".action:.edit:visible")
				.click(function () {
					createLoading();
					var nextUrl = $(this).attr("href");
					$.ajax({
						url: nextUrl + (nextUrl.indexOf("?") > 0 ? "&" : "?") + "ajaxTime=" + new Date().getTime(),
						success: function (data) {
							var edit = createContentDialog({
								data: data,
								submit: function (form) {
											if (form.valid()) {
												showForm(edit, form);
											}
										},
								prevUrl: config.prevUrl
							});
							closeLoading();
							addToHistory(nextUrl);
						},
						error: ajaxException
					});
					return false;
				});

			// detail buttons
			$(".action.detail:visible")
				.click(function () {
					createLoading();
					var nextUrl = $(this).attr("href");
					$.ajax({
						url: nextUrl + (nextUrl.indexOf("?") > 0 ? "&" : "?") + "ajaxTime=" + new Date().getTime(),
						success: function (data) {
							var edit = createContentDialog({
								data: data,
								prevUrl: config.prevUrl
							});
							closeLoading();
							addToHistory(nextUrl);
						},
						error: ajaxException
					});
					return false;
				});

			// delete buttons
				$(".action.delete:visible")
				.click(function () {
					createLoading();
					var nextUrl = $(this).attr("href");
					$.ajax({
						url: nextUrl + (nextUrl.indexOf("?") > 0 ? "&" : "?") + "ajaxTime=" + new Date().getTime(),
						success: function (data) {
							var edit = createContentDialog({
								data: data,
								submit: function (form) {
											showForm(edit, form);
										},
								prevUrl: config.prevUrl
							});
							closeLoading();
							addToHistory(nextUrl);
						},
						error: ajaxException
					});
					return false;
				});
			// create buttons
			$(".action.create:visible")
			.click(function () {
				createLoading();
				var nextUrl = $(this).attr("href");
				$.ajax({
					url: nextUrl + (nextUrl.indexOf("?") > 0 ? "&" : "?") + "ajaxTime=" + new Date().getTime(),
					success: function (data) {
						var edit = createContentDialog({
							data: data,
							submit: function (form) {
										if (form.valid()) {
											showForm(edit, form);
										}
									},
							prevUrl: config.prevUrl
						});
						closeLoading();
						addToHistory(nextUrl);
					},
					error: ajaxException
				});
				return false;
			});

		});

		return this;

	};

})(jQuery);