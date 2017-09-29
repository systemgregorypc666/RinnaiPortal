<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="bbs_Detail.aspx.cs" Inherits="RinnaiPortal.bbs_Detail" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <input runat="server" type="text" hidden id="PageTitle" class="page-title" value="" />
    <div id="layout-content-wrapper">
        <div id="layout-content" class="container">
            <div class="panel panel-default">
                <div class="panel-body">
                    <fieldset class="form-horizontal col-xs-12">
                        <asp:HiddenField runat="server" ID="bbs_id" />
                        <div class="col-xs-12">
                            <div class="row form-group  ">
                                <label for="txt_Title"></label>
                                <div class="col-xs-12">
                                    <span style="font-size: large; color: #0000FF"><strong>［<asp:Label runat="server" ID="txt_Title"></asp:Label>］</strong></span>
                                </div>
                            </div>
                            <hr size=1 >
                            <div class="row form-group  ">
                                <asp:Image runat="server" ID="img_Photo" Width="100%" />                              
                            </div>
                            
                            <div class="row form-group  ">
                                <label for="txt_Content" class="col-xs-2 ">公佈事項：</label>
                                <div class="col-xs-10">
                                    <asp:Label runat="server" ID="txt_Content" type="text" style="word-break:break-all"></asp:Label>
                                </div>
                            </div>
                            <div class="row form-group  ">
                                <label for="txt_Http" class="col-xs-2 ">相關連結：</label>
                                <div class="col-xs-10">
                                   <%-- <asp:Label runat="server" ID="txt_Http"></asp:Label>--%>
                                    <asp:HyperLink ID="hyl_Http" runat="server"></asp:HyperLink>
                                </div>
                            </div>
                            <div class="row form-group  ">
                                <label for="hyl_Url" class="col-xs-2 ">相關附件：</label>
                                <div class="col-xs-10">
                                    <asp:HyperLink ID="hyl_Url" runat="server"></asp:HyperLink>
                                </div>
                            </div>
                            <div class="row form-group  ">
                                <label for="lbl_Creator" class="col-xs-2 ">公佈者：</label>
                                <div class="col-xs-10">
                                    <asp:Label runat="server" ID="lbl_Creator"></asp:Label>
                                </div>
                            </div>
                            <div class="row form-group  ">
                                <label for="DefaultEndDateTime" class="col-xs-2 ">張貼結束日：</label>
                                <div class="col-xs-10">
                                    <asp:Label runat="server" ID="DefaultEndDateTime"></asp:Label>
                                </div>
                            </div>
                        </div>
                    </fieldset>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
