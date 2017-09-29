$(function () {
    var alertWidget = '<div id="errorMsg" class="alert alert-danger display-none" role="alert" data-valmsg-summary="true">' +
                    '<span class="glyphicon glyphicon-exclamation-sign" aria-hidden="true"></span>' +
                    '<span class="sr-only">Error:</span>' +
                    '<ul></ul>' +
                    '</div>';

    $('form').on('invalid-form', function (event, validator) {
        var $summary = $(alertWidget).insertAfter('.page-title');
        $summary.css(
            {
                'position': 'fixed',
                'left': '50%',
                'margin-left': '-150px',
                'width': '300px',
                'z-index': '10',
            });

        $(validator.errorList).map(function() {
            return $('<li>' + this.message + '</li>').get();
        }).appendTo($summary.find('ul'));

        $summary.slideDown(2000).delay(3000).slideUp(2000);
        ;
    });
})