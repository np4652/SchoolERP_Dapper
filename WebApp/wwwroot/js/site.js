(() => {
    $('body').on('click', 'input[type="checkbox"]', function () {
        if ($(this).prop('checked')) {
            $(this).attr('value', true);
        } else {
            $(this).attr('value', false);
        }
    });
    $('body').on('submit', 'form', function () {
        ajaxFormSubmit(this)
    });
})();