(function ($) {

	$.fn.ajaxGrid = function (settings) {
		var config = { content: this };

		var showForm = function (dialog, form) {
			createLoading();
			$.ajax({
				type: "POST",
				url: form.attr("action"),
				data: form.serialize(),
				success: function (data) {
					if (data.actionName) {
						$(dialog).dialog("close");
						$.ajax({
							url: (data.area ? "/" + data.area : "") + "/" + data.controllerName + "/" + data.actionName + "?ajaxTime=" + new Date().getTime(),
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

		if (settings) $.extend(config, settings);

		this.each(function () {
			// edit buttons
			$(".grid_edit")
				.click(function () {
					createLoading();
					$.ajax({
						url: this.href + "?ajaxTime=" + new Date().getTime(),
						success: function (data) {
							var edit = createContentDialog('Edit', data,
								{
									"Save": function () {
										var form = $("form", this);
										if (form.valid()) {
											showForm(edit, form);
										}
									},
									"Cancel": function () { $(this).dialog("close") }
								});
							closeLoading();
							edit.dialog("open");
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
					$.ajax({
						url: this.href + "?ajaxTime=" + new Date().getTime(),
						success: function (data) {
							var edit = createContentDialog('Detail', data);
							closeLoading();
							edit.dialog("open");
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
					$.ajax({
						url: this.href + "?ajaxTime=" + new Date().getTime(),
						success: function (data) {
							var edit = createContentDialog('Delete', data,
								{
									"Delete": function () {
										var form = $("form", this);
										showForm(edit, form);
									},
									"Cancel": function () { $(this).dialog("close") }
								});
							closeLoading();
							edit.dialog("open");
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
				$.ajax({
					url: this.href + "?ajaxTime=" + new Date().getTime(),
					success: function (data) {
						var edit = createContentDialog('Create', data,
							{
								"Save": function () {
									var form = $("form", this);
									if (form.valid()) {
										showForm(edit, form);
									}
								},
								"Cancel": function () { $(this).dialog("close") }
							});
						closeLoading();
						edit.dialog("open");
					},
					error: ajaxException
				});
				return false;
			});

		});

		return this;

	};

})(jQuery);