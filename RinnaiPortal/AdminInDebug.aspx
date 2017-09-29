<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdminInDebug.aspx.cs" Inherits="RinnaiPortal.AdminInDebug" %>


<!DOCTYPE html>

<html runat="server" xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <script src="Scripts/jquery-1.10.2.min.js"></script>
    <script src="Scripts/jquery-confirm.min.js"></script>
    <script src="Scripts/bootstrap.min.js"></script>

    <link href="Content/bootstrap.min.css" rel="stylesheet" />
    <link href="Content/jquery-confirm.css" rel="stylesheet" />

</head>

<body>
    <form id="form1" runat="server">

        <asp:HiddenField ID="hdnEmpName" runat="server" />

        <script>
            $(function () {
                var empName = $('#hdnEmpName').val();
                $.alert({
                    type: 'orange',
                    typeAnimated: true,
                    theme: 'dark',
                    title: '提醒!',
                    content: '<h3 style="color: red;">Rinnai Portal</h3><p><br />' +
                        'Dear ：<span style="color:yellow;">' + empName + '</span> ， 很抱歉，您的帳號目前無法登入，資訊人員處理中，請稍候! </p>' +
                        '<p><span style="color: #999999;">若有疑問，請洽資訊課專員：劉俊晨 分機：223 謝謝!</span></p>',
                    buttons: {
                        confirm:
                            {
                                text: '確定',
                                action: function () {
                                    window.location.href = '/AdminInDebug.aspx';
                                }
                            }
                    }
                })
            })

        </script>
    </form>
</body>
</html>
