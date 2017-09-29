<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MealTaxiReport.aspx.cs" Inherits="RinnaiPortal.Area.Manage.MealTaxiReport" %>
<%@ Import Namespace="RinnaiPortal.Extensions" %>
<%@ Import Namespace="RinnaiPortal.Tools" %>
<asp:Content ID="MealTaxiReportContent" ContentPlaceHolderID="MainContent" runat="server">
    <input runat="server" type="text" hidden id="PageTitle" class='page-title' value="" />
    <div id="layout-content-wrapper">
        <div id="layout-content" class="container">
            <div class="list-toolbar row">


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


                <div class="col-lg-9 input-group" style="float: none">
                    <div class='input-group date col-xs-4 datepicker-left-15 pull-left' id='datetimepicker'>
                        <span class="input-group-addon">加班起</span>
                        <asp:TextBox runat="server" ClientIDMode="Static" ID="StartDateTime" class="form-control" data-date-format="YYYY-MM-DD HH:mm:00" />
                        <span class="input-group-addon">
                            <span class="glyphicon glyphicon-calendar"></span>
                        </span>

                    </div>
                    <div class='input-group date col-xs-5 datepicker-left-15' id='datetimepicker'>
                        <span class="input-group-addon">加班迄</span>
                        <asp:TextBox runat="server" ClientIDMode="Static" ID="EndDateTime" class="form-control" data-date-format="YYYY-MM-DD HH:mm:00" />
                        <span class="input-group-addon">
                            <span class="glyphicon glyphicon-calendar"></span>
                        </span>
                        <span class="input-group-btn">
                            <asp:LinkButton runat="server" class="btn btn-default" ID="queryBtn" OnClick="QueryBtn_Click"> <span class="glyphicon glyphicon-search"></span>&nbsp;查詢 </asp:LinkButton>
                        </span>
                    </div>
                </div>
            </div>

        </div>

        <div class="col-xs-12">
            <div class="col-xs-6">
                <ul class="nav nav-tabs">
                    <li role="presentation" class="active"><a runat="server" id="MealSummaryTitle"></a></li>
                </ul>
                <asp:GridView runat="server" CssClass="table table-striped table-bordered table-condensed pull-left col-xs-6" ID="MealSummary" AutoGenerateColumns="False">
                    <Columns>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:HyperLink runat="server" ClientIDMode="Static" ID="SupportDeptName" Text="加班單位"></asp:HyperLink>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" Text='<%# Eval("SupportDeptName") %>'> </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:HyperLink runat="server" ClientIDMode="Static" ID="Carnivore" Text="葷"></asp:HyperLink>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" Text='<%# Eval("Carnivore") %>'> </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:HyperLink runat="server" ClientIDMode="Static" ID="Vegan" Text="素"></asp:HyperLink>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" Text='<%# Eval("Vegan") %>'> </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:HyperLink runat="server" ClientIDMode="Static" ID="None" Text="不訂餐"></asp:HyperLink>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" Text='<%# Eval("None") %>'> </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:HyperLink runat="server" ClientIDMode="Static" ID="Total" Text="總計"></asp:HyperLink>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" Text='<%# Eval("Total")%>'> </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <RowStyle CssClass="text-center" />
                    <AlternatingRowStyle BackColor="#f9f9f9" CssClass="text-center" />
                    <HeaderStyle CssClass="text-nowrap info" Font-Bold="true" BackColor="#dff0d8" />
                </asp:GridView>

            </div>


            <div class="col-xs-6">
                <ul class="nav nav-tabs">
                    <li role="presentation" class="active"><a runat="server" id="TaxiSummaryTitle"></a></li>
                </ul>
                <asp:GridView runat="server" CssClass="table table-striped table-bordered table-condensed pull-right col-xs-6" ID="TaxiSummary" AutoGenerateColumns="False">
                    <Columns>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:HyperLink runat="server" ClientIDMode="Static" ID="EndDatetime" Text="加班(迄)"></asp:HyperLink>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" Text='<%# Eval("EndDatetime") %>'> </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:HyperLink runat="server" ClientIDMode="Static" ID="Total" Text="人數"></asp:HyperLink>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" Text='<%# Eval("Total") %>'> </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <RowStyle CssClass="text-center" />
                    <AlternatingRowStyle BackColor="#f9f9f9" CssClass="text-center" />
                    <HeaderStyle CssClass="text-nowrap info" Font-Bold="true" BackColor="#dff0d8" />
                </asp:GridView>
            </div>

        </div>
        <div class="col-xs-12" style="float: none;">
            <div class="col-xs-12">
                <ul class="nav nav-tabs">
                    <li role="presentation" class="active"><a>明細</a></li>
                </ul>
                <div class="text-center">
                    <asp:Label runat="server" id="noDataTip" ></asp:Label>
                </div>
                <asp:GridView runat="server" CssClass="table table-striped table-bordered table-condensed" ID="ReportGridView" AutoGenerateColumns="False">
                    <Columns>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:HyperLink runat="server" ClientIDMode="Static" ID="EmployeeID_FK" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("EmployeeID_FK", Request)%>'  Text="員工編號"></asp:HyperLink>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" Text='<%# Eval("EmployeeID_FK") %>'> </asp:Label>
                            </ItemTemplate>
                            <HeaderStyle CssClass="info" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:HyperLink runat="server" ClientIDMode="Static" ID="EmployeeName" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("EmployeeName", Request)%>'  Text="員工姓名"></asp:HyperLink>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" Text='<%# Eval("EmployeeName") %>'> </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <%--                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:HyperLink runat="server" ClientIDMode="Static" ID="SupportDeptID_FK" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("SupportDeptID_FK")%>' Text="加班單位"></asp:HyperLink>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" Text='<%# Eval("SupportDeptID_FK") %>'> </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>--%>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:HyperLink runat="server" ClientIDMode="Static" ID="supportdeptid_fk" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("supportdeptid_fk", Request)%>'  Text="加班單位代碼"></asp:HyperLink>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" Text='<%# Eval("supportdeptid_fk") %>'> </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:HyperLink runat="server" ClientIDMode="Static" ID="DepartmentName" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("DepartmentName", Request)%>'  Text="加班單位"></asp:HyperLink>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" Text='<%# Eval("DepartmentName") %>'> </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:HyperLink runat="server" ClientIDMode="Static" ID="StartDatetime" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("StartDatetime", Request)%>'  Text="加班(起)"></asp:HyperLink>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" Text='<%# Eval("StartDatetime").ToDateTimeFormateString() %>'> </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:HyperLink runat="server" ClientIDMode="Static" ID="EndDatetime" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("EndDatetime", Request)%>'  Text="加班(迄)"></asp:HyperLink>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" Text='<%# Eval("EndDatetime").ToDateTimeFormateString() %>'> </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <%--                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:HyperLink runat="server" ClientIDMode="Static" ID="PayTypeKey" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("PayTypeKey")%>' Text="報酬類型"></asp:HyperLink>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" Text='<%# Eval("PayTypeKey") %>'> </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>--%>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:HyperLink runat="server" ClientIDMode="Static" ID="MealOrderKey" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("MealOrderKey", Request)%>'  Text="餐別"></asp:HyperLink>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" Text='<%# ViewUtils.ParseMealOrder(Eval("MealOrderKey").ToString()) %>'> </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:HyperLink runat="server" ClientIDMode="Static" ID="NationalType" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("NationalType", Request)%>'  Text="國籍"></asp:HyperLink>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" Text='<%# ViewUtils.ParseNationalType(Eval("NationalType").ToString()) %>'> </asp:Label>
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
    </div>
</asp:Content>

