$(AttachGridAjax);
$("#content").ajaxSuccess(AttachGridAjax);

function AttachGridAjax() {

	// edit buttons
	$(".grid_edit:not(.loaded)")
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
									createLoading();
									$.ajax({
										type: "POST",
										url: form.attr("action"),
										data: form.serialize(),
										success: function (data) {
											if (data.actionName) {
												$(edit).dialog("close");
												$.ajax({
													url: "/" + data.controllerName + "/" + data.actionName + "?ajaxTime=" + new Date().getTime(),
													success: function (data) {
														var content = $("#content");
														content.html(data);
														closeLoading();
														content.effect("pulsate", { times: 1 }, 500);
													},
													error: ajaxException
												});
											}
											else {
												fillContentWithData(edit, data);
											}
											closeLoading();
										},
										error: ajaxException
									});
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
		}).addClass("loaded");

	// detail buttons
	$(".grid_details:not(.loaded)")
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
		}).addClass("loaded");

	// delte buttons
	$(".grid_delete:not(.loaded)")
		.click(function () {
			createLoading();
			$.ajax({
				url: this.href + "?ajaxTime=" + new Date().getTime(),
				success: function (data) {
					var edit = createContentDialog('Delete', data,
						{
							"Delete": function () {
								createLoading();
								var form = $("form", this);
								$.ajax({
									type: "POST",
									url: form.attr("action"),
									data: form.serialize(),
									success: function (data) {
										if (data.actionName) {
											$(edit).dialog("close");
											$.ajax({
												url: "/" + data.controllerName + "/" + data.actionName + "?ajaxTime=" + new Date().getTime(),
												success: function (data) {
													var content = $("#content");
													content.html(data);
													closeLoading();
													content.effect("pulsate", { times: 1 }, 500);
												},
												error: ajaxException
											});
										}
										else {
											fillContentWithData(edit, data);
											//edit.effect("pulsate", { times: 1 }, 500);
										}
										closeLoading();
									},
									error: ajaxException
								});
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
		}).addClass("loaded");

	// create buttons
		$(".grid_create:not(.loaded)")
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
										createLoading();
										$.ajax({
											type: "POST",
											url: form.attr("action"),
											data: form.serialize(),
											success: function (data) {
												if (data.actionName) {
													$(edit).dialog("close");
													$.ajax({
														url: "/" + data.controllerName + "/" + data.actionName + "?ajaxTime=" + new Date().getTime(),
														success: function (data) {
															var content = $("#content");
															content.html(data);
															closeLoading();
															content.effect("pulsate", { times: 1 }, 500);
														},
														error: ajaxException
													});
												}
												else {
													fillContentWithData(edit, data);
												}
												closeLoading();
											},
											error: ajaxException
										});
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
			.addClass("loaded");
}