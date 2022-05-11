<%@ Page Language="C#" AutoEventWireup="true" CodeFile="LoginPage.aspx.cs" Inherits="LoginPage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title><%=Application["WebName"] %></title>
    <asp:PlaceHolder runat="server">
        <%: Styles.Render("~/bundles/css") %>
    </asp:PlaceHolder>
</head>
<body class="lock">
    <form id="form1" runat="server" autocomplete="off">
        <div class="container-fluid">
            <div class="row">
                <div class="col-md-12">
                    <div class="lock-header">
                        <span class="glyphicon glyphicon-tower"></span>&nbsp;
                        <span><%=Application["WebName"] %></span>&nbsp;
                        <span class="glyphicon glyphicon-tower"></span>
                    </div>
                    <div class="login-wrap">
                        <div class="metro single-size red">
                            <div class="locked">
                                <i class="glyphicon glyphicon-leaf"></i>
                                <span>Login</span>
                            </div>
                        </div>
                        <div class="metro double-size login green">
                            <div class="input-append lock-input">
                                <span class="glyphicon glyphicon-user"></span>
                                <asp:TextBox ID="tb_UserID" runat="server" MaxLength="100" placeholder="Username"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfv_tb_UserID" runat="server" ErrorMessage="請輸入「帳號」!"
                                    Display="None" ControlToValidate="tb_UserID" ValidationGroup="Login"></asp:RequiredFieldValidator>
                            </div>
                        </div>
                        <div class="metro double-size login yellow">
                            <div class="input-append lock-input">
                                <span class="glyphicon glyphicon-lock"></span>
                                <asp:TextBox ID="tb_UserPwd" runat="server" TextMode="Password" MaxLength="30" placeholder="Password"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfv_tb_UserPwd" runat="server" ErrorMessage="請輸入「密碼」!"
                                    Display="None" ControlToValidate="tb_UserPwd" ValidationGroup="Login"></asp:RequiredFieldValidator>
                            </div>
                        </div>
                        <div class="metro single-size terques login">
                            <asp:LinkButton ID="lbtn_Login" runat="server" CssClass="btn login-btn" OnClick="lbtn_Login_Click" ValidationGroup="Login" OnClientClick="blockBox1_NoMsg('Login')">
                                GO&nbsp;<i class="glyphicon glyphicon-log-in"></i>
                            </asp:LinkButton>
                            <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="true"
                                ShowSummary="false" ValidationGroup="Login" />
                        </div>

                        <div class="login-footer">
                            <div class="remember-hint pull-left">
                                <asp:CheckBox ID="cb_remember" runat="server" Text="記住帳號(7天)" />
                            </div>
                           <%-- <div class="forgot-hint pull-right">
                                <a id="forget-password" class="" href="javascript:;">Forget password?</a>
                            </div>--%>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <asp:PlaceHolder runat="server">
            <%: Scripts.Render("~/bundles/public") %>
            <%: Scripts.Render("~/bundles/modernizr") %>
            <%: Scripts.Render("~/bundles/respond") %>
            <script src="<%=Application["WebUrl"] %>Scripts/blockUI/jquery.blockUI.js"></script>
            <script src="<%=Application["WebUrl"] %>Scripts/blockUI/customFunc.js"></script>
        </asp:PlaceHolder>
    </form>
</body>
</html>
