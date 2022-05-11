<%@ Page Language="C#" MasterPageFile="~/Site_Box.master" AutoEventWireup="true" CodeFile="SignUp.aspx.cs" Inherits="SignUp" %>

<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
    <%: Styles.Render("~/bundles/member-css") %>
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="container df-container-margin">
        <div class="row login singUp">
            <div class="login-area">
                <div class="page-title">
                    <div class="header"><%=this.GetLocalResourceObject("txt_資料提供").ToString()%></div>
                </div>
                <div class="form-horizontal">
                    <div class="form-group">
                        <label class="col-sm-12 col-md-3 field-name"><%=this.GetLocalResourceObject("txt_帳號").ToString()%></label>
                        <div class="col-sm-12 col-md-9">
                            <asp:TextBox ID="tb_Email" runat="server" CssClass="form-control myLogin" type="email" autocomplete="off"></asp:TextBox>
                            <div id="checkOK" class="alert alert-info text-green" style="display: none;">
                                <i class="fa fa-check fa-fw"></i>&nbsp;<%=this.GetLocalResourceObject("tip_帳號可使用").ToString()%>
                            </div>
                            <div id="checkNO" class="alert alert-info text-red" style="display: none;">
                                <i class="fa fa-times fa-fw"></i>&nbsp;<%=this.GetLocalResourceObject("tip_帳號不可使用").ToString()%>
                            </div>
                            <asp:RequiredFieldValidator ID="rfv_tb_Email" runat="server" Display="Dynamic"
                                ControlToValidate="tb_Email" ValidationGroup="Add"><div class="alert alert-danger"><%=this.GetLocalResourceObject("tip_Require").ToString()%></div></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="rev_tb_Email" runat="server" ControlToValidate="tb_Email" Display="Dynamic" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ValidationGroup="Add"><div class="alert alert-danger"><%=this.GetLocalResourceObject("tip_Format").ToString()%></div></asp:RegularExpressionValidator>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-sm-12 col-md-3 field-name"><%=this.GetLocalResourceObject("txt_密碼").ToString()%></label>
                        <div class="col-sm-12 col-md-9">
                            <asp:TextBox ID="tb_Password" runat="server" CssClass="form-control myLogin" TextMode="Password" MaxLength="20"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfv_tb_Password" runat="server" Display="Dynamic"
                                ControlToValidate="tb_Password" ValidationGroup="Add"><div class="alert alert-danger"><%=this.GetLocalResourceObject("tip_Require").ToString()%></div></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="rev_tb_Password" runat="server"
                                Display="Dynamic" ControlToValidate="tb_Password" ValidationExpression="^(?!.*[^\x30-\x39\x41-\x5A\x61-\x7A])(?=.{8,20})(?=.*\d)(?=.*[A-Z]).*$"><div class="alert alert-danger"><%=this.GetLocalResourceObject("tip_密碼規則").ToString()%></div></asp:RegularExpressionValidator>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-sm-12 col-md-3 field-name"><%=this.GetLocalResourceObject("txt_確認密碼").ToString()%></label>
                        <div class="col-sm-12 col-md-9">
                            <asp:TextBox ID="tb_CfmPassword" runat="server" CssClass="form-control myLogin" TextMode="Password"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfv_tb_CfmPassword" runat="server" Display="Dynamic"
                                ControlToValidate="tb_CfmPassword" ValidationGroup="Add"><div class="alert alert-danger"><%=this.GetLocalResourceObject("tip_Require").ToString()%></div></asp:RequiredFieldValidator>
                            <asp:CompareValidator ID="cv_tb_ConfirmPwd" runat="server" Display="Dynamic" ControlToValidate="tb_CfmPassword" ControlToCompare="tb_Password" ValidationGroup="Add"><div class="alert alert-danger"><%=this.GetLocalResourceObject("tip_密碼不一致").ToString()%></div></asp:CompareValidator>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-sm-12 col-md-3 field-name"><%=this.GetLocalResourceObject("txt_驗證碼").ToString()%></label>
                        <div class="col-sm-12 col-md-9">
                            <asp:TextBox ID="tb_VerifyCode" runat="server" MaxLength="5" CssClass="form-control myLogin" rel="nofollow"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfv_tb_VerifyCode" runat="server" Display="Dynamic"
                                ControlToValidate="tb_VerifyCode" ValidationGroup="Add"><div class="alert alert-danger"><%=this.GetLocalResourceObject("tip_Require").ToString()%></div></asp:RequiredFieldValidator>
                        </div>
                    </div>

                    <div class="form-group">
                        <div class="col-md-offset-3 col-md-9">
                            <asp:Image ID="img_Verify" runat="server" ImageAlign="Middle" />
                            <a id="chg-Verify" href="javascript:void(0)"><i class="fa fa-refresh"></i></a>
                        </div>
                    </div>

                    <asp:Button ID="btn_Submit" runat="server" CssClass="btn btn-login" OnClick="btn_Submit_Click" OnClientClick="blockBox1('Add','Processing...')" ValidationGroup="Add" />

                </div>
            </div>
            <div class="row direct_login">
                <div class="page-title">
                    <div class="header"><%=this.GetLocalResourceObject("txt_登入方式2").ToString()%></div>
                </div>

                <ul>
                    <li><a href="<%=Application["WebUrl"] %>oAuth/facebook/callback.aspx">
                        <img src="<%=Application["WebUrl"] %>images/login-facebook.png" alt="facebook" /></a></li>
                    <li><a href="<%=Application["WebUrl"] %>oAuth/weibo/callback.aspx">
                        <img src="<%=Application["WebUrl"] %>images/login-weibo.png" alt="weibo" /></a></li>
                </ul>

            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
    <%: Scripts.Render("~/bundles/blockUI-script") %>
    <script>
        $(function () {
            /* 偵測enter */
            $(".myLogin").keypress(function (e) {
                code = (e.keyCode ? e.keyCode : e.which);
                if (code == 13) {
                    $("#MainContent_btn_Submit").trigger("click");
                    //取消DOM的預設功能事件,避免觸發其他的control submit
                    event.preventDefault();
                }
            });

            /* 驗證碼 */
            $('#chg-Verify').click(function () {
                document.getElementById('MainContent_img_Verify').src = '<%=Application["WebUrl"] %>myHandler/Ashx_CreateValidImg.ashx?r=' + Math.random();
            });

            /* 判斷帳號可否使用 Start */
            $('#MainContent_tb_Email').focusout(function () {
                //取得顯示區塊
                var showOK = $('#checkOK');
                var showNO = $('#checkNO');

                //判斷欄位
                var email = $(this).val();
                if (email == '') {
                    showNO.hide();
                    showOK.hide();

                } else {
                    //API 網址
                    var uri = '<%=Application["WebUrl"]%>Check/CheckAccount/' + encodeURIComponent(email) + '/';

                    // Send an AJAX request
                    $.getJSON(uri)
                        .done(function (data) {
                            //回傳資料轉成大寫
                            var getData = data.toUpperCase();

                            //判斷回傳內容
                            if (getData == 'OK') {
                                showOK.show('fast');
                                showNO.hide();
                            } else if (getData == 'NO') {
                                showOK.hide();
                                showNO.show('fast');
                            } else {
                                showNO.hide();
                                showOK.hide();
                            }
                        })

                        .fail(function (jqxhr, textStatus, error) {
                            //var err = textStatus + ", " + error;
                            //alert("Request Failed: " + err);
                            alert('Please try it later.');
                            showNO.hide();
                            showOK.hide();
                        });
                }
            });

            /* 判斷帳號可否使用 End */
        });
    </script>
</asp:Content>

