<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Ascx_Adv.ascx.cs" Inherits="myController_Ascx_Adv" %>
<asp:PlaceHolder ID="ph_Adv" runat="server">
    <div class="coverflow">
        <div class="Big_Banner">
            <div id="banner-pc" class="carousel slide" data-ride="carousel">
                <div class="carousel-inner df-carousel-inner">
                    <asp:Literal ID="lt_AdvItems" runat="server"></asp:Literal>
                </div>

                <a class="left carousel-control" href="#banner-pc" data-slide="prev"><span class="glyphicon glyphicon-chevron-left"></span></a>
                <a class="right carousel-control" href="#banner-pc" data-slide="next"><span class="glyphicon glyphicon-chevron-right"></span></a>
                <ol class="carousel-indicators">
                    <asp:Literal ID="lt_AdvTarget" runat="server"></asp:Literal>
                </ol>
            </div>
        </div>
    </div>
</asp:PlaceHolder>
