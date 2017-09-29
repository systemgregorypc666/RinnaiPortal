<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WorkflowDetail_C.aspx.cs" Inherits="RinnaiPortal.Area.Sign.WorkflowDetailC" %>
<%@ Import Namespace="RinnaiPortal.Extensions" %>
<%@ Import Namespace="RinnaiPortal.Tools" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="stylesheet" type="text/css" href="/Content/bootstrap.css" />
    <link rel="stylesheet" type="text/css" href="/Content/custom.css" />
    <script src='<%: VirtualPathUtility.ToAbsolute(@"~/Scripts/jquery-1.10.2.js")%>' type="text/javascript"></script>
    <script src='<%: VirtualPathUtility.ToAbsolute(@"~/Scripts/bootstrap.js")%>' type="text/javascript"></script>
    <script src='<%: VirtualPathUtility.ToAbsolute(@"~/Scripts/Sign/main.js")%>' type="text/javascript"></script>
    <script type="text/javascript">
        Dialog1.resize(950, 550);
    </script>
    <style>
        #DialogLayout1 {
            background: #fff;
        }

            #DialogLayout1 .modal-header,
            #DialogLayout1 .modal-body,
            #DialogLayout1 .modal-footer {
                position: fixed;
                right: 0;
                left: 0;
            }

            #DialogLayout1 .modal-header {
                padding: 10px 15px;
                top: 0;
            }

            #DialogLayout1 .modal-body {
                top: 0;
                bottom: 60px;
                max-height: none;
            }

                #DialogLayout1 .modal-body.auto {
                    overflow: auto;
                }

            #DialogLayout1 .modal-footer {
                padding: 9px 20px 10px;
                bottom: 0;
            }

                #DialogLayout1 .modal-footer .page-links {
                    margin: 5px 0;
                }

            #DialogLayout1 .modal-header + .modal-body {
                top: 49px;
            }

            #DialogLayout1 .modal-body:last-child {
                bottom: 0;
            }

            #DialogLayout1 .modal-close {
                position: fixed;
                top: 50px;
                right: 5px;
                width: 20px;
                height: 20px;
                line-height: 1em;
                font-size: 16px;
                font-weight: bold;
                padding: 0;
                z-index: 9999;
                zoom: 1;
                color: #fff;
                background: #333;
                border-radius: 15px;
                opacity: 0.4;
                text-align: center;
            }
    </style>
</head>
<body id="DialogLayout1">

    <div class="modal-dialog">
        <div class="modal-header">
            <h4 class="modal-title" id="myModalLabel"><b>簽核明細 </b></h4>
        </div>
        <div class="modal-body">
            <asp:GridView runat="server" CssClass="table table-striped table-bordered table-condensed text-nowrap" ID="WorkflowDetailGridView" AutoGenerateColumns="False">
                <Columns>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:Label runat="server" ClientIDMode ="Static" ID="SignDocID_FK" Text="文件編號"></asp:Label>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label runat="server" Text='<%# Eval("SignDocID_FK") %>' Width="105px"> </asp:Label>
                        </ItemTemplate>
                        <HeaderStyle CssClass="info" />
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:HyperLink runat="server" ClientIDMode="Static" ID="FormType" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("FormType", Request)%>' Text="表單類型"></asp:HyperLink>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label runat="server" Title='<%# Eval("FormType") %>' Text='<%# Eval("FormType") %>' Width="40px" CssClass="txt-overflow"> </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:HyperLink runat="server" ClientIDMode="Static" ID="SendDate" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("SendDate", Request)%>' Text="送簽日期"></asp:HyperLink>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label runat="server" Title='<%# Eval("SendDate").ToDateTimeFormateString() %>' Text='<%# Eval("SendDate").ToDateTimeFormateString() %>' CssClass="txt-overflow" Width="90px"> </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:HyperLink runat="server" ClientIDMode="Static" ID="DepartmentName" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("DepartmentName", Request)%>' Text="簽核部門"></asp:HyperLink>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label runat="server" Text='<%# Eval("DepartmentName") %>' Width="30px"> </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:HyperLink runat="server" ClientIDMode="Static" ID="FinalStatus" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("FinalStatus", Request)%>' Text="表單狀態"></asp:HyperLink>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label runat="server" Text='<%# ViewUtils.ParseStatus(Eval("FinalStatus").ToString())%>' Width="30px"> </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:HyperLink runat="server" ClientIDMode="Static" ID="EmployeeName" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("EmployeeName", Request)%>' Text="簽核主管"></asp:HyperLink>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label runat="server" Text='<%# Eval("EmployeeName") %>' Width="30px"> </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:HyperLink runat="server" ClientIDMode="Static" ID="Remark" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("Remark", Request)%>' Text="備註"></asp:HyperLink>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label runat="server" Title='<%# Eval("Remark") %>' Text='<%# Eval("Remark") %>' CssClass="txt-overflow" Width="50px"> </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:HyperLink runat="server" ClientIDMode="Static" ID="Status" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("Status", Request)%>' Text="簽核狀態"></asp:HyperLink>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label runat="server" Text='<%# ViewUtils.ParseStatus(Eval("Status").ToString())%>' Width="40px"> </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:HyperLink runat="server" ClientIDMode="Static" ID="Status" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("LogDatetime", Request)%>' Text="建立日期"></asp:HyperLink>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label runat="server" Title='<%# Eval("LogDatetime").ToDateTimeFormateString() %>' Text='<%# Eval("LogDatetime").ToDateTimeFormateString() %>' CssClass="txt-overflow" Width="90px"> </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <RowStyle CssClass="text-left" />
                <AlternatingRowStyle BackColor="#f9f9f9" CssClass="text-left" />
                <HeaderStyle CssClass="text-nowrap info" Font-Bold="true" BackColor="#dff0d8" />
            </asp:GridView>
        </div>
    </div>
    <div class="modal-footer">
        <div runat="server" id="paginationBar" class="text-center"></div>
    </div>

</body>
</html>
