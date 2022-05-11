<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Lot_Result.aspx.cs" Inherits="Lot_Result" %>

<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
    <meta name="description" content="<%=meta_Title %>" />
    <meta property="og:type" content="website" />
    <meta property="og:url" content="<%=meta_Url %>" />
    <meta property="og:title" content="<%=meta_Title %>" />
    <meta property="og:description" content="<%=meta_Desc %>" />
    <meta property="og:image" content="<%=meta_Image %>" />
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="container">
        <div class="page-header">
            <h3>抽獎結果&nbsp;<small><asp:Literal ID="lt_subTitle" runat="server"></asp:Literal></small></h3>
        </div>
        <div class="jumbotron">
            <h2>恭喜您!!<br />
                獲得「<strong><asp:Literal ID="lt_Name" runat="server"></asp:Literal></strong>」&nbsp;<i class="fa fa-smile-o"></i></h2>
            <div style="margin-top: 30px;">
                <div class="row">
                    <div class="col-xs-12 col-sm-6 text-center">
                        <div style="padding-bottom: 20px;">
                            <asp:Literal ID="lt_Pic" runat="server"></asp:Literal>
                        </div>
                    </div>
                    <div class="col-xs-12 col-sm-6">
                        <div style="padding-bottom: 5px;"><i class="fa fa-bullhorn"></i>&nbsp;好康道相報</div>
                        <div class="share-btn">
                            <a href="https://www.facebook.com/sharer/sharer.php?u=<%=Server.UrlEncode(meta_Url)%>" target="_blank" title="Facebook">
                                <span class="fa-stack fa-2x">
                                    <i class="fa fa-square fa-stack-2x facebook"></i>
                                    <i class="fa fa-facebook fa-stack-1x"></i>
                                </span>
                            </a>&nbsp;
                            <a href="https://twitter.com/intent/tweet?text=<%=Server.UrlEncode(meta_Desc)%>&url=<%=Server.UrlEncode(meta_Url)%>&via=ProsKit_Tool" target="_blank" title="Twitter">
                                <span class="fa-stack fa-2x">
                                    <i class="fa fa-square fa-stack-2x twitter"></i>
                                    <i class="fa fa-twitter fa-stack-1x"></i>
                                </span>
                            </a>&nbsp;
                            <a href="http://service.weibo.com/share/share.php?url=<%=Server.UrlEncode(meta_Url)%>&title=<%=Server.UrlEncode(meta_Desc)%>" target="_blank" title="Weibo">
                                <span class="fa-stack fa-2x">
                                    <i class="fa fa-square fa-stack-2x weibo"></i>
                                    <i class="fa fa-weibo fa-stack-1x"></i>
                                </span>
                            </a>
                        </div>
                    </div>
                </div>

            </div>
        </div>

    </div>
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
</asp:Content>

