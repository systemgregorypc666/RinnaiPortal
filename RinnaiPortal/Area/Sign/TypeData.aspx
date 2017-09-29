<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TypeData.aspx.cs" Inherits="RinnaiPortal.Area.Sign.TypeData" %>

<asp:Content ID="TypeDataContent" ContentPlaceHolderID="MainContent" runat="server" ClientIDMode="Static">
	<input runat="server" type="text" hidden id="PageTitle" class='page-title' value="" />
	<div id="layout-content-wrapper">
		<div id="layout-content" class="container">
			<div class="panel panel-default">
				<div class="panel-body">
					<fieldset class="form-horizontal col-xs-12">
						<div class="col-xs-6">
							<asp:HiddenField runat="server" ID="TimeStamp" />
							<asp:HiddenField runat="server" ID="FormID" />
							<div class="form-group required ">
								<label for="FormType" class="col-xs-5 control-label">表單類型</label>
								<div class="col-xs-5">
									<asp:TextBox runat="server" data-val="true" data-val-required="請輸入表單類型" ID="FormType" ClientIDMode="Static" class="form-control"></asp:TextBox>
									<span class="field-validation-valid" data-valmsg-for="FormType" data-valmsg-replace="true"></span>
								</div>
							</div>

							<div class="form-group required ">
								<label for="FilingDepartmentID_FK" class="col-xs-5 control-label">歸檔部門名稱</label>
								<div class="col-xs-5">
									<asp:HiddenField runat="server" ID="FilingDepartmentName"></asp:HiddenField>
									<asp:DropDownList runat="server" data-val="true" CssClass="form-control" data-val-required="請選擇歸檔部門名稱" ID="FilingDepartmentID_FK" OnSelectedIndexChanged="FilingDepartmentID_FK_SelectedIndexChanged" AutoPostBack="true">
									</asp:DropDownList>
									<span class="field-validation-valid" data-valmsg-for="FilingDepartmentID_FK" data-valmsg-replace="true"></span>
								</div>
							</div>
						</div>

						<div class="col-xs-6">
							<div class="form-group required ">
								<label for="SignID_FK" class="col-xs-5 control-label">簽核層級代碼</label>
								<div class="col-xs-5">
									<asp:DropDownList runat="server" data-val="true" CssClass="form-control" data-val-required="請選擇簽核名稱" ID="SignID_FK" OnSelectedIndexChanged="SignName_SelectedIndexChanged" AutoPostBack="true">
									</asp:DropDownList>
									<span class="field-validation-valid" data-valmsg-for="SignID_FK" data-valmsg-replace="true"></span>
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
	<script src='<%: VirtualPathUtility.ToAbsolute(@"~/Scripts/Sign/validate-summary.js") %>' type="text/javascript"></script>

</asp:Content>
