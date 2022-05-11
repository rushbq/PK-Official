
$(function () {
    //點擊展開按鈕時
    $(".open-up").click(function () {
        //左側浮動選單-offset
        var menu = $(".list").offset();
        //左側浮動選單-width
        var menuWidth = $(".list").width();
        //左側浮動選單
        var menuContainer = $(".float-menu");
        //offset left
        var menuLeft = menu.left;

        //判斷選單最左邊的位置 < 0
        if (menuLeft < 0) {
            menuContainer.animate({
                left: 0 //位置移到左邊0的位置
            }, 400, 'swing'); //移動的速度&動畫
        } else {
            //斷選單最左邊的位置 > 0, 則執行以下動作
            menuContainer.animate({
                left: -(menuWidth + menuLeft - 50)
            }, 400 //動畫速度
                , 'swing' //動畫特效
            );
        }
    });

});