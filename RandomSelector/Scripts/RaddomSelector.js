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
	};
}