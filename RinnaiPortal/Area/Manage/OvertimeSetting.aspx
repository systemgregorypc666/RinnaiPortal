<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OvertimeSetting.aspx.cs" Inherits="RinnaiPortal.Area.Manage.OvertimeSetting" %>

<%@ Import Namespace="RinnaiPortal.Extensions" %>
<%@ Import Namespace="RinnaiPortal.Tools" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title></title>
	<link rel="stylesheet" type="text/css" href="/Content/bootstrap.css" />
	<link rel="stylesheet" type="text/css" href="/Content/custom.css" />
	<script src='<%: VirtualPathUtility.ToAbsolute(@"~/Scripts/jquery-1.10.2.js")%>' type="text/javascript"></script>
	<script src='<%: VirtualPathUtility.ToAbsolute(@"~/Scripts/bootstrap.js")%>' type="text/javascript"></script>
	<script src='<%: VirtualPathUtility.ToAbsolute(@"~/Scripts/Sign/main.js")%>' type="text/javascript"></script>
	<script type="text/javascript">
	    Dialog.resize(1200, 600);

	    window.onload = jf_init;
	    function jf_init() {
	        var nHeight = screen.height;//取得使用者螢幕高
	        var nWidth = screen.width;//取得使用者螢幕寬
	        if (nHeight > 768) {
	            var divTarget = document.getElementById("div1");
	            //divTarget.style.height = "500px";
	            divTarget.style.width = "100%";
	        }
	    }
	</script>
	<style>
		#DialogLayout {
			background: #fff;
            
		}

			#DialogLayout .modal-header,
			#DialogLayout .modal-body,
			#DialogLayout .modal-footer,
			#DialogLayout .modal-footerlest {
				position: fixed;
				right: 0;
				left: 0;
			}

			#DialogLayout .modal-header {
				padding: 10px 15px;
				top: 0;
			}

			#DialogLayout .modal-body {
				top: 0;
				bottom: 60px;
				max-height: none;
			}

				#DialogLayout .modal-body.auto {
					overflow: auto;
				}

			#DialogLayout .modal-footer {
				padding: 9px 20px 10px;
				bottom: 0;
			}

			#DialogLayout .modal-footerlest {
				padding: 10px;
				bottom: 0;
			}

			#DialogLayout .modal-footer .page-links {
				margin: 5px 0;
			}

			#DialogLayout .modal-header + .modal-body {
				top: 49px;
			}

			#DialogLayout .modal-body:last-child {
				bottom: 0;
			}

			#DialogLayout .modal-close {
				position: fixed;
				top: 5px;
				right: 5px;
				width: 20px;
				height: 20px;
				line-height: 1em;
				font-size: 16px;
				font-weight: bold;
				padding: 0;
				z-index: 9999;
				zoom: 1;
				color: #fff;
				background: #333;
				border-radius: 15px;
				opacity: 0.4;
				text-align: center;
			}
	</style>
