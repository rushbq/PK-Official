﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Index.aspx.cs" Inherits="myQA_Index" %>

<%@ Register Src="Ascx_Search.ascx" TagName="Ascx_Sh" TagPrefix="ucSearch" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CssContent" runat="Server">
    <link type="text/css" href="<%=cdnUrl %>css/ScienceKits/dev/faq.css?v=20171012" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="Server">
    <div id="main">
        <div class="wrapper">
            <!-- 頁面抬頭 -->
            <section class="title-section">
                <div class="container">
                    <div class="row">
                        <div class="col s12 m12 l6">
                            <h2 class="title-header"><%=Resources.resPublic.menu_700 %></h2>
                        </div>
                        <div class="col s12 m12 l6">
                            <div class="breadcrumb"><a href="<%=webUrl %>"><%=Resources.resPublic.menu_Home %></a></div>
                        </div>
                    </div>
                </div>
            </section>

            <!-- 主要內容區 -->
            <div class="main-content">
                <!-- 常見問題搜尋 -->
                <div class="subheader">
                    <ucSearch:Ascx_Sh ID="Ascx_Sh1" runat="server" />

                    <div class="center-align faq-search-bar-note"><%=Resources.resPublic.txt_FAQ_Search_Title %></div>
                </div>


                <div class="container">
                    <!-- List Start -->
                    <div class="question-row">
                        <asp:Literal ID="lt_Content" runat="server"></asp:Literal>
                    </div>
                    <!-- List End -->
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptContent" runat="Server">
    <script>
        $(function () {
            /* Search */
            $(".faq-Search").keypress(function (event) {
                code = (event.keyCode ? event.keyCode : event.which);
                if (code == 13) {
                    //get string
                    var searchTxt = $(this).val();

                    //redirect
                    location.href = '<%=webUrl%><%:Req_Lang%>/FAQ-Search/?k=' + encodeURIComponent(searchTxt);

                   //停止預設動作(ex:submit)
                   event.preventDefault();
               }
           });

       });
    </script>
</asp:Content>

