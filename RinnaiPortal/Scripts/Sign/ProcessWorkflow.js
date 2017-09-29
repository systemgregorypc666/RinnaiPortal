$(function () {

    var formType = $('iframe').attr('src');
    var ifrmaeRules =
        [
            { "regex": /OvertimeList.aspx/ig, "class": "overtime-size" },
            { "regex": /ForgotPunchList.aspx/ig, "class": "forgotPunch-size" },
            { "regex": /TrainDetail.aspx/ig, "class": "train-size" },
            { "regex": /PageNotFound.html/ig, "class": "pageNotFound-size" },


        ];

    $.each(ifrmaeRules, function (i, rule) {
        if (formType.match(rule["regex"])) {
            $("#MainContent_FormContent").addClass(rule["class"]);
        }
    });
    

});