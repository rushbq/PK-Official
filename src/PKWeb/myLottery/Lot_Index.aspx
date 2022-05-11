<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Lot_Index.aspx.cs" Inherits="Lot_Index" %>

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
            <h3>抽獎活動&nbsp;<small><asp:Literal ID="lt_subTitle" runat="server"></asp:Literal></small></h3>
        </div>
        <asp:Panel ID="pl_Data" runat="server">
            <div style="min-height: 400px">
                <img src="<%=Application["WebUrl"] %>images/lottery/2015-lottery.jpg" class="img-responsive" />
                <div class="text-center" style="padding: 20px;">
                    <button type="button" class="btn btn-primary btn-lg btn-group-justified" onclick="triggerGetGift()"><i class="fa fa-gift"></i>&nbsp;我要抽大獎&nbsp;<i class="fa fa-gift"></i></button>

                    <asp:Button ID="btn_GetGift" runat="server" OnClick="btn_GetGift_Click" Style="display: none" />
                </div>
            </div>
        </asp:Panel>
        <asp:Panel ID="pl_NoData" runat="server" Visible="false">
            <div style="height: 300px;">
                <div class="alert alert-danger">
                    <h3>&nbsp;<i class="fa fa-plane"></i>&nbsp;很抱歉，活動尚未開放或是已結束。</h3>
                </div>
            </div>
        </asp:Panel>

        <asp:Panel ID="pl_NotAgain" runat="server" Visible="false">
            <div style="height: 300px;">
                <div class="alert alert-danger">
                    <h3>&nbsp;<i class="fa fa-exclamation-triangle"></i>&nbsp;很抱歉，您已經抽過獎品囉。</h3>
                </div>
            </div>
        </asp:Panel>

        <asp:Panel ID="pl_NotLogin" runat="server" Visible="false">
            <div style="height: 300px;">
                <div class="alert alert-success">
                    <h3>&nbsp;<i class="fa fa-exclamation-triangle"></i>&nbsp;只要成為會員，就可以參加抽獎!
                        &nbsp;
                        <a href="<%=Application["WebUrl"] %>Login?code=<%=Req_DataID %>"><i class="fa fa-hand-o-right"></i>Let's GO !</a>
                    </h3>
                </div>
            </div>
        </asp:Panel>

        <asp:HiddenField ID="hf_DataID" runat="server" />
    </div>
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
    <%: Scripts.Render("~/bundles/blockUI-script") %>
    <script>
        //Click事件, 觸發搜尋鈕
        function triggerGetGift() {
            blockBox2('<h3>聖誕老人正在找禮物，請稍候...</h3>');

            $('#MainContent_btn_GetGift').trigger('click');

            $.unblockUI();
        }

    </script>

</asp:Content>

