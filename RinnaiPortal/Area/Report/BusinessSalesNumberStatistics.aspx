<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="BusinessSalesNumberStatistics.aspx.cs" Inherits="RinnaiPortal.Area.Report.BusinessSalesNumberStatistics" %>
<asp:Content ID="BusinessSalesNumberStatisticsContent" ContentPlaceHolderID="MainContent" runat="server">
    <input runat="server" type="text" hidden id="PageTitle" class='page-title' value="" />
    <div class="embed-responsive embed-responsive-16by9">
        <iframe runat="server" class="embed-responsive-item" id="BusinessSalesNumberStatisticsFrame"  scrolling="no"  ></iframe>
    </div>
</asp:Content>
