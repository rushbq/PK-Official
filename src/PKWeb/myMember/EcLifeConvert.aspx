<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="EcLifeConvert.aspx.cs" Inherits="Message" %>

<%@ Register Src="~/myController/Ascx_Navi.ascx" TagName="Ascx_Navi" TagPrefix="ucNavi" %>
<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
    <%: Styles.Render("~/bundles/member-css") %>
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="container df-container-margin">
        <!-- 路徑導航 Start -->
        <ucNavi:Ascx_Navi ID="Ascx_Navi1" runat="server" Param_CurrID="606" />
        <!-- 路徑導航 End -->
        <div class="row">
            <div class="col-xs-12">
                <div class="page-title">
                    <div class="header">會員轉換</div>
                </div>
                <asp:PlaceHolder ID="ph_View" runat="server">
                    <p>
                        您目前為EcLife會員，是否要轉換為寶工官網會員?&nbsp;&nbsp;
                    <a href="<%=Application["WebUrl"] %>Login" class="btn btn-default">否</a>
                        <asp:LinkButton ID="lbtn_Convert" runat="server" CssClass="btn btn-success" OnClick="lbtn_Convert_Click">是,我想成為寶工官網會員</asp:LinkButton>
                    </p>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="ph_Msg" runat="server" Visible="false">
                    <p>您的要求已逾時，<a href="<%=Application["WebUrl"] %>Login">請重新登入</a>。</p>
                </asp:PlaceHolder>

                <p></p>
                <p></p>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
</asp:Content>

