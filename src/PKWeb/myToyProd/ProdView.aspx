<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="ProdView.aspx.cs" Inherits="myProd_ProdView" %>

<%@ Register Src="~/myController/Ascx_Navi.ascx" TagName="Ascx_Navi" TagPrefix="ucNavi" %>
<%@ Import Namespace="ExtensionMethods" %>
<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
    <%: Styles.Render("~/bundles/product-css") %>

    <title><%=meta_Title %></title>
    <meta name="keywords" content="<%=meta_Keyword %>" />
    <meta name="description" content="<%=meta_DescSeo %>" />
    <meta property="og:type" content="website" />
    <meta property="og:site_name" content="Pros'Kit" />
    <meta property="og:url" content="<%=meta_Url %>" />
    <meta property="og:title" content="<%=meta_Title %>" />
    <meta property="og:description" content="<%=meta_Desc %>" />
    <meta property="og:image" content="<%=meta_Image %>" />
    <meta property="og:image:width" content="600" />
    <meta property="og:locale" content="zh_TW" />
    <meta property="fb:app_id" content="<%=fn_Param.FB_AppID %>" />
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="container df-container-margin">
        <!-- 路徑導航 Start -->
        <%--<ucNavi:Ascx_Navi ID="Ascx_Navi1" runat="server" Param_CurrID="102" />--%>
        <div>
            <ol class="breadcrumb">
                <li><a><%=Resources.resPublic.title_科學玩具 %></a></li>
                <asp:Literal ID="lt_navbar" runat="server"></asp:Literal>
            </ol>
        </div>
        <!-- 路徑導航 End -->

        <!-- // Content Start // -->
        <!-- 產品內容 -->
        <div class="row product-display-wrap">
            <!--銷售區域, 品號, 新品或停售標示, 品名, 產品圖片-->
            <div class="col-xs-12 col-sm-6 col-md-6 product-display">
                <div class="display-inline">
                    <!-- 品號 -->
                    <p class="title display-group">
                        <asp:Literal ID="lt_ModelNo" runat="server"></asp:Literal>
                    </p>
                    <!-- 新品或停售標示 -->
                    <div class="info-label-box">
                        <asp:PlaceHolder ID="ph_Label_New" runat="server">
                            <div class="new">
                                <h5><%=this.GetLocalResourceObject("txt_新品").ToString()%></h5>
                            </div>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="ph_Label_Rec" runat="server">
                            <div class="new">
                                <h5><%=this.GetLocalResourceObject("txt_推薦").ToString()%></h5>
                            </div>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="ph_Label_Stop" runat="server">
                            <div class="discontinued">
                                <h5><%=this.GetLocalResourceObject("txt_已停售").ToString()%></h5>
                            </div>
                        </asp:PlaceHolder>
                    </div>
                </div>
                <!-- 品名 -->
                <p class="name font-24px">
                    <asp:Literal ID="lt_ModelName" runat="server"></asp:Literal>
                </p>
                <!-- 銷售標籤 -->
                <div class="area-label">
                    <i class="material-icons glyphicon glyphicon-shopping-cart hidden-xs"></i>
                    <ul>
                        <asp:Literal ID="lt_Area" runat="server"></asp:Literal>
                    </ul>
                </div>

                <!-- 產品圖片 -->
                <section class="flexslider">
                    <asp:Literal ID="lt_SlidePic" runat="server"></asp:Literal>
                </section>
            </div>

            <!--品號, 新品或停售標示, 銷售區域, 品名-->
            <div class="col-xs-12 col-sm-6 col-md-6">
                <div class="description">
                    <h4><%=this.GetLocalResourceObject("txt_產品敘述").ToString()%></h4>
                    <!-- 認證符號 -->
                    <div class="icons">
                        <ul class="list-inline">
                            <asp:Literal ID="lt_Icons" runat="server"></asp:Literal>
                        </ul>
                    </div>
                    <!-- 產品簡述 -->
                    <div class="cbm-data">
                        <asp:Literal ID="lt_ProdInfo" runat="server"></asp:Literal>
                    </div>
                    <!-- FAQ, 相關下載, 圖片下載, 立即購買,聯絡我們 按鈕 -->
                    <div class="row btn-wrap">
                        <asp:Literal ID="lt_BuyUrl" runat="server"></asp:Literal>

                        <!-- 常見問題  -->
                        <asp:PlaceHolder ID="ph_FAQ" runat="server">
                            <div class="col-xs-4 col-md-3">
                                <a class="btn-more btn" href="<%=Application["WebUrl"] %>QA/Content/<%=HttpUtility.UrlEncode(Model_No) %>/">
                                    <%=this.GetLocalResourceObject("txt_常見問題").ToString()%>
                                </a>
                            </div>
                        </asp:PlaceHolder>

                        <asp:PlaceHolder ID="ph_download" runat="server">
                            <div class="col-xs-4 col-md-3">
                                <a class="btn-more btn" href="<%=Application["WebUrl"] %>Download/1/<%=HttpUtility.UrlEncode(Model_No) %>/">
                                    <%=this.GetLocalResourceObject("txt_下載").ToString()%>
                                </a>
                            </div>
                        </asp:PlaceHolder>

                        <asp:PlaceHolder ID="ph_gallery" runat="server">
                            <div class="col-xs-4 col-md-3">
                                <a class="btn-more btn" target="_blank" href="<%=Application["WebUrl"] %>Ref/<%=Cryptograph.MD5Encrypt("{0}ProductPic_Zip/{1}_gallery{2}.zip".FormatThis(Param_FileWebUrl, Model_No, "1"),DesKey) %>/<%=Cryptograph.MD5Encrypt("{0}_gallery.zip".FormatThis(Model_No),DesKey) %>/<%=Token %>">
                                    <%=this.GetLocalResourceObject("txt_圖片下載").ToString()%>
                                </a>
                            </div>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="ph_mallPic" runat="server">
                            <div class="col-xs-4 col-md-3">
                                <a class="btn-more btn" target="_blank" href="<%=Application["WebUrl"] %>Ref/<%=Cryptograph.MD5Encrypt("{0}MallPic_Zip/{1}_gallery_{2}.zip".FormatThis(Param_FileWebUrl, Model_No, fn_Language.PKWeb_Lang),DesKey) %>/<%=Cryptograph.MD5Encrypt("{0}_feature.zip".FormatThis(Model_No),DesKey) %>/<%=Token %>">
                                    <%=this.GetLocalResourceObject("txt_特色下載").ToString()%>
                                </a>
                            </div>
                        </asp:PlaceHolder>
                    </div>
                </div>
            </div>
        </div>

        <!-- 產品描述,特性,應用,規格,影片 sm md lg 畫面顯示-->
        <div class="product-collateral" style="position: relative;">
            <nav id="my-navbar" class="navbar navbar-default nav-sticky" role="navigation">
                <div class="container">
                    <ul id="list-pc" class="nav navbar-nav" style="display: flex; justify-content: space-evenly; border-top: 1px solid #ccc; border-bottom: 1px solid #ccc;">
                        <asp:PlaceHolder ID="tab_info1" runat="server" Visible="false">
                            <li>
                                <a class="scroll-name" href="#section1"><%=this.GetLocalResourceObject("txt_產品影片").ToString()%></a>
                            </li>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="tab_info2" runat="server" Visible="false">
                            <li>
                                <a class="scroll-name" href="#section2"><%=this.GetLocalResourceObject("txt_產品特色").ToString()%></a>
                            </li>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="tab_info3" runat="server" Visible="false">
                            <li>
                                <a class="scroll-name" href="#section3"><%=this.GetLocalResourceObject("txt_產品應用").ToString()%></a>
                            </li>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="tab_info4" runat="server" Visible="false">
                            <li>
                                <a class="scroll-name" href="#section4"><%=this.GetLocalResourceObject("txt_產品規格").ToString()%></a>
                            </li>
                        </asp:PlaceHolder>
                    </ul>
                </div>
            </nav>

            <!-- 產品影片 -->
            <asp:PlaceHolder ID="data_info1" runat="server" Visible="false">
                <div id="section1" class="video content container-fluid">
                    <!-- 桌機播放 -->
                    <div class="tab-content-padding hidden-sm hidden-xs">
                        <div class="video-title">
                            <ul>
                                <li class="video-product-name">
                                    <i class="material-icons glyphicon glyphicon-facetime-video"></i>
                                    <%=Model_Name %>
                                </li>
                            </ul>
                        </div>
                        <div class="video-container">
                            <asp:Literal ID="lt_Videos" runat="server"></asp:Literal>
                        </div>
                    </div>
                    <!-- 行動裝置播放-->
                    <div class="tab-content-padding visible-sm visible-xs">
                        <div class="video-title">
                            <ul>
                                <li class="video-product-name">
                                    <i class="material-icons glyphicon glyphicon-facetime-video"></i><%=Model_Name %>
                                </li>
                            </ul>
                        </div>
                        <div class="video-container">
                            <asp:Literal ID="lt_Videos_S" runat="server"></asp:Literal>
                        </div>
                    </div>
                </div>
            </asp:PlaceHolder>

            <!-- 產品特色 -->
            <asp:PlaceHolder ID="data_info2" runat="server">
                <div id="section2" class="feature content container-fluid">
                    <div class="tab-content-padding">
                        <div class="tab-content-padding">
                            <asp:Literal ID="lt_Feature" runat="server"></asp:Literal>
                        </div>
                    </div>
                </div>
            </asp:PlaceHolder>

            <!-- 產品應用-->
            <asp:PlaceHolder ID="data_info3" runat="server">

                <div id="section3" class="apply content container-fluid">
                    <div class="tab-content-padding">
                        <asp:Literal ID="lt_Application" runat="server"></asp:Literal>
                    </div>
                </div>
            </asp:PlaceHolder>

            <!--產品規格-->
            <asp:PlaceHolder ID="data_info4" runat="server">
                <div id="section4" class="specification content container-fluid">
                    <div class="tab-content-padding">
                        <!-- 尺寸示意圖 -->
                        <asp:Literal ID="lt_SpecPic" runat="server"></asp:Literal>
                        <!-- 規格內容 -->
                        <asp:Literal ID="lt_SpecInfo" runat="server"></asp:Literal>
                    </div>
                </div>
            </asp:PlaceHolder>
        </div>
        <!-- // Content End // -->

        <!-- /// Remote Modal Start /// -->
        <div class="modal fade" id="remoteModal-<%=Server.UrlEncode(Model_No) %>" tabindex="-1" role="dialog"></div>
        <!-- /// RemoteModal End /// -->

        <!-- /// Modal Contact us S /// -->
        <div class="modal fade" id="myModalContact" role="dialog">
            <div class="modal-dialog modal-dialog-centered" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal">&times;</button>
                        <div class="page-title">
                            <div class="header">商品諮詢</div>
                        </div>
                        <p class="contact-description">感謝您對【 <span><%=Model_No %></span>】有興趣，請填寫以下資料並送出，即視為同意資料使用，客服將盡快回覆您的需求!</p>
                    </div>
                    <div class="modal-body">
                        <div class="form-group">
                            <label class="col-sm-12 col-md-3">姓名</label>
                            <div class="col-sm-12 col-md-9">
                                <input class="form-control myLogin" type="text" autocomplete="off" placeholder="請填寫您姓名" />
                            </div>
                        </div>
                        <div class="form-group">
                            <label class="col-sm-12 col-md-3">電子郵件</label>
                            <div class="col-sm-12 col-md-9">
                                <input class="form-control myLogin" type="email" autocomplete="off" placeholder="請填寫您的電子郵件地址" />
                            </div>
                        </div>
                        <div class="form-group">
                            <label class="col-sm-12 col-md-3">您的留言</label>
                            <div class="col-sm-12 col-md-9">
                                <textarea rows="5" cols="50"></textarea>
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:Button ID="btn_SendContact" runat="server" Text="Submit" CssClass="btn btn-login" />
                            <asp:HiddenField ID="hf_ModelNo" runat="server" Value='<%=Model_No %>' />
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <!-- /// Modal Contact us E /// -->

        <!-- Modal unSell S -->
        <div class="modal fade" id="myUnsell" role="dialog">
            <div class="modal-dialog modal-dialog-centered" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal">&times;</button>
                        <div class="page-title">
                            <div class="header">Information</div>
                        </div>
                        <p class="contact-description">
                            <%=this.GetLocalResourceObject("txt_未開賣").ToString()%>
                        </p>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <!-- Modal unSell E -->
    </div>
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
    <!-- flexslider JS -->
    <link rel="stylesheet" type="text/css" href="<%=Application["WebUrl"] %>js/flexslider/flexslider.css">
    <script src="<%=Application["WebUrl"] %>js/flexslider/jquery.flexslider-min.js"></script>
    <!-- venobox JS -->
    <link rel="stylesheet" href="<%=Application["WebUrl"] %>js/VenoBox-master/venobox/venobox.css">
    <script src="<%=Application["WebUrl"] %>js/VenoBox-master/venobox/venobox.min.js"></script>
    <script src="<%=Application["WebUrl"] %>js/venobox_zoom.js"></script>
    <script>
        $(function () {
            //flexslider, for product pic & video
            $('.flexslider').flexslider({
                animation: "slide",
                controlNav: "thumbnails",
                Touch: true
            });

            //scrollspy, for tab
            $('body').scrollspy({ target: '#my-navbar' })

            /* MODAL - frame go buy */
            $(".doRemoteUrl").on("click", function () {
                var id = $(this).attr("data-id");   //品號
                var _name = $(this).attr("data-name");   //品名
                var _target = $(this).attr("data-target");
                console.log(id)

                //load html
                var url = '<%=Application["WebUrl"]%>' + "Ajax_Data/Frame_GoBuy.aspx?area=CN&id=" + encodeURIComponent(id) + "&name=" + _name;
                var datablock = $(_target);
                datablock.empty();
                datablock.load(url);

            });
        });
    </script>
</asp:Content>

