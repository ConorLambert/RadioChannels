current_genre = undefined;
current_channel = undefined;

$(document).ready(function () {   
    resizeScreen();
    $(window).resize(resizeScreen);
    var imgWidth = $('.regular-image').width();
    $('.artist').width(imgWidth);    
});


function resizeScreen() {
    var deduct = $(".header-container").innerHeight() + parseInt($("#page").css("padding-top").replace("px", "")) - 5;
    audio_player_height = window.innerHeight - deduct;
    $("#page").css({ "height": (window.innerHeight) + "px", "max-width": window.innerWidth + "px" });
}

function page_ajax_request(dest_url, callback) {
    $('#page').empty();
    if ($("#page").find(".loading").length == 0)
        $("#page").append("<div class='loading'> <div class='loading__circle'></div> <div class='loading__circle'></div> <div class='loading__circle'></div> <div class='loading__circle'></div> <div class='loading__circle'></div> <div class='loading__circle'></div> <div class='loading__circle'></div> <div class='loading__circle'></div> <div class='loading__circle'></div> <div class='loading__circle'></div> <div class='loading__circle'></div> <div class='loading__circle'></div></div>");
    jQuery(function ($) {
        $.ajax({
            type: "GET",
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            url: dest_url,
            cache: false,
            success: function (data) {
                $("#page").remove(".loading");
                $('#page').html(data);
                if (typeof callback === 'function' && callback()) { }
            }
        });
    });
}


function channels_ajax_request(index) {
    if (!$(".page-message").hasClass("hidden"))
        $(".page-message").addClass("hidden");
    if ($("#page").find(".loading").length == 0)
        $("#stations > div").append("<div class='loading'> <div class='loading__circle'></div> <div class='loading__circle'></div> <div class='loading__circle'></div> <div class='loading__circle'></div> <div class='loading__circle'></div> <div class='loading__circle'></div> <div class='loading__circle'></div> <div class='loading__circle'></div> <div class='loading__circle'></div> <div class='loading__circle'></div> <div class='loading__circle'></div> <div class='loading__circle'></div></div>");
    jQuery(function ($) {
        $.ajax({
            type: "GET",
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            url: '/Home/IndexAsync/' + current_genre + '/' + index,
            cache: false,
            success: function (data) {
                var convert = $($.parseHTML(data));
                $(".loading").remove();
                $('#stations > div').append(convert);
                // set the overflow y-scroll
                $("#stations").addClass("set-overflow-y");
                $(convert).find('.activity').each(function (index, elem) {
                    triggerMouseOverIcon(elem);
                    if (isFavourite($(elem).closest(".row").find(".channel-id").attr("id"))) {
                        $(elem).closest(".row").find(".fav-btn").addClass("is-favourite");
                        $(elem).closest(".row").find(".fav-btn").removeClass("fav-btn");
                    }
                });
            }
        });
    });
}