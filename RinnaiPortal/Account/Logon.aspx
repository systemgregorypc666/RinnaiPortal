<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="~/Account/Logon.aspx.cs" Inherits="RinnaiPortal.Logon" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server" ID="BodyContent">
	<fieldset class="form-horizontal">
        <table class="table text-center">
            <tr>
                <td>
                    <asp:Image runat="server" ImageUrl="~/img/logo5.jpg" />
                </td>
            </tr>
        </table>
		<table class="table text-center">
            
			<thead>
				<tr><th>網域帳號登入</th></tr>
			</thead>
			<tr>
				<td>
					帳號：<asp:TextBox ID="Account" runat="server" ></asp:TextBox>
					<asp:RequiredFieldValidator ID="RequiredFieldAccount" runat="server" ControlToValidate="Account" ErrorMessage="必填欄位" ForeColor="Red">*</asp:RequiredFieldValidator>
				</td>
			</tr>
			<tr>
				<td>
					密碼：<asp:TextBox ID="Passwd" runat="server" TextMode="Password"></asp:TextBox>
					<asp:RequiredFieldValidator ID="RequiredFieldPasswd" runat="server" ControlToValidate="Passwd" ErrorMessage="必填欄位" ForeColor="Red">*</asp:RequiredFieldValidator>
				</td>
			</tr>
			<tr>
				<td>
					網域：<asp:TextBox ID="Domain" runat="server" >rinnai</asp:TextBox>
					<asp:RequiredFieldValidator ID="RequiredFieldDomain" runat="server" ControlToValidate="Domain" ErrorMessage="必填欄位" ForeColor="Red">*</asp:RequiredFieldValidator>
				</td>
			</tr>
			<tr>
				<td class="text-center">
					<div class="checkbox inline">
						<label>
							<asp:CheckBox ID="chkPersist" runat="server" />Persist Cookie
						</label>
					</div>
				</td>
			</tr>
			<tr>
				<td>
					<asp:Button ID="Login" runat="server" Text="登入" CssClass="btn btn-primary" OnClick="Login_Click"/>                    
				</td>
			</tr>
			<tr>    
				<td>
					<asp:Label ID="ErrorMessage" runat="server"></asp:Label>
				</td>
			</tr>
		</table>
	</fieldset>
</asp:Content>
