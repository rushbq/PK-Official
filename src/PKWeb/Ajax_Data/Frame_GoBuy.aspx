<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Frame_GoBuy.aspx.cs" Inherits="Ajax_Data_Frame_GoBuy" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>BuyList</title>
    <asp:PlaceHolder runat="server">
        <%: Styles.Render("~/bundles/product-css") %>
    </asp:PlaceHolder>
</head>
<body>
    <form id="form1" runat="server">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal"><span aria-hidden="true">&times;</span><span class="sr-only">Close</span></button>
                    <h4 class="modal-title title"><%=Req_ModelNo %></h4>
                    <h4 class="modal-title name"><%=Req_ModelName %></h4>
                </div>
                <div class="modal-body">
                    <asp:ListView ID="lvData" runat="server" ItemPlaceholderID="ph_Items">
                        <LayoutTemplate>
                            <div class="row">
                                <asp:PlaceHolder ID="ph_Items" runat="server" />
                            </div>
                        </LayoutTemplate>
                        <ItemTemplate>
                            <div class="col-md-6 col-xs-12">
                                <a href="<%#Application["WebUrl"] %>Redirect.aspx?ActType=buy&id=<%#Server.UrlEncode(Req_ModelNo) %>&data=<%=fn_Param.GetCountryCode_byIP() %>&rt=<%#Server.UrlEncode(Eval("Url").ToString().Replace("#品號#",Req_ModelNo)) %>" target="_blank">
                                    <img class="img-responsive" src="<%#Eval("ImgUrl").ToString().Replace("#CDNUrl#",CDNUrl) %>" title="" alt="<%#Req_ModelNo %>" />
                                </a>
                            </div>
                        </ItemTemplate>
                    </asp:ListView>

                    <div class="text-center bulk-purchase">
                        <%=Resources.resPublic.txt_購買聯絡 %><a href="<%=Application["WebUrl"] %>ContactUs/">&nbsp;<i class="fa fa-envelope" aria-hidden="true"></i></a>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
                </div>
            </div>
        </div>


    </form>
</body>
</html>
