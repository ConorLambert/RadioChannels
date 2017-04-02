﻿var api_key = "sJXVu3hXGmKjJiIx";
var channels = [];
var current_volume = 1.0;
var player_displayed = false;
var audio_player_height = 0;

$(document).ready(initializePlayer);

// PLAYER

function initializePlayer() {

    $('#jplayer').jPlayer({        
        solution: 'html,flash',
        swfPath: '"https://cdnjs.cloudflare.com/ajax/libs/jplayer/2.9.2/jplayer/jquery.jplayer.swf',
        supplied: 'mp3,m4a,M3U,M3UA,FLA,WEBMA,WAV,OGA',
        preload: 'none',
        wmode: 'window',
        keyEnabled: true,
        volume: current_volume,
        ready: function () {
            bindPlayerEvents();
            bindVolumeParameters();
        },
        error: function (e) {
            console.log(e);
        }
    });
}

// bind jPlayer events
function bindPlayerEvents() {
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
    
}

// BIND VOLUME PARAMETERS
function bindVolumeParameters() {
    // VOLUME
    $(".volumeBar").click(function (e) {
        var x = e.pageX - $(this).offset().left;
        var width = parseFloat($(".volumeBar").width());
        current_volume = (x / width);        
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
}

function setVolume() {
    $("#jplayer").jPlayer("volume", current_volume);
    $("#mobile-volume").val(current_volume * 100);
    $(".audio-player-volume-bar").css({ width: (current_volume * 100) + "%" });
}



function tunein(channel, elem) {

    // if the player is not shown, then show the player
    if (!player_displayed) {
        $("footer").toggle(800);
        player_displayed = true;
        $("#page").css({ "height": audio_player_height + "px" });
    }

    // adjust scroll viewport            
    $('#stations').animate({
        scrollTop: ($("#stations").scrollTop() + (($(elem).closest(".row").position().top + $("#stations").position().top) + $("footer").outerHeight()) - ($("#stations").height() / 2) - ($(elem).closest(".row").height()))
    }, 1000);

    togglePlayStationControl(elem);
    if (current_channel == elem) { // we have selected the channel that is currently selected              
        toggleStreaming();
    } else {
        // if the player is playing
        if ($("#jplayer").data().jPlayer.status.paused == false)
            togglePlayStationControl(current_channel);  // toggle the icon of the currently playing channel
        if (current_channel !== undefined) {    // if there is a channel playing, remove the styling which indicates so
            $(current_channel).closest(".row").removeClass("is-selected");
            triggerMouseOverIcon(current_channel);
            $(current_channel).closest(".row").find(".label").removeClass("is-playing");    // unstyle "Now Playing" label
        }

        current_channel = elem;
        $(current_channel).closest(".row").addClass("is-selected");
        removeMouseOverIcon(current_channel);   
        $(current_channel).closest(".row").find(".label").addClass("is-playing");
        stream(channel);
        // if this channel is a favourite channel, fill in favourite star of audio channel
        if ($(current_channel).closest(".row").find(".fav-btn").length > 0) { // if is a favourite
            $("#add-favourite").removeClass('is-favourite');    // is favourite is a non-filled in star
            if (!$("#add-favourite").hasClass('fav-btn'))   // fav-btn is a filled in star
                $("#add-favourite").addClass('fav-btn');
        } else {
            $("#add-favourite").removeClass('fav-btn');
            if (!$("#add-favourite").hasClass('is-favourite'))
                $("#add-favourite").addClass('is-favourite');
        }
        if (channel.Logo !== null) {    // if a logo has been encoded 
            $(".audio-player-image img").attr('src', channel.Logo + '?' + Math.random());   // referesh the css to display the image
            $(".audio-player-image img").show();
        } else {
            $(".audio-player-image img").hide();
        }
        $('.audio-player-song-name').text($(elem).closest(".row").find(".track").text());
    }
}

// li channel item play button control
function togglePlayStationControl(item) {
    $(item).toggleClass("icon-pause icon-play");
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




function nextStation() {
    if (current_channel === undefined) return null;
    var channel_to_trigger;
    if ($(current_channel).closest(".row").next().length === 0) // if we are at the end of the current list of stations
        channel_to_trigger = $("#stations > div > .row:nth-child(1)").find(".activity"); // go back to the first station
    else
        channel_to_trigger = $(current_channel).closest(".row").next().find(".activity"); 
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