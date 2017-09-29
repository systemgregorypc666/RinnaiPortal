<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OvertimeSummary.aspx.cs" Inherits="RinnaiPortal.Area.Sign.Forms.OvertimeSummary" %>
<%@ Import Namespace="RinnaiPortal.Extensions" %>
<%@ Import Namespace="RinnaiPortal.Tools" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title></title>
	<link rel="stylesheet" type="text/css" href='/Content/bootstrap.css' />
	<script src='<%: VirtualPathUtility.ToAbsolute(@"~/Scripts/jquery-1.10.2.js") %>' type="text/javascript"></script>
	<script src='<%: VirtualPathUtility.ToAbsolute(@"~/Scripts/bootstrap.js") %>' type="text/javascript"></script>
	<script src='<%: VirtualPathUtility.ToAbsolute(@"~/Scripts/Sign/main.js") %>' type="text/javascript"></script>
	<script src='<%: VirtualPathUtility.ToAbsolute(@"~/Scripts/Sign/Forms/Overtime.js") %>' type="text/javascript"></script>
	<script type="text/javascript">
	    Dialog.resize(1200, 600);

	    window.onload = jf_init;
	    function jf_init() {
	        var nHeight = screen.height;//取得使用者螢幕高
	        var nWidth = screen.width;//取得使用者螢幕寬
	        if (nHeight > 768) {
	            var divTarget = document.getElementById("div1");
	            //divTarget.style.height = "500px";
	            divTarget.style.width = "1150px";
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

				#DialogLayout .modal-footer .page-links {
					margin: 5px 0;
				}

			#DialogLayout .modal-footerlest {            
				padding:10px;
				bottom: 0;
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
<body id="DialogLayout">

	<div class="modal-dialog">
		<div class="modal-header">
			<h4 class="modal-title" id="myModalLabel"><b> 確認簽核明細 </b></h4>
		</div>
		<div class="modal-body">
			<asp:HiddenField runat="server" ClientIDMode="Static" ID="FormSeries" />
			<div id="div1" style="width:900px;height:350px;overflow:auto;">
            <asp:GridView runat="server" CssClass="table table-striped table-bordered table-condensed text-nowrap" ID="OvertimeSummaryGridView" AutoGenerateColumns="False">
				<Columns>
					<asp:TemplateField>
						<HeaderTemplate>
							<asp:HyperLink runat="server" ClientIDMode="Static" ID="SignDocID" Text="文件編號" ></asp:HyperLink>
						</HeaderTemplate>
						<ItemTemplate>
							<asp:Label runat="server" Text='<%# Eval("SignDocID") %>' CssClass="signDocID"> </asp:Label>
							<asp:TextBox runat="server" Text='<%# Eval("SN") %>' CssClass="sn hide" ID="SN" ClientIDMode="Static"> </asp:TextBox>
							<asp:TextBox runat="server" Text='<%# Eval("FormID") %>' CssClass="formID hide" ID="FormID" ClientIDMode="Static"> </asp:TextBox>
							<asp:TextBox runat="server" Text='<%# Eval("ApplyDatetime") %>' CssClass="ApplyDatetime hide" ID="ApplyDatetime" ClientIDMode="Static"> </asp:TextBox>
							<asp:TextBox runat="server" Text='<%# Eval("DepartmentID") %>' CssClass="departmentID hide" ID="DepartmentID" ClientIDMode="Static"> </asp:TextBox>
							<asp:TextBox runat="server" Text='<%# Eval("SupportDeptID") %>' CssClass="supportDeptID hide" ID="SupportDeptID" ClientIDMode="Static"> </asp:TextBox>
						</ItemTemplate>
						<HeaderStyle CssClass="info" />
					</asp:TemplateField>
					<asp:TemplateField>
						<HeaderTemplate>
							<asp:HyperLink runat="server" ClientIDMode="Static" ID="EmployeeID" Text="員工編號" ></asp:HyperLink>
						</HeaderTemplate>
						<ItemTemplate>
							<asp:Label runat="server" Text='<%# Eval("EmployeeID") %>' CssClass="employeeID"> </asp:Label>
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField>
						<HeaderTemplate>
							<asp:HyperLink runat="server" ClientIDMode="Static" ID="EmployeeName" Text="員工姓名" ></asp:HyperLink>
						</HeaderTemplate>
						<ItemTemplate>
							<asp:Label runat="server" Text='<%# Eval("EmployeeName") %>' CssClass="employeeName"> </asp:Label>
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField>
						<HeaderTemplate>
							<asp:HyperLink runat="server" ClientIDMode="Static" ID="NationalType" Text="國別" ></asp:HyperLink>
						</HeaderTemplate>
						<ItemTemplate>
							<asp:Label runat="server" Text='<%# ViewUtils.ParseNationalType(Eval("NationalType").ToString()) %>' CssClass="nationalType"> </asp:Label>
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField>
						<HeaderTemplate>
							<asp:HyperLink runat="server" ClientIDMode="Static" ID="DepartmentName" Text="部門名稱" ></asp:HyperLink>
						</HeaderTemplate>
						<ItemTemplate>
							<asp:Label runat="server" Text='<%# Eval("DepartmentName") %>' CssClass="departmentName"> </asp:Label>
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField>
						<HeaderTemplate>
							<asp:HyperLink runat="server" ClientIDMode="Static" ID="StartDatetime" Text="加班時間(起)" ></asp:HyperLink>
						</HeaderTemplate>
						<ItemTemplate>
							<asp:Label runat="server" Text='<%# Eval("StartDatetime").ToDateTimeFormateString() %>' CssClass="startDateTime"> </asp:Label>
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField>
						<HeaderTemplate>
							<asp:HyperLink runat="server" ClientIDMode="Static" ID="EndDateTime" Text="加班時間(迄)" ></asp:HyperLink>
						</HeaderTemplate>
						<ItemTemplate>
							<asp:Label runat="server" Text='<%# Eval("EndDateTime").ToDateTimeFormateString() %>' CssClass="endDateTime"> </asp:Label>
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField>
						<HeaderTemplate>
							<asp:HyperLink runat="server" ClientIDMode="Static" ID="SupportDeptName" Text="支援部門名稱" ></asp:HyperLink>
						</HeaderTemplate>
						<ItemTemplate>
							<asp:Label runat="server" Text='<%# Eval("SupportDeptName") %>' CssClass="supportDeptName"> </asp:Label>
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField>
						<HeaderTemplate>
							<asp:HyperLink runat="server" ClientIDMode="Static" ID="PayTypeValue" Text="報酬類型" ></asp:HyperLink>
						</HeaderTemplate>
						<ItemTemplate>
							<asp:TextBox runat="server" Text='<%# Eval("PayTypeKey") %>' CssClass="payTypeKey hide"> </asp:TextBox>
							<asp:Label runat="server" Text='<%# ViewUtils.ParsePayType(Eval("PayTypeKey").ToString()) %>' CssClass="payTypeValue"> </asp:Label>
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField>
						<HeaderTemplate>
							<asp:HyperLink runat="server" ClientIDMode="Static" ID="MealOrderValue" Text="訂餐類型" ></asp:HyperLink>
						</HeaderTemplate>
						<ItemTemplate>
							<asp:TextBox runat="server" Text='<%# Eval("MealOrderKey") %>' CssClass="mealOrderKey hide"> </asp:TextBox>
							<asp:Label runat="server" Text='<%# ViewUtils.ParseMealOrder(Eval("MealOrderKey").ToString()) %>' CssClass="mealOrderValue"> </asp:Label>
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField>
						<HeaderTemplate>
							<asp:HyperLink runat="server" ClientIDMode="Static" ID="Note" Text="備註" ></asp:HyperLink>
						</HeaderTemplate>
						<ItemTemplate>
							<asp:Label runat="server" Text='<%# Eval("Note") %>' CssClass="note"> </asp:Label>
						</ItemTemplate>
					</asp:TemplateField>
				</Columns>
				<RowStyle CssClass="text-center" />
				<AlternatingRowStyle BackColor="#f9f9f9" CssClass="text-center" />
				<HeaderStyle CssClass="text-nowrap info" Font-Bold="true" BackColor="#dff0d8" />
			</asp:GridView>
            </div>
		</div>
	</div>
	<div class="modal-footer">
		<div runat="server" id="paginationBar" class="text-center"></div>
	</div>
	<div class="modal-footerlest">
		<button class="btn btn-primary text-center pull-right ApplyForm"><span class="glyphicon glyphicon-floppy-saved"></span>&nbsp;送出簽核</button>
		<button class="btn text-center pull-right Coverbtn" style="display: none;" >
			<span class="glyphicon glyphicon-floppy-saved"></span>
			<img class="loader" src='<%: VirtualPathUtility.ToAbsolute(@"~/icon/ajax-loader.gif") %>' />
		</button>

	</div>

</body>
</html>

