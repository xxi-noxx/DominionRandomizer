var RandomSelector = RandomSelector || {};

$(function () {
	RandomSelector.initialize();
});

new function () {
	RandomSelector.initialize = function () {
		$('#ExpansionIDList_99').on('change', function () {
			if ($(this).prop('checked')) {
				$('#promCard').show();
			} else {
				$('#promCard').hide();
			}
		});

		$('a.toTop').on('click', function () {
			$('html, body').animate({ scrollTop: 0 }, 400);
		});

		$('#recommendList').on("change", function () {
			if ($(this).val() !== '') {
				$('html, body').animate({ scrollTop: $('#' + $(this).val()).offset().top - 100 });
			}
		});

		$('#recommendJump').on('click', function () {
			$('#recommendList').trigger('change');
		});
	};
};