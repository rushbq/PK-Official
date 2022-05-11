<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Error.aspx.cs" Inherits="Page_Error" %>

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
                        <div class="metro big terques">
                            <span>Ouch!</span>
                        </div>
                        <div class="metro single-size green">
                            <span class="warning">壞</span>
                        </div>
                        <div class="metro single-size yellow">
                            <span class="warning">掉</span>
                        </div>
                        <div class="metro single-size purple">
                            <span class="warning">了</span>
                        </div>
                        <div class="metro double-size red">
                            <span class="page-txt">請<a href="#">按我回報</a><br />詳細錯誤情形</span>
                        </div>
                        <div class="metro single-size gray">
                            <a href="<%=Application["WebUrl"] %>" class="btn-home"><i class="glyphicon glyphicon-home"></i></a>
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
