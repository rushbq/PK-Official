<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="default.aspx.cs" Inherits="event_Default" %>


<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
    <%: Styles.Render("~/bundles/member-css") %>

    <title>Pro'sKit | 2022感恩月登錄活動</title>
    <meta name="keywords" content="寶工, 宝工, 工具, Proskit, Prokits, Tool, <%:eventTitle %>" />
    <meta name="description" content="<%:eventTitle %>" />
    <meta property="og:url" content="<%=Application["WebUrl"] %>event/<%:eventFolder %>/" />
    <meta property="og:title" content="<%:eventTitle %>" />
    <meta property="og:description" content="<%:eventTitle %>" />
    <meta property="og:image" content="<%=Application["WebUrl"] %>event/<%:eventFolder %>/act2.jpg?v=0429" />
    <meta property="og:image:width" content="600" />
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="Server">
    <div style="margin-bottom: 10px;">
        <img src="act1.jpg?v=0429" id="goEvent" class="img-responsive" alt="登錄活動" title="" style="width: 100%; cursor: pointer;" />
    </div>
    <div class="container-fluid">
        <div class="row">
            <div class="col-md-6">
                <a href="https://shop.prokits.com.tw/?utm_source=prokits.com.tw&utm_medium=banner&utm_term=%E7%AB%8B%E5%8D%B3%E8%B3%BC%E8%B2%B7&utm_content=MenuBar&utm_campaign=proskitMenu" target="_blank" class="btn btn-warning btn-lg btn-block">去 購 物</a>
            </div>
            <div class="col-md-6">
                <asp:Button ID="btn_Go" runat="server" CssClass="btn btn-primary btn-lg btn-block" OnClick="btn_Go_Click" Text="去 登 錄" Enabled="true" />
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
    <script>
        $(function () {
            $("#goEvent").click(function () {
                $("#MainContent_btn_Go").trigger("click");
            });
        });
    </script>
</asp:Content>

