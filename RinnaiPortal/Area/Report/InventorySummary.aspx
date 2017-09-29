<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="InventorySummary.aspx.cs" Inherits="RinnaiPortal.Area.Report.InventorySummary" %>
<asp:Content ID="InventorySummaryContent" ContentPlaceHolderID="MainContent" runat="server">
    <input runat="server" type="text" hidden id="PageTitle" class='page-title' value="" />
    <div class="embed-responsive embed-responsive-16by9">
        <iframe runat="server" class="embed-responsive-item" id="InventorySummaryFrame"  scrolling="no"  ></iframe>
    </div>
</asp:Content>
