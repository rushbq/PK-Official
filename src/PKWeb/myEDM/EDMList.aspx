<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="EDMList.aspx.cs" Inherits="myEDM_EDMList" %>

<%@ Register Src="~/myController/Ascx_Navi.ascx" TagName="Ascx_Navi" TagPrefix="ucNavi" %>
<%@ Import Namespace="ExtensionMethods" %>
<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
    <%: Styles.Render("~/bundles/distributor-css") %>
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="container df-container-margin">
        <!-- 路徑導航 Start -->
        <ucNavi:Ascx_Navi ID="Ascx_Navi1" runat="server" />
        <!-- 路徑導航 End -->

        <!-- Content Start -->
        <div class="page-title">
            <div class="pull-left header"><%=Req_Year %></div>
            <div class="pull-right form-inline">
                <div class="form-group input-group">
                    <asp:TextBox ID="tb_Subject" runat="server" CssClass="form-control myEnter" autocomplete="off" MaxLength="20"></asp:TextBox>
                    <div class="input-group-btn">
                        <button type="button" class="btn btn-success doSearch"><span class="fa fa-search"></span></button>
                    </div>
                </div>
                <div class="form-group">
                    <asp:DropDownList ID="ddl_Year" runat="server" CssClass="form-control" OnTextChanged="btn_Search_Click" AutoPostBack="true"></asp:DropDownList>                    
                </div>
                <asp:Button ID="btn_Search" runat="server" OnClick="btn_Search_Click" Style="display: none;" />
            </div>
            <div class="clearfix"></div>
        </div>

        <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemDataBound="lvDataList_ItemDataBound">
            <LayoutTemplate>
                <div id="listTable" class="page-body">
                    <asp:PlaceHolder ID="ph_Items" runat="server" />
                </div>
            </LayoutTemplate>
            <ItemTemplate>
                <div class="content-info">
                    <div class="media">
                        <asp:Literal ID="lt_Pic" runat="server"></asp:Literal>
                        <div class="media-body">
                            <a href="<%=Application["File_WebUrl"] %>PKEDM/EDM_Html/<%#Eval("ID").ToString()%>/public.html" target="_blank">
                                <h5 class="date"><%#Eval("Time").ToString().ToDateString("yyyy/MM/dd") %></h5>
                                <h4 class="media-title"><%#Eval("Label") %></h4>
                            </a>
                        </div>
                    </div>
                </div>
            </ItemTemplate>
            <EmptyDataTemplate>
                <div class="exception-info">
                    <div class="icon"><i class="fa fa-exclamation-triangle"></i></div>
                    <div class="message">Oops! <%=Resources.resPublic.txt_查無資料 %></div>
                </div>
            </EmptyDataTemplate>
        </asp:ListView>

        <div class="footer-year">
            <h4><%=this.GetLocalResourceObject("txt_年份").ToString()%></h4>
            <ul class="list-inline">
                <asp:Literal ID="lt_listYear" runat="server"></asp:Literal>
                <asp:PlaceHolder ID="ph_history" runat="server" Visible="false">
                    <li><a href="<%=historyUrl() %>" target="_blank"><i class="fa fa-history fa-fw" aria-hidden="true"></i><%=this.GetLocalResourceObject("txt_歷史記錄").ToString()%></a></li>
                </asp:PlaceHolder>
            </ul>
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
                effect: "fadeIn"
            });

            /* Search click */
            $(".doSearch").click(function () {
                $("#MainContent_btn_Search").trigger("click");
            });

            /* Enter 偵測 */
            $(".myEnter").keypress(function (e) {
                code = (e.keyCode ? e.keyCode : e.which);
                if (code == 13) {
                    $("#MainContent_btn_Search").trigger("click");
                    event.preventDefault();
                }
            });
        });
    </script>
</asp:Content>

