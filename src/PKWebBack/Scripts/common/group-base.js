//判斷核取方塊選項(ClientValidate) - 區域
function check_AreaCode(sender, args) {
    var flagNum = 0;
    var optList = document.getElementById("MainContent_cbl_Area");
    var inArr = optList.getElementsByTagName('input');
    for (var i = 0; i < inArr.length; i++) {
        if (inArr[i].type == "checkbox") {
            if (inArr[i].checked == true) {
                flagNum += 1;
            }
        }
    }
    if (flagNum == 0) {
        args.IsValid = false;
    }
    else {
        args.IsValid = true;
    }
}

/* 頁面載入後 */
$(function () {
    //Click事件, 觸發儲存
    $("#triggerSave").click(function () {
        blockBox1('Add', '資料處理中...');
        $('#MainContent_btn_doSave').trigger('click');
    });

    /* tooltip */
    $('.tip').tooltip({
        html: true,
        //trigger: 'focus',  /* click | hover | focus */
        placement: 'bottom' /*  top | bottom | left | right | auto */
    });

    //使用jQueryUI 將radio Group
    $(".showRadioGrp").buttonset();

    //滑動到指定內容區 Start
    if (dataID != '') {
        //取得元素
        var _thisID = $('#data');

        //滑動至指定ID
        $('html, body').animate({
            scrollTop: $(_thisID).offset().top - 50
        }, 600);
    }
    //滑動到指定內容區 End

});


