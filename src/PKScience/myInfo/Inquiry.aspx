<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Inquiry.aspx.cs" Inherits="myInfo_Inquiry" %>

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
                            <h2 class="title-header"><%=resPublic.menu_405 %></h2>
                        </div>
                        <div class="col s12 m12 l6">
                            <div class="breadcrumb"><a href="<%:webUrl %>"><%=resPublic.menu_Home %></a>&nbsp;/&nbsp;<%=resPublic.menu_400 %></div>
                        </div>
                    </div>
                </div>
            </section>

            <!-- 主要內容 -->
            <div class="main-content">
                <div class="container">
                    <div class="row">
                        <div class="col s12">
                            <div class="row">
                                <div class="input-field col s12 m6">
                                    <asp:TextBox ID="tb_FirstName" runat="server" MaxLength="20"></asp:TextBox>
                                    <label><%=this.GetLocalResourceObject("fld_名").ToString()%>&nbsp;*</label>
                                </div>
                                <div class="input-field col s12 m6">
                                    <asp:TextBox ID="tb_LastName" runat="server" MaxLength="20"></asp:TextBox>
                                    <label><%=this.GetLocalResourceObject("fld_姓").ToString()%>&nbsp;*</label>
                                </div>
                            </div>
                            <div class="row">
                                <div class="input-field col s12 m3">
                                    <asp:DropDownList ID="ddl_AreaCode" runat="server" CssClass="browser-default"></asp:DropDownList>
                                </div>
                                <div class="input-field col s12 m3">
                                    <select id="ddl_Country" class="browser-default"></select>
                                    <asp:TextBox ID="tb_CountryValue" runat="server" Style="display: none;"></asp:TextBox>
                                </div>
                                <div class="input-field col s12 m6">
                                    <asp:TextBox ID="tb_Tel" runat="server" MaxLength="20"></asp:TextBox>
                                    <label><%=this.GetLocalResourceObject("fld_電話").ToString()%>&nbsp;*</label>
                                </div>
                            </div>
                            <div class="row">
                                <div class="input-field col s12">
                                    <asp:TextBox ID="tb_Email" runat="server" MaxLength="150"></asp:TextBox>
                                    <label><%=this.GetLocalResourceObject("fld_Email").ToString()%>&nbsp;*</label>
                                </div>
                            </div>
                            <div class="row">
                                <div class="input-field col s12">
                                    <asp:DropDownList ID="ddl_ClassID" runat="server"></asp:DropDownList>
                                </div>
                            </div>
                            <div class="row">
                                <div class="input-field col s12">
                                    <asp:TextBox ID="tb_Message" runat="server" CssClass="materialize-textarea" TextMode="MultiLine" MaxLength="300"></asp:TextBox>
                                    <label><%=this.GetLocalResourceObject("fld_您的訊息").ToString()%>&nbsp;*</label>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col s12">
                                    <a id="chg-Verify" href="javascript:void(0)">
                                        <asp:Image ID="img_Verify" runat="server" ImageAlign="Middle" />
                                        <i class="material-icons">autorenew</i>
                                    </a>
                                    <div class="input-field inline">
                                        <asp:TextBox ID="tb_VerifyCode" runat="server" MaxLength="5"></asp:TextBox>
                                        <label><%=this.GetLocalResourceObject("fld_驗證碼").ToString()%>&nbsp;*</label>
                                    </div>

                                    <asp:Panel ID="pl_Valid" runat="server" CssClass="card-panel red" Visible="false">
                                        <span class="white-text"><%=this.GetLocalResourceObject("txt_驗證碼").ToString()%></span>
                                    </asp:Panel>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col s12">
                                    <p>
                                        <asp:CheckBox ID="cb_agree" runat="server" />
                                        <label for="BodyContent_cb_agree">
                                            <%=this.GetLocalResourceObject("txt_同意").ToString()%>
                                            <a href="<%=webUrl %><%:Req_Lang %>/PrivacyPolicy" target="_blank"><%=this.GetLocalResourceObject("txt_隱私權聲明").ToString()%></a>
                                        </label>
                                    </p>
                                </div>
                            </div>

                            <div class="row">
                                <div class="col s7 m7">
                                    <asp:Panel ID="pl_Require" runat="server" CssClass="card-panel red" Visible="false">
                                        <span class="white-text"><%=this.GetLocalResourceObject("txt_必填").ToString()%></span>
                                    </asp:Panel>
                                </div>
                                <div id="formGo" class="col s5 m5 right-align" style="display: none;">
                                    <asp:LinkButton ID="lbtn_Submit" runat="server" CssClass="btn waves-effect waves-light" OnClick="lbtn_Submit_Click"><%=this.GetLocalResourceObject("txt_傳送").ToString() %><i class="material-icons right">send</i></asp:LinkButton>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptContent" runat="Server">
    <script type="text/javascript">
        /* ----- Init Start ----- */
        $(function () {

            //宣告
            var menu_AreaCode = 'select#BodyContent_ddl_AreaCode';
            var menu_Country = 'select#ddl_Country';

            //onchange事件 - 洲別
            $(menu_AreaCode).change(function () {
                var GetVal = $(menu_AreaCode + ' option:selected').val();
                //取得國家
                GetCountries(GetVal);

            });

            //onchange事件 - 國家
            $(menu_Country).change(function () {
                var GetVal = $(menu_Country + ' option:selected').val();

                //填入選擇的國家
                $("#BodyContent_tb_CountryValue").val(GetVal);

            });

            //觸發器 - 若洲別有帶預設值, 則自動觸發onchange
            $(menu_AreaCode).trigger("change");

        });
        /* ----- Init End ----- */

        /* 取得洲別國家 - 連動選單 Start */
        function GetCountries(AreaCode) {
            //宣告 - 取得物件,國家
            var myMenu = $('select#ddl_Country');
            myMenu.empty();
            myMenu.append($('<option></option>').val('').text('loading.....'));

            //判斷洲別編號是否空白
            if (AreaCode.length == 0) {
                SetMenuEmpty(myMenu);
                return false;
            }

            //這段必須加入, 不然會有No Transport的錯誤
            jQuery.support.cors = true;

            //API網址
            var uri = '<%=fn_Param.ApiUrl %>place/countries/<%=fn_Language.Web_Lang%>/?AreaCode=' + AreaCode + '&showAll=Y';

            // Send an AJAX request
            $.getJSON(uri)
                .done(function (data) {
                    //清空選項
                    myMenu.empty();

                    //加入選項
                    myMenu.append($('<option></option>').val('').text('-- <%=this.GetLocalResourceObject("fld_國家").ToString()%> --'));
                    $.each(data, function (key, item) {
                        myMenu.append($('<option></option>').val(item.CountryCode).text(item.CountryName + ' (' + item.CountryCode + ')'))
                    });

                    //取得值並設為selected
                    var getVal = $("#BodyContent_tb_CountryValue").val();
                    myMenu.val(getVal);

                })
                .fail(function (jqxhr, textStatus, error) {
                    var err = textStatus + ", " + error;
                    //alert("無法取得資料\n\r" + err);
                });

            }

            //重設選單
            function SetMenuEmpty(menuID) {
                //清空選項
                menuID.empty();

                //加入空白選項
                menuID.append($('<option></option>').val('').text('-- <%=this.GetLocalResourceObject("fld_國家").ToString()%> --'));

            }
            /* 取得洲別國家 - 連動選單 End */
    </script>
    <script>
        $(function () {
            //載入選單
            $('select').material_select();


            /* Document Load, Postback時判斷是否已勾選 */
            var get_check = $('#BodyContent_cb_agree');
            var get_submit = $('#formGo');

            if (get_check.prop("checked")) {
                get_submit.prop("style", "display:block");

            } else {
                get_submit.prop("style", "display:none");
            }


            /* 驗證碼 */
            $('#chg-Verify').click(function () {
                document.getElementById('BodyContent_img_Verify').src = '<%=fn_Param.WebUrl %>myHandler/Ashx_CreateValidImg.ashx?r=' + Math.random();
            });

            /* 同意鈕 */
            $('#BodyContent_cb_agree').click(function () {
                var get_submit = $('#formGo');

                if ($(this).prop("checked")) {
                    get_submit.prop("style", "display:block");

                } else {
                    get_submit.prop("style", "display:none");
                }
            });

        });
    </script>
</asp:Content>

