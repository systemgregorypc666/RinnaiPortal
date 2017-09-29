<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="WorkflowQueryList.aspx.cs" Inherits="RinnaiPortal.Area.Sign.WorkflowQueryList" %>

<%@ Import Namespace="RinnaiPortal.Extensions" %>
<%@ Import Namespace="RinnaiPortal.Tools" %>

<asp:Content ID="WorkflowQueryContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        #myModal > div {
            width: 80%;
            padding: 20px;
        }

            #myModal > div > div > div.modal-body.stepdialog {
                background-color: #a4a4a8;
            }

        #signStep {
            margin-bottom: 10px;
        }

        .stepdialog img {
            width: 70%;
            margin: auto;
        }
    </style>
    <input runat="server" type="text" hidden id="PageTitle" class='page-title' value="" />
    <div id="layout-content-wrapper">
        <div id="layout-content" class="container">

            <div id="signStep">
                <button type="button" class="btn btn-info btn-lg" data-toggle="modal" data-target="#myModal">點我看簽核四步驟流程</button>
            </div>
            <div class="list-toolbar">
                <div class="col-lg-1 input-group pull-right">
                    <span class="input-group-addon">每頁</span>
                    <asp:DropDownList runat="server" ID="pageSizeSelect" class="form-control" data-val="true" data-val-number="The field PageSize must be a number." data-val-required="The PageSize field is required." Style="width: 70px" OnSelectedIndexChanged="PageSize_SelectedIndexChanged" AutoPostBack="true">
                        <asp:ListItem Text="10" Value="10"></asp:ListItem>
                        <asp:ListItem Text="20" Value="20"></asp:ListItem>
                        <asp:ListItem Text="30" Value="30"></asp:ListItem>
                    </asp:DropDownList>
                    <span class="input-group-addon">筆</span>
                    <span class="input-group-addon">總共&nbsp;
							<asp:Label runat="server" ID="totalRowsCount"></asp:Label>
                        &nbsp;筆</span>
                </div>

                <div class="row">
                    <div class="col-lg-3 pull-left" style="float: none; display: inline-block">
                        <div class="input-group">
                            <asp:TextBox runat="server" ID="queryTextBox" class="form-control" placeholder="簽核編號" />
                            <span class="input-group-btn">
                                <asp:LinkButton runat="server" class="btn btn-default" ID="queryBtn" OnClick="QueryBtn_Click"> <span class="glyphicon glyphicon-search"></span>&nbsp;查詢 </asp:LinkButton>
                            </span>
                        </div>
                    </div>
                    <div class='input-group col-lg-2 pull-left'>
                        <span class="input-group-addon">簽核狀態</span>
                        <asp:DropDownList runat="server" ID="finalStatusSelect" class="form-control" data-val="true" data-val-required="The PageSize field is required." OnSelectedIndexChanged="FinalStatusSelect_SelectedIndexChanged" AutoPostBack="true">
                            <asp:ListItem Text="未結案" Value=""></asp:ListItem>
                            <asp:ListItem Text="結案" Value="6"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
            </div>

            <div class="text-center">
                <asp:Label runat="server" ID="noDataTip"></asp:Label>
            </div>
            <asp:GridView runat="server" CssClass="table table-striped table-bordered table-condensed" ID="WorkflowQueryGridView" OnRowCommand="WorkflowQueryGridView_RowCommand" OnRowDeleting="WorkflowQueryGridView_RowDeleting" OnRowDataBound="WorkflowQueryGridView_RowDataBound" DataKeyNames="SignDocID,FormID_FK,FinalStatus,EmployeeID_FK,CurrentSignLevelDeptID_FK,Remainder,ChiefID_Up" AutoGenerateColumns="False">
                <Columns>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:HyperLink runat="server" ClientIDMode="Static" ID="SignDocID" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("SignDocID", Request)%>' Text="文件編號"></asp:HyperLink>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lblId" runat="server" Text='<%# Bind("SignDocID") %>'> </asp:Label>
                        </ItemTemplate>
                        <HeaderStyle CssClass="info" />
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:HyperLink runat="server" ClientIDMode="Static" ID="FormType" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("FormType", Request)%>' Text="表單類型"></asp:HyperLink>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <a runat="server" cssclass="btn btn-link" target="dialog" href='<%# GetFormUrl(Eval("SignDocID"), Eval("EmployeeID_FK"), Eval("FormID"))%>' text=''><%# Eval("FormType") %><i class="glyphicon glyphicon-info-sign" style="top: 3px"></i></a>
                            <input runat="server" id="FormID" visible="false" value='<%# Eval("FormID") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:HyperLink runat="server" ClientIDMode="Static" ID="EmployeeID_FK" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("EmployeeID_FK", Request)%>' Text="送簽人員編號"></asp:HyperLink>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label runat="server" Text='<%# Eval("EmployeeID_FK") %>'> </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:HyperLink runat="server" ClientIDMode="Static" ID="EmployeeName" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("EmployeeName", Request)%>' Text="送簽人員姓名"></asp:HyperLink>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label runat="server" Text='<%# Eval("EmployeeName") %>'> </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:HyperLink runat="server" ClientIDMode="Static" ID="SendDate" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("SendDate", Request)%>' Text="送簽日期"></asp:HyperLink>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label runat="server" Text='<%# Eval("SendDate").ToDateTimeFormateString() %>'> </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:HyperLink runat="server" ClientIDMode="Static" ID="CurrentSignLevelDeptName" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("CurrentSignLevelDeptName", Request)%>' Text="簽核部門"></asp:HyperLink>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label runat="server" Text='<%# Eval("CurrentSignLevelDeptName") %>'> </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:HyperLink runat="server" ClientIDMode="Static" ID="FinalStatus" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("FinalStatus", Request)%>' Text="表單狀態"></asp:HyperLink>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label runat="server" ClientIDMode="Static" ID="final_status" Text='<%# ViewUtils.ParseStatus( Eval("FinalStatus").ToString()) %>'> </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:HyperLink runat="server" ClientIDMode="Static" ID="Status" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("Status", Request)%>' Text="當前簽核狀態"></asp:HyperLink>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label runat="server" Text='<%# ViewUtils.ParseStatus(Eval("Status").ToString())%>'> </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="明細" HeaderStyle-CssClass="text-center">
                        <ItemStyle CssClass="vertical-middle" />
                        <ItemTemplate>
                            <asp:HyperLink runat="server" CommandName="Detail" CssClass="btn btn-info btn-xs" Target="dialog" NavigateUrl='<%# "~/Area/Sign/WorkflowDetail.aspx?signDocID=" + Eval("SignDocID") %>'
                                Text="明細" Width="50px"><span class="glyphicon glyphicon-list-alt"></span></asp:HyperLink>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="編輯" HeaderStyle-CssClass="text-center">
                        <ItemStyle CssClass="vertical-middle" />
                        <ItemTemplate>
                            <asp:HyperLink runat="server" CommandName="Edit" ClientIDMode="Static" ID="Edit" CssClass="btn btn-warning btn-xs" NavigateUrl='<%# GetEditUrl(Eval("SignDocID"), Eval("FormID")) %>'
                                Text="編輯" Width="50px"><span class="glyphicon glyphicon-pencil"></span></asp:HyperLink>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="抽單" HeaderStyle-CssClass="text-center">
                        <ItemStyle CssClass="vertical-middle" />
                        <ItemTemplate>
                            <asp:LinkButton ID="Pump" runat="server" Text="抽回" CommandName="Pump" OnClientClick="return confirm('確定抽單？');">
                                <%--<asp:Image ID="Image1" runat="server" ImageUrl="~/img/Remove.png" style="border-width: 0px;" Height="25" Width="25" />--%>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="刪除" HeaderStyle-CssClass="text-center">
                        <ItemStyle CssClass="vertical-middle" />
                        <ItemTemplate>
                            <%--<asp:Button ID="delete_btn" runat="server" Text="刪除" CommandName="Delete" OnClientClick="return confirm('確定要刪除此筆資料嗎?');">
                             </asp:Button>--%>
                            <asp:LinkButton ID="Del" runat="server" CommandName="Delete" OnClientClick="return confirm('是否刪除？');">
                                <asp:Image ID="Image1" runat="server" ImageUrl="~/img/Remove.png" Style="border-width: 0px;" Height="25" Width="25" />
                            </asp:LinkButton>
                            <%--<asp:HyperLink runat="server" CommandName="Del" ClientIDMode="Static" ID="Del" CssClass="btn btn-danger btn-xs" NavigateUrl='<%# GetEditUrl(Eval("SignDocID"), Eval("FormID")) %>'
								Text="刪除" Width="50px"><span class="glyphicon glyphicon-pencil"></span></asp:HyperLink>--%>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <RowStyle CssClass="text-center" />
                <AlternatingRowStyle BackColor="#f9f9f9" CssClass="text-center" />
                <HeaderStyle CssClass="text-nowrap info" Font-Bold="true" BackColor="#dff0d8" />
            </asp:GridView>
        </div>
        <div runat="server" id="paginationBar" class="text-center"></div>
    </div>
    <%--#0004 增加簽核四步驟--%>
    <!-- 簽核四步驟Dialog -->
    <div class="modal fade" id="myModal" role="dialog">
        <div class="modal-dialog">

            <!-- Modal content-->
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title">簽核四步驟</h4>
                </div>
                <div class="modal-body stepdialog">
                    <h2>step 1</h2>
                    <img src="../../img/SetpTeach/1.jpg" alt="Alternate Text" />
                    <h2>step 2</h2>
                    <img src="../../img/SetpTeach/2.jpg" alt="Alternate Text" />
                    <h2>step 3</h2>
                    <img src="../../img/SetpTeach/3.jpg" alt="Alternate Text" />
                    <h2>step 4</h2>
                    <img src="../../img/SetpTeach/4.jpg" alt="Alternate Text" />
                    <p></p>
                </div>

                <div class="modal-footer">
                    <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        jQuery(function ($) {

            //$('#myModal').modal();
            //$('tr').find("#final_status").each(function () {
            //    if ($(this).text() != "駁回" && $(this).text() != "草稿") {
            //        var target = $(this).closest('tr').find("#Edit");
            //        target.remove();
            //    }
            //});
        });
    </script>
</asp:Content>