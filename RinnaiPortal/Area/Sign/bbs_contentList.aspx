<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="bbs_contentList.aspx.cs" Inherits="RinnaiPortal.Area.Sign.bbs_contentList" %>
<%@ Import Namespace="RinnaiPortal.Extensions" %>
<%@ Import Namespace="RinnaiPortal.Tools" %>
<asp:Content ID="bbs_contentListContent" ContentPlaceHolderID="MainContent" runat="server">
<input runat="server" type="text" hidden id="PageTitle" class='page-title' value="" />
    <div id="layout-content-wrapper">
        <div id="layout-content" class="container">
            <div class="list-toolbar">
                <a class="btn btn-success pull-left" href="bbs_content.aspx">
                    <span class="glyphicon glyphicon-plus"></span>&nbsp;新增
                </a>

                <div class="col-lg-1 input-group pull-right">
                    <span class="input-group-addon">每頁</span>
                    <asp:DropDownList runat="server" ID="pageSizeSelect"  class="form-control" data-val="true" data-val-number="The field PageSize must be a number." data-val-required="The PageSize field is required." Style="width: 70px" OnSelectedIndexChanged="PageSize_SelectedIndexChanged" AutoPostBack="true">
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
                            <asp:TextBox runat="server" ID="queryTextBox" class="form-control" placeholder="主題" />
                            <span class="input-group-btn">
                                <asp:LinkButton runat="server" class="btn btn-default" ID="queryBtn" OnClick="QueryBtn_Click"> <span class="glyphicon glyphicon-search"></span>&nbsp;查詢 </asp:LinkButton>
                            </span>
                        </div>
                    </div>
                </div>
            </div>
            <div class="text-center">
                <asp:Label runat="server" id="noDataTip" ></asp:Label>
            </div>
            <asp:GridView runat="server" CssClass="table table-striped table-bordered table-condensed" ID="BbsGridView" OnRowDeleting="BbsGridView_RowDeleting" DataKeyNames="bbs_id" AutoGenerateColumns="False">
                <Columns>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:HyperLink runat="server" ClientIDMode="Static" ID="bbs_id" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("bbs_id", Request)%>' Text="序號"></asp:HyperLink>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label runat="server" Text='<%# Eval("bbs_id") %>'> </asp:Label>
                        </ItemTemplate>
                        <HeaderStyle CssClass="info" />
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:HyperLink runat="server" ClientIDMode="Static" ID="bbs_title" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("bbs_title", Request)%>' Text="公佈主題"></asp:HyperLink>
                        </HeaderTemplate>
                        <ItemTemplate>
<%--                            <a runat="server" cssclass="btn btn-link" target="_blank" href='<%# GetFormUrl(Eval("bbs_id"))%>' text=''><%# Eval("bbs_title") %><i class="glyphicon glyphicon-info-sign" style="top: 3px"></i></a>--%>
                                <asp:HyperLink runat="server" Target="_blank" NavigateUrl='<%# "~/Area/Sign/bbs_Detail.aspx?bbs_id=" + Eval("bbs_id") %>'><%# Eval("bbs_title") %></asp:HyperLink>
                            <%--<asp:Label runat="server" Text='<%# Eval("bbs_title") %>'> </asp:Label>--%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
						<HeaderTemplate>
<%--							<asp:Label runat="server" ClientIDMode="Static" ID="StartDateTime" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("StartDateTime", Request)%>' Text="張貼日期"></asp:Label>--%>
                            <asp:HyperLink runat="server" ClientIDMode="Static" ID="StartDateTime" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("StartDateTime", Request)%>' Text="張貼日期"></asp:HyperLink>
						</HeaderTemplate>
						<ItemTemplate>
							<asp:Label runat="server" Text='<%# Eval("StartDateTime").ToDateTimeFormateString() %>'> </asp:Label>
						</ItemTemplate>
						<ItemStyle CssClass="StartDateTime" />
					</asp:TemplateField>
					<asp:TemplateField>
						<HeaderTemplate>
