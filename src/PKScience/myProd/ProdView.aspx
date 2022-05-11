<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="ProdView.aspx.cs" Inherits="myProd_ProdView" %>

<%@ Import Namespace="PKLib_Method.Methods" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CssContent" runat="Server">
    <title><%=meta_TitleSeo %></title>
    <meta name="keywords" content="<%=meta_Keyword %>" />
    <meta name="description" content="<%=meta_DescSeo %>" />
    <meta property="og:type" content="website" />
    <meta property="og:site_name" content="<%=webName %>" />
    <meta property="og:url" content="<%=meta_Url %>" />
    <meta property="og:title" content="<%=meta_Title %>" />
    <meta property="og:description" content="<%=meta_Desc %>" />
    <meta property="og:image" content="<%=meta_Image %>" />
    <meta property="og:locale" content="zh_TW" />

    <link type="text/css" href="<%=cdnUrl %>css/ScienceKits/pub/products.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="Server">
    <div id="main">
        <div class="wrapper">

            <!-- 頁面抬頭 -->
            <section class="title-section">
                <div class="container">
                    <div class="row">
                        <div class="col s12 m12 l6">
                            <h2 class="title-header">
                                <asp:Literal ID="lt_Header" runat="server"></asp:Literal></h2>
                        </div>
                        <div class="col s12 m12 l6">
                            <div class="breadcrumb"><a href="<%=webUrl %>"><%=Resources.resPublic.menu_Home %></a>&nbsp;/&nbsp;<asp:Literal ID="lt_TypeName" runat="server"></asp:Literal></div>
                        </div>
                    </div>
                </div>
            </section>

            <!-- 主要內容區 -->
            <div class="main-content">
                <asp:Repeater ID="lvDataList" runat="server" OnItemDataBound="lvDataList_ItemDataBound">
                    <ItemTemplate>
                        <!-- 產品單圖 與 產品資訊 -->
                        <div class="container">
                            <div class="product-essential">
                                <div class="row">

                                    <!-- 產品圖 -->
                                    <div class="col s12 m12 l6">
                                        <%#GetData_AllPic(Eval("PicGroup").ToString(), Eval("ModelNo").ToString(),"slide") %>
                                        <%#GetData_AllPic(Eval("PicGroup").ToString(), Eval("ModelNo").ToString(),"venobox") %>
                                    </div>

                                    <!-- 產品資訊 -->
                                    <div class="col s12 m12 l6">
                                        <div class="product-violator">
                                            <asp:PlaceHolder ID="ph_NewMark" runat="server">
                                                <div class="new-label">NEW</div>
                                            </asp:PlaceHolder>
                                        </div>

                                        <!-- 特色簡述 -->
                                        <div class="feature-list">
                                            <%#Eval("ShortDesc").ToString().Replace("\r\n","<br>") %>
                                        </div>


                                        <!--我要購買與說明書下載-->
                                        <div class="product-shop">
                                            <div class="model-munber"><%#Eval("ModelNo") %></div>
                                        </div>
                                        <div style="border-bottom: 1px solid #e0e0e0;">
                                            <div class="row">
                                                <div class="col s12">
                                                    <a href="<%#fn_Param.ShopUrl(Eval("ModelNo").ToString()) %>" target="_blank" type="button" class="btn btn-primary btn-block btn-addtocart" title="<%=this.GetLocalResourceObject("txt_我要購買").ToString()%>">
                                                        <span><%=this.GetLocalResourceObject("txt_我要購買").ToString()%></span>
                                                    </a>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <asp:PlaceHolder ID="ph_Url1" runat="server">
                                                    <div class="col s6">
                                                        <a href="<%#Eval("Url1") %>" type="button" class="btn btn-success btn-block" title="<%=this.GetLocalResourceObject("txt_說明書").ToString()%>" target="_blank">
                                                            <span><%=this.GetLocalResourceObject("txt_說明書").ToString()%></span>&nbsp;<i class="material-icons">book</i>
                                                        </a>
                                                    </div>
                                                </asp:PlaceHolder>
                                                <asp:PlaceHolder ID="ph_Download" runat="server">
                                                    <div class="col s6">
                                                        <a href="#dw-modal" type="button" class="btn btn-success btn-block" title="<%=this.GetLocalResourceObject("txt_下載說明書").ToString()%>">
                                                            <span><%=this.GetLocalResourceObject("txt_下載說明書").ToString()%></span>&nbsp;<i class="material-icons">cloud_download</i>
                                                        </a>
                                                    </div>
                                                </asp:PlaceHolder>
                                            </div>
                                            <div class="row">
                                                <asp:PlaceHolder ID="ph_Url2" runat="server">
                                                    <div class="col s6">
                                                        <a href="<%#Eval("Url2") %>" type="button" class="btn btn-success btn-block" title="<%=this.GetLocalResourceObject("txt_組裝影片").ToString()%>" target="_blank">
                                                            <span><%=this.GetLocalResourceObject("txt_組裝影片").ToString()%></span>&nbsp;<i class="material-icons">videocam</i>
                                                        </a>
                                                    </div>
                                                </asp:PlaceHolder>
                                                <div class="col s6">
                                                    <a href="<%=fn_Param.WebUrl %><%:Req_Lang %>/FAQ-Search/?k=<%#Server.UrlEncode(Eval("ModelNo").ToString()) %>" type="button" class="btn btn-success btn-block">
                                                        <span><%=this.GetLocalResourceObject("txt_常見問題").ToString()%></span>&nbsp;<i class="material-icons">help_outline</i>
                                                    </a>
                                                </div>
                                            </div>
                                        </div>

                                        <!-- addthis -->
                                        <div class="addthis_inline_share_toolbox addthis-padding"></div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        </div>

                        <!-- 產品說明及特色圖 -->
                        <div class="product-collateral">
                            <div class="container">
                                <%#Eval("FullDesc") %>

                                <!-- 回列表 -->
                                <div class="center-align go-list">
                                    <a href="<%=webUrl %><%:Req_Lang %>/Products" class="waves-effect waves-light btn"><i class="material-icons right">list</i><%=this.GetLocalResourceObject("btn_goList").ToString()%></a>
                                </div>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
                <!-- 說明書下載彈出視窗 -->
                <div id="dw-modal" class="modal modal-View">
                    <div class="modal-content">
                        <h4>
                            <asp:Literal ID="lt_ModalHeader" runat="server"></asp:Literal>
                        </h4>
                        <table class="responsive-table bordered striped">
                            <thead>
                                <tr>
                                    <th><%=this.GetLocalResourceObject("fld_檔名").ToString() %></th>
                                    <th><%=this.GetLocalResourceObject("fld_語系").ToString() %></th>
                                    <th></th>
                                </tr>
                            </thead>
                            <asp:Repeater ID="dtFiles" runat="server">
                                <ItemTemplate>
                                    <tbody>
                                        <tr>
                                            <td><%#Eval("DwFileName") %></td>
                                            <td><%#Eval("LangName") %></td>
                                            <td>
                                                <a href="<%#webUrl %>myHandler/Ashx_Download.ashx?f=<%#HttpUtility.UrlEncode("ProdFiles/1/")%>&r=<%#HttpUtility.UrlEncode(Eval("DwFile").ToString()) %>&d=<%#HttpUtility.UrlEncode(Eval("DwFileName").ToString()) %>" target="_blank" class="waves-effect waves-light btn"><i class="material-icons">file_download</i></a>
                                            </td>
                                        </tr>
                                    </tbody>
                                </ItemTemplate>
                            </asp:Repeater>
                        </table>
                    </div>
                    <div class="modal-footer">
                        <a href="#!" class="modal-action modal-close waves-effect waves-green btn-flat">Close</a>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptContent" runat="Server">
    <script type="text/javascript">
        $(function () {
            //Modal
            $(".modal-View").modal();
        });
    </script>

    <!-- lazyload 圖片延遲載入 -->
    <script src="<%=cdnUrl %>js/lazyload/jquery.lazyload.min.js"></script>
    <script type="text/javascript">
        $(function () {
            $("img.lazy").lazyload();
        });
    </script>

    <!-- flexslider -->
    <link rel="stylesheet" type="text/css" href="<%=cdnUrl %>js/flexslider/flexslider.css" />
    <script src="<%=cdnUrl %>js/flexslider/jquery.flexslider-min.js"></script>
    <script>
        $(window).load(function () {
            $('.flexslider').flexslider({
                animation: "slide",
                controlNav: "thumbnails"
            });
        });
    </script>

    <!-- VenoBox -->
    <link rel="stylesheet" type="text/css" href="<%=cdnUrl %>js/venobox/venobox.css" media="screen" />
    <script type="text/javascript" src="<%=cdnUrl %>js/venobox/venobox.min.js"></script>
    <script>
        $(function () {
            //Slider圖片點擊
            $('.zoomPic').click(function () {
                var getID = $(this).attr("data-rel");

                //觸發對應的照片Venobox
                $("#" + getID).trigger("click");
            });

            //Venobox圖片放大
            $('.venobox').venobox({
                numeratio: true, // default: false 左上角照片頁碼
                border: "5px"
            });
        });
    </script>
</asp:Content>

