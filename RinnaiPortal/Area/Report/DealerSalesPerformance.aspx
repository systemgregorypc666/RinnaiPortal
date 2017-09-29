<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DealerSalesPerformance.aspx.cs" Inherits="RinnaiPortal.Area.Report.DealerSalesPerformance" %>
<asp:Content ID="DealerSalesPerformanceContent" ContentPlaceHolderID="MainContent" runat="server">
    <input runat="server" type="text" hidden id="PageTitle" class='page-title' value="" />
    <div class="embed-responsive embed-responsive-16by9">
        <iframe runat="server" class="embed-responsive-item" id="DealerSalesPerformanceFrame"  scrolling="no"  ></iframe>
    </div>
</asp:Content>
