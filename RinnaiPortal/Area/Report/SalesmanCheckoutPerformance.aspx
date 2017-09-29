<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SalesmanCheckoutPerformance.aspx.cs" Inherits="RinnaiPortal.Area.Report.SalesmanCheckoutPerformance" %>
<asp:Content ID="SalesmanCheckoutPerformanceContent" ContentPlaceHolderID="MainContent" runat="server">
    <input runat="server" type="text" hidden id="PageTitle" class='page-title' value="" />
    <div class="embed-responsive embed-responsive-16by9">
        <iframe runat="server" class="embed-responsive-item" id="SalesmanCheckoutPerformanceFrame"  scrolling="no"  ></iframe>
    </div>
</asp:Content>
