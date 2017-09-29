<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TrainDetail01.aspx.cs" Inherits="RinnaiPortal.Area.Sign.Forms.TrainDetail01" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="true"
        AsyncPostBackTimeout="3600">
    </asp:ScriptManager>
    <input runat="server" type="text" hidden id="PageTitle" class="page-title" value="" />
    <div id="layout-content-wrapper">
        <div id="layout-content" class="container">
            <div class="panel panel-default">
                <div class="panel-body">
                    <fieldset class="form-horizontal col-xs-12">
                        <asp:HiddenField runat="server" ID="TimeStamp" />
                        <div class="col-xs-6">
					        <asp:HiddenField runat="server" ID="hd_CLID" />
                            <asp:HiddenField runat="server" ID="hd_SID" />
					        <div class="form-group row">
						        <label for="CLID" class="col-xs-3">課程代號</label>
						        <div class="col-xs-9">
							        <asp:Label runat="server" ID="CLID"></asp:Label>
						        </div>
					        </div>
					        <div class="form-group row">
						        <label for="Start_Date" class="col-xs-3">開課日期</label>
						        <div class="col-xs-9 ">
							        <asp:Label runat="server" ID="Start_Date"></asp:Label>
						        </div>
					        </div>
                            <div class="form-group row">
						        <label for="SID" class="col-xs-3">學員編號</label>
						        <div class="col-xs-9 ">
							        <asp:Label runat="server" ID="SID"></asp:Label>
						        </div>
					        </div>
				        </div>
                        <div class="col-xs-6">
					        <div class="form-group row">
						        <label for="CLNAME" class="col-xs-3">課程名稱</label>
						        <div class="col-xs-9 ">
							        <asp:Label runat="server" ID="CLNAME"></asp:Label>
						        </div>
					        </div>
					        <div class="form-group row">
						        <label for="HOURS" class="col-xs-3">時數</label>
						        <div class="col-xs-9 ">
							        <asp:Label runat="server" ID="HOURS"></asp:Label>
						        </div>
					        </div>
                            <div class="form-group row">
						        <label for="SNAME" class="col-xs-3">學員姓名</label>
						        <div class="col-xs-9 ">
							        <asp:Label runat="server" ID="SNAME"></asp:Label>
						        </div>
					        </div>
				        </div>
                    </fieldset>
                </div>
                &nbsp;&nbsp
                <asp:Label ID="txterror" runat="server" ForeColor="Red"></asp:Label>
                <br />
                <div class="text-left">
                    <%--<hr style="margin:6px;height:1px;border:0px;background-color:#D5D5D5;color:#D5D5D5;"/>--%>
                    <asp:PlaceHolder ID="PlaceHolder1" runat="server"></asp:PlaceHolder>
                    <asp:RadioButtonList ID="RadioButtonList1" runat="server"></asp:RadioButtonList>
                </div>
                <div class="text-center panel-footer">
					<button type="button" class="btn btn-default" onclick="history.back();">
						<span class="glyphicon glyphicon-arrow-up"></span>&nbsp;回上一頁</button>

					<asp:Button ClientIDMode="Static" runat="server" ID="SaveBtn" type="submit" CssClass="btn btn-primary" OnClick="SaveBtn_Click" data-loading-text="Loading..." Text="確定送出"></asp:Button>

					<asp:LinkButton runat="server" ClientIDMode="Static" ID="CoverBtn" CssClass="CoverBtn btn display-none">
						<span class="glyphicon glyphicon-floppy-saved"></span>
						<img class="loader" src='<%: VirtualPathUtility.ToAbsolute(@"~/icon/ajax-loader.gif") %>' />
					</asp:LinkButton>

					<a runat="server" id="RefreshBtn" class="btn btn-warning display-none" href="javascript:location.href=document.URL">
						<span class="glyphicon glyphicon-refresh"></span>&nbsp;重新整理</a>
				</div>
            </div>
        </div>
    </div>
</asp:Content>
