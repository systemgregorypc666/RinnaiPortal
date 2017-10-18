<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Overtime.aspx.cs" Inherits="RinnaiPortal.Area.Sign.Forms.Overtime" %>

<asp:Content ID="OvertimeContent" ContentPlaceHolderID="MainContent" runat="server">
    <style type="text/css">
        .container {
            width: 100% !important;
        }

        body input {
            font-size: 1vh;
        }

        .employeeID {
            width: 80px;
        }

        .employeeName {
            width: 150px;
        }

        .realendDateTime {
            width: 130px;
        }


        .supportDept {
            width: 145px;
        }

        .note {
            width: 160px;
        }

        .payType {
            width: 8%;
        }

        .mealOrder {
            width: 7%;
        }

        .block:hover {
            background-color: #f5f5ff;
            cursor: pointer;
        }

        .th-required::before {
            content: "✱";
            color: red;
            padding-right: 5px;
        }

        .auto-style1 {
            color: #FF0000;
            font-weight: 700;
        }

        #layout-content > div.OvertimeSummary.panel.panel-default > table > tbody > tr:nth-child(1) > th.employeeName.th-required {
            text-align: center;
            width: 14%;
        }

        #layout-content > div.OvertimeSummary.panel.panel-default > table > tbody > tr:nth-child(1) > th.realendDateTime.th-required {
            width: 12%;
        }

        .startDateTime, .endDateTime {
            width: 20%;
        }

        .diaplayTime {
            margin-top: 5px;
        }

        .supportDept, {
            width: 13%;
        }
    </style>
    <script type="text/javascript">
        $(function () {
            $('#DefaultDeptName').select2();
            $("#DefaultSupportDeptName").select2();


            $('.block').click(function () {
                if ($(this).find('i').hasClass('glyphicon-chevron-up')) {
                    $(this).find('i').removeClass('glyphicon-chevron-up').addClass('glyphicon-chevron-down');
                    $('.OvertimeSetting, .OvertimeTitle').hide();
                } else {
                    $(this).find('i').removeClass('glyphicon-chevron-down').addClass('glyphicon-chevron-up');
                    $('.OvertimeSetting, .OvertimeTitle').show();
                }
            });

            $("[selectAll]").change(function () {
                $($(this).attr('selectAll')).prop("checked", $(this).prop("checked"));
            });

            var addRows = function (responses) {
                $.each(responses, function (i, response) {
                    //debugger;
                    var $res = $(response).appendTo($('.OvertimeSummary table'));
                    var $datetimeElem = $res.find(".startDateTime,.endDateTime");
                    $datetimeElem.find('input').datetimepicker();

                    $('.supportDept select').select2();
                    //新增 #0007 為了時間容易顯示不被檔住

                    ////將DateTimepicker綁定事件
                    //$datetimeElem.find('input').change(function () {
                    //    //新增的input顯示時間
                    //    $(this).next('.diaplayTime').val($(this).val().substr(10, $(this).val().length - 1));
                    //});

                    //$datetimeElem.each(function (i, elem) {
                    //    var $timeInput = $('<input type="text" />')
                    //    $timeInput.addClass('form-control');
                    //    $timeInput.addClass('diaplayTime');
                    //    $timeInput.attr('data-date-format', 'HH:mm:00');
                    //    $timeInput.prop('disabled', true);
                    //    $timeInput.datetimepicker();
                    //    var timeValuelue = $(elem).find('div').find('input').val();
                    //    var getTimeValue = timeValuelue.substr(10, timeValuelue.length - 1)
                    //    $timeInput.attr('value', getTimeValue);
                    //    $(elem).find('div').append($timeInput);
                    //});

                    //新增 #0007

                    $res.find(".employeeName select").change(function () {
                        //console.log($(this));
                        var $this = $(this);
                        $this.closest('tr').find('.employeeID input').val($this.val());
                        //debugger;
                        var nationalType = $this.find("option:selected").attr('nationaltype');
                        //var nationalType = $(this).children(":selected").attr('nationaltype');
                        $this.closest('tr').find('.nationalType').val(nationalType);
                        //先清空部門資料, 等待 response取回
                        $this.closest('tr').find('.departmentID').val('');
                        $this.closest('tr').find('.departmentName').val('');
                        $.ajax({
                            type: "post",
                            url: "../../../Service/Sign/Forms/Overtime.asmx/GetDepartmentData",
                            data: { "employeeID": $this.closest('tr').find('.employeeID input').val() },
                            success: function (responses) {
                                $this.closest('tr').find('.departmentID').val(responses['DepartmentID']);
                                $this.closest('tr').find('.departmentName').val(responses['DepartmentName']);
                            },
                            error: function (e) {
                                alert('新增多列失敗!' + e.responseJSON.ErrorMessage);
                                console.log(e.responseJSON);
                            }
                        });
                    })
                });
            };

            var getDefaultData = function () {
                return {
                    "ApplyID_FK": $('#ApplyID_FK').val(),
                    "DepartmentID_FK": $('#DepartmentID_FK').val(),
                    "DefaultDeptID": $('#DefaultDeptName option:selected').val(),
                    "DefaultDeptName": $('#DefaultDeptName option:selected').text(),
                    "DefaultStartDateTime": $('#DefaultStartDateTime').val(),
                    "DefaultSupportDeptID": $('#DefaultSupportDeptName option:selected').val(),
                    "DefaultSupportDeptName": $('#DefaultSupportDeptName  option:selected').text(),
                    "DefaultMealOrderKey": $('#DefaultMealOrderValue option:selected').val(),
                    "DefaultMealOrderValue": $('#DefaultMealOrderValue option:selected').text(),
                    "DefaultPayTypeKey": $('#DefaultPayTypeValue option:selected').val(),
                    "DefaultPayTypeValue": $('#DefaultPayTypeValue option:selected').text(),
                    "DefaultEndDateTime": $('#DefaultEndDateTime').val(),
                    "DefaultNote": $('#DefaultNote').val(),
                    "DefaultRealEndDateTime": $('#DefaultRealEndDateTime').val(),
                };
            }

            $('.CreateDetailData').click(function () {
                var note = $('#DefaultNote').val();

                if (note.length > 100)
                {
                    alert('加班原因不得超過100字!');
                    return;
                }

                var defaultData = getDefaultData();
                var isValid = true;
                $('.required input, .required select').each(function () {
                    if (!this.value) {
                        isValid = false;
                        return false;
                    };
                });
                if (!isValid) { alert('預設支援單位為必填!\n預設加班時間(起)為必填!\n預設加班時間(迄)為必填!'); return false; }


                $('.OvertimeSetting, .OvertimeTitle').hide();
                $('.OvertimeSummary, .save-footer').removeClass('hidden');
                $('.block').click();



                $.ajax({
                    type: "post",
                    url: "../../../Service/Sign/Forms/Overtime.asmx/CreateTableByDefaultValue",
                    data: defaultData,
                    success: function (responses) {
                        addRows(responses);

                    },
                    beforeSend: function () {
                        $('.create-footer .CreateDetailData').hide();
                        $('.create-footer.CoverBtn').show();
                        $('.data-loadding').show();
                        $('.save-footer').hide();
                        $('.AddRow').hide();
                    },
                    complete: function () {
                        $('.create-footer .CreateDetailData').show();
                        $('.create-footer .CoverBtn').hide();
                        $('.data-loadding').hide();
                        $('.save-footer').show();
                        $('.AddRow').show();
                    },
                    error: function (e) {
                        alert("產生明細資料失敗!" + e.responseJSON.ErrorMessage);
                        console.log(e.responseJSON);
                    }
                });
            });

            $('.AddRow').click(function () {
                var defaultData = getDefaultData();
                defaultData['IsAddRow'] = 'True';
                //debugger;
                var $NewRowEmpID = $('#NewRowEmpID');
                if ($NewRowEmpID.val()) { defaultData.NewRowEmpID = $NewRowEmpID.val(); }

                $.ajax({
                    type: "post",
                    url: "../../../Service/Sign/Forms/Overtime.asmx/CreateTableByDefaultValue",
                    data: defaultData,
                    success: function (responses) {
                        addRows(responses);
                    },
                    error: function (e) {
                        alert(e.responseJSON.ErrorMessage);
                        console.log(e.responseJSON);
                    }
                });
            });

            $('.SaveData').click(function () {
                var applyID = $('#ApplyID_FK').val();
                var applyDeptID = $('#ApplyDeptID_FK').val();
                var signDocID = $('#SignDocID').val();
                var formSeries = $('#FormSeries').val();
                var request = [];

                var isValid = true;
                var verifyTargets = [
                    { 'required': 'employeeID' },
                    { 'required': 'startDateTime' },
                    { 'required': 'endDateTime' },
                    { 'required': 'payTypeKey' },
                    { 'required': 'mealOrderKey' },
                    { 'required': 'note' },
                    //{ 'required': 'realendDateTime' },
                    { 'required': 'departmentID' }];

                var errorMsg = '';
                $.each($('.OvertimeSummary tr'), function (i, $tr) {
                    var $isCheck = $(this).find('.check input').prop('checked');
                    if ($isCheck) {
                        var data = {};
                        data['sn'] = $(this).find('.sn').val();
                        data['applyID'] = applyID;
                        data['applyDeptID'] = applyDeptID;
                        data['employeeID'] = $(this).find('.employeeName option:selected').val();
                        data['employeeName'] = $(this).find('.employeeName option:selected').text().replace(data['employeeID'], '').trim();
                        data['departmentID'] = $(this).find('.departmentID input').val();
                        data['startDateTime'] = $(this).find('.startDateTime input').val();
                        data['endDateTime'] = $(this).find('.endDateTime input').val();
                        data['supportDeptID_FK'] = $(this).find('.supportDept option:selected').val();
                        data['supportDeptName'] = $(this).find('.supportDept option:selected').text();
                        data['note'] = $(this).find('.note input').val();
                        //data['realendDateTime'] = $(this).find('.realendDateTime input').val();
                        data['payTypeKey'] = $(this).find('.payType option:selected').val();
                        data['payTypeValue'] = $(this).find('.payType option:selected').text();
                        data['mealOrderKey'] = $(this).find('.mealOrder option:selected').val();
                        data['mealOrderValue'] = $(this).find('.mealOrder option:selected').text();
                        data['nationalType'] = $(this).find('.nationalType').val();
                        data['departmentID'] = $(this).find('.departmentID').val();
                        data['departmentName'] = $(this).find('.departmentName').val();

                        //required field : employeeID endDateTime mealOrderKey payTypeKey startDateTime note
                        $.each(verifyTargets, function (j, target) {
                            if (data[target.required] == '') {
                                errorMsg += '第' + i + '列欄位 : ' + target.required + '為必填欄位!\n';
                                isValid = false;
                            }
                        });
                        if (data['startDateTime'] >= data['endDateTime']) {
                            debugger;
                            errorMsg += '第' + i + '列 : 加班時間(起)必須小於加班時間(迄)!\n';
                            isValid = false;
                        }
                        request.push(data);
                    }
                });

                //判斷是否勾選資料
                if (!request[0]) { alert('請勾選要儲存的資料!'); return false; }
                //判斷是否為相同單位
                var firstSupDeptID = request[0].supportDeptID_FK;
                var firstSupDeptName = request[0].supportDeptName;
                var firstStartDate = new Date(request[0].startDateTime);
                var firstEndDate = new Date(request[0].endDateTime);
                var employeeIDContent = [];
                $.each($(request), function (index, data) {
                    //debugger;
                    //if (data['supportDeptID_FK'] != firstSupDeptID) {
                    //    errorMsg += '同張簽核支援單位必須相同!';
                    //    isValid = false;
                    //}
                    //20170218 add卡控-----S
                    var addstartDateTime = new Date(data['startDateTime']);
                    var addendDateTime = new Date(data['endDateTime']);
                    //var ii = parseInt(index) + 1;
                    //alert(addstartDateTime.toDateString());
                    //alert(addendDateTime.toDateString());
                    //alert(firstStartDate.toDateString());
                    if (addstartDateTime.toDateString() != firstStartDate.toDateString() || addendDateTime.toDateString() != firstEndDate.toDateString()) {
                        errorMsg += '工號' + data['employeeID'] + ' : ' + '同張簽核加班起迄日期必須相同!';
                        isValid = false;
                    }
                    //20170218 add卡控-----E
                    employeeIDContent.push(data['employeeID']);
                });

                //判斷是否有重複資料
                if (employeeIDContent.length != $.unique(employeeIDContent).length) {
                    errorMsg += '儲存的資料表中不得有重複的人員資料!';
                    isValid = false;
                }

                if (!isValid) {
                    alert(errorMsg);
                    return false;
                }

                request.push({ "applyID": applyID }, { "currentSignLevelDeptID": firstSupDeptID }, { "currentSignLevelDeptName": firstSupDeptName }, { "signDocID": signDocID }, { "formSeries": formSeries });
                //debugger;
                $.ajax({
                    type: "post",
                    //contentType: "application/json",
                    traditional: true,
                    url: "../../../Service/Sign/Forms/Overtime.asmx/SaveData",
                    data: JSON.stringify(request),
                    success: function (responses) {
                        if (responses['SignDocID']) {
                            if (responses['message'])
                            { alert(responses['message']); }
                            if (responses['message_alert'])
                            { alert(responses['message_alert']); }
                            alert('存檔成功，請確認明細!');
                            //debugger;
                            $('.OvertimeSummary').attr("href", "/Area/Sign/Forms/OvertimeSummary.aspx?signDocID=" + responses['SignDocID']);
                            $('.isClick').val('true');
                            $('.OvertimeSummary').click();
                            $('.isClick').val('false');
                        } else {
                            alert(responses['ErrorMessage']);
                        }
                    },
                    error: function (e) {
                        alert('存檔失敗!' + e.responseJSON.ErrorMessage);
                        console.log(e.responseJSON);
                    },
                    beforeSend: function () {
                        $('.save-footer .SaveData').hide();
                        $('.save-footer .CoverBtn').show();
                    },
                    complete: function () {
                        $('.save-footer .SaveData').show();
                        $('.save-footer .CoverBtn').hide();
                    },

                });
            });

            $('.ApplyForm').click(function () {
                var request = [];
                $.each($('#OvertimeSummaryGridView tr'), function (i, $tr) {
                    var data = {};
                    if ($(this).find('th').length > 0) { return; }
                    data['sn'] = $(this).find('.sn').val();
                    data['signDocID'] = $(this).find('.signDocID').text();
                    data['employeeID'] = $(this).find('.employeeID').text();
                    data['employeeName'] = $(this).find('.employeeName').text();
                    data['nationalType'] = $(this).find('.nationalType').text();
                    data['departmentID'] = $(this).find('.departmentID').val();
                    data['departmentName'] = $(this).find('.departmentName').text();
                    data['startDateTime'] = $(this).find('.startDateTime').text();
                    data['endDateTime'] = $(this).find('.endDateTime').text();
                    //data['realendDateTime'] = $(this).find('.realendDateTime').text();
                    data['supportDeptID_FK'] = $(this).find('.supportDeptID').val();
                    data['supportDeptName'] = $(this).find('.supportDeptName').text();
                    data['payTypeKey'] = $(this).find('.payTypeKey').val();
                    data['payTypeValue'] = $(this).find('.payTypeValue').text();
                    data['mealOrderKey'] = $(this).find('.mealOrderKey').val();
                    data['mealOrderValue'] = $(this).find('.mealOrderValue').text();
                    data['note'] = $(this).find('.note').text();
                    request.push(data);
                });

                var firstSupDeptID = request[0].supportDeptID_FK;
                var signDocID = request[0].signDocID;
                var formSeries = $("#FormSeries").val();
                request.push({ "currentSignLevelDeptID": firstSupDeptID }, { "signDocID": signDocID }, { "formSeries": formSeries });
                $.ajax({
                    type: "post",
                    url: "../../../Service/Sign/Forms/Overtime.asmx/ApplyForm",
                    data: JSON.stringify(request),
                    success: function (responses) {
                        alert(responses['Message']);
                        window.parent.location.href = "../WorkflowQueryList.aspx?orderField=SendDate&descending=True";
                    },
                    error: function (e) {
                        alert('存檔失敗!' + e.responseJSON.ErrorMessage);
                        console.log(e.responseJSON);
                    },
                    beforeSend: function () {
                        $('.modal-footerlest .ApplyForm').hide();
                        $('.modal-footerlest .Coverbtn').show();
                    },
                    complete: function () {
                        $('.modal-footerlest .ApplyForm').show();
                        $('.modal-footerlest .Coverbtn').hide();
                    },
                });
            });

            if ($('#SignDocID').val()) {
                $('.OvertimeSetting, .OvertimeTitle').hide();
                $('.block').find('i').removeClass('glyphicon-chevron-up').addClass('glyphicon-chevron-down');
                $('.OvertimeSummary').attr("href", "/Area/Sign/Forms/OvertimeSummary.aspx?signDocID=" + $('#SignDocID').val());
                //編輯草稿 /駁回
                $.ajax({
                    type: "post",
                    url: "../../../Service/Sign/Forms/Overtime.asmx/QueryOvertimeData",
                    data: { "SignDocID": $('#SignDocID').val() },
                    success: function (responses) {
                        addRows(responses);
                        $('.OvertimeSummary, .save-footer').removeClass('hidden');
                    },
                    beforeSend: function () {
                        $('.data-loadding').show();
                    },
                    complete: function () {
                        $('.data-loadding').hide();
                    }
                });
            }
        });
    </script>
    <input type="text" runat="server" hidden id="PageTitle" class='page-title' value="" />
    <asp:HiddenField runat="server" ID="SignDocID" Value="" ClientIDMode="Static" />
    <%--<asp:HiddenField runat="server" ID="CurrentSignLevel" value="" ClientIDMode="Static" />--%>
    <asp:HiddenField runat="server" ID="FormSeries" ClientIDMode="Static" />
    <div class="text-center container block"><i class="Arrow glyphicon glyphicon-chevron-up"></i></div>
    <div id="" class="OvertimeTitle form-horizontal form-group">
        <div class="col-xs-6">
            <div class="form-group">
                <label for="ApplyName" class="col-xs-3 control-label">申請人</label>
                <div class="col-xs-3 margin-top-7">
                    <asp:Label runat="server" ID="ApplyName" ClientIDMode="Static" CssClass="form-control-static"></asp:Label>
                    <asp:HiddenField runat="server" ID="ApplyID_FK" ClientIDMode="Static" />
                </div>
            </div>
        </div>
        <div class="col-xs-6">
            <div class="form-group">
                <label for="ApplyDeptName" class="col-xs-3 control-label">申請人部門</label>
                <div class="col-xs-3 margin-top-7">
                    <asp:Label runat="server" ID="ApplyDeptName" CssClass="form-control-static" ClientIDMode="Static"></asp:Label>
                    <asp:HiddenField runat="server" ID="ApplyDeptID_FK" ClientIDMode="Static" />
                </div>
            </div>
        </div>
    </div>
    <div id="layout-content-wrapper">
        <div id="layout-content" class="container">
            <div id="" class="OvertimeSetting panel panel-default">
                <div class="panel-body">
                    <div class="form-horizontal col-xs-12">
                        <div class="col-xs-6">
                            <div class="form-group">
                                <label for="DefaultDept" class="col-xs-5 control-label">預設加班單位</label>
                                <div class="col-xs-5">
                                    <asp:DropDownList runat="server" CssClass="form-control" ID="DefaultDeptName" AutoPostBack="false" ClientIDMode="Static">
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <div class="form-group required">
                                <label for="DefaultStartDateTime" class="col-xs-5 control-label">預設加班日期時間(起)</label>
                                <div class='input-group date col-xs-5 datepicker-left-15' id='datetimepicker'>
                                    <asp:TextBox runat="server" ClientIDMode="Static" ID="DefaultStartDateTime" class="form-control" data-date-format="YYYY-MM-DD HH:mm:00" />
                                    <span class="input-group-addon">
                                        <span class="glyphicon glyphicon-calendar"></span>
                                    </span>
                                </div>
                            </div>

                            <div class="form-group required ">
                                <label for="DefaultSupportDept" class="col-xs-5 control-label">預設支援單位</label>
                                <div class="col-xs-5">
                                    <asp:HiddenField runat="server" ID="TrueSupportDeptName"></asp:HiddenField>
                                    <asp:DropDownList runat="server" CssClass="form-control" ID="DefaultSupportDeptName" OnSelectedIndexChanged="DefaultSupportDeptName_SelectedIndexChanged" AutoPostBack="true" ClientIDMode="Static">
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <div class="form-group ">
                                <label for="DefaultMealOrder" class="col-xs-5 control-label">訂餐需求</label>
                                <div class="col-xs-5">
                                    <asp:DropDownList runat="server" CssClass="form-control" ID="DefaultMealOrderValue" AutoPostBack="false" ClientIDMode="Static">
                                    </asp:DropDownList>
                                </div>
                            </div>

                        </div>
                        <div class="col-xs-6">
                            <div class="form-group">
                                <label for="DefaultPayType" class="col-xs-5 control-label">預設請領方式</label>
                                <div class="col-xs-5">
                                    <asp:DropDownList runat="server" CssClass="form-control" ID="DefaultPayTypeValue" AutoPostBack="false" ClientIDMode="Static">
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="form-group required">
                                <label for="DefaultEndDateTime" class="col-xs-5 control-label">預設加班日期時間(迄)</label>
                                <div class='input-group date col-xs-5 datepicker-left-15' id='datetimepicker'>
                                    <asp:TextBox runat="server" ClientIDMode="Static" ID="DefaultEndDateTime" class="form-control" data-date-format="YYYY-MM-DD HH:mm:00" />
                                    <span class="input-group-addon">
                                        <span class="glyphicon glyphicon-calendar"></span>
                                    </span>
                                </div>
                            </div>

                            <div class="form-group  ">
                                <label for="DefaultNote" class="col-xs-5 control-label">預設加班原因</label>
                                <div class="col-xs-5">
                                    <asp:TextBox runat="server" ID="DefaultNote" type="text" class="form-control" TextMode="MultiLine" Height="83" ClientIDMode="Static"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="text-center panel-footer create-footer">
                    <a class="CreateDetailData btn btn-primary">
                        <span class="glyphicon glyphicon-ok"></span>&nbsp;產生加班單明細                        
                    </a>

                    <a class="CoverBtn btn display-none">
                        <span class="glyphicon glyphicon-ok"></span>
                        <img class="loader" src='<%: VirtualPathUtility.ToAbsolute(@"~/icon/ajax-loader.gif") %>' />
                    </a>
                </div>
            </div>
            <div class="auto-style1">
                注意事項：
                &nbsp;<br />
                一、員工如因工作需求有加班必要，應事先提出「加班申請單」經由權責主管確認有加班必要,簽核同意後才可進行加班。
                &nbsp;<br />
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 請單位主管依工作需求作好加班管控。「加班請依公司出勤時間準時提供勞務」。
                &nbsp;<br />
                二、延長之工作時間，員工得於加班單申請時選擇加班費或換休假，換休假六個月內未休完，依當時加班工資請領加班費。 
                
            </div>
            <div class="data-loadding  text-center display-none">
                <div>資料讀取中...</div>
                <image src='<%: VirtualPathUtility.ToAbsolute(@"~/icon/ajax-loader-bar.gif") %>'></image>
            </div>
            <div class="OvertimeSummary panel panel-default hidden" id="">
                <div class="panel-heading">
                    <div class="col-xs-10">
                        <div style="line-height: 30px;">加班單明細</div>
                    </div>
                    <div class="col-xs-2 input-group input-group-sm">
                        <input id="NewRowEmpID" type="text" class="form-control " placeholder="輸入工號或新增空白列">
                        <span class="input-group-btn">
                            <button class="AddRow btn btn-success" type="button"><i class="glyphicon glyphicon-plus"></i></button>
                        </span>
                    </div>
                </div>
                <table class="TableDetial table table-bordered" id="">
                    <tr>
                        <th class="text-center">
                            <label class="btn btn-xs btn-info glyphicon glyphicon-ok">
                                <input selectall=".check input" class="hidden" type="checkbox" /></label></th>
                        <%--<th>序號</th>--%>
                        <th class="employeeID" hidden="true">員工編號</th>
                        <th class="employeeName th-required">員工編號/姓名</th>
                        <th class="startDateTime th-required">加班日期時間(起)</th>
                        <th class="endDateTime th-required">加班日期時間(迄)</th>
                        <th class="realendDateTime th-required">實際下班時間</th>
                        <th class="supportDept th-required">支援單位</th>
                        <th class="payType th-required">請領方式</th>
                        <th class="mealOrder th-required">訂餐需求</th>
                        <th class="note th-required">加班原因</th>
                    </tr>
                </table>
            </div>
            <div class="text-center panel-footer save-footer hidden">

                <a class="SaveData btn btn-primary">
                    <span class="glyphicon glyphicon-floppy-saved"></span>&nbsp;儲存草稿且 <span class="glyphicon glyphicon-envelope"></span>&nbsp;通知主管
                </a>

                <a class="CoverBtn btn display-none">
                    <span class="glyphicon glyphicon-floppy-saved"></span>
                    <img class="loader" src='<%: VirtualPathUtility.ToAbsolute(@"~/icon/ajax-loader.gif") %>' />
                </a>
                <a class="OvertimeSummary btn btn-info display-none" target="dialog" href='' disabled='disabled'>
                    <span class="glyphicon glyphicon-list-alt"></span>&nbsp;確認明細
                </a>
            </div>
        </div>
    </div>
    <%--<script src='<%: VirtualPathUtility.ToAbsolute(@"~/Scripts/Sign/Forms/Overtime.js") %>' type="text/javascript"></script>--%>
</asp:Content>
