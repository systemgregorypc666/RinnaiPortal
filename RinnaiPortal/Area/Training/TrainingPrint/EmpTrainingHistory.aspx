<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EmpTrainingHistory.aspx.cs" Inherits="RinnaiPortal.Area.Training.TrainingPrint.EmpTrainingHistory" %>
<asp:Content ID="EmpTrainingHistoryContent" ContentPlaceHolderID="MainContent" runat="server">
	<input runat="server" type="text" hidden id="PageTitle" class='page-title' value="" />
	<div class="embed-responsive embed-responsive-16by9">
		<iframe runat="server" class="embed-responsive-item" id="EmpTrainingHistoryFrame1"></iframe>
	</div>
</asp:Content>
