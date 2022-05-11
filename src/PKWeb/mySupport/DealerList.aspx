<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="DealerList.aspx.cs" Inherits="mySupport_DealerList" %>

<%@ Register Src="~/myController/Ascx_Navi.ascx" TagName="Ascx_Navi" TagPrefix="ucNavi" %>
<%@ Import Namespace="ExtensionMethods" %>
<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
    <%: Styles.Render("~/bundles/support-css") %>
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="container df-container-margin">
        <!-- 路徑導航 Start -->
        <ucNavi:Ascx_Navi ID="Ascx_Navi1" runat="server" Param_CurrID="254" />
        <!-- 路徑導航 End -->

        <!-- Content Start -->
        <div class="Where_To_Buy_Zone">
            <asp:PlaceHolder ID="ph_asshole" runat="server" Visible="false">
                <%--IP=China only--%>
                <h3 class="page-header">中国与港澳台地区</h3>
                <div class="row">
                    <div class="col-md-2 col-sm-3 col-xs-6 country">
                        <a href="<%=Application["WebUrl"] %>WhereToBuy/CN">
                            <img src="<%=Application["WebUrl"] %>myHandler/Ashx_ImageDownload.ashx?FilePath=3NhOt0%2fu%2fA%2f77QmbzeFMLYr6yKL9FEsQLqlzPIpx%2b3HUnxn6vu%2bf0%2f2B%2fJ87S9tn" alt="Shops" width="40" style="display: inline;" /><span>中国</span>
                        </a>
                    </div>
                    <div class="col-md-2 col-sm-3 col-xs-6 country">
                        <a href="<%=Application["WebUrl"] %>WhereToBuy/HK">
                            <span>香港</span>
                        </a>
                    </div>
                    <div class="col-md-2 col-sm-3 col-xs-6 country">
                        <a href="<%=Application["WebUrl"] %>WhereToBuy/TW">
                            <span>台湾</span>
                        </a>
                    </div>
                </div>
            </asp:PlaceHolder>


            <asp:Literal ID="lt_Content" runat="server"></asp:Literal>
        </div>
        <!-- Content End -->
    </div>
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
    <%: Scripts.Render("~/bundles/lazyload-script") %>
    <script>
        $(function () {
            /* lazyload */
            $("img.lazy").lazyload({
                effect: "fadeIn",
                event: "loadme"
            });
            //觸發Lazyload
            $("img.lazy").trigger("loadme");
        });
    </script>
</asp:Content>

