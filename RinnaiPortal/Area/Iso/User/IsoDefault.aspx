<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="IsoDefault.aspx.cs" Inherits="RinnaiPortal.Area.Iso.Default" %>

<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="RinnaiPortal.Entities" %>

<asp:Content ID="IsoDefault" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .iso-main-div {
            border: 1px solid #ccc;
            padding: 25px;
            border-radius: 6px;
            padding-top: 20px;
            padding-bottom: 30px;
        }

        .get-num-div {
            border: 1px solid #ccc;
            padding: 5px;
            border-radius: 6px;
        }

        .get-num-btn {
            width: 100%;
            height: 50px;
        }

        .span-result {
            font-size: 21px;
        }

        .text-center {
            text-align: center;
        }

        #isoDocList th, #isoDocList > tbody > tr > td {
            text-align: center !Important;
            vertical-align: middle;
        }
    </style>
    <div class="iso-main-div">
        <div class="row" style="padding-left: 15px;">
            <div class="col-md-2 get-num-div" runat="server" id="divNum">
                <asp:Button ID="GetNewNumBtn" runat="server" Text="Iso文件取號申請" CssClass="btn btn-info get-num-btn" OnClick="GetNewNumBtn_Click" />

                <%--    <div runat="server" id="resultDiv" visible="false">
                    <p>取號狀態：</p>
                    <span class="label label-success span-result">
                        <asp:Literal ID="resultStatus" runat="server"></asp:Literal>
                    </span>
                    <p style="margin-top: 20px;">取得號碼：</p>
                    <span class="label label-info span-result">
                        <asp:Literal ID="resultNum" runat="server"></asp:Literal>
                    </span>
                </div>--%>
            </div>
        </div>

        <div class="row">
            <div class="col-md-12" runat="server" id="IsoList" visible="true">
                <div>
                    <% if (BindData.Count > 0)
                       { %>

                    <table id="isoDocList" class="table table-bordered data-table">
                        <thead>
                            <tr style="background-color: #d9534f; text-align: center;">
                                <td colspan="6"><span style="background-color: #d9534f; color: white;">列表</span></td>
                            </tr>
                            <tr>
                                <th>編號</th>
                                <th>管理</th>
                                <th>文件編號</th>
                                <th>申請狀態</th>
                                <th>申請時間</th>
                                <th>申請人帳號</th>

                            </tr>
                        </thead>
                        <tbody>
                            <% foreach (var data in BindData)
                               {
                                   string statusDesc = string.Empty;
                                   switch (data.APP_ST)
                                   {
                                       case "W":
                                           statusDesc = "待審核";
                                           break;
                                       case "Y":
                                           statusDesc = "已核准";
                                           break;
                                       case "N":
                                           statusDesc = "拒絕";
                                           break;
                                       default:
                                           break;
                                   }
                            %>
                            <tr>
                                <td><%= data.ID %></td>
                                <td>
                                    <input type="button" value="檢視" class="btn btn-success view-btn" id="<%= data.ID %>" />
                                </td>
                                <td><%= data.ISO_NUM %></td>
                                <td><%= statusDesc %></td>
                                <td><%= data.APP_DT %></td>
                                <td><%= data.APP_USR %></td>

                            </tr>
                            <% } %>
                        </tbody>
                    </table>

                    <% }%>
                </div>
            </div>
        </div>
    </div>
    <script>
        $('.view-btn').on('click', function () {
            var id = $(this).attr('id');
            window.location.href = window.location.href = '/Area/Iso/User/IsoDocDetalis.aspx?docId=' + id;
        });
        var test = '';
    </script>
</asp:Content>
