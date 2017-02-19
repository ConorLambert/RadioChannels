﻿var api_key = "sJXVu3hXGmKjJiIx";
var channels = [];
var offset = 0;
var current_genre = undefined;
var current_channel = undefined;
var current_volume = 1.0;
var oldx = 0;
var newx = 0;
var total_base_genres = 0;
var player_displayed = false;

$(document).ready(function () {
    initializePlayer();   

    if (window.location.href.includes("favourites"))
        triggerFavouritesGenreSelect();
    else
        triggerGenreSelect();
    triggerMouseOverIcon();


    // VOLUME
    $(".volumeBar").click(function (e) {
        var x = e.pageX - $(this).offset().left;
        var width = parseFloat($(".volumeBar").width());
        var new_volume = ((x / width) * 100);
        // $(".audio-player-volume-bar").css({ width: new_volume + "%" });
        current_volume = new_volume / 100;
        //$("#jplayer").jPlayer("volume", current_volume);
        setVolume();
    });

    $(".volume-down").click(function () {
        current_volume = current_volume - 0.05 > 0 ? (current_volume - 0.05) : 0;
        setVolume();
    });

    $(".volume-up").click(function () {
        current_volume = current_volume + 0.05 < 1.0 ? (current_volume + 0.05) : 1.0;
        setVolume();
    });

    $(".volume-chooser").on("input", ".volume", function (e) {
        volume = $(e.currentTarget).val();
        current_volume = volume / 100;
        setVolume();
    });

    var imgWidth = $('.regular-image').width();
    $('.artist').width(imgWidth);

    $("#navigation a").on("click", function () {
        $("#navigation li").removeClass("nav-clicked"); // Remove all highlights
        $(this).parent('li').addClass("nav-clicked"); // Add the class only for actually clicked element
    });
});


// RADIAL MENU

function initializeRadialMenu() {
    var buttons = document.querySelectorAll(".radmenu a");

    for (var i = 0, l = buttons.length; i < l; i++) {
        var button = buttons[i];
        button.onclick = setSelected;
    }

    total_base_genres = $("#categories").find(".general-genre-container").length;

    function setSelected(e) {

        var target = e.target || e.srcElement;

        if (!this.classList.contains("selected") && $(target).text() !== "Genres" && current_genre !== $(target).text().toLowerCase()) {
              // remove the current list of stations if any
            current_genre = $(target).text().toLowerCase();
            if (window.location.href.includes("favourites")) {
                getFavouritesOf(current_genre);
                $("#categories li").removeClass("clicked"); // Remove all highlights
                $(target).parent('li').addClass("clicked");
                triggerMouseOverIcon();
            } else {
                $('#stations > div').empty();
                channels_ajax_request(0);
                triggerScrollEvent();
                if (!window.location.href.includes(encodeURIComponent(current_genre)))  // if we have moved back to this page then, dont push it
                    history.pushState(current_genre, null, "/#index/" + current_genre);
            }
        }
        
        // push the current state onto the history (URL should append the current genre as a hashbang)
        /*
        if (!window.location.href.includes(encodeURIComponent(current_genre)))  // if we have moved back to this page then, dont push it
            history.pushState(current_genre, null, "/#index/" + current_genre);
        */
        // Add the class only for actually clicked element
        
        if ($(target).parent('li').hasClass("concrete-genre")) {
            // list channels
        } else {

            if ($(target).text() !== "Genres" && !this.parentNode.classList.contains("radmenu")) {
                $(target).parent("li").addClass("clicked");
            }

            if (this.classList.contains("selected")) {  // if centre menu item selected and has already been selected go up
                this.classList.remove("selected");
                if ($(target).text() !== "Genres" || !this.parentNode.classList.contains("radmenu")) {
                    // detach this element from categories and place it back to where it orignally came from
                    // we can achieve this by finding the li element which has the clicekd class attached to it and append it there
                    var ul_to_detach = $(target).next("ul").detach();
                    var a_to_detach = $(target).detach();
                    $("#categories .clicked").append(a_to_detach[0]);
                    $("#categories .clicked").append(ul_to_detach[0]);
                    $("#categories .clicked").removeClass("clicked");
                    target = a_to_detach[0];
                    var genre_selector = $("#categories > a"); // this.parentNode.parentNode.parentNode.querySelector("a");
                    $(genre_selector).addClass("selected");
                    placeMenuItems($(genre_selector).next("ul"));
                } else {
                    this.classList.add("show");
                }
            } else {
                this.classList.add("selected");
                $(target).css({ 'transform': 'rotate(-' + 0 + 'deg)' });
                if (!this.parentNode.classList.contains("radmenu")) {
                    this.parentNode.parentNode.parentNode.querySelector("a").classList.remove("selected")
                } else {
                    this.classList.remove("show");
                }
                if ($(target).text() !== "Genres" && !this.parentNode.classList.contains("radmenu")) {
                    $("#categories").append($(target).parent("li").children().detach());
                    target = $("#categories > .selected");
                }
                if ($(target).next("ul").length > 0)
                    placeMenuItems($(target).next("ul"));
            }

            return false;
        }
    }
}

