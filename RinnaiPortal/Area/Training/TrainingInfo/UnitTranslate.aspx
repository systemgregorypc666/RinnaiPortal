<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="UnitTranslate.aspx.cs" Inherits="RinnaiPortal.Area.Training.InfoMaintain.UnitTranslate" %>
<asp:Content ID="UnitTranslateContent" ContentPlaceHolderID="MainContent" runat="server">
	<input runat="server" type="text" hidden id="PageTitle" class='page-title' value="" />
	<div class="embed-responsive embed-responsive-16by9">
		<iframe runat="server" class="embed-responsive-item" id="UnitTranslateFrame1"></iframe>
	</div>
</asp:Content>
