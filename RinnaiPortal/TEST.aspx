<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TEST.aspx.cs" Inherits="RinnaiPortal.TEST" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataSourceID="SqlDataSource1">
            <Columns>
                <asp:BoundField DataField="EmployeeID" HeaderText="EmployeeID" SortExpression="EmployeeID" />
                <asp:BoundField DataField="EmployeeName" HeaderText="EmployeeName" SortExpression="EmployeeName" />
                <asp:BoundField DataField="DepartmentID_FK" HeaderText="DepartmentID_FK" SortExpression="DepartmentID_FK" />
                <asp:BoundField DataField="CostDepartmentID" HeaderText="CostDepartmentID" SortExpression="CostDepartmentID" />
                <asp:BoundField DataField="AgentID" HeaderText="AgentID" SortExpression="AgentID" />
                <asp:CheckBoxField DataField="Disabled" HeaderText="Disabled" SortExpression="Disabled" />
                <asp:BoundField DataField="DisabledDate" HeaderText="DisabledDate" SortExpression="DisabledDate" />
                <asp:BoundField DataField="ADAccount" HeaderText="ADAccount" SortExpression="ADAccount" />
                <asp:BoundField DataField="Creator" HeaderText="Creator" SortExpression="Creator" />
                <asp:BoundField DataField="CreateDate" HeaderText="CreateDate" SortExpression="CreateDate" />
                <asp:BoundField DataField="Modifier" HeaderText="Modifier" SortExpression="Modifier" />
                <asp:BoundField DataField="ModifyDate" HeaderText="ModifyDate" SortExpression="ModifyDate" />
                <asp:BoundField DataField="AccessType" HeaderText="AccessType" SortExpression="AccessType" />
                <asp:BoundField DataField="NationalType" HeaderText="NationalType" SortExpression="NationalType" />
                <asp:BoundField DataField="SexType" HeaderText="SexType" SortExpression="SexType" />
            </Columns>
        </asp:GridView>
        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:LIUTEST %>" SelectCommand="SELECT * FROM [Employee]"></asp:SqlDataSource>
    </div>
    </form>
</body>
</html>