function placeMenuItems(e) {

    var rotate_offset = 0;
    var items = $(e).children("li");
    var translateX = 50;

    var degree = 360 / total_base_genres;

    if ($(e).parent(".general-genre-container").length > 0) {
        var obj = $(e).closest('.general-genre-container');
        $(obj).removeAttr("style");
        $(obj).children('a').removeAttr("style");
    }

    // for each li element 
    $(items).each(function (index, elem) {
        rotate(elem, degree * (index + 1), translateX);
    });

}

function rotate(elem, degree, translateX) {
    var starting_point = 270;
    $(elem).css({
        WebkitTransform: 'rotate(' + ((starting_point + degree) % 360) + 'deg), translateX(' + translateX + 'px)',
        '-moz-transform': 'rotate(' + ((starting_point + degree) % 360) + 'deg), translateX(' + translateX + 'px)',
        'transform': 'rotate(' + ((starting_point + degree) % 360) + 'deg) translateX(' + translateX + 'px)'
    });
    //$(elem).find("a").removeAttr('style');
    // a element within the menu item is rotated to the same degree as the menu item itself but negatively
    $(elem).find("a").css({ WebkitTransform: 'rotate(-' + ((starting_point + degree) % 360) + 'deg)' });
    $(elem).find("a").css({ '-moz-transform': 'rotate(-' + ((starting_point + degree) % 360) + 'deg)' });
    $(elem).find("a").css({ 'transform': 'rotate(-' + ((starting_point + degree) % 360) + 'deg)' });
}






function setVolume() {
    $("#jplayer").jPlayer("volume", current_volume);
    $("#mobile-volume").val(current_volume * 100);
    $(".audio-player-volume-bar").css({ width: (current_volume * 100) + "%" });
}


// EVENTS

function triggerGenreSelect() {
    findHeader();
    init();
    initializeRadialMenu();
    $("#categories").on("click", "a", function () {
        /*
        if ($(this).text() === "Genres")
            return;
        $('#stations > div').empty();  // remove the current list of stations if any
        current_genre = $(this).text().toLowerCase();
        channels_ajax_request(0);
        triggerScrollEvent();
        // push the current state onto the history (URL should append the current genre as a hashbang)
        if (!window.location.href.includes(encodeURIComponent(current_genre)))  // if we have moved back to this page then, dont push it
            history.pushState(current_genre, null, "/#index/" + current_genre);
        // Add the class only for actually clicked element
        */
    });
}

function triggerFavouritesGenreSelect() {
    findHeader();
    init();
    initializeRadialMenu();
    $("#categories").on("click", "a", function () {
        /*
        var genre = $(this).text();
        getFavouritesOf(genre);
        $("#categories li").removeClass("clicked"); // Remove all highlights
        $(this).parent('li').addClass("clicked");
        triggerMouseOverIcon();
        */
    });
}

function getFavouritesOf(genre) {
    $('#stations > div').detach();  // remove the current list of stations if any   
    $('#stations').append("<div class='container-fluid header-container'></div>");
    $(favourites).find(".row").each(function (index, elem) {
        if ($(elem).find(".genre").text().toLowerCase() === genre)
            $("#stations > div").append($(elem).clone());
    });
    if (!window.location.href.includes("/" + encodeURIComponent(genre)))  // if we have moved back to this page then, dont push it
        history.pushState(genre, null, "/#favourites/" + genre);
}

function initializeChannelItem(elem) {
    $(elem).removeClass("is-selected");
    $(elem).find(".label").removeClass("is-playing");
    if (!$(elem).find(".activity").hasClass('icon-headphones'))
        $(elem).find(".activity").addClass('icon-headphones');
    $(elem).find(".activity").removeClass('icon-play');
    $(elem).find(".activity").removeClass('icon-pause');
}

function triggerMouseOverIcon(elem) {
    if (elem == undefined) elem = ".activity";
    $(elem).on('mouseover', function () {
        $(this).toggleClass('icon-headphones icon-play');
    }).on('mouseout', function () {
        $(this).toggleClass('icon-headphones icon-play');
    });
    if (!$(elem).hasClass('icon-headphones')) $(elem).addClass('icon-headphones');
    $(elem).removeClass('icon-play');
}

