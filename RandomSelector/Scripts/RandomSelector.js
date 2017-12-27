var RandomSelector = RandomSelector || {};

$(function () {
    RandomSelector.initialize();
});

new function () {
    RandomSelector.initialize = function () {
        // 拡張でプロモが選択された場合
        $('input[name=ExpansionIDList][value=Promotion]').on('change', function () {
            if ($(this).prop('checked')) {
                $('#promCard').show();
            } else {
                $('#promCard').hide();
            }
        });

        // 拡張選択Helper
        $('#allExpansionSelect').on('click', function () {
            $('#expansionArea input[name=ExpansionIDList][type=checkbox]:not([value=Promotion])').prop('checked', true).change();
            return false;
        });
        $('#allExpansionClear').on('click', function () {
            $('#expansionArea input[name=ExpansionIDList][type=checkbox]:not([value=Promotion])').prop('checked', false).change();
            return false;
        });
        $('#priorityClear').on('click', function () {
            $('#expansionArea input[type=radio][name=PriorityExpansion]').prop('checked', false);
            return false;
        });

        // PageTopAnchor
        $('a.toTop').on('click', function () {
            $('html, body').animate({ scrollTop: 0 }, 400);
        });

        // 推奨セット
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