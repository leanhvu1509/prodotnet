$(document).ready(function (){
    $('.bannerslide img:gt(0)').hide();             //ẩn tất cả các ảnh tu thu 2-4
    setInterval(function () //Setup thời gian chuyển hình ảnh bằng hàm setInterval
    {   
        $('.bannerslide :first-child').slideDown() //FadeOut là ảnh đang hiện
            .next('img').slideDown() //fadeIn ảnh tiếp theo
            .end().appendTo('.bannerslide');        // chuyển vị trí ảnh xuống cuối
    }, 3000);//thoi gian chuyen
});