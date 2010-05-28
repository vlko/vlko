$(AttachPagerAjax);
$("#content").ajaxSuccess(AttachPagerAjax);

function AttachPagerAjax() {
	$(".pager a:not(.loading)")
		.click(function () {
			createLoading();
			$.ajax({
			    url: this.href +"&ajaxTime=" + new Date().getTime(),
				success: function (data) {
					var content = $("#content");
					content.html(data);
					closeLoading();
					content.effect("pulsate", { times: 1 }, 500);
				},
				error: ajaxException
			});
			return false;
		}).addClass("loading")
		.button();
	$(".pager .pager_next").button({
		icons: {
			primary: 'ui-icon-triangle-1-e'
		},
		text: false
	});
	$(".pager .pager_prev").button({
		icons: {
			primary: 'ui-icon-triangle-1-w'
		},
		text: false
	});
}