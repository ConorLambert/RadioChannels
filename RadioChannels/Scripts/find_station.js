var api_key = "sJXVu3hXGmKjJiIx";
var channels = [];
var offset = 0;
var current_genre = "";
var current_channel = undefined;
var current_volume = 1.0;
var oldx = 0;
var newx = 0;


$(document).ready(function () {        
    initializePlayer();
    triggerGenreSelect();

    // VOLUME
    $(".volumeBar").click(function (e) {
        var x = e.pageX - $(this).offset().left;
        var width = parseFloat($(".volumeBar").width());
        var new_volume = ((x / width) * 100);
        $(".audio-player-volume-bar").css({ width: new_volume + "%" });
        current_volume = new_volume / 100;
        $("#jplayer").jPlayer("volume", current_volume);
    });    

    $(".volume-down").click(function () {        
        current_volume = current_volume - 0.05 > 0 ? (current_volume - 0.05) : 0;
        $("#jplayer").jPlayer("volume", current_volume);
        $(".audio-player-volume-bar").css({ width: (current_volume * 100) + "%" });
    });

    $(".volume-up").click(function () {        
        current_volume = current_volume + 0.05 < 1.0 ? (current_volume + 0.05) : 1.0;
        $("#jplayer").jPlayer("volume", current_volume);
        $(".audio-player-volume-bar").css({ width: (current_volume * 100) + "%" });
    });

    var imgWidth = $('.regular-image').width();
    $('.artist').width(imgWidth);

    $("#navigation a").on("click", function () {
        $("#navigation li").removeClass("nav-clicked"); // Remove all highlights
        $(this).parent('li').addClass("nav-clicked"); // Add the class only for actually clicked element
    });
});


// EVENTS

function triggerGenreSelect() {
    $("#genres").on("click", "a", function () {
        $('#stations ul').empty();  // remove the current list of stations if any
        current_genre = $(this).text().toLowerCase();
        channels_ajax_request(0);
        triggerScrollEvent();
    });

    $("#categories a").on("click", function () {
        $("#categories li").removeClass("clicked"); // Remove all highlights
        $(this).parent('li').addClass("clicked"); // Add the class only for actually clicked element
    });
}

function triggerMouseOverIcon(elem) {
    if (elem == undefined) elem = ".activity";
    $(elem).on('mouseover', function () {
        $(this).toggleClass('icon-headphones icon-play');
    }).on('mouseout', function () {
        $(this).toggleClass('icon-headphones icon-play');
    });
    $(elem).addClass('icon-headphones');
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
        flashreset: function () {},
        error: function (e) {            
            console.log(e);
        }
    });
}

// events

$("#jplayer").bind($.jPlayer.event.play, function (event) {
    $(".audio-player-controls").find(".audio-player-button").removeClass("icon-play");
    $(".audio-player-controls").find(".audio-player-button").addClass("icon-pause");
    $(".audio-player-progress").addClass("loading");  
    // connect stream to waves canvas
});

$("#jplayer").bind($.jPlayer.event.pause, function (event) { // Add a listener to report the time play began
    $(".audio-player-controls").find(".audio-player-button").removeClass("icon-pause");
    $(".audio-player-controls").find(".audio-player-button").addClass("icon-play");
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
    $("audio-player-progress-bar").css({width: "25%"});
});


function tunein(channel, elem) {
    // adjust scroll viewport            
    $('#stations').animate({
        scrollTop: $("#stations").scrollTop() + ($(elem).position().top - $("#stations").position().top) - ($("#stations").height() / 2) + ($(elem).height() / 2)
    }, 1000);
    
    togglePlayStationControl(elem);
    if (current_channel === elem) { // we have selected the channel that is currently selected              
        toggleStreaming();    
    } else {          
        if ($("#jplayer").data().jPlayer.status.paused == false) 
            togglePlayStationControl(current_channel);
        if (current_channel !== undefined) {
            $(current_channel).closest("li").removeClass("is-selected");
            triggerMouseOverIcon($('span:first', current_channel));
            $(current_channel).closest("li").find(".label").removeClass("is-playing");
        }
        current_channel = elem;
        $(current_channel).closest("li").addClass("is-selected");
        removeMouseOverIcon($('span:first', current_channel));
        $(current_channel).closest("li").find(".label").addClass("is-playing");
        stream(channel);
        if (channel.Logo !== null) {
            $(".audio-player-image img").attr('src', channel.Logo + '?' + Math.random());
            $(".audio-player-image img").show();
        } else {
            $(".audio-player-image img").hide();
        }
        $('.audio-player-song-name').text(channel.CurrentTrack);
    }      
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
    $(item).find(".activity").toggleClass("icon-pause icon-play");
}


function nextStation() {
    if (current_channel === undefined) return null;
    var channel_to_trigger;
    if ($(current_channel).closest("li").next().length === 0)
        channel_to_trigger = $("#stations ul li:nth-child(1)").find(".image"); // .trigger("onclick");
    else
        channel_to_trigger = $(current_channel).closest("li").next().find(".image"); // .trigger("onclick");
    // setup the icons to be swapped for tunein function 
    $('span:first', channel_to_trigger).removeClass("icon-headphones");
    $('span:first', channel_to_trigger).addClass("icon-play");
    $(channel_to_trigger).trigger("onclick");
}

function previousStation() {
    if (current_channel === undefined) return null;
    var channel_to_trigger;
    if ($(current_channel).closest("li").prev().length === 0)
        channel_to_trigger = $("#stations ul li:nth-child(" + $('#stations ul li').length + ")").find(".image");     
    else
        channel_to_trigger = $(current_channel).closest("li").prev().find(".image");
    // setup the icons to be swapped for tunein function 
    $('span:first', channel_to_trigger).removeClass("icon-headphones");
    $('span:first', channel_to_trigger).addClass("icon-play");
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
    if ($('#stations ul').children('li') !== null)
        count = $('#stations ul li').length;
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



// FAVOURITES

function addToFavourites(elem) {    

    if ($(elem).is("#add-favourite")) {   // if the favourite button was clicked from the audio control panel
        if (current_channel == undefined) {  // if there is nothing playing
            $("#page").prepend('<span class="tooltiptext">No channel is currently playing</span>');
            $(".tooltiptext").delay(3000).fadeOut();
            return;
        }
    }

    var channel_name = encodeURIComponent($(current_channel).closest(".image").next(".info").find(".title").text());

    jQuery(function ($) {
        $.ajax({
            type: "POST",
            contentType: "text; charset=utf-8",
            url: '/Favourites/AddFavourite/?id=' + channel_name,
            cache: false,
            success: function (response) {
                if (response != null && response.success) {
                    $("#page").prepend('<span class="tooltiptext">' + response.responseText + '</span>');
                    $(".tooltiptext").delay(3000).fadeOut();
                } else {
                    alert("success error");
                }
            },
            error: function (response) {
                alert(response.responseText);  
            }
        });
    });
}

function removeFromFavourites(channel) {

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
                $('#stations ul').append(data);
                triggerMouseOverIcon();
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