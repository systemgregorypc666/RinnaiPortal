<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="IsoManageDetalis.aspx.cs" Inherits="RinnaiPortal.Area.Iso.Manage.IsoManageDetalis" %>

<asp:Content ID="IsoDetalis" ContentPlaceHolderID="MainContent" runat="server">

    <link href="../../../Content/font-awesome-4.7.0/css/font-awesome.min.css" rel="stylesheet" />
    <script src="../../../Scripts/datepicker-zh-TW.js"></script>
    <link href="../../../Content/select2.min.css" rel="stylesheet" />
    <script src="../../../Scripts/select2.full.min.js"></script>
    <style>
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
            margin-bottom: 10px;
        }

        .upBtn {
            margin-top: 10px;
        }

        #isoFileList th, #isoFileList > tbody > tr > td {
            text-align: center !Important;
            vertical-align: middle;
        }

        .radio-div {
            margin-right: 10px;
        }

        .ra-group {
            font-size: 13px;
            margin-right: 5px;
        }

        .d-div {
            border: 1px solid black;
            border-radius: 5px;
            margin-bottom: 5px;
            margin-right: 5px;
        }

        input[type="text"], input[type="password"], input[type="email"], input[type="tel"], input[type="select"] {
            width: 100% !important;
            max-width: 100%;
        }

        #MainContent_resultNum {
            font-size: 35px;
            text-align: center;
        }

        .iso-level {
            color: red;
            font-size: 35px;
            text-align: center;
            height: 60px;
            background-color: #ccc;
            font-weight: bold;
        }

        #MainContent_submitFormBtn {
            float: right;
        }
    </style>
    <input type="hidden" id="hdnUserID" value="<%= HttpContext.Current.User.Identity.Name%>" />
    <div runat="server" id="resultDiv" class="resultDiv">
        <div class="row">

            <div class="d-div alert alert-success" style="height: 59px; padding-top: 17px; padding-left: 21px; margin-bottom: 10px;">
                <label>資料庫文件編號：</label>
                <span class="label label-success span-result" style="font-size: 21px;">
                    <asp:Literal ID="resultDocID" runat="server"></asp:Literal>
                </span>
                <div style="display: inline-block; float: right;">
                    <label>申請人資訊：</label>
                    <span class="label label-warning span-result" style="font-size: 21px;">
                        <asp:Literal ID="empID" runat="server"></asp:Literal>

                        <asp:Literal ID="empName" runat="server"></asp:Literal>

                        <asp:Literal ID="empDepName" runat="server"></asp:Literal>
                    </span>
                </div>

            </div>

            <div class="d-div col-md-3 alert alert-danger" style="height: 203px;">
                <p>取號狀態：</p>

                <div class="radio-div">

                    <label>
                        <input type="radio" name="app" value="W" class="ra-group" />待審核
                    </label>
                </div>

                <div class="radio-div">

                    <label>
                        <input type="radio" name="app" value="Y" class="ra-group" />核准
                    </label>
                </div>

                <div class="radio-div">

                    <label>
                        <input type="radio" name="app" value="N" class="ra-group" />拒絕
                    </label>
                </div>

                <div>
                    <asp:TextBox ID="rejectRemark" CssClass="rejectRemark" Width="100%" Height="60px" Enabled="true" runat="server"></asp:TextBox>
                </div>
            </div>

            <div class="d-div col-md-8 alert alert-info" style="width: 74%;">
                <p>文件號碼：</p>
                <asp:TextBox ID="resultNum" CssClass="iso-num form-control" runat="server"></asp:TextBox>

                <div>
                    <p>文件階級：</p>
                    <asp:TextBox ID="TextBox1" CssClass="iso-level form-control" runat="server" Enabled="false"></asp:TextBox>
                </div>

                <div runat="server" id="docLevelDiv">
                    <label>
                        <input type="radio" name="docLevel" value="0" class="ra-group2" />未選擇
                    </label>

                    <label>
                        <input type="radio" name="docLevel" value="1" class="ra-group2" />一階文件
                    </label>
                    <label>
                        <input type="radio" name="docLevel" value="2" class="ra-group2" />二階文件
                    </label>
                    <label>
                        <input type="radio" name="docLevel" value="3" class="ra-group2" />三階文件
                    </label>
                    <label>
                        <input type="radio" name="docLevel" value="4" class="ra-group2" />四階文件
                    </label>
                </div>
            </div>


        </div>

        <div class="row">
            <div class="d-div" style="float: right; padding: 5px;">
                <div style="width: 100%; margin-right: 0px; display: inline-flex; padding-top: 11px; display: inline-flex; background-color: slategray;" class="d-div" runat="server" id="uploadContrllerDiv">
                    <input type="button" class="btn btn-danger" value="開啟/隱藏 發行管理員" id="uploadControlerBtn" style="margin-bottom: 10px;" />
                </div>

                <div>
                    <input type="button" class="btn btn-info" id="btnList" value="返回列表" style="float: left;" />
                    <asp:Button ID="submitFormBtn" runat="server" Text="送出" OnClientClick="return chkIsNullOrEmpty();" OnClick="setIsoNum_Click" CssClass="btn btn-success" />
                </div>
            </div>

        </div>
    </div>

    <div class="row">
        <div class="col-md-6">
            <div>

                <% if (Detalis.UserFiles.Count > 0)
                   {  %>

                <table id="isoFileList" class="table table-bordered data-table">
                    <thead>
                        <tr style="background-color: #f0ad4e; text-align: center;">
                            <td colspan="6"><span style="background-color: #f0ad4e; color: white;">User檔案列表</span></td>
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
                        <% foreach (var file in Detalis.UserFiles)
                           {
                               int index = Detalis.UserFiles.IndexOf(file) + 1;
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
        <div class="col-md-6">
            <div>

                <% if (Detalis.ManageFiles.Count > 0)
                   {  %>

                <table id="isoFileList" class="table table-bordered data-table">
                    <thead>
                        <tr style="background-color: #7bf698; text-align: center;">
                            <td colspan="8"><span style="background-color: #7bf698; color: #d9534f;">文管發行檔案列表</span></td>
                        </tr>
                        <tr>
                            <th>No</th>
                            <th>管理</th>
                            <th>品名</th>
                            <th>擔當單位</th>
                            <th>生效日期</th>
                            <th>版次</th>
                            <th>頁次</th>
                            <th>發行設定</th>
                        </tr>
                    </thead>
                    <tbody>
                        <% foreach (var file in Detalis.ManageFiles)
                           {
                               int index = Detalis.ManageFiles.IndexOf(file) + 1;
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
                            <td><%= file.BUD_DT %></td>
                            <td><%= file.BUD_DT %></td>
                            <td>
                                <input type="button" name="name" value="設定" /></td>
                        </tr>
                        <% } %>
                    </tbody>
                </table>

                <%  }
                %>
            </div>
        </div>
    </div>

    <div class="up-main-div" style="display: none;">
        <div class="file-upload-div" runat="server" id="uploadDiv">
            <button id="closeUpDiv" type="button" style="float: right; color: #a94481;"><i class="fa fa-window-close" aria-hidden="true"></i></button>
            <center><button  id="advSetting"  type="button" style="color: slategray;"><i class="fa fa-arrow-up" aria-hidden="true"></i></button></center>
            <p>
                <asp:Label ID="NumLabel" runat="server" Text="發行管理員"></asp:Label>
            </p>
            <%--            <asp:FileUpload ID="FileUpload1" runat="server" CssClass="file" AllowMultiple="true" data-show-upload="false" data-show-preview="false" />--%>
            <asp:FileUpload ID="FileUpload1" runat="server" CssClass="file-input" />
            <style>
                .publish-div {
                    margin-top: 20px;
                    width: 100%;
                    border: 1px solid #bf7000;
                    border-radius: 5px;
                    background-color: white;
                }
            </style>
            <div class="publish-div" style="padding-top: 25px; display: none;">
                <div class="container">
                    <div class="row">
                        <center><label class="alert alert-danger" style="width: 100%;" id="publishHead">發行設定</label></center>

                        <div class="col-md-6">
                            <div class="form-group">
                                <label>發行單位:</label>
                                <div style="float:right;">
                                <label for="publishAll"><input type="checkbox" id="publishAll" name="name" value="ALL" />全選</label>
                                </div>
                                
                                <select id="publishDep" name="publishDep" multiple>
                                    <option value="UNIT-5000">生產部</option>
                                    <option value="UNIT-5910">沖床烤漆課</option>
                                    <option value="UNIT-5920">切削課</option>
                                    <option value="UNIT-5930">組立課</option>
                                    <option value="UNIT-5960">水箱課</option>
                                    <option value="UNIT-5800">開發本部</option>
                                    <option value="UNIT-5200">生管課</option>
                                    <option value="UNIT-5300">倉儲課</option>
                                    <option value="UNIT-5400">品管課</option>
                                    <option value="UNIT-6000">資材部</option>
                                    <option value="UNIT-2200">管理部</option>
                                    <option value="UNIT-3000">營業本部</option>
                                    <option value="UNIT-7000">海外事業部</option>
                                    <option value="UNIT-9999">各營業所</option>
                                    <option value="UNIT-1100">管理代表</option>
                                    <option value="UNIT-2700">安全衛生部</option>
                                </select>
                            </div>

                            <div class="form-group">
                                <label for="">文件編號:</label>
                                <input type="text" class="form-control publish-doc-num" name="publishDocNum">
                            </div>
                            <div class="form-group">
                                <label for="">品名:</label>
                                <input type="text" class="form-control" name="publishProductName">
                            </div>
                            <div class="form-group">
                                <label for="">擔當單位:</label>
                                <input type="text" class="form-control" name="publishTakeDepID">
                            </div>

                        </div>

                        <div class="col-md-6">
                            <div class="form-group">
                                <label for="">生效日期:</label>
                                <input type="text" class="form-control publishDate" name="publishEffDate">
                            </div>

                            <div class="form-group">
                                <label for="">版次:</label>
                                <input type="text" class="form-control" name="publishVersion">
                            </div>

                            <div class="form-group">
                                <label for="">頁次:</label>
                                <input type="text" class="form-control" name="publishPage">
                            </div>
                            <asp:Button ID="SubmitBtn" runat="server" Text="確定上傳" CssClass="btn btn-success upBtn" OnClick="Button1_Click" OnClientClick="return FileUploadValid();" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <script>
        //給號驗證
        function chkIsNullOrEmpty() {
            var num = $('.iso-num').val();
            var radioSelected = $('input[name=app]:checked').val()
            var radioDocLevel = $('input[name=docLevel]:checked').val()
            if (radioSelected == 'Y' && num == "") {
                alert("請輸入ISO編號");
                return false;
            }
            else if (radioSelected == 'Y' && radioDocLevel == "0") {
                alert("請選擇文件階級");
                return false;
            }
            else {
                return true;
            }
        }

        //發行驗證
        function FileUploadValid() {
            var s = $(".file-caption-name").attr('title')
            if (typeof s === 'undefined' || s == '') {
                alert('請上傳檔案');
                return false;
            }
            else {
                return true;
            }
        }
    </script>

    <script>
        $(function () {
            //Datepicker繁體設定
            $.datepicker.setDefaults($.datepicker.regional["zh-TW"]);

            //初始參數
            var docNum = "<% = Detalis.IsoNumber%>";
            var status = "<%= Detalis.ApplicationStatus %>";
            var docLevel = "<%= Detalis.IsoDocmentLevel %>";

            $('#publishAll').on('click', function () {
                if ($(this).is(':checked')) {
                    var ary = [];
                    $('#publishDep > option').each(function (e, i) {
                        ary.push($(i).val());
                    });
                    $("#publishDep").val(ary).trigger("change");
                } else {
                    $("#publishDep").val(null).trigger("change");
                }
            });

            $('#publishDep').select2(
                {
                    placeholder: "請選擇發行單位",
                    width: '100%'
                });
            //檔案上傳
            $(".file-input").fileinput({
                showPreview: false,
                showUpload: false,
                browseLabel: '請選擇檔案...'
            });


            //Datepicker init
            $('.publishDate').datepicker(
                {
                    dateFormat: 'yy-mm-dd'
                });

            //Radio bind
            if (status != '') {
                $('.ra-group').each(function () {
                    if ($(this).val() == status) {
                        $(this).prop('checked', true);
                    }
                })
                if ($(this).val() == 'N') {
                    $('.rejectRemark').removeAttr('disabled');
                }
                else {
                    $('.rejectRemark').attr('disabled', 'disabled');
                }
            }

            if (docLevel != '') {
                $('.ra-group2').each(function () {
                    if ($(this).val() == docLevel) {
                        $(this).prop('checked', true);
                        var getTxt = $(this).parent('label').text().trim();
                        $('.iso-level').val(getTxt);
                    }
                })
            }
            else {
                $('.ra-group2').each(function () {
                    if ($(this).val() == '0') {
                        $(this).prop('checked', true);
                        var getTxt = $(this).parent('label').text().trim();
                        $('.iso-level').val(getTxt);
                    }
                })
            }


            $('.ra-group').click(function () {
                if ($(this).val() == 'N') {
                    $('.rejectRemark').removeAttr('disabled');
                }
                else {
                    $('.rejectRemark').attr('disabled', 'disabled');
                }
            });


            $('.ra-group2').click(function () {
                if ($(this).is(':checked')) {
                    var getTxt = $(this).parent('label').text().trim();
                    $('.iso-level').val(getTxt);
                }
            });

            //發行管理員
            $("#advSetting").click(function () {
                var divH = $('.file-upload-div').outerHeight();
                if (divH <= 186) {
                    $('.file-upload-div').animate({ height: '100%' }, 200);
                    $(this).find('i').removeClass().addClass('fa fa-arrow-down');
                    $('#publishHead').html(docNum + '發行設定');
                    $('.publish-doc-num').val(docNum);
                    $('.publish-div').show();
                }
                else {
                    $('.file-upload-div').animate({ height: '186px' }, 200);
                    $(this).find('i').removeClass().addClass('fa fa-arrow-up');
                    $('.publish-div').hide();
                }
            }
            );

            $('#closeUpDiv').click(function () {
                $('.up-main-div').fadeOut(400);
            });

            $('#uploadControlerBtn').on('click', function () {
                var $elem = $('.up-main-div');
                if ($elem.is(":visible")) {
                    $('.up-main-div').fadeOut(400);
                }
                else {
                    $('.up-main-div').fadeIn(1000);
                }
            });

            //回列表
            $('#btnList').on('click', function () {
                window.location.href = window.location.href = '/Area/Iso/Manage/IsoManage.aspx';
            });

            //顯示檔案
            $('.dl-btn').on('click', function () {
                var path = $(this).attr('id');
                window.open(path, '_blank');
            });
        })
    </script>
</asp:Content>
