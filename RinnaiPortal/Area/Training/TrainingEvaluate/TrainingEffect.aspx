<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TrainingEffect.aspx.cs" Inherits="RinnaiPortal.Area.Training.TrainingEvaluate.TrainingEffect" %>
<asp:Content ID="TrainingEffectContent" ContentPlaceHolderID="MainContent" runat="server">
	<input runat="server" type="text" hidden id="PageTitle" class='page-title' value="" />
	<div class="embed-responsive embed-responsive-16by9">
		<iframe runat="server" class="embed-responsive-item" id="TrainingEffectFrame1"></iframe>
	</div>
</asp:Content>
