<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="MemberData.aspx.cs" Inherits="MemberData" %>


<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
    <%: Styles.Render("~/bundles/member-css") %>
    <%: Styles.Render("~/bundles/DTpicker-css") %>
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="container df-container-margin">
        <div class="login login-bg">
            <div class="login-area">
                <div class="page-title">
                    <div class="header"><%=this.GetLocalResourceObject("txt_資料提供").ToString()%></div>
                </div>
                <div class="form-horizontal">
                    <div class="form-group">
                        <label class="col-md-2 text-right"><%=this.GetLocalResourceObject("txt_名").ToString()%></label>
                        <div class="col-md-4 has-error">
                            <asp:TextBox ID="tb_LastName" runat="server" CssClass="form-control myLogin" MaxLength="50"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfv_tb_LastName" runat="server" Display="Dynamic"
                                ControlToValidate="tb_LastName" ValidationGroup="Add"><div class="alert alert-danger"><%=this.GetLocalResourceObject("tip_Require").ToString()%></div></asp:RequiredFieldValidator>
                        </div>
                        <label class="col-md-2 text-right"><%=this.GetLocalResourceObject("txt_姓").ToString()%></label>
                        <div class="col-md-4 has-error">
                            <asp:TextBox ID="tb_FirstName" runat="server" CssClass="form-control myLogin" MaxLength="50"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfv_tb_FirstName" runat="server" Display="Dynamic"
                                ControlToValidate="tb_FirstName" ValidationGroup="Add"><div class="alert alert-danger"><%=this.GetLocalResourceObject("tip_Require").ToString()%></div></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-md-2 text-right"><%=this.GetLocalResourceObject("txt_性別").ToString()%></label>
                        <div class="col-md-10">
                            <asp:RadioButtonList ID="rbl_Sex" runat="server" RepeatDirection="Horizontal">
                                <asp:ListItem Value="1">&nbsp;Male&nbsp;&nbsp;&nbsp;</asp:ListItem>
                                <asp:ListItem Value="2">&nbsp;Female</asp:ListItem>
                            </asp:RadioButtonList>
                            <asp:RequiredFieldValidator ID="rfv_rbl_Sex" runat="server" Display="Dynamic"
                                ControlToValidate="rbl_Sex" ValidationGroup="Add"><div class="alert alert-danger"><%=this.GetLocalResourceObject("tip_Require").ToString()%></div></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-md-2 text-right"><%=this.GetLocalResourceObject("txt_生日").ToString()%></label>
                        <div class="col-md-10 form-inline">
                            <div class="input-group date showDate" data-link-field="MainContent_tb_Birthday">
                                <asp:TextBox ID="show_sDate" runat="server" CssClass="form-control text-center myLogin" ReadOnly="true"></asp:TextBox>
                                <span class="input-group-addon"><span class="glyphicon glyphicon-remove"></span></span>
                                <span class="input-group-addon"><span class="glyphicon glyphicon-calendar"></span></span>
                            </div>
                            <asp:TextBox ID="tb_Birthday" runat="server" Style="display: none"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-md-2 text-right"><%=this.GetLocalResourceObject("txt_國家").ToString()%></label>
                        <div class="col-md-10 form-inline has-error">
                            <asp:DropDownList ID="ddl_AreaCode" runat="server" CssClass="form-control myLogin"></asp:DropDownList>
                            <select id="ddl_Country" class="form-control myLogin"></select>
                            <asp:TextBox ID="tb_DataValue" runat="server" Style="display: none;"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfv_tb_DataValue" runat="server" Display="Dynamic"
                                ControlToValidate="tb_DataValue" ValidationGroup="Add"><div class="alert alert-danger"><%=this.GetLocalResourceObject("tip_Require").ToString()%></div></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-md-2 text-right"><%=this.GetLocalResourceObject("txt_公司").ToString()%></label>
                        <div class="col-md-10">
                            <asp:TextBox ID="tb_Company" runat="server" CssClass="form-control myLogin" MaxLength="80"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-md-2 text-right"><%=this.GetLocalResourceObject("txt_地址").ToString()%></label>
                        <div class="col-md-10">
                            <asp:TextBox ID="tb_Address" runat="server" CssClass="form-control myLogin" MaxLength="150"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-md-2 text-right"><%=this.GetLocalResourceObject("txt_電話").ToString()%></label>
                        <div class="col-md-10">
                            <asp:TextBox ID="tb_Tel" runat="server" CssClass="form-control myLogin" MaxLength="30"></asp:TextBox>
                            <asp:CompareValidator ID="cv_tb_Tel" runat="server" ControlToValidate="tb_Tel"
                                Display="Dynamic" Operator="DataTypeCheck" Type="Integer" ValidationGroup="Add"><div class="alert alert-danger"><%=this.GetLocalResourceObject("tip_Format").ToString()%></div></asp:CompareValidator>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-md-2 text-right"><%=this.GetLocalResourceObject("txt_手機").ToString()%></label>
                        <div class="col-md-10">
                            <asp:TextBox ID="tb_Mobile" runat="server" CssClass="form-control myLogin" MaxLength="30"></asp:TextBox>
                            <asp:CompareValidator ID="cv_tb_Mobile" runat="server" ControlToValidate="tb_Mobile"
                                Display="Dynamic" Operator="DataTypeCheck" Type="Integer" ValidationGroup="Add"><div class="alert alert-danger"><%=this.GetLocalResourceObject("tip_Format").ToString()%></div></asp:CompareValidator>
                            <%-- <asp:CustomValidator ID="cv_CheckPhone" runat="server" ClientValidationFunction="ClientValidate_Phone" Display="Dynamic" ValidationGroup="Add">
                                <div class="alert alert-danger"><%=this.GetLocalResourceObject("txt_電話").ToString()%>/<%=this.GetLocalResourceObject("txt_手機").ToString()%>,<%=this.GetLocalResourceObject("tip_Require").ToString()%></div>
                            </asp:CustomValidator>--%>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-md-2 text-right"><%=this.GetLocalResourceObject("txt_qq").ToString()%></label>
                        <div class="col-md-10">
                            <asp:TextBox ID="tb_IM_QQ" runat="server" CssClass="form-control myLogin" MaxLength="100"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-md-2 text-right"><%=this.GetLocalResourceObject("txt_wechat").ToString()%></label>
                        <div class="col-md-10">
                            <asp:TextBox ID="tb_IM_Wechat" runat="server" CssClass="form-control myLogin" MaxLength="100"></asp:TextBox>
                        </div>
                    </div>
                    <div class="text-right">
                        <asp:Button ID="btn_Submit" runat="server" CssClass="btn btn-green-block btn-commit" OnClick="btn_Submit_Click" OnClientClick="blockBox1('Add','Processing...')" ValidationGroup="Add" />
                    </div>
                </div>
            </div>

            <div class="login-area dealer-box" style="padding: 10px;">
                <div class="page-title">
                    <div class="header"><%=this.GetLocalResourceObject("txt_加入經銷商").ToString()%></div>
                </div>
                <%=this.GetLocalResourceObject("txt_加入經銷商說明").ToString()%>

                <div class="text-right">
                    <a href="<%=Application["WebUrl"] %>DealerApply" class="btn btn-green-block"><%=this.GetLocalResourceObject("txt_我想成為經銷商").ToString()%></a>
                    <a href="#" class="btn btn-orange" data-toggle="modal" data-target="#modal_Dealer"><%=this.GetLocalResourceObject("txt_我已經是經銷商").ToString()%></a>

                </div>
            </div>

            <!-- 已是經銷商Modal Start -->
            <div id="modal_Dealer" class="modal fade" tabindex="-1" role="dialog" style="display: none;">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">×</span></button>
                            <h4 class="modal-title"><%=this.GetLocalResourceObject("txt_確認公司名表頭").ToString()%></h4>
                        </div>
                        <div class="modal-body">
                            <p><%=this.GetLocalResourceObject("txt_確認公司名說明").ToString()%></p>

                            <div class="form-group">
                                <label for="MainContent_tb_cfm_Company" class="control-label"><%=this.GetLocalResourceObject("txt_公司").ToString()%></label>
                                <asp:TextBox ID="tb_cfm_Company" runat="server" CssClass="form-control" MaxLength="80"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfv_tb_cfm_Company" runat="server" Display="Dynamic"
                                    ControlToValidate="tb_cfm_Company" ValidationGroup="join"><div class="alert alert-danger"><%=this.GetLocalResourceObject("tip_Require").ToString()%></div></asp:RequiredFieldValidator>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
                            <asp:Button ID="btn_Join" runat="server" CssClass="btn btn-primary" OnClick="btn_Join_Click" OnClientClick="blockBox1('join','Processing...')" ValidationGroup="join" />
                        </div>
                    </div>
                </div>
            </div>
            <!-- 已是經銷商Modal End -->
        </div>

    </div>
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
    <%: Scripts.Render("~/bundles/blockUI-script") %>
    <script>
        $(function () {
            /* 偵測enter */
            $(".myLogin").keypress(function (e) {
                code = (e.keyCode ? e.keyCode : e.which);
                if (code == 13) {
                    $("#MainContent_btn_Submit").trigger("click");
                }
            });
        });

        /* 電話/手機擇一必填 */
        //function ClientValidate_Phone(sender, args) {
        //    var objPhone = document.getElementById("MainContent_tb_Tel");
        //    var objCellPhone = document.getElementById("MainContent_tb_Mobile");
        //    if (objPhone.value == '' && objCellPhone.value == '') {
        //        args.IsValid = false;
        //    }
        //    else {
        //        args.IsValid = true;
        //    }

        //}
    </script>
    <%-- DatePicker Start --%>
    <%: Scripts.Render("~/bundles/DTpicker-script") %>
    <script>
        $(function () {
            $('.showDate').datetimepicker({
                format: 'yyyy/mm/dd',   //目前欄位格式
                linkFormat: 'yyyy/mm/dd',   //鏡像欄位格式
                todayBtn: false,     //顯示today
                todayHighlight: false,   //將today設置高亮
                autoclose: true,    //選擇完畢後自動關閉
                startView: 4,    //選擇器開啟後，顯示的視圖(4:10年 ; 3:12月 ; 2:該月 ; 1:該日全時段 ; 0:該時段的各個時間,預設5分間隔)
                maxView: 4,
                minView: 2,
                forceParse: false
            });

        });

    </script>
    <%-- DatePicker End --%>
    <script type="text/javascript">
        /* ----- Init Start ----- */
        $(function () {

            //宣告
            var menu_AreaCode = 'select#MainContent_ddl_AreaCode';
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
                $("#MainContent_tb_DataValue").val(GetVal);

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
            var uri = '<%=Application["API_WebUrl"] %>place/countries/<%=fn_Language.PKWeb_Lang%>/?AreaCode=' + AreaCode + '&showAll=Y';

            // Send an AJAX request
            $.getJSON(uri)
                .done(function (data) {
                    //清空選項
                    myMenu.empty();

                    //加入選項
                    myMenu.append($('<option></option>').val('').text('-- Country --'));
                    $.each(data, function (key, item) {
                        myMenu.append($('<option></option>').val(item.CountryCode).text(item.CountryName + ' (' + item.CountryCode + ')'))
                    });

                    //取得值並設為selected
                    var getVal = $("#MainContent_tb_DataValue").val();
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
            menuID.append($('<option></option>').val('').text('-- Country --'));

        }
        /* 取得洲別國家 - 連動選單 End */
    </script>
</asp:Content>

