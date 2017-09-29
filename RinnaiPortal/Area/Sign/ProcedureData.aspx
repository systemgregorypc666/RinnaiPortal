<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ProcedureData.aspx.cs" Inherits="RinnaiPortal.Area.Sign.ProcedureData" %>

<asp:Content ID="ProcedureDataContent" ContentPlaceHolderID="MainContent" runat="server">
	<input runat="server" type="text" hidden id="PageTitle" class='page-title' value="" />
	<div id="layout-content-wrapper">
		<div id="layout-content" class="container">
			<div class="panel panel-default">
				<div class="panel-body">
					<fieldset class="form-horizontal col-xs-12">
						<div class="col-xs-6">
							<div class="form-group required ">
							<asp:HiddenField runat="server" ID="TimeStamp" />
								<label for="SignID" class="col-xs-5 control-label">簽核代碼</label>
								<div class="col-xs-5">
									<asp:TextBox runat="server" data-val="true" data-val-required="請輸入簽核代碼" ID="SignID" class="form-control" ></asp:TextBox>
									<span class="field-validation-valid" data-valmsg-for="SignID" data-valmsg-replace="true"></span>
								</div>
							</div>

							<div class="form-group required">
								<label for="MaxLevel" class="col-xs-5 control-label">最高簽核層級</label>
								<div class="col-xs-5">
									<asp:TextBox runat="server" data-val="true" data-val-required="請輸入最高簽核層級" ID="MaxLevel" type="text" class="form-control"></asp:TextBox>
									<span class="field-validation-valid" data-valmsg-for="MaxLevel" data-valmsg-replace="true"></span>
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
							<div class="form-group required offest-onerow">
								<label for="SignLevel" class="col-xs-5 control-label">簽核層數</label>
								<div class="col-xs-5">
									<asp:TextBox runat="server" data-val="true" data-val-required="請輸入簽核層數" ID="SignLevel" type="text" class="form-control"></asp:TextBox>
									<span class="field-validation-valid" data-valmsg-for="SignLevel" data-valmsg-replace="true"></span>
								</div>
							</div>

							<div class="form-group">
								<label for="DisabledDate" class="col-xs-5 control-label">帳號停用日期</label>
								<div class='input-group date col-xs-5 datepicker-left-15' id='datetimepicker'>
									<asp:TextBox runat="server" data-val-required="請輸入帳號停用日期" ClientIDMode="Static" ID="DisabledDate" class="form-control" data-date-format="YYYY-MM-DD HH:mm:00" />
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
	<script src='<%: VirtualPathUtility.ToAbsolute(@"~/Scripts/Sign/validate-summary.js") %>'  type="text/javascript"></script>



</asp:Content>
