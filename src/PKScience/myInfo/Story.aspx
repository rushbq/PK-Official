<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Story.aspx.cs" Inherits="myInfo_Story" %>

<%@ Import Namespace="Resources" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CssContent" runat="Server">
    <link type="text/css" href="<%=cdnUrl %>css/ScienceKits/pub/about.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="Server">
    <div id="main">
        <div class="wrapper">

            <!-- 頁面抬頭 -->
            <section class="title-section">
                <div class="container">
                    <div class="row">
                        <div class="col s12 m12 l6">
                            <h2 class="title-header"><%=resPublic.menu_401 %></h2>
                        </div>
                        <div class="col s12 m12 l6">
                            <div class="breadcrumb"><a href="<%:webUrl %>"><%=resPublic.menu_Home %></a>&nbsp;/&nbsp;<%=resPublic.menu_400 %></div>
                        </div>
                    </div>
                </div>
            </section>

            <!-- 主要內容 -->
            <div class="main-content">
                <div class="container about">
                    <div id="ajaxHtml"></div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptContent" runat="Server">
    <script>
        $(function () {
            var WebUrl = '<%=webUrl%>';
            var CDNUrl = '<%=cdnUrl%>';
            var Lang = '<%=fn_Language.Web_Lang%>';
            var strWaiting = '<h3>Loading...<h3>';

            //顯示等待中
            $('#ajaxHtml').html(strWaiting);

            //載入Html
            $.ajax({
                url: WebUrl + 'myInfo/html/' + Lang + '/Story.html?v=0104',
                dataType: "html"

            }).done(function (response) {
                //輸出Html
                $('#ajaxHtml').html(response.replace(/#cdnUrl#/g, CDNUrl));


            }).fail(function (jqXHR, textStatus) {
                //alert(sysErr + textStatus + '-QuickList');
            });



        });

    </script>
</asp:Content>

