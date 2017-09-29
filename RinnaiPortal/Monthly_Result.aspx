<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Monthly_Result.aspx.cs" Inherits="RinnaiPortal.Monthly_Result" %>
<%@ Import Namespace="RinnaiPortal.Extensions" %>
<%@ Import Namespace="RinnaiPortal.Tools" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <input runat="server" type="text" hidden id="PageTitle" class='page-title' value="" />
    <div id="layout-content-wrapper">
        <div id="layout-content" class="container">
            <div class="list-toolbar row">
                <div class='input-group col-lg-2 pull-left'>
					<span class="input-group-addon">選擇員工</span>
					<asp:HiddenField runat="server" ID="DeptUserName"></asp:HiddenField>
					<asp:DropDownList runat="server" CssClass="form-control" Width="150px" ID="ddl_DeptUser" AutoPostBack="true"  OnSelectedIndexChanged="ddl_DeptUser_SelectedIndexChanged">
					</asp:DropDownList>
				</div>
            </div>
        </div>
        <div class="col-xs-12">
            <div class="text-left" style="color: #586B7A">
                工號【<asp:Label runat="server" ID="lbl_UserID" ></asp:Label>】姓名【<asp:Label runat="server" ID="lbl_UserName" ></asp:Label>】<strong>各月份出勤統計</strong></br><strong>目前剩餘換休時數：</strong>
                <strong><asp:Label runat="server" ID="lb_ADDOFFHOURS" style="color: #FF0000" ></asp:Label></strong><strong> H</strong>
                <hr style="margin:6px;height:1px;border:0px;background-color:#D5D5D5;color:#D5D5D5;"/>                                                                       
            </div>
            <div class="text-center">
				<asp:Label runat="server" ID="noDataTip"></asp:Label>
			</div>
            <asp:GridView runat="server" CssClass="table table-bordered table-condensed" ID="MonthlyGridView" onrowdatabound="MonthlyGridView_RowDataBound" AutoGenerateColumns="False">
                <Columns>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:HyperLink runat="server" ClientIDMode="Static" ID="PAYYYYYMM" Text="">計薪<br>月份</asp:HyperLink>
                        </HeaderTemplate>
                        <ItemTemplate>
                                <asp:HyperLink runat="server" Target="_self" NavigateUrl='<%# "Daily_Result.aspx?PAYYYYYMM=" + Eval("PAYYYYYMM")+"&UserID="+ Eval("EmployeeID") %>'><%# Eval("PAYYYYYMM") %></asp:HyperLink>
                        </ItemTemplate>
                    </asp:TemplateField>
                                       
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:HyperLink runat="server" ClientIDMode="Static" ID="OFFWORK3" Text="">曠職</asp:HyperLink>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label runat="server" ID="OFFWORK3" Text='<%# Eval("OFFWORK3") %>'> </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:HyperLink runat="server" ClientIDMode="Static" ID="OffWork14" Text="">出差</asp:HyperLink>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label runat="server" ID="OffWork14" Text='<%# Eval("OffWork14") %>'> </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:HyperLink runat="server" ClientIDMode="Static" ID="RECREATEDAYS" Text="">特休</asp:HyperLink>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label runat="server" ID="RECREATEDAYS" Text='<%# Eval("RECREATEDAYS") %>'> </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:HyperLink runat="server" ClientIDMode="Static" ID="OFFHOURS" Text="">換休</asp:HyperLink>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label runat="server" ID="OFFHOURS" Text='<%# Eval("OFFHOURS") %>'> </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:HyperLink runat="server" ClientIDMode="Static" ID="OFFWORK2" Text="">病假</asp:HyperLink>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label runat="server" ID="OFFWORK2" Text='<%# Eval("OFFWORK2") %>'> </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                        <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:HyperLink runat="server" ClientIDMode="Static" ID="OFFWORK1" Text="">事假</asp:HyperLink>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label runat="server" ID="OFFWORK1" Text='<%# Eval("OFFWORK1") %>'> </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:HyperLink runat="server" ClientIDMode="Static" ID="OFFWORK5M" Text="">遲到</asp:HyperLink>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label runat="server" ID="OFFWORK5M" Text='<%# Eval("OFFWORK5M") %>'> </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:HyperLink runat="server" ClientIDMode="Static" ID="OFFWORK6M" Text="">早退</asp:HyperLink>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label runat="server" ID="OFFWORK6M" Text='<%# Eval("OFFWORK6M") %>'> </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:HyperLink runat="server" ClientIDMode="Static" ID="OFFWORK9" Text="">無薪</asp:HyperLink>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label runat="server" ID="OFFWORK9" Text='<%# Eval("OFFWORK9") %>'> </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:HyperLink runat="server" ClientIDMode="Static" ID="LOSTTIMES" Text="">忘刷</asp:HyperLink>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label runat="server" ID="LOSTTIMES" Text='<%# Eval("LOSTTIMES") %>'> </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:HyperLink runat="server" ClientIDMode="Static" ID="OFFWORKHOURS" Text="">差假<br>合計</asp:HyperLink>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label runat="server" ID="OFFWORKHOURS" Text='<%# Eval("OFFWORKHOURS") %>'> </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:HyperLink runat="server" ClientIDMode="Static" ID="OVERWORK1" Text="">一般<br>加班</asp:HyperLink>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label runat="server" ID="OVERWORK1" Text='<%# Eval("OVERWORK1") %>'> </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:HyperLink runat="server" ClientIDMode="Static" ID="OVERWORK2" Text="">逾時<br>加班</asp:HyperLink>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label runat="server" ID="OVERWORK2" Text='<%# Eval("OVERWORK2") %>'> </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:HyperLink runat="server" ClientIDMode="Static" ID="OVERWORK3" Text="">超時<br>加班</asp:HyperLink>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label runat="server" ID="OVERWORK3" Text='<%# Eval("OVERWORK3") %>'> </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:HyperLink runat="server" ClientIDMode="Static" ID="OVERWORK4" Text="">假日<br>加班</asp:HyperLink>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label runat="server" ID="OVERWORK4" Text='<%# Eval("OVERWORK4") %>'> </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:HyperLink runat="server" ClientIDMode="Static" ID="MEALDELAY" Text="">誤<br>餐</asp:HyperLink>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label runat="server" ID="MEALDELAY" Text='<%# Eval("MEALDELAY") %>'> </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:HyperLink runat="server" ClientIDMode="Static" ID="MEALDELAY2" Text="">值<br>班</asp:HyperLink>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label runat="server" ID="MEALDELAY2" Text='<%# Eval("MEALDELAY") %>'> </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:HyperLink runat="server" ClientIDMode="Static" ID="OVERWORKHOURS" Text="">加班<br>合計</asp:HyperLink>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label runat="server" ID="OVERWORKHOURS" Text='<%# Eval("OVERWORKHOURS") %>'> </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:HyperLink runat="server" ClientIDMode="Static" ID="ADDHOURS" Text="">換休<br>加班</asp:HyperLink>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label runat="server" ID="ADDHOURS" Text='<%# Eval("ADDHOURS") %>'> </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <%--<asp:TemplateField>
                        <HeaderTemplate>
                            <asp:HyperLink runat="server" ClientIDMode="Static" ID="ADDOFFHOURS" Text="">剩餘<br>換休</asp:HyperLink>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label runat="server" ID="ADDOFFHOURS" Text='<%# Eval("ADDOFFHOURS") %>'> </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>        --%>                
                </Columns>
                <RowStyle CssClass="text-center" />
                <AlternatingRowStyle BackColor="#f9f9f9" CssClass="text-center" />
                <HeaderStyle CssClass="text-nowrap info" Font-Bold="true" BackColor="#dff0d8" />
            </asp:GridView>
            ※出差時數＋請假時數＝差假合計<br>
            ※最上方之下拉選單可切換查詢相同部門的員工出勤統計
            <asp:GridView runat="server" CssClass="table table-bordered table-condensed" ID="RecreateGridView" AutoGenerateColumns="False">
                <Columns>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:HyperLink runat="server" ClientIDMode="Static" ID="PAYYYYYMM" Text="查詢年度"></asp:HyperLink>
                        </HeaderTemplate>
                        <ItemTemplate>
                                <asp:Label runat="server" ID="YEAR" Text='<%# Eval("YEAR") %>'> </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                                       
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:HyperLink runat="server" ClientIDMode="Static" ID="COUNTBEGDATE" Text="特休使用開始日期 "></asp:HyperLink>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label runat="server" ID="OFFWORK3" Text='<%# Eval("COUNTBEGDATE") %>'> </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:HyperLink runat="server" ClientIDMode="Static" ID="COUNTENDDATE" Text="特休使用結束日期 "></asp:HyperLink>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label runat="server" ID="OffWork14" Text='<%# Eval("COUNTENDDATE") %>'> </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:HyperLink runat="server" ClientIDMode="Static" ID="RECREATEDAYS" Text="特休時數(H)"></asp:HyperLink>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label runat="server" ID="RECREATEDAYS" Text='<%# Eval("RECREATEDAYS") %>'> </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:HyperLink runat="server" ClientIDMode="Static" ID="USEDDAYS" Text="已休時數(H)"></asp:HyperLink>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label runat="server" ID="OFFHOURS" Text='<%# Eval("USEDDAYS") %>'> </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:HyperLink runat="server" ClientIDMode="Static" ID="UNUSEDDAYS" Text="剩餘可休(H)"></asp:HyperLink>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label runat="server" ID="OFFWORK2" Text='<%# Eval("USD") %>'> </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>                                        
                </Columns>
                <RowStyle CssClass="text-center" />
                <AlternatingRowStyle BackColor="#f9f9f9" CssClass="text-center" />
                <HeaderStyle CssClass="text-nowrap info" Font-Bold="true" BackColor="#dff0d8" />
            </asp:GridView>
        </div>
    </div>   
</asp:Content>
