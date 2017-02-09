var api_key = "sJXVu3hXGmKjJiIx";
var channels = [];
var offset = 0;
var current_genre = "";
var current_channel = "";


$(document).ready(function () {
    initializePlayer();
    $("#genres").on("click", "a", function () {        
        $('#stations ul').empty();  // remove the current list of stations if any
        current_genre = $(this).text().toLowerCase();        
        channels_ajax_request(0);
        triggerScrollEvent();
    });
    // VOLUME
    $(".audio-player-volume-bar").click(function (e) {
        var x = e.pageX - $(this).offset().left;
        $("audio-player-volume-bar").css({ width: "25%" });
        $("#jplayer").jPlayer("volume", x);
    });
});



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
        volume: 1.0,
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
});

$("#jplayer").bind($.jPlayer.event.pause, function (event) { // Add a listener to report the time play began
    $(".audio-player-controls").find(".audio-player-button").removeClass("icon-pause");
    $(".audio-player-controls").find(".audio-player-button").addClass("icon-play");
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
    togglePlayStationControl(elem);
    if (current_channel === elem) { // we have selected the channel that is currently selected              
        toggleStreaming();    
    } else {          
        if ($("#jplayer").data().jPlayer.status.paused == false) 
            togglePlayStationControl(current_channel);
        current_channel = elem;
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

