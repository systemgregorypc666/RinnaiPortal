<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ForgotPunch.aspx.cs" Inherits="RinnaiPortal.Area.Sign.Forms.ForgotPunch" %>

<asp:Content ID="ForgotPunchContent" ContentPlaceHolderID="MainContent" runat="server">
	<input type="text" runat="server" hidden id="PageTitle" class='page-title' />
	<div id="layout-content-wrapper">
		<div id="layout-content" class="container">
			<div class="panel panel-default">
				<div class="panel-body">
					<asp:HiddenField runat="server" ID="FormSeries" ClientIDMode="Static" />
					<fieldset class="form-horizontal col-xs-12">
						<div class="col-xs-6">
							<div class="form-group  ">
								<label for="SignDocID_FK" class="col-xs-5 control-label">簽核編號</label>
								<div class="col-xs-5 margin-top-7">
									<asp:Label runat="server" ID="SignDocID_FK"></asp:Label>
								</div>
							</div>

							<div class="form-group required">
								<label for="ApplyID_FK" class="col-xs-5 control-label">申請人編號</label>
								<div class="col-xs-5 margin-top-7">
									<asp:Label runat="server" data-val="true" ID="ApplyID_FK" data-val-required="請輸入申請人編號" CssClass="form-control-static"></asp:Label>
									<span class="field-validation-valid" data-valmsg-for="ApplyID_FK" data-valmsg-replace="true"></span>
								</div>
							</div>

							<div class="form-group required">
								<label for="EmployeeID_FK" class="col-xs-5 control-label">忘刷員工姓名</label>
								<div class="col-xs-5">
									<asp:HiddenField runat="server" ID="EmployeeName"></asp:HiddenField>
									<asp:DropDownList runat="server" data-val="true" CssClass="form-control" data-val-required="請選忘刷員工姓名" ID="EmployeeID_FK" AutoPostBack="true" OnSelectedIndexChanged="EmployeeID_FK_SelectedIndexChanged">
									</asp:DropDownList>
									<span class="field-validation-valid" data-valmsg-for="EmployeeID_FK" data-valmsg-replace="true"></span>
								</div>
							</div>

							<div class="form-group required ">
								<label for="PeriodType" class="col-xs-5 control-label">忘刷類型</label>
								<div class="col-xs-5">
									<asp:DropDownList runat="server" data-val="true" CssClass="form-control" data-val-required="請選擇忘刷類型" ID="PeriodType" AutoPostBack="true" OnSelectedIndexChanged="PeriodType_SelectedIndexChanged">
									</asp:DropDownList>
									<span class="field-validation-valid" data-valmsg-for="PeriodType" data-valmsg-replace="true"></span>
								</div>
							</div>

							<div class="form-group">
								<label for="ForgotPunchInDateTime" class="col-xs-5 control-label">上班忘刷日期/時間</label>
								<div class='input-group date col-sm-5 datepicker-left-15' id='datetimepicker'>
									<asp:TextBox runat="server" data-val-required="請輸入上班忘刷日期" ClientIDMode="Static" ID="ForgotPunchInDateTime" class="form-control" data-date-format="YYYY-MM-DD HH:mm:00" OnTextChanged="SelectedDateTime_TextChanged" AutoPostBack="true" />
									<span class="input-group-addon">
										<span class="glyphicon glyphicon-calendar"></span>
									</span>
								</div>
							</div>

							<div class="form-group  ">
								<label for="RealPunchIn" class="col-xs-5 control-label">上班實際刷卡日期/時間</label>
								<div class="col-xs-5 margin-top-7">
									<asp:Label runat="server" ID="RealPunchIn" ClientIDMode="Static"></asp:Label>
								</div>
							</div>

							<div class="form-group">
								<label for="ForgotPunchOutDateTime" class="col-xs-5 control-label">下班忘刷日期/時間</label>
								<div class='input-group date col-sm-5 datepicker-left-15' id='datetimepicker'>
									<asp:TextBox runat="server" data-val-required="請輸入下班忘刷日期" ClientIDMode="Static" ID="ForgotPunchOutDateTime" class="form-control" data-date-format="YYYY-MM-DD HH:mm:00" OnTextChanged="SelectedDateTime_TextChanged" AutoPostBack="true" />
									<span class="input-group-addon">
										<span class="glyphicon glyphicon-calendar"></span>
									</span>
								</div>
							</div>

							<div class="form-group  ">
								<label for="RealPunchOut" class="col-xs-5 control-label">下班實際刷卡日期/時間</label>
								<div class="col-xs-5 margin-top-7">
									<asp:Label runat="server" ID="RealPunchOut" ClientIDMode="Static"></asp:Label>
								</div>
							</div>

						</div>

						<div class="col-xs-6">
							<div class="form-group required">
								<label for="ApplyName" class="col-xs-5 control-label">申請人姓名</label>
								<div class="col-xs-5 margin-top-7">
									<asp:Label runat="server" data-val="true" ID="ApplyName" CssClass="" data-val-required="請輸入申請人姓名"></asp:Label>
									<span class="field-validation-valid" data-valmsg-for="DepartmentName" data-valmsg-replace="true"></span>
								</div>
							</div>

							<div class="form-group required">
								<label for="DisabledDate" class="col-xs-5 control-label">申請日期</label>
								<div class="col-xs-5 margin-top-7">
									<asp:Label runat="server" ID="ApplyDateTime"></asp:Label>
								</div>
							</div>

							<div class="form-group required ">
								<label for="DepartmentID_FK" class="col-xs-5 control-label">忘刷員工部門</label>
								<div class="col-xs-5">
									<asp:DropDownList runat="server" data-val="true" CssClass="form-control" data-val-required="請選擇忘刷員工部門" ID="DepartmentID_FK" AutoPostBack="true" Enabled="false">
									</asp:DropDownList>
									<span class="field-validation-valid" data-valmsg-for="DepartmentID_FK" data-valmsg-replace="true"></span>
								</div>
							</div>

							<div class="form-group  ">
								<label for="Note" class="col-xs-5 control-label">原因說明</label>
								<div class="col-xs-5">
									<asp:TextBox runat="server" ID="Note" type="text" class="form-control" TextMode="MultiLine" Height="83"></asp:TextBox>
								</div>
							</div>

						</div>
					</fieldset>
				</div>

				<div class="text-center panel-footer">
					<%--<button type="button" class="btn btn-default" onclick="history.back();">
						<span class="glyphicon glyphicon-arrow-up"></span>&nbsp;回上一頁
					</button>--%>

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
	<script src='<%: VirtualPathUtility.ToAbsolute(@"~/Scripts/Sign/validate-summary.js") %>' type="text/javascript"></script>

</asp:Content>


