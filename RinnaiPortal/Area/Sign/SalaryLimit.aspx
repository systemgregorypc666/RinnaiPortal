<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SalaryLimit.aspx.cs" Inherits="RinnaiPortal.Area.Sign.SalaryLimit" %>

<asp:Content ID="SalaryLimitContent" ContentPlaceHolderID="MainContent" runat="server">
	<input runat="server" type="text" hidden id="PageTitle" class='page-title' value="" />
	<div id="layout-content-wrapper">
		<div id="layout-content" class="container">
			<div class="panel panel-default">
				<div class="panel-body">
					<fieldset class="form-horizontal col-xs-12">
						<asp:HiddenField runat="server" ID="TimeStamp" />
						<asp:HiddenField runat="server" ID="SN" />

						<div class="col-xs-6">
							<div class="form-group required">
								<label for="LimitDate" class="col-xs-3 control-label">結薪日期</label>
								<div class='input-group col-xs-5' id='datetimepicker'>
									<asp:TextBox runat="server" data-val="true" data-val-required="請輸結薪日期" ClientIDMode="Static" ID="LimitDate" class="form-control" data-date-format="YYYY-MM-DD 00:00:00" />
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

					<asp:Button ClientIDMode="Static" runat="server" ID="SaveBtn" type="submit" CssClass="btn btn-primary" OnClick="SaveBtn_Click" data-loading-text="Loading..." Text="送出" PostBackUrl="~/Area/Sign/SalaryLimit.aspx"></asp:Button>

					<asp:LinkButton runat="server" ClientIDMode="Static" ID="CoverBtn" CssClass="CoverBtn btn display-none">
						<span class="glyphicon glyphicon-floppy-saved"></span>
						<img class="loader" src='<%: VirtualPathUtility.ToAbsolute(@"~/icon/ajax-loader.gif") %>'  />
					</asp:LinkButton>

					<a runat="server" id="RefreshBtn" class="btn btn-warning display-none" href="javascript:location.href=document.URL">
						<span class="glyphicon glyphicon-refresh"></span>&nbsp;重新整理</a>
				</div>
			</div>
		</div>
	</div>
	<script src='<%: VirtualPathUtility.ToAbsolute(@"~/Scripts/Sign/validate-summary.js") %>'></script>
</asp:Content>
