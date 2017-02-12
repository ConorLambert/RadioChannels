function getFavourites() {
    // if the user is already logged in, then the client already has the favourites in memory
    if (favourites !== undefined)
        ajax_request('/Favourites/IndexPartial', triggerMouseOverIcon);

    // if the user is logged out then we inform them that they need to log in to use this feature
    // NOTE: the user could be logged out because of inactivity for some time


}

function isFavourite(channel) {
    // check channel against favourites
    return hasChannel(channel);
}

function hasChannel(name) {
    if (favourites !== undefined) {
        var result = false;
        $(favourites).find(".title").each(function (index, elem) {
            if ($(elem).text() === name) {
                result = true;
                return false;
            }
        });
        return result;
    }
}

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