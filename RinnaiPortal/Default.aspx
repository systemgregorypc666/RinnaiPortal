<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="RinnaiPortal._Default" %>
<%@ Import Namespace="RinnaiPortal.Extensions" %>
<%@ Import Namespace="RinnaiPortal.Tools" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <%--<asp:Button ID="fuck" runat="server" Text="mailtest" OnClick="fuck_Click" />--%>
	<%--<div class="jumbotron">
		<asp:Image runat="server" ImageUrl="~/img/QualityIsOurDestiny.jpg" Width="100%" />
	</div>
	<div class="row" style="font-family: 'Microsoft JhengHei UI'">
		<div class="col-md-4 ">
			<h2><b>宗旨</b></h2>
			<p>「和」塑造充滿人性的人格。</p>
			<p>「氣」以正確的人生觀，樹立遠大志向。</p>
			<p>「真」從基礎學起，科學地思考問題。</p>
		</div>
		<div class="col-md-4">
			<h2><b>林內企業使命觀</b></h2>
			<p>林內通過「熱能」向社會提供「舒適的生活」。</p>
		</div>
		<div class="col-md-4">
			<h2><b>品質基本理念</b></h2>
			<p>品質就是我們的生命。</p>
		</div>
	</div>--%>
    <div class="sitebody" id="sitebody">
        <div class="sidebar_left" id="sidebar_left">
           
            <div class="right_side horzPad" id="right_side">
                <h3><a href="24IMG.aspx" target="_blank">倫 理 綱 領</a></h3>
                 <%--<ul class="submenu1">
                    <li><a href="24IMG.aspx">倫理綱領</a></li>                  
                </ul>--%>
                <h3>檔 案<span class="dark"> 下 載</span></h3>
                <ul class="submenu1">
                    
                    <li><a href="Files/phone.gif" target="_blank">電話分機表</a></li>
                    <li><a href="Files/FetchFromFile.pdf" target="_blank">年度行事曆(營業所)</a></li>
                    <li><a href="Files/FetchFromFile_總.pdf" target="_blank">年度行事曆(總公司)</a></li>                 
                </ul>
                <%--<h3>系 統<span class="dark"> 連 結</span></h3>
                <ul class="submenu1">
                    <li><a href="http://eip.rinnai.com.tw/Discount" target="_blank">WEB ERP</a></li> 
                </ul>--%>
                <h3>福 委<span class="dark"> 專 區</span></h3>
                <ul class="submenu1">                    
                    <li><a href="Files/員工福利項目.pdf" target="_blank">員工福利金</a></li> 
                    <li><a href="Files/特約廠商一覽表(整合版).pdf" target="_blank">特約廠商</a></li>
                    <li><a href="Files/detail_rule.pdf" target="_blank">員工旅遊辦法</a></li>
                    <li><a href="Files/員工旅遊招募表.xlsx" target="_blank">員工旅遊招募表</a></li> 
                    <li><a href="Files/旅平險簡式費率表.xls" target="_blank">旅平險簡式費率表</a></li> 
                    <li><a href="Files/旅遊費用申請表.doc" target="_blank">旅遊費用申請表</a></li>                    
                </ul>
            </div>
        </div>
        <div class="sidebar_right" id="sidebar_right">
            <div class="right_side horzPad" id="right_side2">
                <h3>公 司<span class="dark"> 簡 介</span></h3>
                <ul class="submenu1">
                    <li><a href="Files/rinnai_Ch.pps" target="_blank">中文版</a></li>
                    <li><a href="Files/rinnai_En.pps" target="_blank">英文版</a></li>
                    <li><a href="Files/rinnai_Jp.pps" target="_blank">日文版</a></li>            
                </ul>             
            </div>
        </div>
        <div class="content" id="content">
            <div style="width:99%;padding-left: 10px;">
                <div class="text-left" style="color: #586B7A">
                    <strong>您的各月份出勤統計</strong>
                    (<asp:HyperLink runat="server" ClientIDMode="Static" ID="Monthly" Text="">查看更多</asp:HyperLink>)
                    <hr style="margin:6px;height:1px;border:0px;background-color:#D5D5D5;color:#D5D5D5;"/>                                                                       
                </div>
                <asp:GridView runat="server" CssClass="table table-bordered table-condensed" ID="DailyGridView" onrowdatabound="DailyGridView_RowDataBound" AutoGenerateColumns="False">
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
                                <asp:HyperLink runat="server" ClientIDMode="Static" ID="OFFWORK3" Text="">曠<br>職</asp:HyperLink>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" ID="OFFWORK3" Text='<%# Eval("OFFWORK3") %>'> </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:HyperLink runat="server" ClientIDMode="Static" ID="OffWork14" Text="">出<br>差</asp:HyperLink>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" ID="OffWork14" Text='<%# Eval("OffWork14") %>'> </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:HyperLink runat="server" ClientIDMode="Static" ID="RECREATEDAYS" Text="">特<br>休</asp:HyperLink>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" ID="RECREATEDAYS" Text='<%# Eval("RECREATEDAYS") %>'> </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:HyperLink runat="server" ClientIDMode="Static" ID="OFFHOURS" Text="">換<br>休</asp:HyperLink>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" ID="OFFHOURS" Text='<%# Eval("OFFHOURS") %>'> </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:HyperLink runat="server" ClientIDMode="Static" ID="OFFWORK2" Text="">病<br>假</asp:HyperLink>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" ID="OFFWORK2" Text='<%# Eval("OFFWORK2") %>'> </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                            <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:HyperLink runat="server" ClientIDMode="Static" ID="OFFWORK1" Text="">事<br>假</asp:HyperLink>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" ID="OFFWORK1" Text='<%# Eval("OFFWORK1") %>'> </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:HyperLink runat="server" ClientIDMode="Static" ID="OFFWORK5M" Text="">遲<br>到</asp:HyperLink>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" ID="OFFWORK5M" Text='<%# Eval("OFFWORK5M") %>'> </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:HyperLink runat="server" ClientIDMode="Static" ID="OFFWORK6M" Text="">早<br>退</asp:HyperLink>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" ID="OFFWORK6M" Text='<%# Eval("OFFWORK6M") %>'> </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:HyperLink runat="server" ClientIDMode="Static" ID="OFFWORK9" Text="">無<br>薪</asp:HyperLink>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" ID="OFFWORK9" Text='<%# Eval("OFFWORK9") %>'> </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:HyperLink runat="server" ClientIDMode="Static" ID="LOSTTIMES" Text="">忘<br>刷</asp:HyperLink>
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
                        </asp:TemplateField>--%>                        
                    </Columns>
                    <RowStyle CssClass="text-center" />
                    <AlternatingRowStyle BackColor="#f9f9f9" CssClass="text-center" />
                    <HeaderStyle CssClass="text-nowrap info" Font-Bold="true" BackColor="#dff0d8" />
                </asp:GridView>
            </div>
            <%--<div runat="server" id="paginationBar_Daily" class="text-center"></div>--%>

            <div style="width:99%;padding-left: 10px;">
                <div class="text-left" style="color: #586B7A">
                    <strong>公佈欄</strong>
                    <hr style="margin:6px;height:1px;border:0px;background-color:#D5D5D5;color:#D5D5D5;"/>                                                                       
                </div>
               
                <asp:GridView runat="server" CssClass="table table-bordered table-condensed" ID="BbsGridView" DataKeyNames="bbs_id" AutoGenerateColumns="False">
                    <Columns>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:HyperLink runat="server" ClientIDMode="Static" ID="bbs_title" Text="公佈主題"></asp:HyperLink>
                            </HeaderTemplate>
                            <ItemTemplate>
                                    <asp:HyperLink runat="server" Target="_blank" NavigateUrl='<%# "~/Area/Sign/bbs_Detail.aspx?bbs_id=" + Eval("bbs_id") %>'><%# Eval("bbs_title") %></asp:HyperLink>
                            </ItemTemplate>
                        </asp:TemplateField>
                                       
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:HyperLink runat="server" ClientIDMode="Static" ID="Creator" Text="張貼者"></asp:HyperLink>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" Text='<%# Eval("Creator") %>'> </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:HyperLink runat="server" ClientIDMode="Static" ID="CreateDate" Text="公佈日期"></asp:HyperLink>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" Text='<%# Eval("StartDateTime").ToDateTimeFormateString("yyyy-MM-dd") %>'> </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                       
                    </Columns>
                    <RowStyle CssClass="text-left" />
                    <AlternatingRowStyle BackColor="#f9f9f9" CssClass="text-left" />
                    <HeaderStyle CssClass="text-nowrap info" Font-Bold="true" BackColor="#dff0d8" />
                </asp:GridView>
		        <%--<asp:Image runat="server" ImageUrl="~/img/QualityIsOurDestiny.jpg" Width="100%" />--%>             
	        </div>
            <div runat="server" id="paginationBar" class="text-center"></div>

            

            <%--<div class="row" style="font-family: 'Microsoft JhengHei UI'">
		        <div class="col-md-3 ">
			        <h2><b>宗旨</b></h2>
			        <p>「和」塑造充滿人性的人格。</p>
			        <p>「氣」以正確的人生觀，樹立遠大志向。</p>
			        <p>「真」從基礎學起，科學地思考問題。</p>
		        </div>
		        <div class="col-md-4">
			        <h2><b>林內企業使命觀</b></h2>
			        <p>林內通過「熱能」向社會提供「舒適的生活」。</p>
		        </div>
		        <div class="col-md-4">
			        <h2><b>品質基本理念</b></h2>
			        <p>品質就是我們的生命。</p>
		        </div>
	        </div>--%>
        </div>　    
    </div>
</asp:Content>
