<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TrainingClass.aspx.cs" Inherits="RinnaiPortal.Area.Training.InfoMaintain.TrainingClass" %>
<asp:Content ID="TrainingClassContent" ContentPlaceHolderID="MainContent" runat="server">
	<input runat="server" type="text" hidden id="PageTitle" class='page-title' value="" />
	<div class="embed-responsive embed-responsive-16by9">
		<iframe runat="server" class="embed-responsive-item" id="TrainingClassFrame1"></iframe>
	</div>
</asp:Content>
