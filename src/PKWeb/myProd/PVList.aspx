<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="PVList.aspx.cs" Inherits="mySupport_PVList" %>

<%@ Register Src="~/myController/Ascx_Navi.ascx" TagName="Ascx_Navi" TagPrefix="ucNavi" %>
<%@ Import Namespace="ExtensionMethods" %>
<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
    <%: Styles.Render("~/bundles/support-css") %>
    <%: Styles.Render("~/bundles/venobox-css") %>
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="container df-container-margin">
        <!-- 路徑導航 Start -->
        <%--<ucNavi:Ascx_Navi ID="Ascx_Navi1" runat="server" Param_CurrID="101" />--%>
        <!-- 路徑導航 End -->
        <div class="page-header">
            <h3>
                <%=Resources.resPublic.title_工具專區 %>&nbsp;-&nbsp;<%=Resources.resPublic.home_產品影片 %>
            </h3>
        </div>
        <div class="search">
            <div class="form-inline">
                <asp:DropDownList ID="ddl_ProdClass" runat="server" CssClass="form-control"></asp:DropDownList>
                <asp:TextBox ID="tb_Keyword" runat="server" CssClass="form-control myEnter" placeholder="Enter a keyword." MaxLength="20"></asp:TextBox>
                <asp:Button ID="btn_Search" runat="server" CssClass="btn btn-success" ValidationGroup="Search" OnClick="btn_Search_Click" OnClientClick="blockBox2_NoMsg()" />
            </div>
        </div>
        <!-- Content Start -->

        <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemDataBound="lvDataList_ItemDataBound">
            <LayoutTemplate>
                <div class="row Knowledge_Info">
                    <asp:PlaceHolder ID="ph_Items" runat="server" />
                </div>

                <asp:Literal ID="lt_Pager" runat="server"></asp:Literal>
            </LayoutTemplate>
            <ItemTemplate>
                <div class="col-xs-12 col-sm-6 col-md-3">
                    <a class="zoomVideo" data-type="iframe" href="<%#Eval("PV_Uri") %>">
                        <div class="box">
                            <div class="row">
                                <div class="col-xs-4 col-sm-12 col-md-12">
                                    <asp:Literal ID="lt_Pic" runat="server"></asp:Literal>
                                </div>
                                <div class="col-xs-8 col-sm-12 col-md-12">
                                    <div class="content">
                                        <p class="title"><%#Eval("PV_Title") %></p>
                                        <p class="name"><%#Eval("PV_PubDate").ToString().ToDateString("yyyy/MM/dd") %></p>
                                    </div>
                                    <div class="clearfix"></div>
                                </div>
                            </div>
                        </div>
                    </a>
                </div>
            </ItemTemplate>
            <EmptyDataTemplate>
                <div class="exception-info">
                    <div class="icon"><i class="fa fa-exclamation-triangle"></i></div>
                    <div class="message">Oops! <%=Resources.resPublic.txt_查無資料 %></div>
                </div>
            </EmptyDataTemplate>
        </asp:ListView>

        <!-- Content End -->
    </div>

</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
    <%: Scripts.Render("~/bundles/blockUI-script") %>
    <%: Scripts.Render("~/bundles/lazyload-script") %>
    <%: Scripts.Render("~/bundles/venobox-script") %>
    <script>
        $(function () {
            /* lazyload */
            $("img.lazy").lazyload({
                effect: "fadeIn"
            });

            /* Venobox */
            $('.zoomVideo').venobox();

            /* Enter 偵測 */
            $(".myEnter").keypress(function (e) {
                code = (e.keyCode ? e.keyCode : e.which);
                if (code == 13) {
                    $("#MainContent_btn_Search").trigger("click");
                }
            });
        });
    </script>
</asp:Content>

