<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="PhysicalShop.aspx.cs" Inherits="myShop_PhysicalShop" %>

<%@ Import Namespace="Resources" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CssContent" runat="Server">
    <link type="text/css" href="<%=cdnUrl %>css/ScienceKits/pub/where_to_buy.css" rel="stylesheet" />
    <style>
        .tabs .tab a {
            color: #000;
            font-size: 1.1em;
            background-color: #efefef;
        }
            /*Black color to the text*/

            .tabs .tab a:hover {
                background-color: #eee;
                color: #000;
            }
            /*Text color on hover*/

            .tabs .tab a.active {
                background-color: #888;
                color: #fff;
                font-size: 1.2em;
                font-weight: bold;
            }
        /*Background and text color when a tab is active*/

        .tabs .indicator {
            background-color: #000;
        }
        /*Color of underline*/
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="Server">
    <div id="main">
        <div class="wrapper">
            <!-- 頁面抬頭 -->
            <section class="title-section">
                <div class="container">
                    <div class="row">
                        <div class="col s12 m12 l6">
                            <h2 class="title-header"><%=resPublic.menu_202 %></h2>
                        </div>
                        <div class="col s12 m12 l6">
                            <div class="breadcrumb"><a href="<%:webUrl %>"><%=resPublic.menu_Home %></a>&nbsp;/&nbsp;<%=resPublic.menu_200 %></div>
                        </div>
                    </div>
                </div>
            </section>

            <!-- 主要內容 -->
            <div class="main-content">
                <div class="container">
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
                url: WebUrl + 'myShop/html/' + Lang + '/physical.html?v=20210930',
                dataType: "html"

            }).done(function (response) {
                //輸出Html
                $('#ajaxHtml').html(response.replace(/#cdnUrl#/g, CDNUrl));

                //call tab ui
                $('.tabs').tabs();


            }).fail(function (jqXHR, textStatus) {
                //alert(sysErr + textStatus + '-QuickList');
            });

        });
    </script>
</asp:Content>

