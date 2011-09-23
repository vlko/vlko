(function ($) {

	$.fn.ajaxPager = function (settings) {
		var config = { content: this };

		if (settings) $.extend(config, settings);

		this.each(function () {
			$(".pagination a[href]:not(.loading)", config.content)
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
				}).addClass("loading");
		});

		return this;

	};

})(jQuery);