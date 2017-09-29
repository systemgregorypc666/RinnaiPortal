<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ProfessorProfileList.aspx.cs" Inherits="RinnaiPortal.Area.Training.BasicSetting.ProfessorProfileList" %>
<asp:Content ID="ProfessorProfileListContent" ContentPlaceHolderID="MainContent" runat="server">
	<input runat="server" type="text" hidden id="PageTitle" class='page-title' value="" />
	<div class="embed-responsive embed-responsive-16by9">
		<iframe runat="server" class="embed-responsive-item" id="ProfessorProfileListFrame1"></iframe>
	</div>
</asp:Content>
