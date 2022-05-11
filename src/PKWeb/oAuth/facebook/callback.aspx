<%@ Page Language="C#" AutoEventWireup="true" CodeFile="callback.aspx.cs" Inherits="oAuth_facebook_callback" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title><%=Application["WebName"] %></title>
    <link href="../../Css/font-awesome.min.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div style="text-align: center;">
            <h2><i class="fa fa-circle-o-notch fa-spin"></i>Please Wait....</h2>
        </div>
    </form>
</body>
</html>
