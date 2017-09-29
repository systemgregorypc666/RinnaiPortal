<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ProductionSalesStockNew.aspx.cs" Inherits="RinnaiPortal.Area.Report.ProductionSalesStockNew" %>
<asp:Content ID="ProductionSalesStockNewContent" ContentPlaceHolderID="MainContent" runat="server">
    <input runat="server" type="text" hidden id="PageTitle" class='page-title' value="" />
    <div class="embed-responsive embed-responsive-16by9">
        <iframe runat="server" class="embed-responsive-item" id="ProductionSalesStockNewFrame"  scrolling="no"  ></iframe>
    </div>
</asp:Content>
