<%@ Page Title="包材庫存盤點" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="WriteDone.aspx.cs" Inherits="myStock_WriteDone" %>

<%@ Import Namespace="ExtensionMethods" %>
<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
    <%: Styles.Render("~/bundles/product-css") %>
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="container">
        <div class="page-header">
            <h3>填寫完成
            </h3>
        </div>
        <!-- Content Start -->
        <div>
            <h4>包材庫存盤點已填寫完畢,感謝您的配合.</h4>
            <p>&nbsp;</p>
            <p>&nbsp;</p>
            <p>&nbsp;</p>
        </div>
        <div class="text-right">
            <a href="<%=Application["WebUrl"] %>" class="btn btn-primary"><i class="fa fa-home"></i>&nbsp;回首頁</a>
        </div>
        <!-- Content End -->
    </div>

</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
</asp:Content>

