(function ($) {

	$.fn.ajaxGrid = function (settings) {
		var config = { content: this };

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
			$(".grid_edit:visible")
				.click(function () {
					createLoading();
					var nextUrl = $(this).attr("href");
					$.ajax({
						url: nextUrl + (nextUrl.indexOf("?") > 0 ? "&" : "?") + "ajaxTime=" + new Date().getTime(),
						success: function (data) {
							var edit = createContentDialog({
								data: data,
								buttons: {
									"Save": {
										text: "Save",
										"class": "f_right",
										click: function () {
											var form = $("form", this.inner);
											if (form.valid()) {
												showForm(edit, form);
											}
										}
									},
									"Back": function () { this.close(); }
								},
								prevUrl: getCurrentHistoryUrl()
							});
							closeLoading();
							addToHistory(nextUrl);
						},
						error: ajaxException
					});
					return false;
				})
				.button({
					icons: {
						primary: 'ui-icon-pencil'
					},
					text: false
				});

			// detail buttons
			$(".grid_details:visible")
				.click(function () {
					createLoading();
					var nextUrl = $(this).attr("href");
					$.ajax({
						url: nextUrl + (nextUrl.indexOf("?") > 0 ? "&" : "?") + "ajaxTime=" + new Date().getTime(),
						success: function (data) {
							var edit = createContentDialog({
								data: data,
								prevUrl: getCurrentHistoryUrl()
							});
							closeLoading();
							addToHistory(nextUrl);
						},
						error: ajaxException
					});
					return false;
				})
				.button({
					icons: {
						primary: 'ui-icon-note'
					},
					text: false
				});

			// delete buttons
			$(".grid_delete:visible")
				.click(function () {
					createLoading();
					var nextUrl = $(this).attr("href");
					$.ajax({
						url: nextUrl + (nextUrl.indexOf("?") > 0 ? "&" : "?") + "ajaxTime=" + new Date().getTime(),
						success: function (data) {
							var edit = createContentDialog({
								data: data,
								buttons:
								{
									"Delete": {
										text: "Delete",
										"class": "f_right",
										click: function () {
											var form = $("form", this.inner);
											showForm(edit, form);
										}
									},
									"Back": function () { this.close(); }
								},
								prevUrl: getCurrentHistoryUrl()
							});
							closeLoading();
							addToHistory(nextUrl);
						},
						error: ajaxException
					});
					return false;
				})
				.button({
					icons: {
						primary: 'ui-icon-trash'
					},
					text: false
				});
			// create buttons
			$(".grid_create:visible")
			.click(function () {
				createLoading();
				var nextUrl = $(this).attr("href");
				$.ajax({
					url: nextUrl + (nextUrl.indexOf("?") > 0 ? "&" : "?") + "ajaxTime=" + new Date().getTime(),
					success: function (data) {
						var edit = createContentDialog({
							data: data,
							buttons:
							{
								"Create": {
									text: "Create",
									"class": "f_right",
									click: function () {
										var form = $("form", this.inner);
										if (form.valid()) {
											showForm(edit, form);
										}
									}
								},
								"Back": function () { this.close(); }
							},
							prevUrl: getCurrentHistoryUrl()
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