function removeMouseOverIcon(elem) {
    $(elem).off('mouseover');
    $(elem).off('mouseout');
}



// PLAYER

function initializePlayer() {

    $('#jplayer').jPlayer({
        error: function (event) {
            console.log(event);
            stream.isPlaying = false;
            refreshPlayingStreamButtonState();
        },
        solution: 'html,flash',
        swfPath: '"https://cdnjs.cloudflare.com/ajax/libs/jplayer/2.9.2/jplayer/jquery.jplayer.swf',
        supplied: 'mp3,m4a,M3U,M3UA,FLA,WEBMA,WAV,OGA',
        preload: 'none',
        wmode: 'window',
        keyEnabled: true,
        volume: current_volume,
        ready: function () {

        },
        flashreset: function () { },
        error: function (e) {
            console.log(e);
        }
    });
}

// events

$("#jplayer").bind($.jPlayer.event.play, function (event) {
    $(".audio-player-button").removeClass("icon-play");
    $(".audio-player-button").addClass("icon-pause");
    $(".audio-player-progress").addClass("loading");
    // connect stream to waves canvas
});

$("#jplayer").bind($.jPlayer.event.pause, function (event) { // Add a listener to report the time play began
    $(".audio-player-button").removeClass("icon-pause");
    $(".audio-player-button").addClass("icon-play");
    // remove hover
});

$("#jplayer").bind($.jPlayer.event.error, function (event) { // Add a listener to report the time play began
    togglePlayStationControl(current_channel);  // reset control back to default
});

$("#jplayer").bind($.jPlayer.event.loadstart, function (event) {
    // progress bar    
    $(".audio-player-progress").addClass("loading");
});

$("#jplayer").bind($.jPlayer.event.playing, function (event) {
    // progress bar    
    $(".audio-player-progress").removeClass("loading");
    $("audio-player-progress-bar").css({ width: "25%" });
});


function tunein(channel, elem) {

    // if the player is not shown, then show the player
    if (!player_displayed) {
        $("footer").toggle(800);
        player_displayed = true;
    }

    // adjust scroll viewport            
    $('#stations').animate({
        scrollTop: $("#stations").scrollTop() + ($(elem).position().top - $("#stations").position().top) - ($("#stations").height() / 2) + ($(elem).height() / 2)
    }, 1000);

    togglePlayStationControl(elem);
    if (current_channel == elem) { // we have selected the channel that is currently selected              
        toggleStreaming();
    } else {
        if ($("#jplayer").data().jPlayer.status.paused == false)
            togglePlayStationControl(current_channel);
        if (current_channel !== undefined) {
            $(current_channel).closest(".row").removeClass("is-selected");
            triggerMouseOverIcon(current_channel);
            $(current_channel).closest(".row").find(".label").removeClass("is-playing");
        }
        current_channel = elem;
        $(current_channel).closest(".row").addClass("is-selected");
        removeMouseOverIcon(current_channel);
        $(current_channel).closest(".row").find(".label").addClass("is-playing");
        stream(channel);
        if (channel.Logo !== null) {
            $(".audio-player-image img").attr('src', channel.Logo + '?' + Math.random());
            $(".audio-player-image img").show();
        } else {
            $(".audio-player-image img").hide();
        }
        $('.audio-player-song-name').text($(elem).closest(".row").find(".track").text());
    }
}

function isPlaying(elem) {
    return $(elem).closest(".row").find(".channel-id").attr("id") === $(current_channel).closest(".row").find(".channel-id").attr("id");
}

function toggleStreaming() {
    if ($("#jplayer").data().jPlayer.status.paused == false) {
        $('#jplayer').jPlayer('pause');
    } else {
        $('#jplayer').jPlayer('play');
    }
}

function continueStream() {
    // continue streaming the currently playing channel
    togglePlayStationControl(current_channel);
    toggleStreaming();
}

// li channel item play button control
function togglePlayStationControl(item) {
    $(item).toggleClass("icon-pause icon-play");
}


function nextStation() {
    if (current_channel === undefined) return null;
    var channel_to_trigger;
    if ($(current_channel).closest(".row").next().length === 0)
        channel_to_trigger = $("#stations > div > .row:nth-child(1)").find(".activity"); // .trigger("onclick");
    else
        channel_to_trigger = $(current_channel).closest(".row").next().find(".activity"); // .trigger("onclick");
    // setup the icons to be swapped for tunein function 
    $(channel_to_trigger).removeClass("icon-headphones");
    $(channel_to_trigger).addClass("icon-play");
    $(channel_to_trigger).trigger("onclick");
}

