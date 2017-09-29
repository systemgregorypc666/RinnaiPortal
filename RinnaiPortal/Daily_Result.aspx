<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Daily_Result.aspx.cs" Inherits="RinnaiPortal.Daily_Result" %>
<%@ Import Namespace="RinnaiPortal.Extensions" %>
<%@ Import Namespace="RinnaiPortal.Tools" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
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
                工號【<asp:Label runat="server" ID="lbl_UserID" ></asp:Label>】姓名【<asp:Label runat="server" ID="lbl_UserName" ></asp:Label>】計薪月份：<asp:Label runat="server" ID="lbl_YYYYMM" ></asp:Label> 刷卡出勤紀錄，共有 <asp:Label runat="server" ID="lbl_Count" ></asp:Label> 筆紀錄
                </br><strong>目前剩餘換休時數：</strong><strong><asp:Label runat="server" ID="lb_ADDOFFHOURS" style="color: #FF0000" ></asp:Label></strong><strong> H</strong>
                <hr style="margin:6px;height:1px;border:0px;background-color:#D5D5D5;color:#D5D5D5;"/>                                                                       
            </div>
            <div class="text-center">
				<asp:Label runat="server" ID="noDataTip"></asp:Label>
			</div>
            <asp:GridView runat="server" CssClass="table table-bordered table-condensed" ID="DailyGridView" onrowdatabound="DailyGridView_RowDataBound" AutoGenerateColumns="False">
                <Columns>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:HyperLink runat="server" ClientIDMode="Static" ID="DUTYDATE" Text="">日期</asp:HyperLink>
                        </HeaderTemplate>
                        <ItemTemplate>
                                <asp:Label runat="server" ID="DUTYDATE" Text='<%# Eval("DUTYDATE") %>'> </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                                       
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:HyperLink runat="server" ClientIDMode="Static" ID="BeginTime" Text="">上班<br>刷卡</asp:HyperLink>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label runat="server" ID="BeginTime" Text='<%# Eval("BeginTime") %>'> </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:HyperLink runat="server" ClientIDMode="Static" ID="EndTime" Text="">下班<br>刷卡</asp:HyperLink>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label runat="server" ID="EndTime" Text='<%# Eval("EndTime") %>'> </asp:Label>
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
                            <asp:HyperLink runat="server" ClientIDMode="Static" ID="OFFWORK1" Text="">事假</asp:HyperLink>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label runat="server" ID="OFFWORK1" Text='<%# Eval("OFFWORK1") %>'> </asp:Label>
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
                            <asp:HyperLink runat="server" ClientIDMode="Static" ID="OFFWORK8" Text="">產假</asp:HyperLink>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label runat="server" ID="OFFWORK8" Text='<%# Eval("OFFWORK8") %>'> </asp:Label>
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
                            <asp:HyperLink runat="server" ClientIDMode="Static" ID="OFFWORKHOURS" Text="">差假<br>合計</asp:HyperLink>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label runat="server" ID="OFFWORKHOURS" Text='<%# Eval("OFFWORKHOURS") %>'> </asp:Label>
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
                            <asp:HyperLink runat="server" ClientIDMode="Static" ID="MealDelay" Text="">誤餐</asp:HyperLink>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label runat="server" ID="MealDelay" Text='<%# Eval("MealDelay") %>'> </asp:Label>
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
                            <asp:HyperLink runat="server" ClientIDMode="Static" ID="ADDHOURS" Text="">換休<br>加班</asp:HyperLink>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label runat="server" ID="ADDHOURS" Text='<%# Eval("ADDHOURS") %>'> </asp:Label>
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
                            <asp:HyperLink runat="server" ClientIDMode="Static" ID="RemarkOff" Text="">備 註</asp:HyperLink>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label runat="server" ID="RemarkOff" Text='<%# Eval("RemarkOff") %>'> </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>                       
                </Columns>
                <RowStyle CssClass="text-center" />
                <AlternatingRowStyle BackColor="#f9f9f9" CssClass="text-center" />
                <HeaderStyle CssClass="text-nowrap info" Font-Bold="true" BackColor="#dff0d8" />
            </asp:GridView>
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
