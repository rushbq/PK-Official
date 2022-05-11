<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="QAList.aspx.cs" Inherits="QAList" %>

<%@ Import Namespace="ExtensionMethods" %>
<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
    <%: Styles.Render("~/bundles/product-css") %>
    <%: Styles.Render("~/bundles/support-css") %>
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="container df-container-margin">
        <div class="page-header df-page-header">
            <h3><%=Resources.resPublic.title_常見問題 %></h3>
        </div>
        <!-- 關鍵字找檔案搜尋 -->
        <div class="form-group input-group">
            <asp:TextBox ID="tb_QAwords1" runat="server" MaxLength="20" CssClass="form-control myEnter"></asp:TextBox>
            <asp:Button ID="btn_Search" runat="server" OnClick="btn_Search_Click" Style="display: none;" />
            <div class="input-group-btn">
                <button type="button" class="btn btn-search doSearch"><span class="fa fa-search"></span></button>
            </div>
        </div>

        <!-- Content Start -->
        <div class="products_info">
            <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items">
                <LayoutTemplate>
                    <div class="row">
                        <asp:PlaceHolder ID="ph_Items" runat="server" />
                    </div>

                    <asp:Literal ID="lt_Pager" runat="server"></asp:Literal>
                </LayoutTemplate>
                <ItemTemplate>
                    <div class="col-xs-6 col-sm-4 col-md-3">
                        <div class="thumbnail">
                            <div class="box">
                                <div class="row">
                                    <div class="col-xs-12 col-sm-12 col-md-12">
                                        <div class="faq-image-container">
                                            <a href="<%#Application["WebUrl"] %>QA/Content/<%#HttpUtility.UrlEncode(Eval("ModelNo").ToString()) %>/<%=Req_Keyword %>/" title="<%#Eval("ModelName") %>">
                                                <%# Get_Pic(Eval("PhotoGroup").ToString(), Eval("ModelNo").ToString()) %>
                                            </a>
                                        </div>
                                    </div>
                                    <div class="col-xs-12 col-sm-12 col-md-12">
                                        <div class="content">
                                            <p class="title"><%#Eval("ModelNo") %></p>
                                            <p class="name"><%#Eval("ModelName") %></p>
                                        </div>
                                        <!-- 常見問題/商品特色按鈕 -->
                                        <div class="btn-box">
                                            <a href="<%#Application["WebUrl"] %>QA/Content/<%#HttpUtility.UrlEncode(Eval("ModelNo").ToString()) %>/<%=Req_Keyword %>/" class="btn btn-problem hidden-xs"><%=this.GetLocalResourceObject("tip_QA").ToString() %></a>
                                            <a href="<%#Application["WebUrl"] %>Product/<%#HttpUtility.UrlEncode(Eval("ModelNo").ToString()) %>/" class="btn btn-more hidden-xs"><%=this.GetLocalResourceObject("tip_產品資訊").ToString() %></a>
                                        </div>
                                    </div>
                                </div>
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