<%--							<asp:Label runat="server" ClientIDMode="Static" ID="EndDateTime" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("EndDateTime", Request)%>' Text="結束日期"></asp:Label>--%>
                            <asp:HyperLink runat="server" ClientIDMode="Static" ID="EndDateTime" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("EndDateTime", Request)%>' Text="結束日期"></asp:HyperLink>
						</HeaderTemplate>
						<ItemTemplate>
							<asp:Label runat="server" Text='<%# Eval("EndDateTime").ToDateTimeFormateString() %>'> </asp:Label>
						</ItemTemplate>
						<ItemStyle CssClass="EndDateTime" />
					</asp:TemplateField>                              
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:HyperLink runat="server" ClientIDMode="Static" ID="Creator" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("Creator", Request)%>' Text="張貼者"></asp:HyperLink>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label runat="server" Text='<%# Eval("Creator") %>'> </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:HyperLink runat="server" ClientIDMode="Static" ID="CreateDate" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("CreateDate", Request)%>' Text="建立日期"></asp:HyperLink>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label runat="server" Text='<%# Eval("CreateDate").ToDateTimeFormateString() %>'> </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <%--<asp:TemplateField>
                        <HeaderTemplate>
                            <asp:HyperLink runat="server" ClientIDMode="Static" ID="Modifier" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("Modifier", Request)%>' Text="編輯者"></asp:HyperLink>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label runat="server" Text='<%# Eval("Modifier") %>'> </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:HyperLink runat="server" ClientIDMode="Static" ID="ModifyDate" NavigateUrl='<%# WebUtils.GetGridViewHeadUrl("ModifyDate", Request)%>' Text="編輯日期"></asp:HyperLink>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label runat="server" Text='<%# Eval("ModifyDate").ToDateTimeFormateString() %>'> </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>--%>
                    <asp:TemplateField HeaderText="編輯" HeaderStyle-CssClass="text-center">
                        <ItemStyle CssClass="vertical-middle" />
                        <ItemTemplate>
                            <asp:HyperLink runat="server" CommandName="Edit" CssClass="btn btn-warning btn-xs" NavigateUrl='<%# "~/Area/Sign/bbs_content.aspx?BbsID=" + Eval("bbs_id") %>'
                                Text="編輯" Width="50px"><span class="glyphicon glyphicon-pencil"></span></asp:HyperLink>
                        </ItemTemplate>                            
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="刪除" HeaderStyle-CssClass="text-center">
						<ItemStyle CssClass="vertical-middle" />
						<ItemTemplate>
                             <%--<asp:Button ID="delete_btn" runat="server" Text="刪除" CommandName="Delete" OnClientClick="return confirm('確定要刪除此筆資料嗎?');" >
                                 
                             </asp:Button>--%>
                            <asp:LinkButton ID="Del" runat="server" CommandName="Delete" OnClientClick="return confirm('是否刪除？');">
                                <asp:Image ID="Image1" runat="server" ImageUrl="~/img/Remove.png" style="border-width: 0px;" Height="25" Width="25" />
                            </asp:LinkButton>
							<%--<asp:HyperLink runat="server" CommandName="Del" ClientIDMode="Static" ID="Del" CssClass="btn btn-danger btn-xs" NavigateUrl='<%# GetEditUrl(Eval("SignDocID"), Eval("FormID")) %>'
								Text="刪除" Width="50px"><span class="glyphicon glyphicon-pencil"></span></asp:HyperLink>--%>
						</ItemTemplate>
					</asp:TemplateField>
                </Columns>
                <RowStyle CssClass="text-center" />
                <AlternatingRowStyle BackColor="#f9f9f9" CssClass="text-center" />
                <HeaderStyle CssClass="text-nowrap info" Font-Bold="true" BackColor="#dff0d8" />
            </asp:GridView>
        </div>
        <div runat="server" id="paginationBar" class="text-center"></div>
    </div>
</asp:Content>
