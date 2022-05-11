<%@ Page Language="C#" AutoEventWireup="true" CodeFile="LoginFail.aspx.cs" Inherits="LoginFail" %>

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
    <form id="form1" runat="server">
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
                                <i class="glyphicon glyphicon-exclamation-sign"></i>
                            </div>
                        </div>
                        <div class="metro double-size login green">
                            <span class="page-txt">登入失敗</span>
                        </div>
                        <div class="metro double-size login yellow">
                            <asp:Label ID="lb_ErrCode" runat="server" CssClass="page-txt"></asp:Label>
                        </div>
                        <div class="metro single-size terques login">
                            <asp:LinkButton ID="lbtn_Login" runat="server" CssClass="btn login-btn" OnClick="lbtn_Login_Click">重登&nbsp;<i class="glyphicon glyphicon-log-in"></i></asp:LinkButton>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <asp:PlaceHolder runat="server">
            <%: Scripts.Render("~/bundles/public") %>
            <%: Scripts.Render("~/bundles/modernizr") %>
            <%: Scripts.Render("~/bundles/respond") %>
        </asp:PlaceHolder>
    </form>
</body>
</html>
