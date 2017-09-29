<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AfClassMatrixChartALL.aspx.cs" Inherits="RinnaiPortal.Area.Training.TrainingPrint.AfClassMatrixChartALL" %>
<asp:Content ID="AfClassMatrixChartALLContent" ContentPlaceHolderID="MainContent" runat="server">
	<input runat="server" type="text" hidden id="PageTitle" class='page-title' value="" />
	<div class="embed-responsive embed-responsive-16by9">
		<iframe runat="server" class="embed-responsive-item" id="AfClassMatrixChartALLFrame1"></iframe>
	</div>
</asp:Content>
