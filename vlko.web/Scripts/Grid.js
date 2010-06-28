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
						$(dialog).dialog("close");
						var nextUrl = (data.area ? "/" + data.area : "") + "/" + data.controllerName + "/" + data.actionName;
						$.ajax({
							url: nextUrl + "?ajaxTime=" + new Date().getTime(),
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
						fillContentWithData(dialog, data);
					}
					closeLoading();
				},
				error: ajaxException
			});
		}

		this.each(function () {
			// edit buttons
			$(".grid_edit")
				.click(function () {
					createLoading();
					var nextUrl = $(this).attr("href");
					$.ajax({
						url: nextUrl + "?ajaxTime=" + new Date().getTime(),
						success: function (data) {
							var edit = createContentDialog({
								title: 'Edit', 
								data: data,
								buttons: {
									"Save": function () {
										var form = $("form", this);
										if (form.valid()) {
											showForm(edit, form);
										}
									},
									"Cancel": function () { $(this).dialog("close") }
								},
								prevUrl: getCurrentHistoryUrl()
								});
							closeLoading();
							edit.dialog("open");
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
			$(".grid_details")
				.click(function () {
					createLoading();
					var nextUrl = $(this).attr("href");
					$.ajax({
						url: nextUrl + "?ajaxTime=" + new Date().getTime(),
						success: function (data) {
							var edit = createContentDialog({
								title: 'Detail', 
								data: data,
								prevUrl: getCurrentHistoryUrl()
								});
							closeLoading();
							edit.dialog("open");
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
			$(".grid_delete")
				.click(function () {
					createLoading();
					var nextUrl = $(this).attr("href");
					$.ajax({
						url: nextUrl + "?ajaxTime=" + new Date().getTime(),
						success: function (data) {
							var edit = createContentDialog({
								title: 'Delete',
								data: data,
								buttons:
								{
									"Delete": function () {
										var form = $("form", this);
										showForm(edit, form);
									},
									"Cancel": function () { $(this).dialog("close") }
								},
								prevUrl: getCurrentHistoryUrl()
								});
							closeLoading();
							edit.dialog("open");
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
			$(".grid_create")
			.click(function () {
				createLoading();
				var nextUrl = $(this).attr("href");
				$.ajax({
					url: nextUrl + "?ajaxTime=" + new Date().getTime(),
					success: function (data) {
						var edit = createContentDialog({
							title: 'Create',
							data: data,
							buttons:
							{
								"Save": function () {
									var form = $("form", this);
									if (form.valid()) {
										showForm(edit, form);
									}
								},
								"Cancel": function () { $(this).dialog("close") }
							},
							prevUrl: getCurrentHistoryUrl()
							});
						closeLoading();
						edit.dialog("open");
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