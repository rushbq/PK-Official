<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Login" %>

<%@ Register Src="~/myController/Ascx_Navi.ascx" TagName="Ascx_Navi" TagPrefix="ucNavi" %>
<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
    <%: Styles.Render("~/bundles/member-css") %>
    <%: Styles.Render("~/bundles/venobox-css") %>
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="container df-container-margin login-wrap">
        <!--Login Start-->
        <div class="login login-bg">
            <div class="login-area">
                <div class="page-title">
                    <div class="header"><%=this.GetLocalResourceObject("txt_會員登入").ToString()%></div>
                </div>
                <div class="form-horizontal">
                    <div class="form-group">
                        <label class="col-sm-12 col-md-3 field-name"><%=this.GetLocalResourceObject("txt_帳號").ToString()%></label>
                        <div class="col-sm-12 col-md-9">
                            <asp:TextBox ID="tb_Email" runat="server" CssClass="form-control myLogin" type="email" autocomplete="off"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfv_tb_Email" runat="server" Display="Dynamic"
                                ControlToValidate="tb_Email" ValidationGroup="Login"><div class="alert alert-danger"><%=this.GetLocalResourceObject("tip_Require").ToString()%></div></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="rev_tb_Email" runat="server" ControlToValidate="tb_Email" Display="Dynamic" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ValidationGroup="Login"><div class="alert alert-danger"><%=this.GetLocalResourceObject("tip_Format").ToString()%></div></asp:RegularExpressionValidator>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-sm-12 col-md-3 field-name"><%=this.GetLocalResourceObject("txt_密碼").ToString()%></label>
                        <div class="col-sm-12 col-md-9">
                            <asp:TextBox ID="tb_Password" runat="server" CssClass="form-control myLogin" TextMode="Password"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfv_tb_Password" runat="server" Display="Dynamic"
                                ControlToValidate="tb_Password" ValidationGroup="Login"><div class="alert alert-danger"><%=this.GetLocalResourceObject("tip_Require").ToString()%></div></asp:RequiredFieldValidator>
                        </div>
                    </div>

                    <div class="form-group remember-group">
                        <label class="remember">
                            <asp:CheckBox ID="cb_Remember" runat="server" CssClass="myLogin" Checked="true" />&nbsp;<%=this.GetLocalResourceObject("txt_記住我").ToString()%>
                        </label>
                    </div>

                    <div class="form-group">
                        <button type="button" class="btn btn-login" onclick="triggerLogin()"><%=this.GetLocalResourceObject("txt_登入").ToString()%></button>
                        <asp:Button ID="btn_Login" runat="server" OnClick="btn_Login_Click" ValidationGroup="Login" Style="display: none;" />
                    </div>

                    <div class="form-group other-btn">
                        <a href="<%=Application["WebUrl"] %>ForgotPwd" class="myBox" data-type="iframe" data-title="<%=this.GetLocalResourceObject("txt_忘記密碼").ToString()%>"><%=this.GetLocalResourceObject("txt_忘記密碼").ToString()%></a>

                        <div class="line"></div>

                        <a href="<%=Application["WebUrl"] %>SignUp" class="myBox" data-type="iframe" data-title="<%=this.GetLocalResourceObject("txt_前往註冊").ToString()%>"><%=this.GetLocalResourceObject("txt_前往註冊").ToString()%></a>
                    </div>
                </div>
            </div>
            <div class="direct_login">
                <div class="page-title">
                    <div class="header"><%=this.GetLocalResourceObject("txt_登入方式2").ToString()%></div>
                </div>
                <ul>
                    <li><a href="<%=Application["WebUrl"] %>oAuth/facebook/callback.aspx">
                        <img src="<%=Application["WebUrl"] %>images/login-facebook.png" alt="facebook" /></a></li>
                    <li><a href="<%=Application["WebUrl"] %>oAuth/weibo/callback.aspx" class="myBox" data-type="iframe" data-title="Weibo">
                        <img src="<%=Application["WebUrl"] %>images/login-weibo.png" alt="weibo" /></a></li>
                </ul>
            </div>
        </div>

        <!-- 忘記密碼 跳窗 -->
        <div id="forget-pwd" style="display: none;">
            <div class="login">
                <div class="login-area">
                    <div class="page-title">
                        <div class="header">忘記密碼</div>
                    </div>
                    <div class="form-horizontal">
                        <p>請輸入您當初註冊時所填寫的 E-mail 帳號，以更改或重置您的密碼。</p>
                        <div class="form-group">
                            <label class="col-sm-12 col-md-3">帳號</label>
                            <div class="col-sm-12 col-md-9">
                                <%--<input name="ctl00$MainContent$tb_Email" id="MainContent_tb_Email"
                                    class="form-control myLogin" type="email" autocomplete="off"
                                    placeholder="您的電子郵件地址" />
                                <span id="MainContent_rfv_tb_Email" style="display: none;">
                                    <div class="alert alert-danger">此為必填欄位</div>
                                </span>
                                <span id="MainContent_rev_tb_Email" style="display: none;">
                                    <div class="alert alert-danger">輸入格式不正確</div>
                                </span>--%>
                            </div>
                        </div>
                        <div class="form-group ">
                            <%--<input type="submit" name="btn-submit" value="送出" class="btn btn-login">--%>
                        </div>
                    </div>
                </div>
            </div>
        </div>



    </div>
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
    <%: Scripts.Render("~/bundles/blockUI-script") %>
    <%: Scripts.Render("~/bundles/venobox-script") %>
    <script>
        $(function () {
            $(".myLogin").keypress(function (e) {
                code = (e.keyCode ? e.keyCode : e.which);
                if (code == 13) {
                    triggerLogin();
                    return false;
                }
            });

            //會員註冊
            $('.myBox').venobox({
                framewidth: '95%',        // default: ''
                frameheight: '550px',       // default: ''
                border: '1px',             // default: '0'
                bgcolor: '#fff',         // default: '#fff'
                titleattr: 'data-title',    // default: 'title'
                numeratio: false,            // default: false
                infinigall: false            // default: false
            });
        });

        //觸發登入
        function triggerLogin() {
            blockBox1('Login', 'Processing...');

            $('#MainContent_btn_Login').trigger('click');

            $.unblockUI();
        }
    </script>
</asp:Content>

