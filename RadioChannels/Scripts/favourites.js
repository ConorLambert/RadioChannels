function getFavourites() {
    // if the user is already logged in, then the client already has the favourites in memory
    if (favourites !== undefined) {
        ajax_request('/Favourites/IndexPartial');
    } else {
        window.location.pathname = '/favourites/all';
    }
}

function isFavourite(channel) {
    // check channel against favourites
    return hasChannel(channel);
}

function hasChannel(name) {
    if (favourites !== undefined) {
        return findFavourite(name) !== undefined;
    }
}

function findFavourite(id) {
    var result = undefined;
    $(favourites).find(".channel-id").each(function (index, elem) {
        if ($(elem).attr("id") === id) {
            result = elem;
            return false;
        }
    });
    return result;
}


function toggleFavourite(channel, elem) {
    if ($(elem).is("#add-favourite")) {   // if the favourite button was clicked from the audio control panel
        if (current_channel == undefined) {  // if there is nothing playing
            $("#page").prepend('<span class="tooltiptext">No channel is currently playing</span>');
            $(".tooltiptext").delay(3000).fadeOut();
            return;
        }
    }

    var channel_name = encodeURIComponent($(elem).closest(".row").find(".channel-title").text());
    if ($(elem).hasClass("is-favourite")) {
        removeFromFavourites(channel, channel_name, $(elem).closest(".row"));
    } else {
        addToFavourites(channel, channel_name, $(elem).closest(".row"));
    }
}

function addToFavourites(channel, channel_name, elem) {
    $(".tooltiptext").remove();
    jQuery(function ($) {
        $.ajax({
            type: "POST",
            contentType: "text; charset=utf-8",
            url: '/Favourites/AddFavourite/?id=' + channel.Id + '&name=' + channel_name,
            cache: false,
            success: function (response) {
                if (response != null && response.success) {
                    $("#page").prepend('<span class="tooltiptext" style="margin-left:' + ((screen.width / 2) - 200) + 'px;">' + response.responseText + '</span>');
                    $(".tooltiptext").delay(3000).fadeOut();
                    // add to favourites collection
                    $(elem).find(".fav-btn").addClass("is-favourite");
                    $(elem).find(".fav-btn").removeClass("fav-btn");
                    $(favourites).append($(elem).clone());
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

function removeFromFavourites(channel, channel_name, elem) {
    $(".tooltiptext").remove();
    // remove from favourites
    jQuery(function ($) {
        $.ajax({
            type: "PUT",
            contentType: "text; charset=utf-8",
            url: '/Favourites/RemoveFavourite/?id=' + channel.Id + '&name=' + channel_name,
            cache: false,
            success: function (response) {
                if (response != null && response.success) {
                    $("#page").prepend('<span class="tooltiptext" style="margin-left:' + ((screen.width / 2) - 200) + 'px;">' + response.responseText + '</span>');
                    $(".tooltiptext").delay(3000).fadeOut();
                    // remove from favourites in search
                    $(elem).find(".is-favourite").addClass("fav-btn");
                    $(elem).find(".is-favourite").removeClass("is-favourite");
                    // remove from favourites
                    $(findFavourite(channel.Id)).closest(".row").remove();
                    if (window.location.href.includes("favourites") && current_genre !== "Genres")
                        $(elem).closest(".row").remove();                    
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