</head>
<form runat="server">
	<body id="DialogLayout">

		<div  style="overflow-y:scroll;">
			<div class="modal-header">
				<h4 class="modal-title" id="myModalLabel"><b>加班資料維護 </b></h4>
			</div>
			<div class="modal-body">
				<div class="col-xs-6">
					<div class="form-group row">
						<div class='col-xs-5 input-group '>
							<span class="input-group-addon">計薪月份</span>
							<asp:DropDownList runat="server" ClientIDMode="static" ID="PayRange" class="form-control" data-val="true" AutoPostBack="true">
							</asp:DropDownList>
						</div>
					</div>

				</div>

				<div>
                    <div id="div1" style="width:830px;height:280px;overflow-x:auto;overflow-y:auto;">
				    <asp:GridView runat="server" CssClass="table table-striped table-bordered table-condensed " ID="OvertimeSettingGridView" AutoGenerateColumns="False">
					    <Columns>
						    <asp:TemplateField HeaderText="SignDocID_FK">
							    <HeaderTemplate>
								    <asp:Label runat="server" ClientIDMode="Static" Text="簽核編號"></asp:Label>
							    </HeaderTemplate>
							    <ItemTemplate>
								    <asp:Label runat="server" ID="SignDocID_FK" Text='<%# Eval("SignDocID_FK") %>'> </asp:Label>
							    </ItemTemplate>
							    <HeaderStyle CssClass="info" />
							    <ItemStyle CssClass="SignDocID_FK" />
						    </asp:TemplateField>
						    <asp:TemplateField HeaderText="ApplyID_FK">
							    <HeaderTemplate>
								    <asp:Label runat="server" ClientIDMode="Static" Text="申請人編號"></asp:Label>
							    </HeaderTemplate>
							    <ItemTemplate>
								    <asp:Label runat="server" ID="ApplyID_FK" Text='<%# Eval("ApplyID_FK") %>'> </asp:Label>
							    </ItemTemplate>
							    <ItemStyle CssClass="ApplyID_FK" />
						    </asp:TemplateField>
						    <asp:TemplateField HeaderText="ApplyDateTime">
							    <HeaderTemplate>
								    <asp:Label runat="server" ClientIDMode="Static" Text="申請日期"></asp:Label>
							    </HeaderTemplate>
							    <ItemTemplate>
								    <asp:Label runat="server" ID="ApplyDateTime" Text='<%# Eval("ApplyDateTime").ToDateTimeFormateString() %>'> </asp:Label>
							    </ItemTemplate>
							    <ItemStyle CssClass="ApplyDateTime" />
						    </asp:TemplateField>
						    <asp:TemplateField HeaderText="EmployeeID_FK">
							    <HeaderTemplate>
								    <asp:Label runat="server" ClientIDMode="Static" Text="員工編號"></asp:Label>
							    </HeaderTemplate>
							    <ItemTemplate>
								    <asp:Label runat="server" ID="EmployeeID_FK" Text='<%# Eval("EmployeeID_FK") %>'> </asp:Label>
							    </ItemTemplate>
							    <ItemStyle CssClass="EmployeeID_FK" />
						    </asp:TemplateField>
						    <asp:TemplateField HeaderText="DepartmentID_FK">
							    <HeaderTemplate>
								    <asp:Label runat="server" ClientIDMode="Static" Text="部門編號"></asp:Label>
							    </HeaderTemplate>
							    <ItemTemplate>
								    <asp:Label runat="server" ID="DepartmentID_FK" Text='<%# Eval("DepartmentID_FK") %>'> </asp:Label>
							    </ItemTemplate>
							    <ItemStyle CssClass="DepartmentID_FK" />
						    </asp:TemplateField>
						    <asp:TemplateField HeaderText="StartDateTime">
							    <HeaderTemplate>
								    <asp:Label runat="server" ClientIDMode="Static" Text="加班起"></asp:Label>
							    </HeaderTemplate>
							    <ItemTemplate>
								    <asp:Label runat="server" ID="StartDateTime" Text='<%# Eval("StartDateTime").ToDateTimeFormateString() %>'> </asp:Label>
							    </ItemTemplate>
							    <ItemStyle CssClass="StartDateTime" />
						    </asp:TemplateField>
						    <asp:TemplateField HeaderText="EndDateTime">
							    <HeaderTemplate>
								    <asp:Label runat="server" ClientIDMode="Static" Text="加班迄"></asp:Label>
							    </HeaderTemplate>
							    <ItemTemplate>
								    <asp:Label runat="server" ID="EndDateTime" Text='<%# Eval("EndDateTime").ToDateTimeFormateString() %>'> </asp:Label>
							    </ItemTemplate>
							    <ItemStyle CssClass="EndDateTime" />
						    </asp:TemplateField>
						    <asp:TemplateField HeaderText="SupportDeptID_FK" Visible="false">
							    <HeaderTemplate>
								    <asp:Label runat="server" ClientIDMode="Static" Text="支援單位代碼"></asp:Label>
							    </HeaderTemplate>
							    <ItemTemplate>
								    <asp:Label runat="server" ID="SupportDeptID_FK" Text='<%# Eval("SupportDeptID_FK") %>'> </asp:Label>
							    </ItemTemplate>
							    <ItemStyle CssClass="SupportDeptID_FK" />
						    </asp:TemplateField>
						    <asp:TemplateField HeaderText="PayTypeKey" Visible="false">
							    <HeaderTemplate>
								    <asp:Label runat="server" ClientIDMode="Static" Text="報酬種類"></asp:Label>
							    </HeaderTemplate>
							    <ItemTemplate>
								    <asp:Label runat="server" ID="PayTypeKey" Text='<%# Eval("PayTypeKey") %>'> </asp:Label>
							    </ItemTemplate>
							    <ItemStyle CssClass="PayTypeKey" />
						    </asp:TemplateField>
						    <asp:TemplateField HeaderText="PayTypeValue">
							    <HeaderTemplate>
								    <asp:Label runat="server" ClientIDMode="Static" Text="報酬種類"></asp:Label>
							    </HeaderTemplate>
							    <ItemTemplate>
								    <asp:Label runat="server" ID="PayTypeValue" Text='<%# ViewUtils.ParsePayType(Eval("PayTypeKey").ToString()) %>'> </asp:Label>
							    </ItemTemplate>
							    <ItemStyle CssClass="PayTypeValue" />
						    </asp:TemplateField>
						    <asp:TemplateField HeaderText="TotalHours">
							    <HeaderTemplate>
								    <asp:Label runat="server" ClientIDMode="Static" Text="時數"></asp:Label>
							    </HeaderTemplate>
							    <ItemTemplate>
								    <asp:Label runat="server" ID="TotalHours" Text='<%# Eval("TotalHours") %>'> </asp:Label>
							    </ItemTemplate>
							    <ItemStyle CssClass="TotalHours" />
						    </asp:TemplateField>
						    <asp:TemplateField HeaderText="Note">
							    <HeaderTemplate>
								    <asp:Label runat="server" ClientIDMode="Static" Text="原因"></asp:Label>
							    </HeaderTemplate>
							    <ItemTemplate>
								    <asp:Label runat="server" ID="Note" Text='<%# Eval("Note") %>'> </asp:Label>
							    </ItemTemplate>
							    <ItemStyle CssClass="Note" />
						    </asp:TemplateField>
						    <asp:TemplateField HeaderText="BeginTime">
							    <HeaderTemplate>
								    <asp:Label runat="server" ClientIDMode="Static" Text="上班刷卡"></asp:Label>
							    </HeaderTemplate>
							    <ItemTemplate>
								    <asp:Label runat="server" ID="BeginTime" Text='<%# Eval("BEGINTIME") %>'> </asp:Label>
							    </ItemTemplate>
							    <ItemStyle CssClass="BeginTime" />
						    </asp:TemplateField>
						    <asp:TemplateField HeaderText="EndTime">
							    <HeaderTemplate>
								    <asp:Label runat="server" ClientIDMode="Static" Text="下班刷卡"></asp:Label>
							    </HeaderTemplate>
							    <ItemTemplate>
								    <asp:Label runat="server" ID="EndTime" Text='<%#Eval("ENDTIME") %>'> </asp:Label>
							    </ItemTemplate>
							    <ItemStyle CssClass="EndTime" />
						    </asp:TemplateField>
						    <asp:TemplateField HeaderText="CostDepartmentID">
							    <HeaderTemplate>
								    <asp:Label runat="server" ClientIDMode="Static" Text="成本單位"></asp:Label>
							    </HeaderTemplate>
							    <ItemTemplate>
								    <asp:Label runat="server" ID="CostDepartmentID" Text='<%# Eval("CostDepartmentID") %>'> </asp:Label>
							    </ItemTemplate>
							    <ItemStyle CssClass="CostDepartmentID" />
						    </asp:TemplateField>
						    <asp:TemplateField HeaderText="WorkType">
							    <HeaderTemplate>
								    <asp:Label runat="server" ClientIDMode="Static" Text="工時型別"></asp:Label>
							    </HeaderTemplate>
							    <ItemTemplate>
								    <asp:Label runat="server" ID="WorkType" Text='<%# Eval("WORKTYPE") %>'> </asp:Label>
							    </ItemTemplate>
							    <ItemStyle CssClass="WorkType" />
						    </asp:TemplateField>
					    </Columns>
					    <RowStyle CssClass="text-center" />
					    <AlternatingRowStyle BackColor="#f9f9f9" CssClass="text-center" />
					    <HeaderStyle CssClass=" info" Font-Bold="true" BackColor="#dff0d8" />
				    </asp:GridView>
                    </div>
                </div>
			</div>
		</div>
		<div class="modal-footer">
			<div runat="server" id="paginationBar" class="text-center"></div>
		</div>
		<div class="modal-footerlest">
			<asp:Button ClientIDMode="Static" runat="server" ID="SaveBtn" CssClass="btn btn-primary pull-right" OnClick="SaveBtn_Click" data-loading-text="Loading..." Text="送出"></asp:Button>

			<asp:LinkButton runat="server" ClientIDMode="Static" ID="CoverBtn" CssClass="CoverBtn btn pull-right display-none">
						<span class="glyphicon glyphicon-floppy-saved"></span>
						<img class="loader" src='<%: VirtualPathUtility.ToAbsolute(@"~/icon/ajax-loader.gif") %>'  />
			</asp:LinkButton>

			<a runat="server" id="RefreshBtn" class="btn btn-warning pull-right display-none" href="javascript:location.href=document.URL">
				<span class="glyphicon glyphicon-refresh"></span>&nbsp;重新整理</a>

		</div>
	</body>
</form>
</html>
