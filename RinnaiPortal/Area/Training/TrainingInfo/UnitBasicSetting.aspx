<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="UnitBasicSetting.aspx.cs" Inherits="RinnaiPortal.Area.Training.InfoMaintain.UnitBasicSetting" %>
<asp:Content ID="UnitBasicSettingContent" ContentPlaceHolderID="MainContent" runat="server">
	<input runat="server" type="text" hidden id="PageTitle" class='page-title' value="" />
	<div class="embed-responsive embed-responsive-16by9">
		<iframe runat="server" class="embed-responsive-item" id="UnitBasicSettingFrame1"></iframe>
	</div>
</asp:Content>
