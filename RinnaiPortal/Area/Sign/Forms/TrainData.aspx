<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TrainData.aspx.cs" Inherits="RinnaiPortal.Area.Sign.Forms.TrainData" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="stylesheet" type="text/css" href="../../../Content/bootstrap.css" />
    <link rel="stylesheet" type="text/css" href="../../../Content/custom.css?id=10" />
    <link rel="stylesheet" type="text/css" href="../../../Content/dialog-layout.css" />
    <script src='<%: VirtualPathUtility.ToAbsolute(@"~/Scripts/jquery-1.10.2.js")%>' type="text/javascript"></script>
    <script src='<%: VirtualPathUtility.ToAbsolute(@"~/Scripts/bootstrap.js")%>' type="text/javascript"></script>
    <script src='<%: VirtualPathUtility.ToAbsolute(@"~/Scripts/Sign/main.js")%>' type="text/javascript"></script>

    <style>
        #DialogLayout > div > div.modal-body {
        overflow:auto;
        }
        #signDiv, #trainDiv {
            /* padding: 10px; */
            /* width: 87%; */
            border-bottom: 3px solid rgba(255, 139, 65, 0.9);
            border-radius: 6px;
            height: 240px;
            /*border-top: 1px;
            border-bottom: 1px;
            border-left: 1px;
            border-right: 1px;*/
            /*border-style: solid;
            border-color:  rgba(255, 139, 65, 0.8);*/
        }

            #signDiv:hover, #trainDiv:hover {
                border-color: rgba(255, 139, 65, 1);
            }

        #signEmpName {
            position: relative;
            top: -16px;
            border-radius: 30px;
            text-align: center;
            color: black;
        }

        #studentEmpName {
            position: relative;
            top: -16px;
            border-radius: 30px;
            text-align: center;
            color: black;
        }

        .empNameDiv {
            margin-top: 20px;
        }

        #div1 {
            /* height: 200px; */

            /*border-top: 2px solid #ccc; 
    width: 100%;
    height: 100%;
     overflow: auto; 
    position: relative;
    top: 30px;
     overflow-x: auto; 
     overflow-y: auto;*/
        }
    </style>

    <script type="text/javascript">
        Dialog.resize(1000, 600);

        window.onload = jf_init;
        function jf_init() {
            var nHeight = screen.height;//取得使用者螢幕高
            var nWidth = screen.width;//取得使用者螢幕寬
            if (nHeight > 768) {
                var divTarget = document.getElementById("div1");
                //divTarget.style.height = "500px";
                divTarget.style.width = "100%";
            }
        }
    </script>
</head>
<body id="DialogLayout">
    <%--#0017 調整受訓心得畫面--%>
    <div class="modal-dialog">
        <div class="modal-header">
            <h4 class="modal-title" id="myModalLabel"><b>表單內容 </b></h4>
        </div>
        <div class="modal-body">
            <form runat="server">
                <asp:HiddenField runat="server" ID="SN" />
                <div id="sign-class-info">

                    <div id="signDiv" class="col-xs-5">

                        <%--簽核--%>
                        <div>

                            <div class="row form-group  ">
                                <label for="SignDocID_FK" class="col-xs-3">簽核編號</label>
                                <div class="col-xs-7">
                                    <asp:Label runat="server" ID="SignDocID_FK"></asp:Label>
                                </div>
                            </div>


                            <div class="row form-group  ">
                                <label for="ApplyID_FK" class="col-xs-3">申請人編號</label>
                                <div class="col-xs-7">
                                    <asp:Label runat="server" ID="ApplyID_FK"></asp:Label>

                                </div>
                            </div>

                            <div class="row form-group  ">
                                <label for="ApplyDateTime" class="col-xs-3 ">申請日期</label>
                                <div class="col-xs-8">
                                    <asp:Label runat="server" ID="ApplyDateTime"></asp:Label>
                                </div>
                            </div>
                            <div class="row form-group empNameDiv">
                                <label for="ApplyName" class="col-xs-3">申請人姓名</label>
                                <div id="signEmpName" class="col-xs-5  alert alert-info">
                                    <asp:Label runat="server" ID="ApplyName"></asp:Label>
                                    <asp:HyperLink ID="Signed" runat="server" CssClass="btn btn-info btn-xs" CommandName="Detail" Target="_self" Text="簽核歷程" Width="80px"></asp:HyperLink>
                                </div>
                            </div>

                        </div>
                    </div>

                    <div id="trainDiv" class="col-xs-7">

                        <%--課程--%>
                        <div class="col-xs-12">

                            <div class="form-group row col-xs-5">
                                <label for="CLID" class="col-xs-6">課程代號</label>
                                <div class="col-xs-6">
                                    <asp:Label runat="server" ID="CLID"></asp:Label>
                                </div>
                            </div>
                            <div class="form-group row col-xs-6">
                                <label for="Start_Date" class="col-xs-5">開課日期</label>
                                <div class="col-xs-7" style="font-size: 4px;">
                                    <asp:Label runat="server" ID="Start_Date"></asp:Label>
                                </div>
                            </div>


                            <div class="form-group row col-xs-5">
                                <label for="CLNAME" class="col-xs-6">課程名稱</label>
                                <div class="col-xs-6 ">
                                    <asp:Label runat="server" ID="CLNAME"></asp:Label>
                                </div>
                            </div>

                            <div class="form-group row col-xs-5">
                                <label for="SID" class="col-xs-6">學員編號</label>
                                <div class="col-xs-6">
                                    <asp:Label runat="server" ID="SID"></asp:Label>
                                </div>
                            </div>

                            <div class="form-group row col-xs-5">
                                <label for="HOURS" class="col-xs-6">時數</label>
                                <div class="col-xs-6">
                                    <asp:Label runat="server" ID="HOURS"></asp:Label>
                                </div>
                            </div>

                            <div class="form-group row col-xs-5 empNameDiv">
                                <label for="SNAME" class="col-xs-6">學員姓名</label>
                                <div id="studentEmpName" class="col-xs-6 alert alert-danger">
                                    <asp:Label runat="server" ID="SNAME"></asp:Label>
                                </div>
                            </div>

                        </div>


                    </div>

                </div>




                <%--<div class="row form-group  ">
                        <label class="col-xs-3 ">簽核歷程</label>
                        <div class="col-xs-8">
                            <asp:HyperLink ID="Signed" runat="server"  CssClass="btn btn-info btn-xs" CommandName="Detail" Target="_self" Text="簽核歷程" Width="80px"></asp:HyperLink>                   
                        </div>
                    </div>--%>
                <div id="div1">
                    <asp:PlaceHolder ID="PlaceHolder1" runat="server"></asp:PlaceHolder>
                </div>

            </form>

        </div>
    </div>
</body>
</html>
