<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Admin.aspx.cs" Inherits="RinnaiPortal.Admiiin" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <script src="Scripts/jquery-1.10.2.min.js"></script>
    <script src="Scripts/bootstrap.min.js"></script>
    <link href="Content/bootstrap.min.css" rel="stylesheet" />

    <link href="Content/easy-autocomplete.css" rel="stylesheet" />
    <link href="Content/easy-autocomplete.themes.css" rel="stylesheet" />
    <script src="Scripts/jquery.easy-autocomplete.js"></script>
    <link href="Content/jquery-confirm.css" rel="stylesheet" />
    <script src="Scripts/jquery-confirm.min.js"></script>

    <style>
        .emp-div {
            border: 1px #ccc solid;
            /* width: 400px; */
            /* height: 300px; */
            margin-bottom: 10px;
            padding: 10px;
        }

        .filterDesc {
            color: #aaa;
            font-style: italic;
            font-size: 0.9em;
        }

        .result-div {
            border: 1px solid #ccc;
            background-color: white;
            margin: 20px;
            padding: 12px;
            width: 43%;
        }

        #empID1ID, #empID2ID {
            display: inline;
            font-size: 30px;
            /*background-color: orange;*/
        }

        #Label1, #Label2 {
            display: block;
        }

        #form1 > div.container > div > div.form-group.row > div:nth-child(1) > div.easy-autocomplete.eac-plate-dark,
        #form1 > div.container > div > div.form-group.row > div:nth-child(2) > div.easy-autocomplete.eac-plate-dark {
            display: inline;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <input type="hidden" name="isSetting" value="false" />
        <div class="container">
            <div class="jumbotron">
                <h3 class="alert alert-success">目前位置：
                    <asp:Label ID="hostType" runat="server" Text="Label"></asp:Label>
                </h3>
                <div class="btn-group" style="margin-left: 13px; font-size: 20px; margin-bottom: 22px;">
                    <%--     <select id="testOrRelease" name="testOrRelease">
                        <option value="Debug">測試區</option>
                        <option value="Release">正式區</option>
                    </select>--%>
                </div>

                <div class="form-group row" style="display: inline;">
                    <div class="emp-div col-md-12">
                        <asp:Label for="empID1ID" ID="Label1" runat="server" Text="Label">EMP1</asp:Label>
                        <asp:TextBox ID="empID1ID" class="" runat="server" Width="449px"></asp:TextBox>
                        <asp:Button ID="Button1" runat="server" Text="查詢" class="btn btn-info btn-lg" OnClick="Button1_Click" Height="70px" OnClientClick="clearComment();" />
                        <asp:Button ID="Button2" runat="server" Text="置換" class="btn btn-success btn-lg" OnClick="Button3_Click" Height="70px" OnClientClick="clearComment();" />

                        <div class="result-div">

                            <div class="alert alert-success">
                                <input type="checkbox" name="isOnSetting" id="isOnSetting" value="0" style="width: 20px; height: 20px;" />
                                <label for="isOnSetting">設定為處理中</label>
                            </div>

                            <asp:Label for="empID1EmpId" ID="Label5" runat="server" Text="Label">EmpID</asp:Label>
                            <asp:TextBox ID="empID1EmpId" class="form-control" runat="server" ReadOnly="true" Width=""></asp:TextBox>

                            <asp:Label for="empID1ADAccount" ID="Label3" runat="server" Text="Label">ADAccount</asp:Label>
                            <asp:TextBox ID="empID1ADAccount" class="form-control" runat="server" ReadOnly="true" Width=""></asp:TextBox>
                            <asp:Label for="empID1Name" ID="Label6" runat="server" Text="Label">EmpName</asp:Label>
                            <asp:TextBox ID="empID1Name" class="form-control" runat="server" ReadOnly="true" Width="" ForeColor="Blue"></asp:TextBox>
                            <asp:Label for="empID1Status" ID="Label9" runat="server" Text="Label">Status</asp:Label>
                            <asp:TextBox ID="empID1Status" class="form-control" runat="server" ReadOnly="true" Width="" ForeColor="Blue"></asp:TextBox>

                        </div>
                    </div>

                    <div class="emp-div col-md-12">
                        <asp:Label for="empID1ID" ID="Label2" runat="server" Text="Label">EMP2</asp:Label>
                        <asp:TextBox ID="empID2ID" class="" runat="server" Width="449px"></asp:TextBox>

                        <div class="result-div">

                            <asp:Label for="empID2EmpId" ID="Label7" runat="server" Text="Label">EmpID</asp:Label>
                            <asp:TextBox ID="empID2EmpId" class="form-control" runat="server" ReadOnly="true" Width=""></asp:TextBox>

                            <asp:Label for="empID2ADAccount" ID="Label4" runat="server" Text="Label">ADAccount</asp:Label>
                            <asp:TextBox ID="empID2ADAccount" class="form-control" runat="server" ReadOnly="true" Width=""></asp:TextBox>
                            <asp:Label for="empID2Name" ID="Label8" runat="server" Text="Label">EmpName</asp:Label>

                            <asp:TextBox ID="empID2Name" class="form-control" runat="server" ReadOnly="true" Width="" ForeColor="Red"></asp:TextBox>
                            <asp:Label for="empID2Status" ID="Label10" runat="server" Text="Label">Status</asp:Label>
                            <asp:TextBox ID="empID2Status" class="form-control" runat="server" ReadOnly="true" Width="" ForeColor="Blue"></asp:TextBox>

                        </div>
                    </div>
                </div>

                <div class="form-group">
                    <label for="comment">Comment:</label>
                    <textarea class="form-control" rows="10" id="comment" runat="server"></textarea>
                </div>
            </div>
        </div>
    </form>

    <script>

        function clearComment() {
            $('#comment').val('');
        }

        function authenticateConfirm() {
            var authIsSuccess;
            $.confirm({
                title: 'Prompt!',
                content: '' +
                '<form action="" class="formName">' +
                '<div class="form-group">' +
                '<label>Enter Admin Password here</label>' +
                '<input type="text" placeholder="please enter Admin password" class="pwd form-control" required />' +
                '</div>' +
                '</form>',
                buttons: {
                    formSubmit: {
                        text: 'Submit',
                        btnClass: 'btn-blue',
                        action: function () {
                            var pwd = this.$content.find('.pwd').val();
                            if (pwd == '') {
                                $.alert('please enter Admin password');
                                return false;
                            }
                            else {
                                $.ajax({
                                    async: false,
                                    type: "POST",
                                    url: "/api/AdminServiceApi/AuthenticateConfirm",
                                    data: '=' + pwd,
                                    dataType: "json",
                                    success: function (data) {
                                        authIsSuccess = true;
                                        $.alert('<h2 style="color:green;">Success</h2>');
                                    }, error: function (error) {
                                        authIsSuccess = false;
                                        $.confirm({
                                            title: 'Fail',
                                            content: '<h2 style="color:red;">Password Fail</h2>',
                                            buttons: {
                                                confirm: function () {
                                                    window.location.reload();
                                                }
                                            }
                                        });
                                    }, complete: function () {
                                        sessionStorage.setItem("authIsSuccess", authIsSuccess);
                                    }
                                });
                            }
                        }
                    },
                    cancel: function () {
                        window.location.reload();
                    },
                },
                onContentReady: function () {
                    // bind to events
                    var jc = this;
                    this.$content.find('form').on('submit', function (e) {
                        // if the user submits the form by pressing enter in the field.
                        e.preventDefault();
                        jc.$$formSubmit.trigger('click'); // reference the button and click it
                    });
                }
            });
        }


        function CondirmAlert(msg) {
            //return false;
            $.confirm({
                type: 'orange',
                typeAnimated: true,
                theme: 'dark',
                title: '提醒!',
                content: msg,
                buttons: {
                    confirm: {
                        text: '確定',
                        action: function () {
                        }
                    },
                }
            });
        }

        $(function () {
            /*
            localStorage ，sessionStorage
            如果想要清除所有的資料，則：
            localStorage.clear();  
            如果想移除某個key的value，則：
            localStorage.removeItem(<key>);  
            */
            //保留表單submit之後的值
            var authIsSuccess = sessionStorage.getItem("authIsSuccess");

            if (authIsSuccess !== null) {
                if (authIsSuccess == 'false') {
                    authenticateConfirm();
                }
            }
            else {
                authenticateConfirm();
            }

            if (typeof isSetting !== 'undefined') {
                if (isSetting) {
                    $("#isOnSetting").prop('checked', 'checked');
                }
                else {
                    $("#isOnSetting").prop('checked', '');
                }
            }


            if (typeof isSuccess !== 'undefined') {
                if (isSuccess) {
                    CondirmAlert('置換完成');
                }
                else {
                    CondirmAlert('置換失敗');
                }
            }

            $('#isOnSetting').change(function () {
                var isChk = $("#isOnSetting").is(":checked")
                if (isChk) {
                    $('[name="isSetting"]').val(true);
                }
                else {
                    $('[name="isSetting"]').val(false);
                }
            })
            //規則式 中文判斷
            var regExpCht = /^([\u4e00-\u9fa5])+$/;
            var regExpNum = /^\d*$/;

            window.emp1DescIsCht;
            window.emp2DescIsCht;

            var apiEmpData;

            $.ajax({
                type: "POST",
                async: false,
                dataType: "json",
                contentType: 'application/json; charset=UTF-8',
                url: "/api/AdminServiceApi/GetEmployeesData",
                success: function (data) {
                    apiEmpData = data;
                }, error: function () {
                    console.log("TrainPassStation Query error");
                }
            });
            var optionsForEmp1 = {
                data: apiEmpData,
                placeholder: "請輸入員工資料",
                getValue: function (element) {
                    var value = $('#empID1ID').val();
                    var returnValue;
                    //輸入的是中文
                    if (regExpCht.test(value)) {
                        window.emp1DescIsCht = true;
                        returnValue = element.employeeName;
                    }
                        //輸入數字
                    else if (regExpNum.test(value)) {
                        window.emp1DescIsCht = false;
                        returnValue = element.employeeID;
                    }
                        //輸入英文
                    else {
                        window.emp1DescIsCht = false;
                        returnValue = element.employeeADAccount;
                    }
                    return returnValue;
                },
                template: {
                    type: "custom",
                    method: function (value, item) {
                        var descValue = window.emp1DescIsCht == true ? item.employeeID : item.employeeName;
                        return value + ' - ' + '<span class="filterDesc">' + descValue + '</span>';
                    }
                },
                list: {
                    match: { enabled: true },
                    sort: { enabled: true },
                    showAnimation: { type: "fade" },
                    onSelectItemEvent: function () {
                        var id = $("#empID1ID").getSelectedItemData().employeeID;
                        var adAccount = $("#empID1ID").getSelectedItemData().employeeADAccount;
                        var name = $("#empID1ID").getSelectedItemData().employeeName;
                        var status = $("#empID1ID").getSelectedItemData().nationalType;
                        if (status == 'test') {
                            status = '處理中';
                        }
                        else {
                            status = '未設定';
                        }
                        $('#empID1EmpId').val(id);
                        $('#empID1ADAccount').val(adAccount);
                        $('#empID1Name').val(name);
                        $('#empID1Status').val(status);
                    },
                    maxNumberOfElements: 15,
                },
                //theme: "round"
                theme: "plate-dark"
            };

            var optionsForEmp2 = {
                data: apiEmpData,
                placeholder: "請輸入員工資料",
                getValue: function (element) {
                    var value = $('#empID2ID').val();
                    var returnValue;
                    //輸入的是中文
                    if (regExpCht.test(value)) {
                        window.emp2DescIsCht = true;
                        returnValue = element.employeeName;
                    }
                        //輸入數字
                    else if (regExpNum.test(value)) {
                        window.emp2DescIsCht = false;
                        returnValue = element.employeeID;
                    }
                        //輸入英文
                    else {
                        window.emp2DescIsCht = false;
                        returnValue = element.employeeADAccount;
                    }
                    return returnValue;
                },
                template: {
                    type: "custom",
                    method: function (value, item) {
                        var descValue = window.emp2DescIsCht == true ? item.employeeID : item.employeeName;
                        return value + ' - ' + '<span class="filterDesc">' + descValue + '</span>';
                    }
                },
                list: {
                    match: { enabled: true },
                    sort: { enabled: true },
                    showAnimation: { type: "fade" },
                    onSelectItemEvent: function () {
                        var id = $("#empID2ID").getSelectedItemData().employeeID;
                        var adAccount = $("#empID2ID").getSelectedItemData().employeeADAccount;
                        var name = $("#empID2ID").getSelectedItemData().employeeName;
                        var status = $("#empID2ID").getSelectedItemData().nationalType;
                        if (status == 'test') {
                            status = '處理中';
                        }
                        else {
                            status = '未設定';
                        }
                        $('#empID2EmpId').val(id);
                        $('#empID2ADAccount').val(adAccount);
                        $('#empID2Name').val(name);
                        $('#empID2Status').val(status);
                    },
                    maxNumberOfElements: 15,
                },
                //theme: "round"
                theme: "plate-dark"
            };
            $("#empID1ID").easyAutocomplete(optionsForEmp1);
            $("#empID2ID").easyAutocomplete(optionsForEmp2);
        })
    </script>
</body>
</html>
