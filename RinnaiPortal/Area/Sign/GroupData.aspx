<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GroupData.aspx.cs" Inherits="RinnaiPortal.Area.Sign.GroupData" %>

<asp:Content ID="GroupDataContent" ContentPlaceHolderID="MainContent" runat="server">
	<input runat="server" type="text" hidden id="PageTitle" class='page-title' value="" />
	<div id="layout-content-wrapper">
		<div id="layout-content" class="container">
			<div class="panel panel-default">
				<div class="panel-body">
					<fieldset class="form-horizontal col-xs-12">
						<div class="col-xs-6">
							<asp:HiddenField runat="server" ID="TimeStamp" />
							<div class="form-group required ">
								<label for="GroupType" class="col-xs-5 control-label">群組代碼</label>
								<div class="col-xs-5">
									<asp:TextBox runat="server" data-val="true" data-val-required="請輸入群組代碼" ID="GroupType" type="text" class="form-control"></asp:TextBox>
									<span class="field-validation-valid" data-valmsg-for="GroupType" data-valmsg-replace="true"></span>
								</div>
							</div>
							<div class="form-group required ">
								<label for="Resource" class="col-xs-5 control-label">存取資源</label>
								<div class="col-xs-5">
									<asp:ListBox runat="server" data-val="true" CssClass="form-control" data-val-required="請選擇存取資源" ID="Resource" ClientIDMode="Static" SelectionMode="Multiple"></asp:ListBox>
									<span class="field-validation-valid" data-valmsg-for="Resource" data-valmsg-replace="true"></span>
								</div>
							</div>
						</div>

						<div class="col-xs-6">
							<div class="form-group required ">
								<label for="GroupName" class="col-xs-5 control-label">群組名稱</label>
								<div class="col-sm-5">
									<asp:TextBox runat="server" data-val="true" data-val-required="請輸入群組名稱" ID="GroupName" type="text" class="form-control"></asp:TextBox>
									<span class="field-validation-valid" data-valmsg-for="GroupName" data-valmsg-replace="true"></span>
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
	<script src='<%: VirtualPathUtility.ToAbsolute(@"~/Scripts/Sign/GroupData.js") %>'></script>

</asp:Content>
