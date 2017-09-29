$(function () {
    $('[commandname=SignSubmit]').click(function () {
        
        if (!confirm("確認送出簽核?")) { return;}
        var $tr = $(this).parents('tr');
        if ($tr.length == 0) { return false; }

        var $submit = $tr.find(".SignSubmit");
        var $coverBtn = $tr.find(".Coverbtn");


        var data = {};
        data['ApplyID_FK'] = $tr.find('.SID').text();
        data['ApplyName'] = $tr.find('.SNAME').text();
        data['DepartmentID_FK'] = $tr.find('.UNITID').text();
        data['DepartmentName'] = $tr.find('.UNITNAME').text();
        data['SignDocID_FK'] = $tr.find('.SIGNDOCID').text();
        data['CLID'] = $tr.find('.CLID').text();
        data['FormSeries'] = $('#FormSeries').val();

        $.ajax({
            type: "post",
            url: "../../../Service/Sign/Forms/Train.asmx/ApplyForm",
            data: JSON.stringify(data),
            success: function (responses) {
                if (confirm(responses['message'] + '即將為您跳轉!')) {
                    window.parent.location.href = "../WorkflowQueryList.aspx?orderField=SendDate&descending=True";
                } else {
                    $('#MainContent_queryBtn').click();
                }
            },
            error: function (responses) {
                alert(responses.responseJSON['message']);
            },
            beforeSend: function () {
                $submit.hide();
                $coverBtn.show();
            },
            complete: function () {
                $coverBtn.hide();
                $submit.show();
            },
        });

        
    });
});