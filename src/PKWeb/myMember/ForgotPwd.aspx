<%@ Page Language="C#" MasterPageFile="~/Site_Box.master" AutoEventWireup="true" CodeFile="ForgotPwd.aspx.cs" Inherits="ForgotPwd" %>

<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
    <%: Styles.Render("~/bundles/member-css") %>
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="container df-container-margin">
        <div class="row login">
            <div class="login-area">
                <div class="page-title">
                    <div class="header"><%=this.GetLocalResourceObject("txt_忘記密碼").ToString()%></div>
                </div>
                <div class="form-horizontal">
                    <p><%=this.GetLocalResourceObject("txt_忘記密碼說明").ToString()%></p>
                    <div class="form-group">
                        <label class="col-sm-12 col-md-3"><%=this.GetLocalResourceObject("txt_帳號").ToString()%></label>
                        <div class="col-sm-12 col-md-9">
                            <asp:TextBox ID="tb_Email" runat="server" CssClass="form-control myLogin" type="email" autocomplete="off"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfv_tb_Email" runat="server" Display="Dynamic" ControlToValidate="tb_Email" ValidationGroup="Login"><div class="alert alert-danger"><%=this.GetLocalResourceObject("tip_Require").ToString()%></div></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="rev_tb_Email" runat="server" ControlToValidate="tb_Email" Display="Dynamic" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ValidationGroup="Login"><div class="alert alert-danger"><%=this.GetLocalResourceObject("tip_Format").ToString()%></div></asp:RegularExpressionValidator>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-sm-12 col-md-3"><%=this.GetLocalResourceObject("txt_驗證碼").ToString()%></label>
                        <div class="col-sm-12 col-md-9">
                            <asp:TextBox ID="tb_VerifyCode" runat="server" MaxLength="5" CssClass="form-control myLogin" rel="nofollow"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfv_tb_VerifyCode" runat="server" Display="Dynamic"
                                ControlToValidate="tb_VerifyCode" ValidationGroup="Login"><div class="alert alert-danger"><%=this.GetLocalResourceObject("tip_Require").ToString()%></div></asp:RequiredFieldValidator>
                        </div>

                    </div>
                    <div class="form-group">
                        <div class="col-md-offset-3 col-md-9">
                            <asp:Image ID="img_Verify" runat="server" ImageAlign="Middle" />
                            <a id="chg-Verify" href="javascript:void(0)"><i class="fa fa-refresh"></i></a>
                        </div>
                    </div>

                    <div class="text-right">
                        <asp:Button ID="btn_Submit" runat="server" CssClass="btn btn-login" OnClick="btn_Submit_Click" OnClientClick="blockBox1('Login','Processing...')" ValidationGroup="Login" />
                    </div>
                </div>
            </div>

        </div>

    </div>
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
    <%: Scripts.Render("~/bundles/blockUI-script") %>
    <script>
        $(function () {
            $('#chg-Verify').click(function () {
                document.getElementById('MainContent_img_Verify').src = '<%=Application["WebUrl"] %>myHandler/Ashx_CreateValidImg.ashx?r=' + Math.random();
            });

            $(".myLogin").keypress(function (e) {
                code = (e.keyCode ? e.keyCode : e.which);
                if (code == 13) {
                    $("#MainContent_btn_Submit").trigger("click");
                }
            });
        });
    </script>
</asp:Content>

