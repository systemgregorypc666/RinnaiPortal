<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TrainDetail.aspx.cs" Inherits="RinnaiPortal.Area.Sign.Forms.TrainDetail" %>
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
        Dialog.resize(950, 550);
    </script>
    <style>
        #DialogLayout {
            background: #fff;
        }

            #DialogLayout .modal-header,
            #DialogLayout .modal-body,
            #DialogLayout .modal-footer {
                position: fixed;
                right: 0;
                left: 0;
            }

            #DialogLayout .modal-header {
                padding: 10px 15px;
                top: 0;
            }

            #DialogLayout .modal-body {
                top: 0;
                bottom: 60px;
                max-height: none;
            }

                #DialogLayout .modal-body.auto {
                    overflow: auto;
                }

            #DialogLayout .modal-footer {
                padding: 9px 20px 10px;
                bottom: 0;
            }

                #DialogLayout .modal-footer .page-links {
                    margin: 5px 0;
                }

            #DialogLayout .modal-header + .modal-body {
                top: 49px;
            }

            #DialogLayout .modal-body:last-child {
                bottom: 0;
            }

            #DialogLayout .modal-close {
                position: fixed;
                top: 5px;
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
<body id="DialogLayout">

    <div class="modal-dialog">
        <div class="modal-header">
            <h4 class="modal-title" id="myModalLabel"><b>心得明細 </b></h4>
        </div>
        <div class="modal-body">
            <asp:GridView runat="server" CssClass="table table-striped table-bordered table-condensed" ID="TrainDetailGridView" AutoGenerateColumns="False">
                <Columns>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:HyperLink runat="server" ClientIDMode="Static" ID="CodeName" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("CodeName", Request)%>' Text="項目"></asp:HyperLink>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label runat="server" Text='<%# Eval("CodeName") %>' Width="150px" CssClass="text-left"> </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:HyperLink runat="server" ClientIDMode="Static" ID="Description" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("Description", Request)%>' Text="描述"></asp:HyperLink>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label runat="server" Text='<%# Eval("Description") %>' Width="100%" CssClass="text-left"> </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>                
                <HeaderStyle CssClass="text-nowrap info" Font-Bold="true" BackColor="#dff0d8" />
            </asp:GridView>
        </div>
    </div>
    <div class="modal-footer">
        <div runat="server" id="paginationBar" class="text-center"></div>
    </div>

</body>
</html>

