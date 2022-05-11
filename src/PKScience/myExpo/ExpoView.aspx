<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="ExpoView.aspx.cs" Inherits="myProd_ExpoView" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CssContent" runat="Server">
    <title><%=meta_Title %> | <%=webName %></title>
    <meta name="keywords" content="<%=Resources.resPublic.meta_keyword %>" />
    <meta name="description" content="<%=meta_Desc %>" />
    <meta property="og:type" content="website" />
    <meta property="og:site_name" content="<%=webName %>" />
    <meta property="og:url" content="<%=meta_Url %>" />
    <meta property="og:title" content="<%=meta_Title %>" />
    <meta property="og:description" content="<%=meta_Desc %>" />
    <meta property="og:image" content="<%=meta_Image %>" />
    <meta property="og:locale" content="zh_TW" />

    <link type="text/css" href="<%=cdnUrl %>css/ScienceKits/pub/acivity.css" rel="stylesheet" />
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
                            <div class="breadcrumb"><a href="<%=webUrl %>"><%=Resources.resPublic.menu_Home %></a>&nbsp;/&nbsp;<a href="#!" onclick="history.back(-1)"><%=Resources.resPublic.menu_300 %></a></div>
                        </div>
                    </div>
                </div>
            </section>

            <!-- 主要內容區 -->
            <div class="main-content">
                <div class="container">
                    <!-- [plugin] addthis -->
                    <div style="padding-bottom: 20px;" class="addthis_inline_share_toolbox right-align"></div>

                    <!-- 活動內容 -->
                    <div>
                        <asp:Literal ID="lt_Content" runat="server"></asp:Literal>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptContent" runat="Server">
</asp:Content>

