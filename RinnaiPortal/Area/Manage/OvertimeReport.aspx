<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="OvertimeReport.aspx.cs" Inherits="RinnaiPortal.Area.Manage.OvertimeReport" %>

<%@ Import Namespace="RinnaiPortal.Extensions" %>
<%@ Import Namespace="RinnaiPortal.Tools" %>
<asp:Content ID="OvertimeReportContent" ContentPlaceHolderID="MainContent" runat="server">
	<input runat="server" type="text" hidden id="PageTitle" class='page-title' value="" />
	<div id="layout-content-wrapper">
		<div id="layout-content" class="container">
			<div class="list-toolbar row">

				<div class="input-group pull-right col-lg-1">
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

				<div class='input-group col-lg-2 pull-left'>
					<span class="input-group-addon">簽核狀態</span>
					<asp:DropDownList runat="server" ID="finalStatusSelect" class="form-control" data-val="true" data-val-required="The PageSize field is required." OnSelectedIndexChanged="FinalStatusSelect_SelectedIndexChanged" AutoPostBack="true">
						<asp:ListItem Text="未結案" Value=""></asp:ListItem>
						<asp:ListItem Text="結案" Value="6"></asp:ListItem>
					</asp:DropDownList>
				</div>

				<div class="input-group pull-left">
					<div class='input-group date' id='datetimepicker'>
						<span class="input-group-addon">起</span>
						<asp:TextBox runat="server" ClientIDMode="Static" ID="StartDateTime" class="form-control" data-date-format="YYYY-MM-DD HH:mm:00" />
						<span class="input-group-addon">
							<span class="glyphicon glyphicon-calendar"></span>
						</span>
					</div>
				</div>
				<div class="input-group pull-left">
					<div class='input-group date' id='datetimepicker'>
						<span class="input-group-addon">迄</span>
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
			<div class="text-center">
				<asp:Label runat="server" ID="noDataTip"></asp:Label>
			</div>
			<asp:GridView runat="server" CssClass="table table-striped table-bordered table-condensed pull-left col-xs-6" ID="ReportGridView" AutoGenerateColumns="False">
				<Columns>
					<asp:TemplateField>
						<HeaderTemplate>
							<asp:HyperLink runat="server" ClientIDMode="Static" ID="SignDocID" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("SignDocID", Request)%>' Text="簽核代碼"></asp:HyperLink>
						</HeaderTemplate>
						<ItemTemplate>
							<asp:Label runat="server" Text='<%# Eval("SignDocID") %>'> </asp:Label>
						</ItemTemplate>
					</asp:TemplateField>
                    <asp:TemplateField>
						<HeaderTemplate>
							<asp:HyperLink runat="server" ClientIDMode="Static" ID="Applyname" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("Applyname", Request)%>' Text="填單人員"></asp:HyperLink>
						</HeaderTemplate>
						<ItemTemplate>
							<asp:Label runat="server" Text='<%# Eval("Applyname") %>'> </asp:Label>
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField>
						<HeaderTemplate>
							<asp:HyperLink runat="server" ClientIDMode="Static" ID="EmployeeID" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("EmployeeID", Request)%>' Text="員工編號"></asp:HyperLink>
						</HeaderTemplate>
						<ItemTemplate>
							<asp:Label runat="server" Text='<%# Eval("EmployeeID") %>'> </asp:Label>
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField>
						<HeaderTemplate>
							<asp:HyperLink runat="server" ClientIDMode="Static" ID="EmployeeName" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("EmployeeName", Request)%>' Text="員工姓名"></asp:HyperLink>
						</HeaderTemplate>
						<ItemTemplate>
							<asp:Label runat="server" Text='<%# Eval("EmployeeName") %>'> </asp:Label>
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField>
						<HeaderTemplate>
							<asp:HyperLink runat="server" ClientIDMode="Static" ID="SendDate" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("SendDate", Request)%>' Text="送出日期"></asp:HyperLink>
						</HeaderTemplate>
						<ItemTemplate>
							<asp:Label runat="server" Text='<%# Eval("SendDate").ToDateTimeFormateString() %>'> </asp:Label>
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField>
						<HeaderTemplate>
							<asp:HyperLink runat="server" ClientIDMode="Static" ID="FinalStatus" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("FinalStatus", Request)%>' Text="簽核狀態"></asp:HyperLink>
						</HeaderTemplate>
						<ItemTemplate>
							<asp:Label runat="server" CssClass="FinalStatus" Text='<%# ViewUtils.ParseStatus(Eval("FinalStatus").ToString())%>'> </asp:Label>
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField>
						<HeaderTemplate>
							<asp:HyperLink runat="server" ClientIDMode="Static" ID="Remainder" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("Remainder", Request)%>' Text="剩餘層數"></asp:HyperLink>
						</HeaderTemplate>
						<ItemTemplate>
							<asp:Label runat="server" Text='<%# Eval("Remainder") %>'> </asp:Label>
						</ItemTemplate>
						<HeaderStyle CssClass="info" />
					</asp:TemplateField>
					<asp:TemplateField>
						<HeaderTemplate>
							<asp:HyperLink runat="server" ClientIDMode="Static" ID="ChiefID" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("ChiefID", Request)%>' Text="主管編號"></asp:HyperLink>
						</HeaderTemplate>
						<ItemTemplate>
							<asp:Label runat="server" Text='<%# Eval("ChiefID") %>'> </asp:Label>
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField>
						<HeaderTemplate>
							<asp:HyperLink runat="server" ClientIDMode="Static" ID="ChiefName" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("ChiefName", Request)%>' Text="主管姓名"></asp:HyperLink>
						</HeaderTemplate>
						<ItemTemplate>
							<asp:Label runat="server" Text='<%# Eval("ChiefName") %>'> </asp:Label>
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField>
						<HeaderTemplate>
							<asp:HyperLink runat="server" ClientIDMode="Static" ID="Status" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("Status", Request)%>' Text="明細狀態"></asp:HyperLink>
						</HeaderTemplate>
						<ItemTemplate>
							<asp:Label runat="server" Text='<%# ViewUtils.ParseStatus(Eval("Status").ToString()) %>'> </asp:Label>
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField>
						<HeaderTemplate>
							<asp:HyperLink runat="server" ClientIDMode="Static" ID="PayType" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("PayType", Request)%>' Text="報酬類型"></asp:HyperLink>
						</HeaderTemplate>
						<ItemTemplate>
							<asp:Label runat="server" Text='<%# ViewUtils.ParsePayType(Eval("PayType").ToString()) %>'> </asp:Label>
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField>
						<HeaderTemplate>
							<asp:HyperLink runat="server" ClientIDMode="Static" ID="SupportDeptName" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("SupportDeptName", Request)%>' Text="支援單位"></asp:HyperLink>
						</HeaderTemplate>
						<ItemTemplate>
							<asp:Label runat="server" Text='<%# Eval("SupportDeptName") %>'> </asp:Label>
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField>
						<HeaderTemplate>
							<asp:HyperLink runat="server" ClientIDMode="Static" ID="StartDatetime" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("StartDatetime", Request)%>' Text="加班(起)"></asp:HyperLink>
						</HeaderTemplate>
						<ItemTemplate>
							<asp:Label runat="server" Text='<%# Eval("StartDatetime").ToDateTimeFormateString() %>'> </asp:Label>
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField>
						<HeaderTemplate>
							<asp:HyperLink runat="server" ClientIDMode="Static" ID="EndDatetime" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("EndDatetime", Request)%>' Text="加班(迄)"></asp:HyperLink>
						</HeaderTemplate>
						<ItemTemplate>
							<asp:Label runat="server" Text='<%# Eval("EndDatetime").ToDateTimeFormateString() %>'> </asp:Label>
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField>
						<HeaderTemplate>
							<asp:HyperLink runat="server" ClientIDMode="Static" ID="TotalHours" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("TotalHours", Request)%>' Text="時數"></asp:HyperLink>
						</HeaderTemplate>
						<ItemTemplate>
							<asp:Label runat="server" Text='<%# Eval("TotalHours") %>'> </asp:Label>
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField>
						<HeaderTemplate>
							<asp:HyperLink runat="server" ClientIDMode="Static" ID="IsHoliday" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("IsHoliday", Request)%>' Text="假日"></asp:HyperLink>
						</HeaderTemplate>
						<ItemTemplate>
							<asp:Label runat="server" Text='<%# ViewUtils.ParseBoolean(Eval("IsHoliday").ToString()) %>'> </asp:Label>
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField>
						<HeaderTemplate>
							<asp:HyperLink runat="server" ClientIDMode="Static" ID="AutoInsert" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("AutoInsert", Request)%>' Text="寫入志元"></asp:HyperLink>
						</HeaderTemplate>
						<ItemTemplate>
							<asp:Label runat="server" CssClass="AutoInsert" Text='<%# ViewUtils.ParseBoolean(Eval("AutoInsert").ToString()) %>'> </asp:Label>
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField HeaderText="手動寫入" HeaderStyle-CssClass="text-center">
						<ItemStyle CssClass="vertical-middle" />
						<ItemTemplate>
							<asp:HyperLink runat="server" CommandName="Edit"  Target="dialog" ClientIDMode="static" ID="InsertIntoSmartMan" CssClass="btn btn-warning btn-xs" NavigateUrl='<%# "~/Area/Manage/OvertimeSetting.aspx?SignDocID=" + Eval("SignDocID")+"&StartDateTime="+ Session["StartDateTime"] +"&EndDateTime="+ Session["EndDateTime"] %>'
								Text="手動寫入" Width="50px"><span class="glyphicon glyphicon-pencil"></span></asp:HyperLink>
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
	<script type="text/javascript">
		jQuery(function ($) {
			$('tr').find(".FinalStatus").each(function () {
				if ($(this).text() != "結案") {
					var target = $(this).closest('tr').find("#InsertIntoSmartMan");
					target.remove();
				}
			});
			$('tr').find(".AutoInsert").each(function () {
			    if ($(this).text() != "否") {
			        var target = $(this).closest('tr').find("#InsertIntoSmartMan");
			        target.remove();
			    }
			});
		});
	</script>
</asp:Content>


