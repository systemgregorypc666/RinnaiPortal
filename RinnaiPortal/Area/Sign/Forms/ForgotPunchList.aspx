<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ForgotPunchList.aspx.cs" Inherits="RinnaiPortal.Area.Sign.Forms.ForgotPunchList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="stylesheet" type="text/css" href="../../../Content/bootstrap.css" />
    <link rel="stylesheet" type="text/css" href="../../../Content/custom.css" />
    <link rel="stylesheet" type="text/css" href="../../../Content/dialog-layout.css" />
    <script src='<%: VirtualPathUtility.ToAbsolute(@"~/Scripts/jquery-1.10.2.js")%>' type="text/javascript"></script>
    <script src='<%: VirtualPathUtility.ToAbsolute(@"~/Scripts/bootstrap.js")%>' type="text/javascript"></script>
    <script src='<%: VirtualPathUtility.ToAbsolute(@"~/Scripts/Sign/main.js")%>' type="text/javascript"></script>
    <script type="text/javascript">
        Dialog.resize(800, 400);
    </script>
</head>
<body id="DialogLayout">
    <div class="modal-dialog">
        <div class="modal-header">
            <h4 class="modal-title" id="myModalLabel"><b>表單內容 </b></h4>
        </div>
        <div class="modal-body">
            <form runat="server">
                <asp:HiddenField runat="server" ID="SN" />

                <div class="col-xs-6">
                    <div class="row form-group  ">
                        <label for="SignDocID_FK" class="col-xs-5 ">簽核編號</label>
                        <div class="col-xs-7">
                            <asp:Label runat="server" ID="SignDocID_FK"></asp:Label>
                        </div>
                    </div>

                    <div class="row form-group  ">
                        <label for="ApplyID_FK" class="col-xs-5 ">申請人編號</label>
                        <div class="col-xs-7">
                            <asp:Label runat="server" ID="ApplyID_FK"></asp:Label>
                        </div>
                    </div>

                    <div class="row form-group  ">
                        <label for="EmployeeID_FK" class="col-xs-5 ">忘刷員工編號</label>
                        <div class="col-xs-7">
                            <asp:Label runat="server" ID="EmployeeID_FK"></asp:Label>
                        </div>
                    </div>

                    <div class="row form-group  ">
                        <label for="DepartmentName" class="col-xs-5 ">忘刷員工部門</label>
                        <div class="col-xs-7">
                            <asp:Label runat="server" ID="DepartmentName"></asp:Label>
                        </div>
                    </div>

                    <div class="row form-group  ">
                        <label for="ForgotPunchInDateTime" class="col-xs-5 ">上班忘刷日期</label>
                        <div class="col-xs-7">
                            <asp:Label runat="server" ID="ForgotPunchInDateTime" type="text"></asp:Label>
                        </div>
                    </div>

                    <div class="row form-group  ">
                        <label for="ForgotPunchOutDateTime" class="col-xs-5 ">下班忘刷日期</label>
                        <div class="col-xs-7">
                            <asp:Label runat="server" ID="ForgotPunchOutDateTime" type="text"></asp:Label>
                        </div>
                    </div>

                </div>


                <div class="col-xs-6">
                    <div class="row form-group  ">
                        <label for="ApplyDateTime" class="col-xs-4 ">申請日期</label>
                        <div class="col-xs-8">
                            <asp:Label runat="server" ID="ApplyDateTime"></asp:Label>
                        </div>
                    </div>

                    <div class="row form-group  ">
                        <label for="ApplyName" class="col-xs-4 ">申請人姓名</label>
                        <div class="col-xs-8">
                            <asp:Label runat="server" ID="ApplyName"></asp:Label>
                        </div>
                    </div>

                    <div class="row form-group  ">
                        <label for="EmployeeName" class="col-xs-4 ">忘刷員工姓名</label>
                        <div class="col-xs-8">
                            <asp:Label runat="server" ID="EmployeeName"></asp:Label>
                        </div>
                    </div>

                    <div class="row form-group  ">
                        <label for="PunchName" class="col-xs-4 ">忘刷類型</label>
                        <div class="col-xs-8">
                            <asp:Label runat="server" ID="PunchName" type="text"></asp:Label>
                        </div>
                    </div>

                    <div class="row form-group  ">
                        <label for="Note" class="col-xs-4 ">原因說明</label>
                        <div class="col-xs-8">
                            <asp:Label runat="server" ID="Note" type="text" style="word-break:break-all"></asp:Label>
                        </div>
                    </div>
                    <div class="row form-group  ">
                        <label class="col-xs-4 ">簽核歷程</label>
                        <div class="col-xs-8">
                            <asp:HyperLink ID="Signed" runat="server"  CssClass="btn btn-info btn-xs" CommandName="Detail" Target="_self" Text="簽核歷程" Width="80px"></asp:HyperLink>                   
                        </div>
                    </div>
                </div>
            </form>
        </div>
    </div>
</body>
</html>
