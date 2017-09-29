<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OvertimeList.aspx.cs" Inherits="RinnaiPortal.Area.Sign.Forms.OvertimeList" %>
<%@ Import Namespace="RinnaiPortal.Extensions" %>
<%@ Import Namespace="RinnaiPortal.Tools" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
	<title></title>
	<link rel="stylesheet" type="text/css" href="../../../Content/bootstrap.css" />
	<link rel="stylesheet" type="text/css" href="../../../Content/custom.css" />
	<link rel="stylesheet" type="text/css" href="../../../Content/dialog-layout.css" />
	<script src='<%: VirtualPathUtility.ToAbsolute(@"~/Scripts/jquery-1.10.2.js")%>' type="text/javascript"></script>
	<script src='<%: VirtualPathUtility.ToAbsolute(@"~/Scripts/bootstrap.js")%>' type="text/javascript"></script>
	<script src='<%: VirtualPathUtility.ToAbsolute(@"~/Scripts/Sign/main.js")%>' type="text/javascript"></script>
	<script src='<%: VirtualPathUtility.ToAbsolute(@"~/Scripts/Sign/Forms/OvertimeList.js")%>' type="text/javascript"></script>
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
</head>
<body id="DialogLayout">
	<div class="modal-dialog">
		<div class="modal-header">
			<h4 class="modal-title" id="myModalLabel"><b>表單內容 </b></h4>
		</div>
		<div class="modal-body">
			<form runat="server">
				<div class="col-xs-6">
					<asp:HiddenField runat="server" ID="SN" />
					<div class="form-group row">
						<label for="SignDocID" class="col-xs-5 control-label">簽核編號</label>
						<div class="col-xs-7">
							<asp:Label runat="server" ID="SignDocID"></asp:Label>
						</div>
					</div>
					<div class="form-group row">
						<label for="EmployeeID_FK" class="col-xs-5 control-label">申請人編號</label>
						<div class="col-xs-7 ">
							<asp:Label runat="server" ID="EmployeeID_FK"></asp:Label>
						</div>
					</div>
				</div>

				<div class="col-xs-6">
					<div class="form-group row">
						<label for="SendDate" class="col-xs-4 control-label">申請日期</label>
						<div class="col-xs-8 ">
							<asp:Label runat="server" ID="SendDate"></asp:Label>
						</div>
					</div>
					<div class="form-group row">
						<label for="ApplyName" class="col-xs-4 control-label">申請人姓名</label>
						<div class="col-xs-8 ">
							<asp:Label runat="server" ID="ApplyName"></asp:Label>
						</div>
					</div>
                    <div class="form-group row">
                        <label class="col-xs-4 ">簽核歷程</label>
                        <div class="col-xs-8">
                            <asp:HyperLink ID="Signed" runat="server"  CssClass="btn btn-info btn-xs" CommandName="Detail" Target="_self" Text="簽核歷程" Width="80px"></asp:HyperLink>                   
                        </div>
                    </div>
				</div>
                <div id="div1" style="width:830px;height:280px;overflow-x:auto;overflow-y:auto;">
				<asp:GridView runat="server" CssClass="table table-condensed table-bordered table-striped" ID="OvertimeGridView" DataKeys="EmployeeID,StartDateTime" onrowdatabound="OvertimeGridView_RowDataBound" AutoGenerateColumns="false">
					<Columns>
						<asp:TemplateField>
							<HeaderTemplate>
								<asp:Label runat="server" ClientIDMode="Static" ID="Type" Text="類型"></asp:Label>
							</HeaderTemplate>
							<ItemTemplate>
								<asp:Label runat="server" Text='<%# Eval("Type") %>'> </asp:Label>
							</ItemTemplate>
							<HeaderStyle CssClass="info" />
							<ItemStyle CssClass="Type" />
						</asp:TemplateField>
						<asp:TemplateField>
							<HeaderTemplate>
								<asp:Label runat="server" ClientIDMode="Static" ID="EmployeeID" Text="員工編號"></asp:Label>
							</HeaderTemplate>
							<ItemTemplate>
								<asp:Label id="lbl_EmployeeID" runat="server" Text='<%# Eval("EmployeeID") %>'> </asp:Label>
							</ItemTemplate>
							<ItemStyle CssClass="EmployeeID" />
						</asp:TemplateField>
						<asp:TemplateField>
							<HeaderTemplate>
								<asp:Label runat="server" ClientIDMode="Static" ID="EmployeeName" Text="員工姓名"></asp:Label>
							</HeaderTemplate>
							<ItemTemplate>
								<asp:Label runat="server" Text='<%# Eval("EmployeeName") %>'> </asp:Label>
							</ItemTemplate>
							<ItemStyle CssClass="EmployeeName" />
						</asp:TemplateField>
						<asp:TemplateField>
							<HeaderTemplate>
								<asp:Label runat="server" ClientIDMode="Static" ID="DepartmentName" Text="部門名稱"></asp:Label>
							</HeaderTemplate>
							<ItemTemplate>
								<asp:Label runat="server" Text='<%# Eval("DepartmentName") %>'> </asp:Label>
							</ItemTemplate>
							<ItemStyle CssClass="DepartmentName" />
						</asp:TemplateField>
						<asp:TemplateField>
							<HeaderTemplate>
								<asp:Label runat="server" ClientIDMode="Static" ID="RealStartDateTime" Text="上班時間"></asp:Label>
							</HeaderTemplate>
							<ItemTemplate>
								<asp:Label runat="server" Text='<%# Eval("RealStartDateTime").ToDateTimeFormateString("yyyy-MM-dd HH:mm") %>'> </asp:Label>
							</ItemTemplate>
							<ItemStyle CssClass="RealStartDateTime" />
						</asp:TemplateField>
						<asp:TemplateField>
							<HeaderTemplate>
								<asp:Label runat="server" ClientIDMode="Static" ID="RealEndDateTime" Text="下班時間"></asp:Label>
							</HeaderTemplate>
							<ItemTemplate>
								<asp:Label runat="server" Text='<%# Eval("RealEndDateTime").ToDateTimeFormateString("yyyy-MM-dd HH:mm") %>'> </asp:Label>
							</ItemTemplate>
							<ItemStyle CssClass="RealEndDateTime" />
						</asp:TemplateField>
						<asp:TemplateField>
							<HeaderTemplate>
								<asp:Label  runat="server" ClientIDMode="Static" ID="StartDateTime" Text="加班起"></asp:Label>
							</HeaderTemplate>
							<ItemTemplate>
								<asp:Label id="lbl_StartDateTime" runat="server" Text='<%# Eval("StartDateTime").ToDateTimeFormateString("yyyy-MM-dd HH:mm") %>'> </asp:Label>
							</ItemTemplate>
							<ItemStyle CssClass="StartDateTime" />
						</asp:TemplateField>
						<asp:TemplateField>
							<HeaderTemplate>
								<asp:Label runat="server" ClientIDMode="Static" ID="EndDateTime" Text="加班迄"></asp:Label>
							</HeaderTemplate>
							<ItemTemplate>
								<asp:Label runat="server" Text='<%# Eval("EndDateTime").ToDateTimeFormateString("yyyy-MM-dd HH:mm") %>'> </asp:Label>
							</ItemTemplate>
							<ItemStyle CssClass="EndDateTime" />
						</asp:TemplateField>
                        <asp:TemplateField>
							<HeaderTemplate>
								<asp:Label runat="server" ClientIDMode="Static" ID="totalH" Text="">法定加班時數<br>(上限46H、含本次)</asp:Label>
							</HeaderTemplate>
							<ItemTemplate>
								<asp:Label id="lbl_total" runat="server" Text=''> </asp:Label>
							</ItemTemplate>
							<ItemStyle CssClass="totalH" />
						</asp:TemplateField>
                        <asp:TemplateField>
							<HeaderTemplate>
								<asp:Label runat="server" ClientIDMode="Static" ID="totalH2" Text="">本月加班累計時數<br>(含本次)</asp:Label>
							</HeaderTemplate>
							<ItemTemplate>
								<asp:Label id="lbl_Alltotal" runat="server" Text=''> </asp:Label>
							</ItemTemplate>
							<ItemStyle CssClass="totalH2" />
						</asp:TemplateField>
						<asp:TemplateField>
							<HeaderTemplate>
								<asp:Label runat="server" ClientIDMode="Static" ID="SupportDeptName" Text="支援單位"></asp:Label>
							</HeaderTemplate>
							<ItemTemplate>
								<asp:Label runat="server" Text='<%# Eval("SupportDeptName") %>'> </asp:Label>
							</ItemTemplate>
							<ItemStyle CssClass="SupportDeptName" />
						</asp:TemplateField>
						<asp:TemplateField>
							<HeaderTemplate>
								<asp:Label runat="server" ClientIDMode="Static" ID="PayTypeKey" Text="報酬種類"></asp:Label>
							</HeaderTemplate>
							<ItemTemplate>
								<asp:Label runat="server" Text='<%# ViewUtils.ParsePayType(Eval("PayTypeKey").ToString()) %>'> </asp:Label>
							</ItemTemplate>
							<ItemStyle CssClass="PayTypeKey" />
						</asp:TemplateField>
						<asp:TemplateField>
							<HeaderTemplate>
								<asp:Label runat="server" ClientIDMode="Static" ID="TotalHours" Text="時數"></asp:Label>
							</HeaderTemplate>
							<ItemTemplate>
								<asp:Label runat="server" Text='<%# Eval("TotalHours") %>'> </asp:Label>
							</ItemTemplate>
							<ItemStyle CssClass="TotalHours" />
						</asp:TemplateField>
						<asp:TemplateField>
							<HeaderTemplate>
								<asp:Label runat="server" ClientIDMode="Static" ID="Note" Text="原因"></asp:Label>
							</HeaderTemplate>
							<ItemTemplate>
								<asp:Label runat="server" Text='<%# Eval("Note") %>'> </asp:Label>
							</ItemTemplate>
							<ItemStyle CssClass="Note" />
						</asp:TemplateField>
					</Columns>
					<RowStyle CssClass="text-center" />
					<AlternatingRowStyle BackColor="#f9f9f9" CssClass="text-center" />
					<HeaderStyle CssClass="text-nowrap info" Font-Bold="true" BackColor="#dff0d8" />
				</asp:GridView>
                </div>
			</form>
		</div>
		<div class="modal-footer">
			<div runat="server" id="paginationBar" class="text-center"></div>
		</div>
	</div>

</body>
</html>
