<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="IsoDocDetalis.aspx.cs" Inherits="RinnaiPortal.Area.Iso.User.IsoDocDetalis" %>

<asp:Content ID="IsoDetalis" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        .d-div {
            border: 1px solid black;
            border-radius: 5px;
            height: 135px;
            width: 24%;
            margin-right: 6px;
        }

        .file-upload-div {
            border: 1px solid red;
            padding: 15px;
            border-radius: 6px;
            padding-top: 20px;
            padding-bottom: 30px;
            position: absolute;
            bottom: 0;
            left: 0;
            background-color: #dddbdb;
        }

        .resultDiv {
            border: 1px solid #ccc;
            padding: 25px;
            border-radius: 6px;
            padding-top: 20px;
            padding-bottom: 30px;
            border-top: 3px solid #ccc;
            margin-bottom: 5px;
        }

        .upBtn {
            margin-top: 10px;
        }

        #isoFileList th, #isoFileList > tbody > tr > td {
            text-align: center !Important;
            vertical-align: middle;
        }
    </style>
    <input type="hidden" id="hdnUserID" value="<%= HttpContext.Current.User.Identity.Name%>" />
    <div runat="server" id="resultDiv" class="resultDiv">
        <div class="row">

            <div class="d-div col-md-3">
                <p>資料庫文件編號：</p>
                <span class="label label-success span-result" style="font-size: 21px;">
                    <asp:Literal ID="resultDocID" runat="server"></asp:Literal>
                </span>
            </div>

            <%
                string statusClass = "";
                if (resultStatus.Text == "拒絕")
                {
                    statusClass = "label-danger";
                }
                else
                {
                    statusClass = "label-success";
                }
            %>

            <div class="d-div col-md-3">
                <p>取號狀態：</p>
                <span class="label <%= statusClass %> span-result" style="font-size: 21px;">
                    <asp:Literal ID="resultStatus" runat="server"></asp:Literal>
                </span>
            </div>

            <div class="d-div col-md-3">
                <p>文件號碼：</p>
                <span class="label label-success span-result" style="font-size: 21px;">
                    <asp:Literal ID="resultNum" runat="server"></asp:Literal>
                </span>
                <p style="margin-top: 7px;">文件階級：</p>
                <span class="label label-success span-result" style="font-size: 21px;">
                    <asp:Literal ID="resultDocLevel" runat="server"></asp:Literal>
                </span>
            </div>

            <div class="d-div col-md-3">
                <p>功能操作：</p>

                <div style="display: inline-flex;" runat="server" id="uploadContrllerDiv">
                    <input type="button" class="btn btn-danger" value="開啟/隱藏 上傳管理員" id="uploadControlerBtn" style="margin-bottom: 10px;" />
                </div>

                <div style="display: inline-flex;">
                    <input type="button" class="btn btn-info" id="btnList" value="返回列表" />
                </div>
            </div>
        </div>
    </div>

    <div class="row" runat="server" id="rejectDiv" visible="false">
        <div class="col-md-12" style="margin: 10px;">
            <asp:TextBox ID="rejectRemark" CssClass="rejectRemark alert alert-danger" Width="100%" Height="60px" Enabled="false" runat="server"></asp:TextBox>
        </div>
    </div>


    <div class="row">
        <div class="col-md-12" runat="server" id="IsoList" visible="true">
            <div>

                <% if (Files.Count > 0)
                   {  %>

                <table id="isoFileList" class="table table-bordered data-table">
                    <thead>
                        <tr style="background-color: #f0ad4e; text-align: center;">
                            <td colspan="6"><span style="background-color: #f0ad4e; color: white;">檔案列表</span></td>
                        </tr>
                        <tr>
                            <th>No</th>
                            <th>管理</th>
                            <th>檔案名稱</th>
                            <th>檔案大小(mb)</th>
                            <th>上傳時間</th>

                        </tr>
                    </thead>
                    <tbody>
                        <% foreach (var file in Files)
                           {
                               int index = Files.IndexOf(file) + 1;
                               string fileSize = Math.Round(file.FILE_SZ, 3, MidpointRounding.AwayFromZero).ToString();
                        %>
                        <tr>
                            <td><%= index %></td>
                            <td>
                                <input type="button" value="檢視" class="btn btn-success dl-btn" id="<%= file.URL_PATH %>" />
                            </td>
                            <td><%= file.FILE_NM %></td>
                            <td><%= fileSize %></td>
                            <td><%= file.BUD_DT %></td>

                        </tr>
                        <% } %>
                    </tbody>
                </table>

                <%  }
                %>
            </div>
        </div>
    </div>



    <div class="up-main-div" runat="server" id="upManageDiv">
        <div class="file-upload-div" runat="server" id="uploadDiv">
            <p>
                <asp:Label ID="NumLabel" runat="server" Text="上傳管理員"></asp:Label>
            </p>
            <asp:FileUpload ID="FileUpload1" runat="server" CssClass="file" AllowMultiple="true" data-show-upload="false" />
            <asp:Button ID="SubmitBtn" runat="server" Text="確定上傳" CssClass="btn btn-success upBtn" OnClick="Button1_Click" />
        </div>
    </div>

    <script>
        $(function () {

            $('#uploadControlerBtn').on('click', function () {
                var $elem = $('.up-main-div');
                if ($elem.is(":visible")) {
                    $('.up-main-div').fadeOut(1000);
                }
                else {
                    $('.up-main-div').fadeIn(1000);
                }
            });

            $('#btnList').on('click', function () {
                var id = $('#hdnUserID').val();
                window.location.href = window.location.href = '/Area/Iso/User/IsoDefault.aspx?uId=' + id;
            });

            $('.dl-btn').on('click', function () {
                var path = $(this).attr('id');
                window.open(path, '_blank');
            });
        })

    </script>
</asp:Content>
