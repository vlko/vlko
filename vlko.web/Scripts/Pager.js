(function ($) {

	$.fn.ajaxPager = function (settings) {
		var config = { content: this };

		if (settings) $.extend(config, settings);

		this.each(function () {
			$(".pager a:not(.loading)", config.content)
				.click(function () {
					createLoading();
					var nextUrl = $(this).attr("href");
					$.ajax({
						url: nextUrl + "&ajaxTime=" + new Date().getTime(),
						success: function (data) {
							var content = $(config.content);
							content.html(data);
							closeLoading();
							addToHistory(nextUrl);
							updateEffect(content);
						},
						error: ajaxException
					});
					return false;
				}).addClass("loading")
				.button();
			$(".pager .pager_next", config.content).button({
				icons: {
					primary: 'ui-icon-triangle-1-e'
				},
				text: false
			});
			$(".pager .pager_prev", config.content).button({
				icons: {
					primary: 'ui-icon-triangle-1-w'
				},
				text: false
			});
		});

		return this;

	};

})(jQuery);