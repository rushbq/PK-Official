<%@ Page Title="產品資料維護" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Edit.aspx.cs" Inherits="Prod_Edit" ValidateRequest="false" %>

<%@ Import Namespace="ExtensionMethods" %>
<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
    <%: Styles.Render("~/bundles/JQ-UI-css") %>
    <%: Styles.Render("~/bundles/DTpicker-css") %>
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- Page Header Start -->
    <div class="row">
        <div class="col-md-12">
            <ol class="breadcrumb">
                <li><a href="<%=Application["WebUrl"] %>">首頁</a></li>
                <li><a href="<%=Application["WebUrl"] %>Prod/Search/">產品資料查詢</a></li>
                <li class="active">產品資料維護</li>
            </ol>
        </div>
    </div>
    <!-- Page Header End -->
    <!-- Form Start -->
    <div class="row">
        <div class="col-sm-9 col-md-10">
            <div class="panel panel-info">
                <div class="panel-heading">
                    <div class="pull-left">
                        <span class="glyphicon glyphicon-edit"></span>
                        <span>基本設定</span>
                    </div>
                    <div class="pull-right">
                    </div>
                    <div class="clearfix"></div>
                </div>
                <div id="data" class="collapse in">
                    <div class="panel-body">
                        <!-- Content Start -->
                        <div class="form-horizontal">
                            <div class="form-group">
                                <label class="control-label col-sm-3 col-md-3 col-lg-2">資料編號</label>
                                <div class="col-sm-3 col-md-3 col-lg-4">
                                    <p class="form-control-static">
                                        <strong>
                                            <asp:Label ID="lb_DataID" runat="server" Text="系統自動編號" CssClass="styleGreen"></asp:Label></strong>
                                    </p>
                                </div>
                                <label class="control-label col-sm-3 col-md-3 col-lg-2">目錄 / 頁次</label>
                                <div class="control-label col-sm-3 col-md-3 col-lg-4">
                                    <asp:Label ID="lb_Vol" runat="server"></asp:Label>&nbsp;/&nbsp;
                                    <asp:Label ID="lb_Page" runat="server"></asp:Label>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="control-label col-sm-3 col-md-3 col-lg-2" for="MainContent_tb_Rpt_Folder">上架區域(語系) <em>*</em></label>
                                <div class="col-sm-3 col-md-3 col-lg-4">
                                    <asp:CheckBoxList ID="cbl_Area" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:CheckBoxList>
                                    <asp:CustomValidator ID="cv_check_Area" runat="server" ErrorMessage="請選擇「上架區域」" Display="Dynamic"
                                        ClientValidationFunction="check_AreaCode" ValidationGroup="Add" CssClass="styleRed help-block"></asp:CustomValidator>
                                </div>
                                <label class="control-label col-sm-3 col-md-3 col-lg-2" for="MainContent_tb_Rpt_Folder">顯示未開賣</label>
                                <div class="col-sm-3 col-md-3 col-lg-4">
                                    <asp:CheckBoxList ID="cbl_NoSellArea" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:CheckBoxList>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="control-label col-xs-12 col-sm-3 col-md-3 col-lg-2">品號 <em>*</em></label>
                                <div class="col-xs-7 col-sm-6 col-md-6 col-lg-8">
                                    <asp:TextBox ID="tb_Model_No" runat="server" CssClass="form-control tip" placeholder="輸入品號關鍵字"></asp:TextBox>
                                    <asp:HiddenField ID="hf_myItemVal" runat="server" />

                                    <asp:RequiredFieldValidator ID="rfv_tb_Model_No" runat="server" ErrorMessage="請填寫「品號」" ControlToValidate="tb_Model_No" Display="Dynamic" ValidationGroup="Add" CssClass="styleRed help-block"></asp:RequiredFieldValidator>
                                </div>
                                <div class="col-xs-5 col-sm-3 col-md-3 col-lg-2">
                                    <asp:Button ID="btn_GetIcon" runat="server" Text="帶出認證符號" CssClass="btn btn-warning" OnClick="btn_GetIcon_Click" />
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="control-label col-sm-3 col-md-3 col-lg-2">上架日期 <em>*</em></label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <div class="form-inline">
                                        <div class="input-group date showDate" data-link-field="MainContent_tb_StartDate">
                                            <asp:TextBox ID="show_sDate" runat="server" CssClass="form-control text-center" ReadOnly="true"></asp:TextBox>
                                            <span class="input-group-addon"><span class="glyphicon glyphicon-remove"></span></span>
                                            <span class="input-group-addon"><span class="glyphicon glyphicon-time"></span></span>
                                        </div>
                                        ~
                                        <div class="input-group date showDate" data-link-field="MainContent_tb_EndDate">
                                            <asp:TextBox ID="show_eDate" runat="server" CssClass="form-control text-center" ReadOnly="true"></asp:TextBox>
                                            <span class="input-group-addon"><span class="glyphicon glyphicon-remove"></span></span>
                                            <span class="input-group-addon"><span class="glyphicon glyphicon-time"></span></span>
                                        </div>
                                    </div>
                                    <div class="help-block">
                                        (若結束時間空白,自動填入開始時間 +5 年)
                                    </div>
                                    <div>
                                        <asp:RequiredFieldValidator ID="rfv_sDate" runat="server" ErrorMessage="請選擇「開始時間」" ControlToValidate="tb_StartDate" Display="Dynamic" ValidationGroup="Add" CssClass="styleRed help-block"></asp:RequiredFieldValidator>
                                        <asp:TextBox ID="tb_StartDate" runat="server" Style="display: none"></asp:TextBox>
                                        <asp:TextBox ID="tb_EndDate" runat="server" Style="display: none"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="control-label col-sm-3 col-md-3 col-lg-2">上架狀態 <em>*</em></label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <div class="showRadioGrp">
                                        <asp:RadioButtonList ID="rbl_Display" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:RadioButtonList>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="control-label col-sm-3 col-md-3 col-lg-2" for="MainContent_ddl_Status">產品標記 <em>*</em></label>
                                <div class="col-sm-3 col-md-3 col-lg-4">
                                    <div class="form-inline">
                                        <div class="showRadioGrp">
                                            <asp:RadioButtonList ID="rbl_IsNew" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                                <asp:ListItem Value="Z">熱銷推薦</asp:ListItem>
                                                <asp:ListItem Value="Y">新品專區</asp:ListItem>
                                                <asp:ListItem Value="N" Selected="True">一般品</asp:ListItem>
                                            </asp:RadioButtonList>
                                        </div>
                                    </div>
                                </div>
                                <label class="control-label col-sm-3 col-md-3 col-lg-2" for="MainContent_tb_Sort">自訂排序 <em>*</em></label>
                                <div class="col-sm-3 col-md-3 col-lg-4">
                                    <asp:TextBox ID="tb_Sort" runat="server" MaxLength="3" CssClass="form-control" placeholder="自訂排序" Width="70px" Style="text-align: center;">999</asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfv_tb_Sort" runat="server" ErrorMessage="請輸入「自訂排序」"
                                        Display="Dynamic" ControlToValidate="tb_Sort" ValidationGroup="Add" CssClass="styleRed help-block"></asp:RequiredFieldValidator>
                                    <asp:RangeValidator ID="rv_tb_Sort" runat="server" ErrorMessage="請輸入1 ~ 999 的數字"
                                        Display="Dynamic" Type="Integer" MaximumValue="999" MinimumValue="1" ControlToValidate="tb_Sort"
                                        ValidationGroup="Add" CssClass="styleRed help-block"></asp:RangeValidator>
                                </div>
                                <div class="help-block">
                                    (順序為：熱銷 -> 新品 -> 自訂排序 -> 上架日期, <code>數字小的優先</code>)
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="control-label col-sm-3 col-md-3 col-lg-2">認證符號</label>
                                <div class="col-sm-9 col-md-9 col-lg-10 table-responsive">
                                    <asp:CheckBoxList ID="cbl_CertIcon" runat="server" RepeatDirection="Horizontal" RepeatLayout="Table" RepeatColumns="5" CssClass="table"></asp:CheckBoxList>
                                </div>
                            </div>
                            <asp:PlaceHolder ID="ph_TagInfo" runat="server">
                                <div class="form-group">
                                    <label class="control-label col-sm-3 col-md-3 col-lg-2">
                                        Tag設定
                                    </label>
                                    <div class="col-sm-9 col-md-9 col-lg-10">
                                        <div class="alert alert-danger">
                                            資料存檔後才能設定Tag
                                        </div>
                                    </div>
                                </div>
                            </asp:PlaceHolder>
                            <!-- 新增後才帶出 -->
                            <asp:PlaceHolder ID="ph_TagsList" runat="server" Visible="false">
                                <div class="form-group">
                                    <label class="control-label col-sm-3 col-md-3 col-lg-2">
                                        Tag設定
                                    </label>
                                    <div class="col-sm-9 col-md-9 col-lg-10">
                                        <div class="well well-sm">
                                            1. <b>如何建立新的Tag</b>：填完新的名稱後, 按下「Create New」<br />
                                            2. <b>如何加入已存在的Tag</b>：輸入關鍵字, 選擇下拉清單的項目, 資料會直接更新, 不需按存檔。
                                        </div>
                                        <div class="bq-callout grey">
                                            <h4 class="form-inline">建立新Tag&nbsp;
                                             <input id="newTag" class="form-control" placeholder="填寫Tag名稱" />
                                                <a href="javascript:;" class="btn btn-warning" id="btn_newTag"><span class="glyphicon glyphicon-tags"></span>&nbsp;&nbsp;Create New</a>
                                            </h4>
                                            <div id="block_newTag" class="alert alert-success alert-dismissible" role="alert" style="display: none;">
                                                <button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                                                <strong>建立完成!</strong> 現在可以開始取用.
                                            </div>
                                        </div>
                                        <div class="bq-callout blue">
                                            <h4 class="form-inline">繁中&nbsp;<input class="form-control taglist" placeholder="輸入關鍵字後選取項目" data-lang="zh-tw" data-ctrl="twTags" /></h4>
                                            <div class="row">
                                                <div class="col-sm-12">
                                                    <ul class="list-inline" id="twTags">
                                                    </ul>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="bq-callout green">
                                            <h4 class="form-inline">英文&nbsp;<input class="form-control taglist" placeholder="輸入關鍵字後選取項目" data-lang="en-us" data-ctrl="enTags" /></h4>
                                            <div class="row">
                                                <div class="col-sm-12">
                                                    <ul class="list-inline" id="enTags">
                                                    </ul>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="bq-callout red">
                                            <h4 class="form-inline">簡中&nbsp;<input class="form-control taglist" placeholder="輸入關鍵字後選取項目" data-lang="zh-cn" data-ctrl="cnTags" /></h4>
                                            <div class="row">
                                                <div class="col-sm-12">
                                                    <ul class="list-inline" id="cnTags">
                                                    </ul>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </asp:PlaceHolder>

                            <div class="form-group">
                                <div class="col-sm-12 text-right">
                                    <asp:Literal ID="lt_CreateInfo" runat="server"><p class="form-control-static help-block">資料新增中...</p></asp:Literal>
                                    <asp:Literal ID="lt_UpdateInfo" runat="server"></asp:Literal>
                                </div>

                            </div>

                            <asp:HiddenField ID="hf_flag" runat="server" Value="Add" />
                            <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="Add" ShowMessageBox="true" ShowSummary="false" />
                        </div>
                        <!-- Content End -->
                    </div>
                </div>
            </div>
        </div>
        <!-- buttons -->
        <div class="col-sm-3 col-md-2">
            <div class="FormBtn-fixPos">
                <div class="metro-nav metro-fix-view">
                    <div class="metro-nav-block nav-block-green">
                        <a href="<%=Page_SearchUrl %>" class="text-center">
                            <i class="fa fa-list"></i>
                            <div class="status">返回列表</div>
                        </a>
                    </div>
                    <asp:PlaceHolder ID="ph_Save" runat="server">
                        <div class="metro-nav-block nav-block-blue">
                            <a id="triggerSave" class="text-center" onclick="Get_Item();">
                                <i class="fa fa-floppy-o"></i>
                                <div class="status">資料存檔</div>
                            </a>
                        </div>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="ph_Delete" runat="server" Visible="false">
                        <%-- <div class="metro-nav-block nav-block-red">
                            <asp:LinkButton ID="lbtn_Delete" runat="server" CssClass="text-center" CausesValidation="false" OnClientClick="return confirm('是否確定刪除?')" OnClick="lbtn_Delete_Click">
                            <i class="glyphicon glyphicon-trash"></i>
                            <div class="status">刪除資料</div>
                            </asp:LinkButton>
                        </div>--%>

                        <div class="metro-nav-block nav-block-orange">
                            <a href="<%=Application["WebUrl"] %>Prod/Edit/" class="text-center">
                                <i class="fa fa-pencil-square-o"></i>
                                <div class="status">新增下一筆</div>
                            </a>
                        </div>
                    </asp:PlaceHolder>
                </div>
            </div>
            <div style="display: none;">
                <asp:HiddenField ID="hf_ModelNo" runat="server" />
                <asp:Button ID="btn_doSave" runat="server" Text="Save" OnClick="btn_Save_Click" ValidationGroup="Add" Style="display: none;" />
            </div>
        </div>
    </div>
    <!-- Form End -->
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
    <script>
        //取得資料編號(這項宣告要放在最前方)
        var dataID = '<%=Param_thisID%>';

    </script>
    <%: Scripts.Render("~/bundles/JQ-UI-script") %>
    <%: Scripts.Render("~/bundles/blockUI-script") %>
    <%: Scripts.Render("~/bundles/group-base") %>
    <%: Scripts.Render("~/bundles/DTpicker-script") %>
    <%: Scripts.Render("~/bundles/group-datepicker") %>

    <%-- Autocompelete 品號 Start --%>
    <script>
        $("#MainContent_tb_Model_No").catcomplete({
            minLength: 1,  //至少要輸入 n 個字元
            source: function (request, response) {
                $.ajax({
                    url: "<%=Application["WebUrl"]%>Ajax_Data/AC_ModelNo.aspx",
                    data: {
                        q: request.term
                    },
                    type: "POST",
                    dataType: "json",
                    success: function (data) {
                        if (data != null) {
                            response($.map(data, function (item) {
                                return {
                                    label: item.label,
                                    category: item.category,
                                    value: item.label,
                                    id: item.id
                                }
                            }));
                        }
                    }
                });
            },
            select: function (event, ui) {
                $("#MainContent_tb_Model_No").val(ui.item.label);
                $("#MainContent_hf_myItemVal").val(ui.item.id);

                //觸發Click - 帶出認證符號
                $('#MainContent_btn_GetIcon').trigger('click');

            }
        });

    </script>
    <%-- Autocompelete 品號 End --%>

    <%-- Tag 維護 --%>
    <asp:PlaceHolder ID="ph_TagsScript" runat="server" Visible="false">

        <script>
            //public param
            var valModel = $("#MainContent_hf_ModelNo").val();

            /* Click事件 - New Tag */
            $("#btn_newTag").click(function () {
                var _nameEle = $("#newTag");

                CreateTag(_nameEle.val());

                _nameEle.val('');
            });

            $("#newTag").keypress(function (e) {
                code = (e.keyCode ? e.keyCode : e.which);
                if (code == 13) {
                    $("#btn_newTag").trigger("click");
                }
            });

            $(function () {
                //init
                GetTagList("zh-TW", "twTags");
                GetTagList("en-US", "enTags");
                GetTagList("zh-CN", "cnTags");

            });

            //取得Tag List
            function GetTagList(_lang, _ele) {
                //[Ajax return], 取得資料
                var _data = fn_TagData(valModel, "", _lang, "READ");

                //[Ajax done]
                _data.done(function (callback) {
                    $("#" + _ele).empty().append(callback);
                });

                //[Ajax fail]
                _data.fail(function (jqXHR, textStatus) {
                    event.preventDefault();
                    alert('取資料時發生錯誤 (GetTag)');
                });
            }


            function CreateTag(_name) {
                //check null
                if (_name == '') {
                    event.preventDefault();
                    alert('不可輸入空值!');
                    return false;
                }
                var _data = fn_TagData("", _name, "", "CREATE");

                //[Ajax done]
                _data.done(function (callback) {
                    if (callback == "success") {
                        $("#block_newTag").fadeIn(100).fadeOut(5000);
                    } else {
                        alert('建立失敗');
                        console.log(callback);
                    }
                });

                //[Ajax fail]
                _data.fail(function (jqXHR, textStatus) {
                    event.preventDefault();
                    alert('取資料時發生錯誤 (CreateTag)');
                });
            }


            function UpdateTag(_id, _lang, _ele) {
                //[Ajax return]
                var _data = fn_TagData(_id, "", _lang, "UPDATE");

                //[Ajax done]
                _data.done(function (callback) {
                    if (callback == "success") {
                        GetTagList(_lang, _ele);
                    } else {
                        alert('更新失敗');
                        console.log(callback);
                    }
                });

                //[Ajax fail]
                _data.fail(function (jqXHR, textStatus) {
                    event.preventDefault();
                    alert('取資料時發生錯誤 (UpdateTag)');
                });
            }

            function DelTag(_id, _lang, _ele) {
                //[Ajax return]
                var _data = fn_TagData(_id, "", _lang, "DELETE");

                //[Ajax done]
                _data.done(function (callback) {
                    if (callback == "success") {
                        GetTagList(_lang, _ele);
                    } else {
                        alert('更新失敗');
                        console.log(callback);
                    }
                });

                //[Ajax fail]
                _data.fail(function (jqXHR, textStatus) {
                    event.preventDefault();
                    alert('取資料時發生錯誤 (DelTag)');
                });
            }

            //[Function] Ajax 主體
            // id = tagID
            // name = tagName
            // lang = Language
            // type = READ, CREATE, UPDATE, DELETE
            function fn_TagData(_id, _name, _lang, _type) {
                var request = $.ajax({
                    url: '<%=fn_Param.Web_Url%>' + "myProd/Ashx_TagsAction.ashx",
                    method: "POST",
                    data: {
                        model: valModel, //public param
                        id: _id,
                        name: _name,
                        lang: _lang,
                        type: _type
                    },
                    dataType: "html"
                });

                return request;
            }
        </script>
        <%-- Autocompelete Tag Start --%>
        <script>
            $(".taglist").autocomplete({
                minLength: 1,  //至少要輸入 n 個字元
                source: function (request, response) {
                    $.ajax({
                        url: "<%=Application["WebUrl"]%>Ajax_Data/AC_ProdTags.aspx",
                        data: {
                            q: request.term
                        },
                        type: "POST",
                        dataType: "json",
                        success: function (data) {
                            if (data != null) {
                                response($.map(data, function (item) {
                                    return {
                                        label: item.label,
                                        value: item.label,
                                        id: item.id
                                    }
                                }));
                            }
                        }
                    });
                },
                select: function (event, ui) {
                    var lang = $(this).attr("data-lang");
                    var _ele = $(this).attr("data-ctrl");
                    var id = ui.item.id;
                    //call update
                    UpdateTag(id, lang, _ele);

                    //清除輸入欄
                    $(this).val("");
                    event.preventDefault();
                }
            });

        </script>
        <%-- Autocompelete Tag End --%>
    </asp:PlaceHolder>

</asp:Content>

