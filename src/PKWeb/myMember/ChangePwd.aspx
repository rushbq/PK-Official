<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="ChangePwd.aspx.cs" Inherits="ChangePwd" %>

<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
    <%: Styles.Render("~/bundles/member-css") %>
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="container df-container-margin">
        <div class="login login-bg">
            <div class="login-area">
                <div class="page-title">
                    <div class="header"><%=this.GetLocalResourceObject("txt_變更密碼").ToString()%></div>
                </div>
                <div class="form-horizontal">
                    <div class="form-group">
                        <label class="col-sm-12 col-md-3 field-name"><%=this.GetLocalResourceObject("txt_密碼").ToString()%></label>
                        <div class="col-sm-12 col-md-9">
                            <asp:TextBox ID="tb_Password" runat="server" CssClass="form-control myLogin" TextMode="Password" MaxLength="20"></asp:TextBox>
                            <small class="text-muteds"><%=this.GetLocalResourceObject("txt_變更密碼說明").ToString()%></small>
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

                            <div class="num" style="padding: 10px 0;">
                                <asp:Image ID="img_Verify" runat="server" ImageAlign="Middle" />
                                <a id="chg-Verify" href="javascript:void(0)"><i class="fa fa-refresh"></i></a>
                            </div>
                        </div>
                    </div>
                    <div class="form-group text-right">
                        <asp:Button ID="btn_Submit" runat="server" CssClass="btn btn-green-block" OnClick="btn_Submit_Click" OnClientClick="blockBox1('Add','Processing...')" ValidationGroup="Add" />
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
            $(".myLogin").keypress(function (e) {
                code = (e.keyCode ? e.keyCode : e.which);
                if (code == 13) {
                    $("#MainContent_btn_Submit").trigger("click");
                }
            });

            $('#chg-Verify').click(function () {
                document.getElementById('MainContent_img_Verify').src = '<%=Application["WebUrl"] %>myHandler/Ashx_CreateValidImg.ashx?r=' + Math.random();
            });
        });
    </script>
</asp:Content>