function previousStation() {
    if (current_channel === undefined) return null;
    var channel_to_trigger;
    if ($(current_channel).closest(".row").prev().length === 0)
        channel_to_trigger = $("#stations > div > .row:nth-child(" + $('#stations > div > .row').length + ")").find(".activity");
    else
        channel_to_trigger = $(current_channel).closest(".row").prev().find(".activity");
    // setup the icons to be swapped for tunein function 
    $(channel_to_trigger).removeClass("icon-headphones");
    $(channel_to_trigger).addClass("icon-play");
    $(channel_to_trigger).trigger("onclick");
}


function stream(channel) {

    // HTTP "Content-Type" of "audio/aacp" is not supported
    /*
        var aac = station.AACEnabled && station.Format == 'audio/aacp';
        stream = station;
        stream.mp3 = aac ? url + '&type=.flv' : url;
    */

    var streams = [];
    channel.Playlist.split("\n").forEach(function (line) {
        if (line.trim().charAt(0) !== '#')
            streams.push({ "mp3": line.trim() + "/;" });
    });

    streams.every(function (stream) {
        try {
            $('#jplayer').jPlayer('setMedia', stream).jPlayer('play');
            return false;
        } catch (e) {
            console.log(e);
        }
    });

}


// SEARCH

function addMoreChannels() {
    var count = 0;
    if ($('#stations > div').children('.row') !== null)
        count = $('#stations > div > .row').length;
    channels_ajax_request(count);
}


function triggerScrollEvent() {
    jQuery(function ($) {
        $("#stations").on('scroll', function () {
            if ($(this).scrollTop() + $(this).innerHeight() >= $(this)[0].scrollHeight) {   // user has scrolled down the end of the current set of stations so append more stations
                addMoreChannels();
            }
        })
    });
}


// REFERESH

function refreshInfo(elem) {
    // get channel name and id 
    var channel_name = encodeURIComponent($(elem).closest(".row").find(".title").text());
    var id = $(elem).closest(".row").find(".channel-id").attr("id");

    $(elem).addClass("fa-rotate-90");

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
                track_name = result.currenttrack;
                $(elem).closest(".row").find(".track").text(track_name);
            }
        });
    });
}


function scrollInfo(elem) {

    // the larger the text, the longer the transition time must be and the wider between left and width must be
    if ($(elem)[0].scrollWidth > $(elem).innerWidth()) {
        var percent = ($(elem).innerWidth() / $(elem)[0].scrollWidth) * 100;
        var transition_time = percent / 10;
        $(elem).css({ "left": "-300%", "width": "400%" });
        $(elem).css({ "-webkit-transition": "left 3s, width 3s", "-moz - transition": "left 3s, width 3s", "transition": "left 8s, width 8s" });
    }
}





// AJAX REQUESTS

function channels_ajax_request(index) {
    jQuery(function ($) {
        $.ajax({
            type: "GET",
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            url: '/Home/IndexAsync/' + current_genre + '/' + index,
            cache: false,
            success: function (data) {
                var convert = $($.parseHTML(data));
                $('#stations > div').append(convert);
                $(convert).find('.activity').each(function (index, elem) {
                    // add scroll info event to each element
                    $(elem).closest(".row").find(".transitionable").each(function (index, elem) {
                        $(elem).on("mouseover", function (item) {
                            scrollInfo(elem);
                        })
                        $(elem).on("mouseleave", function (item) {
                            $(elem).css({ "left": "0%", "width": "100%" });
                        })
                    })
                    if (isPlaying(this)) {
                        $("#stations > div > div:eq(" + index + ")").replaceWith($(current_channel).closest(".row").clone());
                        current_channel = $("#stations > div > div:eq(" + index + ") .activity")[0];
                    } else {
                        triggerMouseOverIcon(elem);
                    }
                    if (isFavourite($(elem).closest(".row").find(".channel-id").attr("id"))) {
                        $(elem).closest(".row").find(".fav-btn").addClass("is-favourite");
                        $(elem).closest(".row").find(".fav-btn").removeClass("fav-btn");
                    }
                });
            }
        });
    });
}

function ajax_request(dest_url, callback) {
    jQuery(function ($) {
        $.ajax({
            type: "GET",
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            url: dest_url,
            cache: false,
            success: function (data) {
                $('#page').html(data);
                if (typeof callback === 'function' && callback()) { }
            }
        });
    });
}