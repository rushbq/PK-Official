<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Terms.aspx.cs" Inherits="myInfo_Terms" %>

<%@ Register Src="~/myController/Ascx_Navi.ascx" TagName="Ascx_Navi" TagPrefix="ucNavi" %>
<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
    <%: Styles.Render("~/bundles/privacy-css") %>
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="container df-container-margin">
        <!-- 路徑導航 Start -->
        <ucNavi:Ascx_Navi ID="Ascx_Navi1" runat="server" Param_CurrID="506" />
        <!-- 路徑導航 End -->
        <div id="ajaxHtml"></div>
    </div>
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
    <script>
        $(function () {
            var WebUrl = '<%=Application["WebUrl"]%>';
            var Lang = '<%=fn_Language.PKWeb_Lang%>';
            var strWaiting = '<h3><i class="fa fa-spinner fa-spin"></i> Loading...<h3>';

            //顯示等待中
            $('#ajaxHtml').html(strWaiting);

            //載入Html
            $.ajax({
                url: WebUrl + 'myInfo/html/' + Lang + '/Terms.html?v=20200117',
                dataType: "html"

            }).done(function (response) {
                //輸出Html
                $('#ajaxHtml').html(response.replace(/#Weburl#/g, WebUrl));

               

            }).fail(function (jqXHR, textStatus) {
                //alert(sysErr + textStatus + '-QuickList');
            });



        });

    </script>
</asp:Content>

