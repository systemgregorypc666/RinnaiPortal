<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="StudentsOpinion.aspx.cs" Inherits="RinnaiPortal.Area.Training.TrainingEvaluate.StudentsOpinion" %>
<asp:Content ID="StudentsOpinionContent" ContentPlaceHolderID="MainContent" runat="server">
	<input runat="server" type="text" hidden id="PageTitle" class='page-title' value="" />
	<div class="embed-responsive embed-responsive-16by9">
		<iframe runat="server" class="embed-responsive-item" id="StudentsOpinionFrame1"></iframe>
	</div>
</asp:Content>

