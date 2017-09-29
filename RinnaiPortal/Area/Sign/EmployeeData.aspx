<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EmployeeData.aspx.cs" Inherits="RinnaiPortal.Area.Sign.EmployeeData" %>

<asp:Content ID="EmployeeDataContent" ContentPlaceHolderID="MainContent" runat="server">
	<input runat="server" type="text" hidden id="PageTitle" class='page-title' value="" />
	<div id="layout-content-wrapper">
		<div id="layout-content" class="container">
			<div class="panel panel-default">

				<div class="panel-body">
					<fieldset class="form-horizontal row clearfix">
						<div class="col-xs-6">
							<asp:HiddenField runat="server" ID="TimeStamp" />
							<div class="form-group required ">
								<label for="EmployeeID" class="col-xs-5 control-label">員工編號</label>
								<div class="col-sm-5">
									<asp:TextBox runat="server" data-val="true" data-val-required="請輸入員工編號" ID="EmployeeID" class="form-control" test="123"></asp:TextBox>
									<span class="field-validation-valid" data-valmsg-for="EmployeeID" data-valmsg-replace="true"></span>
								</div>
							</div>
							<div class="form-group">
								<label for="ADAccount" class="col-xs-5 control-label">AD帳號</label>
								<div class="col-sm-5">
									<asp:TextBox runat="server" data-val-required="請輸入AD帳號" ID="ADAccount" class="form-control"></asp:TextBox>
									<span class="field-validation-valid" data-valmsg-for="ADAccount" data-valmsg-replace="true"></span>
								</div>
							</div>
							<div class="form-group">
								<label for="AgentID" class="col-xs-5 control-label">代理人編號</label>
								<div class="col-sm-5">
									<asp:TextBox runat="server" data-val-required="請輸入代理人編號" ID="AgentID" type="text" class="form-control"></asp:TextBox>
									<span class="field-validation-valid" data-valmsg-for="AgentID" data-valmsg-replace="true"></span>
								</div>
							</div>
							<div class="form-group">
								<div class="col-xs-5 control-label top-negative-7">
									<div class="">
										<label>
											<asp:CheckBox runat="server" ID="Disabled" Text="停用" CssClass="checkbox-inline" OnCheckedChanged="Disabled_CheckedChanged" AutoPostBack="true" />
										</label>
									</div>
								</div>
							</div>
							<div class="form-group required ">
								<label for="AccessType" class="col-xs-5 control-label">權限類別</label>
								<div class="col-xs-5">
									<asp:ListBox runat="server" data-val="true" CssClass="form-control" data-val-required="請選擇權限類別" ID="AccessType" ClientIDMode="Static" SelectionMode="Multiple"></asp:ListBox>
									<span class="field-validation-valid" data-valmsg-for="AccessType" data-valmsg-replace="true"></span>
								</div>
							</div>
                            <div class="form-group required">
								<label for="NationalType" class="col-xs-5 control-label">性別</label>
								<div class="col-xs-5">
									<asp:DropDownList runat="server" data-val="true" CssClass="form-control" data-val-required="請選擇性別" ID="SexType" OnSelectedIndexChanged="SexType_SelectedIndexChanged" AutoPostBack="true">
									</asp:DropDownList>
									<span class="field-validation-valid" data-valmsg-for="SexType" data-valmsg-replace="true"></span>
								</div>
							</div>
						</div>

						<div class="col-xs-6">
							<div class="form-group required">
								<label for="EmployeeName" class="col-xs-5 control-label">員工姓名</label>
								<div class="col-sm-5">
									<asp:TextBox runat="server" data-val="true" data-val-required="請輸入員工姓名" ID="EmployeeName" class="form-control"></asp:TextBox>
									<span class="field-validation-valid" data-valmsg-for="EmployeeName" data-valmsg-replace="true"></span>
								</div>
							</div>
							<div class="form-group required ">
								<label for="DepartmentName" class="col-xs-5 control-label">部門名稱</label>
								<div class="col-xs-5">
									<asp:HiddenField runat="server" ID="DepartmentName"></asp:HiddenField>
									<asp:DropDownList runat="server" data-val="true" CssClass="form-control" data-val-required="請選擇部門名稱" ID="DepartmentID_FK" OnSelectedIndexChanged="DepartmentID_FK_SelectedIndexChanged" AutoPostBack="true">
									</asp:DropDownList>
									<span class="field-validation-valid" data-valmsg-for="DepartmentID_FK" data-valmsg-replace="true"></span>
								</div>
							</div>
                            <div class="form-group required ">
								<label for="CostDepartmentName" class="col-xs-5 control-label">成本部門</label>
								<div class="col-xs-5">
									<asp:HiddenField runat="server" ID="CostDepartmentName"></asp:HiddenField>
									<asp:DropDownList runat="server" data-val="true" CssClass="form-control" data-val-required="請選擇部門名稱" ID="CostDepartmentID" OnSelectedIndexChanged="CostDepartmentID_SelectedIndexChanged" AutoPostBack="true">
									</asp:DropDownList>
									<span class="field-validation-valid" data-valmsg-for="CostDepartmentID" data-valmsg-replace="true"></span>
								</div>
							</div>
							<div class="form-group">
								<label for="AgentName" class="col-xs-5 control-label">代理人姓名</label>
								<div class="col-xs-5">
									<asp:DropDownList runat="server" CssClass="form-control" data-val-required="請輸入代理人姓名" ID="AgentName" OnSelectedIndexChanged="AgentName_SelectedIndexChanged" AutoPostBack="true">
									</asp:DropDownList>
									<span class="field-validation-valid" data-valmsg-for="AgentName" data-valmsg-replace="true"></span>
								</div>
							</div>
							<div class="form-group">
								<label for="DisabledDate" class="col-xs-5 control-label">帳號停用日期</label>
								<div class='input-group date col-sm-5 datepicker-left-15' id='datetimepicker'>
									<asp:TextBox runat="server" data-val-required="請輸入帳號停用日期" ClientIDMode="Static" ID="DisabledDate" class="form-control" data-date-format="YYYY-MM-DD HH:mm:00" />
									<span class="input-group-addon">
										<span class="glyphicon glyphicon-calendar"></span>
									</span>
								</div>
							</div>
							<div class="form-group required">
								<label for="NationalType" class="col-xs-5 control-label">國籍</label>
								<div class="col-xs-5">
									<asp:DropDownList runat="server" data-val="true" CssClass="form-control" data-val-required="請選擇國籍" ID="NationalType" OnSelectedIndexChanged="NationalType_SelectedIndexChanged" AutoPostBack="true">
									</asp:DropDownList>
									<span class="field-validation-valid" data-valmsg-for="NationalType" data-valmsg-replace="true"></span>
								</div>
							</div>
						</div>
					</fieldset>
				</div>

				<div class="text-center panel-footer">
					<button type="button" class="btn btn-default" onclick="history.back();">
						<span class="glyphicon glyphicon-arrow-up"></span>&nbsp;回上一頁</button>
					<asp:Button ClientIDMode="Static" runat="server" ID="SaveBtn" type="submit" CssClass="btn btn-primary" OnClick="SaveBtn_Click" data-loading-text="Loading..." Text="送出"></asp:Button>
					<asp:LinkButton runat="server" ClientIDMode="Static" ID="CoverBtn" CssClass="CoverBtn btn display-none">
						<span class="glyphicon glyphicon-floppy-saved"></span>
						<img class="loader" src='<%: VirtualPathUtility.ToAbsolute(@"~/icon/ajax-loader.gif") %>' />
					</asp:LinkButton>

					<a runat="server" id="RefreshBtn" class="btn btn-warning display-none" href="javascript:location.href=document.URL">
						<span class="glyphicon glyphicon-refresh"></span>&nbsp;重新整理</a>
				</div>
			</div>
		</div>
	</div>
	<script src='<%: VirtualPathUtility.ToAbsolute(@"~/Scripts/Sign/validate-summary.js") %>'></script>
	<script src='<%: VirtualPathUtility.ToAbsolute(@"~/Scripts/Sign/EmployeeData.js") %>'></script>


</asp:Content>

