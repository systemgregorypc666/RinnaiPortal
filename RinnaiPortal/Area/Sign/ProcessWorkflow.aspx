<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ProcessWorkflow.aspx.cs" Inherits="RinnaiPortal.Area.Sign.ProcessWorkflow" %>

<asp:Content ID="ProcessWorkflowContent" ContentPlaceHolderID="MainContent" runat="server">
    <style type="text/css">
        .overtime-size {
            width: 100%;
            height: 600px;
        }

        .forgotPunch-size {
            width: 100%;
            height: 320px;
        }

        .train-size {
            width: 100%;
            height: 450px;
        }

        .pageNotFound-size {
            width: 100%;
            height: 250px;
        }
    </style>
    <input type="text" hidden id="PageTitle" class='page-title' value="" />

    <div id="layout-content-wrapper">
        <div id="layout-content" class="container">
            <div class="well">
                <asp:HiddenField runat="server" ID="SignDocID_FK"></asp:HiddenField>
                <asp:HiddenField runat="server" ID="Creator" />
                <asp:HiddenField runat="server" ID="CreateDate" />
                <div class="col-md-12">
                    <div class="form-group row">
                        <div class="col-sm-12">
                            <iframe runat="server" id="FormContent1" width="100%" height="623px"></iframe>
                        </div>
                    </div>
                    <asp:Label ID="txterror" runat="server" ForeColor="Red"></asp:Label>
                    <br />
                    <div class="form-group row">
                        <asp:PlaceHolder ID="PlaceHolder1" runat="server"></asp:PlaceHolder>
                    </div>
                    <div class="form-group required row">
                        <label class="col-sm-1 control-label">裁決</label>
                        <div class="form-inline col-sm-11">
                            <asp:RadioButtonList runat="server" ID="Status" CssClass="radio-inline">
                                <asp:ListItem Value="3" Selected="True">同意</asp:ListItem>
                                <asp:ListItem Value="4">駁回</asp:ListItem>
                            </asp:RadioButtonList>
                        </div>
                    </div>

                    <div class="form-group row">
                        <label for="Remark" class="col-sm-1 control-label">備註</label>
                        <div class="col-sm-11">
                            <asp:TextBox runat="server" data-val-required="請輸入備註" ID="Remark" type="text" class="form-control" TextMode="MultiLine" Height="100" Width="100%"></asp:TextBox>
                            <span class="field-validation-valid" data-valmsg-for="Remark" data-valmsg-replace="true"></span>
                        </div>
                    </div>

                </div>


                <div class="text-center">
                    <hr />

                    <button type="button" class="btn btn-default" onclick="history.back();">
                        <span class="glyphicon glyphicon-arrow-up"></span>&nbsp;回上一頁
                    </button>

                    <asp:Button ClientIDMode="Static" runat="server" ID="SaveBtn" type="submit" CssClass="btn btn-primary" OnClick="SaveBtn_Click" data-loading-text="Loading..." Text="送出" OnClientClick=""></asp:Button>
                    <asp:LinkButton runat="server" ClientIDMode="Static" ID="CoverBtn" CssClass="CoverBtn btn display-none">
                        <span class="glyphicon glyphicon-floppy-saved"></span>
                        <img class="loader" src='<%: VirtualPathUtility.ToAbsolute(@"~/icon/ajax-loader.gif") %>' />
                    </asp:LinkButton>
                </div>
            </div>
        </div>
    </div>
    <script src='<%: VirtualPathUtility.ToAbsolute(@"~/Scripts/Sign/ProcessWorkflow.js") %>' type="text/javascript"></script>
    <script src='<%: VirtualPathUtility.ToAbsolute(@"~/Scripts/Sign/validate-summary.js") %>' type="text/javascript"></script>


    <script>
        //#0012 
        function CondirmAlert() {
            //return false;
            $.confirm({
                type: 'orange',
                typeAnimated: true,
                theme: 'dark',
                title: '提醒!',
                content: '忘刷時間不符合班表時間!',
                buttons: {
                    confirm: {
                        text: '確定',
                        action: function () {
                        }
                    },
                }
            });
        }
        (function ($) {
            $(function () {
                //#0012 
                if (typeof fpTimeIsSuccess !== 'undefined') {
                    if (!fpTimeIsSuccess) {
                        CondirmAlert();
                    }
                }
            })
        })(jQuery);
    </script>
</asp:Content>
