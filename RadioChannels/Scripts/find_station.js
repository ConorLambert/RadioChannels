var api_key = "sJXVu3hXGmKjJiIx";
var channels = [];

function formatChannel(channel) {
    var name = "Unknown";
    var currentTrack = "Unknown";
    var img_tags = '<span class="default-logo"></span>'; 

    if (channel.Name !== null)
        name = channel.Name;
    if (channel.CurrentTrack !== null)
        currentTrack = channel.CurrentTrack;
    if (channel.Logo !== null)
        img_tags = '<img class="logo visible" src="' + channel.Logo + '">';

    return '<div class="image"> <span class="fav-btn"></span> \
                    <span class="default-logo"></span> \
                  </div> \
                  <div class="info"> \
                    <h3 class="title"> <a href="/00sclubhits">' + name + '</a> </h3> \
                    <p class="now-playing"> \
                        <span class="label">now playing</span> \
                        <a class="track" href="/tracks/1054430/00sclubhits">' + currentTrack + '</a> \
                    </p>  \
                  </div>';   
}


// GET ALL CHANNELS BY GENRE
function getChannels(genre) {
    var uri = "";

    if (genre === "all")
        uri = 'api/channels';
    else
        uri = 'api/channels/' + genre;      

    $('#stations ul').empty();
    $.getJSON(uri)
        .done(function (data) {
            var toAppend = "";
            $.each(data, function (key, channel) {  
                channels[key] = channel;                                  
                toAppend += '<li id="'+ key + '">' + formatChannel(channel) + '</li>';                            
            });
            $('#stations ul').append(toAppend);

            // add listener for station selection
            $(".image").on("click", function () {
                // find the station
                var channel = channels[parseInt($(this).closest('li').attr('id'))];
                // send request to stream
                tuneIn(channel);
                if (channel.Logo !== null) {
                    $(".audio-player-image img").attr('src', channel.Logo + '?' + Math.random());
                    $(".audio-player-image img").show();
                } else {
                    $(".audio-player-image img").hide();
                }
                var song_name = $(this).closest('li').find('.track').text();
                $('.audio-player-song-name').text(song_name);
            });
        });    
}

// listen for genre
$(document).ready(function () {

    initializePlayer();
    $("#genres").on("click", "a", function () {
        // remove the current list of stations if any
        $('#stations ul').empty();
        var genre = $(this).text().toLowerCase();
        getChannels(genre);
    });    
});

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
        //play: function () { alert("Play"); },
        //pause: function () { alert("Pause"); },
        //loadstart: function () { alert("Load Start");/*writeInConsole("Player loadstart");*/ },
        //waiting: function () { alert("Waiting");/*writeInConsole("Player waiting");*/ },
        //ended: function () { alert("Ended");},
        flashreset: function () { /*writeInConsole("Player reset");*/ },
        error: function (e) {
            //if (streamingTrack || (stream && stream.isPlaying == true))
            //{
            //    player.jPlayer("setMedia", { mp3: stream.mp3 }).jPlayer("play");
            //}            
            console.log(e);
        }
    });    
}

function tuneIn(channel) {

    // HTTP "Content-Type" of "audio/aacp" is not supported
    /*
        var aac = station.AACEnabled && station.Format == 'audio/aacp';
        stream = station;
        stream.mp3 = aac ? url + '&type=.flv' : url;
    */
    
    var streams = []; 
    channel.Playlist.split("\n").forEach(function (line) {
        if (line.trim().charAt(0) !== '#')                
            streams.push({"mp3" : line.trim() + "/;"});
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