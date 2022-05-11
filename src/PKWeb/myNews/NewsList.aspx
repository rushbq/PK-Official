<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="NewsList.aspx.cs" Inherits="myNews_NewsList" %>

<%@ Import Namespace="ExtensionMethods" %>
<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
    <%: Styles.Render("~/bundles/news-css") %>
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="container df-container-margin">
        <div class="page-header">
            <h3><%=Resources.resPublic.title_消息公告 %>
            </h3>
        </div>

        <!-- News Start -->
        <div class="page-title">
            <div class="pull-left header"><%=Req_Year %></div>
            <div class="pull-right">
                <div class="form-inline">
                    <label for="MainContent_ddl_Year" class="hidden-xs"><%=this.GetLocalResourceObject("txt_年份").ToString()%></label>
                    <asp:DropDownList ID="ddl_Year" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddl_Year_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                </div>
            </div>
            <div class="clearfix"></div>
        </div>

        <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemDataBound="lvDataList_ItemDataBound">
            <LayoutTemplate>
                <div class="page-body">
                    <div class="card-deck row">
                        <asp:PlaceHolder ID="ph_Items" runat="server" />
                    </div>
                </div>
            </LayoutTemplate>
            <ItemTemplate>
                <div class="card col-xs-12 col-md-4">
                    <a href="<%=Application["WebUrl"] %>News/View/<%#Cryptograph.MD5Encrypt(Eval("News_ID").ToString(), Application["DesKey"].ToString()) %>/">
                        <asp:Literal ID="lt_Pic" runat="server"></asp:Literal>
                        <div class="card-body">
                            <h5 class="card-title">
                                <%#Eval("News_Title") %>
                            </h5>
                            <p class="card-text">
                                <%#Eval("News_Desc").ToString().Replace("\r","<br/>") %>
                            </p>
                        </div>
                        <div class="card-footer"><small class="text-muted"><%#Eval("News_PubDate").ToString().ToDateString("yyyy/MM/dd") %></small></div>
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

        <div class="subtitle">
            <h4><%=this.GetLocalResourceObject("txt_年份").ToString()%></h4>
            <ul class="list-inline">
                <asp:Literal ID="lt_listYear" runat="server"></asp:Literal>
            </ul>
        </div>
        <!-- News End -->
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
        });
    </script>
</asp:Content>

