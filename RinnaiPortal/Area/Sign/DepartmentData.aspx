<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DepartmentData.aspx.cs" Inherits="RinnaiPortal.Area.Sign.DepartmentData" %>

<asp:Content ID="DepartmentDataContent" ContentPlaceHolderID="MainContent" runat="server">
	<input runat="server" type="text" hidden id="PageTitle" class='page-title' value="" />
	<div id="layout-content-wrapper">
		<div id="layout-content" class="container">
			<div class="panel panel-default">
				<div class="panel-body">
					<fieldset class="form-horizontal col-xs-12">
						<div class="col-xs-6">
							<asp:HiddenField runat="server" ID="TimeStamp" />
							<div class="form-group required ">
								<label for="DepartmentID" class="col-xs-5 control-label">部門代碼</label>
								<div class="col-xs-5">
									<asp:TextBox runat="server" data-val="true" data-val-required="請輸入部門代碼" ID="DepartmentID" type="text" class="form-control"></asp:TextBox>
									<span class="field-validation-valid" data-valmsg-for="DepartmentID" data-valmsg-replace="true"></span>
								</div>
							</div>
							<div class="form-group required ">
								<label for="ChiefID" class="col-xs-5 control-label">主管姓名</label>
								<div class="col-sm-5">
									<asp:HiddenField runat="server" ID="ChiefName"></asp:HiddenField>
									<asp:DropDownList runat="server" data-val="true" CssClass="form-control" data-val-required="請選擇主管姓名" ID="ChiefID_FK" AutoPostBack="true" OnSelectedIndexChanged="ChiefID_FK_SelectedIndexChanged">
									</asp:DropDownList>
									<span class="field-validation-valid" data-valmsg-for="ChiefID" data-valmsg-replace="true"></span>
								</div>
							</div>


							<div class="form-group required ">
								<label for="DepartmentLevel" class="col-xs-5 control-label">層級</label>
								<div class="col-sm-5">
									<asp:TextBox runat="server" data-val="true" data-val-required="請輸入層級" ID="DepartmentLevel" type="text" class="form-control"></asp:TextBox>
									<span class="field-validation-valid" data-valmsg-for="DepartmentLevel" data-valmsg-replace="true"></span>
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
						</div>


						<div class="col-xs-6">
							<div class="form-group required ">
								<label for="DepartmentName" class="col-xs-5 control-label">部門名稱</label>
								<div class="col-sm-5">
									<asp:TextBox runat="server" data-val="true" data-val-required="請輸入部門名稱" ID="DepartmentName" type="text" class="form-control"></asp:TextBox>
									<span class="field-validation-valid" data-valmsg-for="DepartmentName" data-valmsg-replace="true"></span>
								</div>
							</div>

							<div class="form-group required ">
								<label for="UpperDepartmentID" class="col-xs-5 control-label">上層部門名稱</label>
								<div class="col-sm-5">
									<asp:HiddenField runat="server" ID="UpperDepartmentName"></asp:HiddenField>
									<asp:DropDownList runat="server" data-val="true" CssClass="form-control" data-val-required="請選擇上層部門名稱" ID="UpperDepartmentID" AutoPostBack="true" OnSelectedIndexChanged="UpperDepartmentID_SelectedIndexChanged">
									</asp:DropDownList>
									<span class="field-validation-valid" data-valmsg-for="UpperDepartmentID" data-valmsg-replace="true"></span>
								</div>
							</div>

							<div class="form-group required">
								<label for="FilingEmployeeID_FK" class="col-xs-5 control-label">歸檔員工姓名</label>
								<div class="col-sm-5">
									<asp:HiddenField runat="server" ID="FilingEmployeeName"></asp:HiddenField>
									<asp:DropDownList runat="server" data-val="true" CssClass="form-control" data-val-required="請選擇歸檔員工姓名" ID="FilingEmployeeID_FK" AutoPostBack="true" OnSelectedIndexChanged="FilingEmployeeID_FK_SelectedIndexChanged">
									</asp:DropDownList>
									<span class="field-validation-valid" data-valmsg-for="FilingEmployeeID_FK" data-valmsg-replace="true"></span>
								</div>
							</div>
							<div class="form-group">
								<label for="DisabledDate" class="col-xs-5 control-label">停用日期</label>
								<div class='input-group date col-sm-5 datepicker-left-15' id='datetimepicker'>
									<asp:TextBox runat="server" data-val-required="請輸停用日期" ClientIDMode="Static" ID="DisabledDate" class="form-control" data-date-format="YYYY-MM-DD HH:mm:00" />
									<span class="input-group-addon">
										<span class="glyphicon glyphicon-calendar"></span>
									</span>
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



</asp:Content>
