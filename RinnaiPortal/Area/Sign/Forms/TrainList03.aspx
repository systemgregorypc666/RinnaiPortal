<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TrainList03.aspx.cs" Inherits="RinnaiPortal.Area.Sign.Forms.TrainList03" %>

<%@ Import Namespace="RinnaiPortal.Tools" %>
<%@ Import Namespace="RinnaiPortal.Extensions" %>
<asp:Content ID="TrainListContent" ContentPlaceHolderID="MainContent" runat="server">
	<input runat="server" type="text" hidden id="PageTitle" class='page-title' value="" />
	<div id="layout-content-wrapper">
		<div id="layout-content" class="container">
			<div class="list-toolbar">
				<%--<a class="btn btn-success pull-left" href="AgentData.aspx">
					<span class="glyphicon glyphicon-plus"></span>&nbsp;新增
				</a>--%>

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

				<div class="row text-center">
					<div class="col-lg-3" style="float: none; display: inline-block">
						<div class="input-group">
							<asp:TextBox runat="server" ID="queryTextBox" class="form-control" placeholder="課程代號 or 學員工號" />
							<span class="input-group-btn">
								<asp:LinkButton runat="server" class="btn btn-default" ID="queryBtn" OnClick="QueryBtn_Click"> <span class="glyphicon glyphicon-search"></span>&nbsp;查詢 </asp:LinkButton>
							</span>
						</div>
					</div>
				</div>

			</div>
			<div class="text-center">
				<asp:Label runat="server" ID="noDataTip"></asp:Label>
			</div>

			<asp:HiddenField runat="server" ClientIDMode="static" ID="FormSeries"/>

			<asp:GridView runat="server" CssClass="table table-striped table-bordered table-condensed" ID="TrainGridView" AutoGenerateColumns="False">
				<Columns>
					<asp:TemplateField>
						<HeaderTemplate>
							<asp:Label runat="server" ClientIDMode="Static" ID="RowNum">序號</asp:Label>
						</HeaderTemplate>
						<ItemTemplate>
							<asp:Label runat="server" Text='<%# Eval("RowNum") %>' CssClass="RowNum"> </asp:Label>
						</ItemTemplate>
						<HeaderStyle CssClass="info" />
					</asp:TemplateField>
					<asp:TemplateField>
						<HeaderTemplate>
							<asp:HyperLink runat="server" ClientIDMode="Static" ID="CLID" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("CLID", Request)%>' Text="課程代號"></asp:HyperLink>
						</HeaderTemplate>
						<ItemTemplate>
							<asp:Label runat="server" Text='<%# Eval("CLID") %>' CssClass="CLID"> </asp:Label>
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField>
						<HeaderTemplate>
							<asp:HyperLink runat="server" ClientIDMode="Static" ID="CLNAME" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("CLNAME", Request)%>' Text="課程名稱"></asp:HyperLink>
						</HeaderTemplate>
						<ItemTemplate>
							<asp:Label runat="server" Text='<%# Eval("CLNAME") %>' CssClass="CLNAME"> </asp:Label>
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField>
						<HeaderTemplate>
							<asp:HyperLink runat="server" ClientIDMode="Static" ID="STARTDATE" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("STARTDATE", Request)%>' Text="開課日期"></asp:HyperLink>
						</HeaderTemplate>
						<ItemTemplate>
							<asp:Label runat="server" Text='<%# Eval("STARTDATE").ToDateTimeFormateString("yyyyMMdd") %>' CssClass="STARTDATE"> </asp:Label>
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField>
						<HeaderTemplate>
							<asp:HyperLink runat="server" ClientIDMode="Static" ID="HOURS" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("HOURS", Request)%>' Text="時數"></asp:HyperLink>
						</HeaderTemplate>
						<ItemTemplate>
							<asp:Label runat="server" Text='<%# Eval("HOURS") %>' CssClass="HOURS"> </asp:Label>
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField>
						<HeaderTemplate>
							<asp:HyperLink runat="server" ClientIDMode="Static" ID="SID" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("SID", Request)%>' Text="學員編號"></asp:HyperLink>
						</HeaderTemplate>
						<ItemTemplate>
							<asp:Label runat="server" Text='<%# Eval("SID") %>' CssClass="SID"> </asp:Label>
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField>
						<HeaderTemplate>
							<asp:HyperLink runat="server" ClientIDMode="Static" ID="SNAME" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("SNAME", Request)%>' Text="學員姓名"></asp:HyperLink>
						</HeaderTemplate>
						<ItemTemplate>
							<asp:Label runat="server" Text='<%# Eval("SNAME") %>' CssClass="SNAME"> </asp:Label>
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField>
						<HeaderTemplate>
							<asp:HyperLink runat="server" ClientIDMode="Static" ID="UNITID" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("UNITID", Request)%>' Text="單位代號"></asp:HyperLink>
						</HeaderTemplate>
						<ItemTemplate>
							<asp:Label runat="server" Text='<%# Eval("UNITID") %>' CssClass="UNITID"> </asp:Label>
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField>
						<HeaderTemplate>
							<asp:HyperLink runat="server" ClientIDMode="Static" ID="UNITNAME" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("UNITNAME", Request)%>' Text="單位名稱"></asp:HyperLink>
						</HeaderTemplate>
						<ItemTemplate>
							<asp:Label runat="server" Text='<%# Eval("UNITNAME") %>' CssClass="UNITNAME"> </asp:Label>
						</ItemTemplate>
					</asp:TemplateField>
                    <asp:TemplateField HeaderText="填寫" HeaderStyle-CssClass="text-center">
						<ItemStyle CssClass="vertical-middle" />
						<ItemTemplate>
							<asp:HyperLink runat="server" CommandName="Edit" ClientIDMode="Static" ID="Edit" CssClass="btn btn-warning btn-xs" NavigateUrl='<%# "~/Area/Sign/Forms/TrainDetail03.aspx?CLID=" + Eval("CLID") + "&SID=" + Eval("SID") %>'
								Text="編輯" Width="50px"><span class="glyphicon glyphicon-pencil"></span></asp:HyperLink>
						</ItemTemplate>
					</asp:TemplateField>
					<%--<asp:TemplateField>
						<HeaderTemplate>
							<asp:HyperLink runat="server" ClientIDMode="Static" ID="SIGNDOCID" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("SIGNDOCID", Request)%>' Text="簽核編號"></asp:HyperLink>
						</HeaderTemplate>
						<ItemTemplate>
							<asp:Label runat="server" Text='<%# Eval("SIGNDOCID") %>' CssClass="SIGNDOCID"> </asp:Label>
						</ItemTemplate>
					</asp:TemplateField>--%>
					<%--<asp:TemplateField HeaderText="明細" HeaderStyle-CssClass="text-center">
						<ItemStyle CssClass="vertical-middle" />
						<ItemTemplate>
							<asp:HyperLink runat="server" CommandName="Detail" CssClass="btn btn-info btn-xs" Target="dialog" NavigateUrl='<%# "~/Area/Sign/Forms/TrainDetail01.aspx?CLID=" + Eval("CLID") + "&SID=" + Eval("SID") %>'
								Text="明細" Width="50px"><span class="glyphicon glyphicon-list-alt"></span></asp:HyperLink>
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField HeaderText="簽核" HeaderStyle-CssClass="text-center">
						<ItemStyle CssClass="vertical-middle" />
						<ItemTemplate>
							<asp:HyperLink runat="server" CommandName="SignSubmit" CssClass="btn btn-primary btn-xs SignSubmit"
								Text="簽核" Width="50px"><span class="glyphicon glyphicon-send"></span></asp:HyperLink>

							<button class="btn btn-xs text-center Coverbtn" style="display: none;">
								<span class="glyphicon glyphicon-send"></span>
								<img class="loader" src='<%: VirtualPathUtility.ToAbsolute(@"~/icon/ajax-loader.gif") %>' />
							</button>
						</ItemTemplate>
					</asp:TemplateField>--%>
				</Columns>
				<RowStyle CssClass="text-center" />
				<AlternatingRowStyle BackColor="#f9f9f9" CssClass="text-center" />
				<HeaderStyle CssClass="text-nowrap info" Font-Bold="true" BackColor="#dff0d8" />
			</asp:GridView>
		</div>
		<div runat="server" id="paginationBar" class="text-center"></div>
	</div>
	<%--<script src='<%: VirtualPathUtility.ToAbsolute(@"~/Scripts/Sign/Forms/TrainList.js") %>' type="text/javascript"></script>--%>

</asp:Content>
