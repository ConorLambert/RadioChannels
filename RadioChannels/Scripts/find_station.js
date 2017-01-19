var api_key = "sJXVu3hXGmKjJiIx";

function formatChannel(channel) {
    return channel.Name + ': ' + channel.Genre;
}

// GET ALL CHANNELS
function getAllChannels() {
    var uri_all_channels = 'api/channels';

    console.log("Get All Channels " + uri_all_channels);
    // Send an AJAX request
    $.getJSON(uri_all_channels)
        .done(function (data) {
            // On success, 'data' contains a list of products.
            $.each(data, function (key, item) {
                // USE DIV LAYOUT FOR EACH STATION
                $('<li>', { text: formatChannel(item) }).appendTo($('#stations ul'));
            });
        });
}

// GET ALL CHANNELS BY GENRE
function getChannels(genre) {
    if (genre === "all") {
        getAllChannels();
    } else {
        var uri_all_channels = 'api/channels/' + genre;
        console.log("Get Genre " + uri_all_channels);
        $.getJSON(uri_all_channels)
            .done(function (data) {
                // On success, 'data' contains a list of products.
                console.log("Got the data");
                $.each(data, function (key, item) {
                    // USE DIV LAYOUT FOR EACH STATION
                    console.log("Iterating");
                    $('<li>', { text: formatChannel(item) }).appendTo($('#stations ul'));
                });
            });
    }
}

// listen for genre
$(document).ready(function () {
    $("#genres").on("click", "a", function () {
        var genre = $(this).text().toLowerCase();
        getChannels(genre);
    }); 
});

function tuneIn(station_id) {
    var request = "http://yp.shoutcast.com<base>?id=" + station_id;
}