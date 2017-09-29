<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ForgotPunchListD.aspx.cs" Inherits="RinnaiPortal.Area.Sign.Forms.ForgotPunchListD" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <!--link rel="stylesheet" type="text/css" href="../../../Content/bootstrap.css" /-->
    <link rel="stylesheet" type="text/css" href="../../../Content/custom.css" />
    <link rel="stylesheet" type="text/css" href="../../../Content/dialog-layout.css" />
    <script src='<%: VirtualPathUtility.ToAbsolute(@"~/Scripts/jquery-1.10.2.js")%>' type="text/javascript"></script>
    <script src='<%: VirtualPathUtility.ToAbsolute(@"~/Scripts/bootstrap.js")%>' type="text/javascript"></script>
    <script src='<%: VirtualPathUtility.ToAbsolute(@"~/Scripts/Sign/main.js")%>' type="text/javascript"></script>
    <script type="text/javascript">
        Dialog.resize(750, 350);
    </script>
    <style>
        .modal-title {
          margin: 0;
          line-height: 1.42857143;
        }
        .modal-title2 {
          margin: 0;
          line-height: 1.42857143;
        }
        .modal-header {
            padding: 10px 15px;
            top: 0;
        }
        .modal-header2 {
            padding: 10px 15px;
            top: 0;
        }
    </style>
</head>
<body id="DialogLayout">
    <div  >
        <div class="modal-header2">
            <h4 class="modal-title" id="myModalLabel"><b>表單內容 </b></h4>
        </div>
        <div >
            <form runat="server">
                <asp:HiddenField runat="server" ID="SN" />
                <table>
                    <tr>
                        <th style="color: #FFFFFF; background-color: #333399;">
                            <label for="SignDocID_FK" class="col-xs-5 ">簽核編號：</label>
                        </th>
                        <td>
                             <asp:Label runat="server" ID="SignDocID_FK"></asp:Label>
                        </td>
                    </tr>
                </table>
            </form>
        </div>
    </div>
    <div>

    </div>
    <div >
        <div class="modal-header2">
            <h4 class="modal-title2" id="myModalLabel_S"><b>簽核明細 </b></h4>
        </div>
        <div >
            
            <asp:GridView runat="server" ID="WorkflowDetailGridView" AutoGenerateColumns="False">
                

            </asp:GridView>

            
        </div>
    </div>
</body>
</html>
