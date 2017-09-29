<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="bbs_content.aspx.cs" Inherits="RinnaiPortal.bbs_content" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
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
						<asp:HiddenField runat="server" ID="bbs_id" />
                        <div class="col-xs-6">
                            <div class="form-group required ">
								<label for="txt_Title" class="col-xs-5 control-label">主題</label>
								<div class="col-xs-5">
									<asp:TextBox runat="server" data-val="true" data-val-required="請輸入主題" ID="txt_Title" type="text" Width="550px" class="form-control"></asp:TextBox>
									<span class="field-validation-valid" data-valmsg-for="txt_Title" data-valmsg-replace="true"></span>
								</div>
							</div>
                            <div class="form-group required">
								<label for="DefaultStartDateTime" class="col-xs-5 control-label">張貼起始日</label>
								<div class='input-group date col-xs-5 datepicker-left-15' id='datetimepicker1'>
									<asp:TextBox runat="server" ClientIDMode="Static" data-val-required="請輸入張貼起始日" ID="DefaultStartDateTime" class="form-control" data-date-format="YYYY-MM-DD 00:00:00" />
									<span class="input-group-addon">
										<span class="glyphicon glyphicon-calendar"></span>
									</span>
								</div>
							</div>
                            <div class="form-group required">
								<label for="DefaultEndDateTime" class="col-xs-5 control-label">張貼結束日</label>
								<div class='input-group date col-xs-5 datepicker-left-15' id='datetimepicker2'>
									<asp:TextBox runat="server" ClientIDMode="Static" data-val-required="請輸入張貼結束日" ID="DefaultEndDateTime" class="form-control" data-date-format="YYYY-MM-DD 00:00:00" />
									<span class="input-group-addon">
										<span class="glyphicon glyphicon-calendar"></span>
									</span>
								</div>
							</div>
                            <div class="form-group required ">
								<label for="txt_Content" class="col-xs-5 control-label">公告內容</label>
								<div class="col-xs-5">
									<asp:TextBox runat="server" data-val="true" data-val-required="請輸入公告內容" ID="txt_Content" type="text" TextMode="MultiLine"  Width="550px" Height="150px" class="form-control"></asp:TextBox>
									<span class="field-validation-valid" data-valmsg-for="txt_Content" data-valmsg-replace="true"></span>
								</div>
							</div>
                            <div class="form-group">
								<label for="txt_Http" class="col-xs-5 control-label">相關連結</label>
								<div class="col-xs-5">
                                    <asp:TextBox runat="server" data-val-required="請輸入相關連結" ID="txt_Http" class="form-control" Text="http://" Width="500px"></asp:TextBox>
									<span class="field-validation-valid" data-valmsg-for="txt_Http" data-valmsg-replace="true"></span>
								</div>
							</div>
                            <div class="form-group">
								<label for="File_Photo" class="col-xs-5 control-label">相關圖檔</label>
								<div class="col-xs-5">
                                    <asp:HyperLink ID="HyperLink_FILENAME1" runat="server" Text="檢視圖檔" Visible="false" Target="_blank"></asp:HyperLink>
                                    <asp:Button ID="Button_DelFILE1" runat="server" Text="刪除" CssClass="cssbutton" Visible="false" CausesValidation="false" OnCommand="Button_DelFILE1_Command" Height="25px" />
									<cc1:ConfirmButtonExtender ID="Button_DelFILE1_ConfirmButtonExtender" runat="server" ConfirmText="確定刪除此圖檔？" Enabled="True" TargetControlID="Button_DelFILE1">
                                </cc1:ConfirmButtonExtender>
                                    <asp:FileUpload ID="File_Photo" runat="server" />                                           
								</div>
							</div>
                            <div class="form-group">
								<label for="File_Up1" class="col-xs-5 control-label">相關附件</label>
								<div class="col-xs-5">
                                    <asp:HyperLink ID="HyperLink_FILENAME2" runat="server" Text="檢視附件" Visible="false" Target="_blank"></asp:HyperLink>
									<asp:Button ID="Button_DelFILE2" runat="server" Text="刪除" CssClass="cssbutton" Visible="false" CausesValidation="false" OnCommand="Button_DelFILE2_Command" Height="25px" />
                                    <cc1:ConfirmButtonExtender ID="Button_DelFILE2_ConfirmButtonExtender" runat="server" ConfirmText="確定刪除此附件？"
                                        Enabled="True" TargetControlID="Button_DelFILE2">
                                    </cc1:ConfirmButtonExtender>
                                    <asp:FileUpload ID="File_Up1" runat="server" />                                           
								</div>
							</div>
                            <%--<div class="form-group">
								<label for="File_Up2" class="col-xs-5 control-label">相關附件2</label>
								<div class="col-xs-5">
									<asp:FileUpload ID="File_Up2" runat="server" />                                           
								</div>
							</div>
                            <div class="form-group">
								<label for="File_Up3" class="col-xs-5 control-label">相關附件3</label>
								<div class="col-xs-5">
									<asp:FileUpload ID="File_Up3" runat="server" />                                           
								</div>
							</div>--%>
                        </div>
                        <%--<div class="col-xs-6">

                        </div>--%>
                    </fieldset>
                </div>
                <div class="text-center panel-footer">
					<button type="button" class="btn btn-default" onclick="history.back();">
						<span class="glyphicon glyphicon-arrow-up"></span>&nbsp;回上一頁</button>

					<asp:Button ClientIDMode="Static" runat="server" ID="SaveBtn" type="submit" CssClass="btn btn-primary" OnClick="SaveBtn_Click" data-loading-text="Loading..." Text="送出"></asp:Button>

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
