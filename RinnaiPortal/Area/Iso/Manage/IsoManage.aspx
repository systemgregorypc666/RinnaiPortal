<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="IsoManage.aspx.cs" Inherits="RinnaiPortal.Area.Iso.Manage.IsoManage" %>

<asp:Content ID="IsoManage" ContentPlaceHolderID="MainContent" runat="server">
    <link href="../../../Content/select2.min.css" rel="stylesheet" />
    <script src="../../../Scripts/select2.full.min.js"></script>

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

        .bak-green {
            background-color: #5cb85c;
            color: white;
        }

        .bak-red {
            background-color: #d9534f;
            color: white;
        }

        input[type="text"], input[type="password"], input[type="email"], input[type="tel"], input[type="select"] {
            width: 100% !important;
            max-width: 100%;
        }
    </style>
    <div class="iso-main-div">
        <div class="row">

            <div class="col-md-12">
                <div class="form-group">
                    <label for="">文件編號/申請人帳號:</label>
                    <asp:TextBox ID="qry" runat="server" CssClass="form-control"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label for="">申請狀態:</label>
                    <select id="isoStatus" class="isoStatus" name="isoStatus">
                        <option value="">不限</option>
                        <option value="W">待審核</option>
                        <option value="Y">核准</option>
                        <option value="N">拒絕</option>
                    </select>
                </div>

                <div class="form-group">
                    <label for="">文件階級:</label>
                    <select id="docLevel" class="docLevel" name="docLevel">
                        <option value="0">不限</option>
                        <option value="1">一階</option>
                        <option value="2">二階</option>
                        <option value="3">三階</option>
                        <option value="4">四階</option>
                    </select>
                </div>
            </div>
            <div class="col-md-12">
                <div class="form-group">
                    <asp:Button ID="Button1" runat="server" CssClass="btn btn-info" Text="搜尋" OnClick="Button1_Click" />
                </div>
            </div>
        </div>

        <div class="row">
            <div class="col-md-12" runat="server" id="IsoList" visible="true">
                <div>

                    <table id="isoDocList" class="table table-bordered data-table">
                        <thead>
                            <tr style="background-color: #d9edf7; text-align: center;">
                                <td colspan="8"><span style="background-color: #d9edf7; color: #2487f3;">ISO文件列表(文管)</span></td>
                            </tr>
                            <tr>
                                <th>No</th>
                                <th>管理</th>
                                <th>文件編號</th>
                                <th>文件階級</th>
                                <th>申請狀態</th>
                                <th>檔案數量</th>
                                <th>申請時間</th>
                                <th>申請人帳號</th>
                            </tr>
                        </thead>
                        <tbody>
                            <% if (IsoViewModel.Data.Count == 0)
                               { %>
                            <tr>
                                <td colspan="8"><span style="color: red;">查無資料!</span></td>
                            </tr>
                            <% }%>

                            <% foreach (var data in IsoViewModel.Data)
                               {
                                   int index = IsoViewModel.Data.IndexOf(data) + 1;
                                   string statusDesc = string.Empty;
                                   string statusStyle = string.Empty;
                                   switch (data.ApplicationStatus)
                                   {
                                       case "W":
                                           statusDesc = "待審核";
                                           break;
                                       case "Y":
                                           statusDesc = "已核准";
                                           statusStyle = "bak-green";
                                           break;
                                       case "N":
                                           statusDesc = "拒絕";
                                           statusStyle = "bak-red";
                                           break;
                                       default:
                                           break;
                                   }
                            %>
                            <tr>
                                <td><%= data.ID %></td>
                                <td>
                                    <input type="button" value="管理" class="btn btn-success view-btn" id="<%= data.ID %>" />
                                </td>
                                <td><%= data.IsoNumber %></td>
                                <td><%= data.IsoDocmentLevel %></td>
                                <td class="<%= statusStyle %>"><%= statusDesc %></td>
                                <td><%= data.UserFiles.Count %></td>
                                <td><%= data.ApplicationDate %></td>
                                <td><%= data.Applicant %></td>
                            </tr>
                            <% } %>
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    </div>
    <script>
        $(function () {
            $(".isoStatus").val('<%= Filter.Status %>').trigger('change');
            $(".docLevel").val('<%= Filter.Level %>').trigger('change');

            $('.isoStatus,.docLevel').select2(
                {
                    width: '100%'
                });
            $('.view-btn').on('click', function () {
                var id = $(this).attr('id');
                window.location.href = window.location.href = '/Area/Iso/Manage/IsoManageDetalis.aspx?docId=' + id;
            });
        })
    </script>
</asp:Content>