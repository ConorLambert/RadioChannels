$(document).ready(function () {
    triggerMouseOverIcon();
});

function initializeChannelItem(elem) {
    $(elem).removeClass("is-selected");
    $(elem).find(".label").each(function (index, item) {
        $(item).removeClass("is-playing");
    });
    $(elem).find(".activity").each(function (index, item) {
        if (!$(item).hasClass('icon-headphones'))
            $(item).addClass('icon-headphones');
        $(item).removeClass('icon-play');
        $(item).removeClass('icon-pause');
        triggerMouseOverIcon(item);
    });
}

function triggerMouseOverIcon(elem) {   // elem refers to <span class="activity fa-icons" ...
    if (elem == undefined) elem = ".activity";
    $(elem).on('mouseover', function () {
        $(this).toggleClass('icon-headphones icon-play');
    }).on('mouseout', function () {
        $(this).toggleClass('icon-headphones icon-play');
    });
    if (!$(elem).hasClass('icon-headphones')) $(elem).addClass('icon-headphones');
    $(elem).removeClass('icon-play');
}

// executed when a channel is playing
function removeMouseOverIcon(elem) {
    $(elem).off('mouseover');
    $(elem).off('mouseout');
}

// when the user scrolls to the end of the page
function triggerScrollEvent() {
    jQuery(function ($) {
        $("#stations").on('scroll', function () {
            if ($(this).scrollTop() + $(this).innerHeight() + 2 >= $(this)[0].scrollHeight) {   // user has scrolled down the end of the current set of stations so append more stations
                addMoreChannels();
            }
        })
    });
}

function addMoreChannels() {
    var count = 0;
    if ($('#stations > div').children('.row') !== null)
        count = $('#stations > div > .row').length;
    channels_ajax_request(count);
}

function refreshInfo(elem) {
    // get channel name and id 
    var channel_name = encodeURIComponent($(elem).closest(".row").find(".channel-title").text());
    var id = $(elem).closest(".row").find(".channel-id").attr("id");   

    // get channel name
    var url = '/api/Channels/GetChannel/?id=' + id + '&name=' + channel_name;
    jQuery(function ($) {
        $.ajax({
            type: "GET",
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            url: url,
            cache: false,
            success: function (data) {
                var result = JSON.parse(data);
                $(elem).closest(".row").find(".track").text(result.currenttrack);
            }
        });
    });
}