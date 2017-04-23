var total_base_genres = 0;

$(document).ready(initializeRadialMenu);

function initializeRadialMenu() {

    // classie.js animation functions to detect scroll over Radial Menu area
    init();
    findHeader();    

    var buttons = document.querySelectorAll(".radmenu a");

    for (var i = 0, l = buttons.length; i < l; i++) {
        var button = buttons[i];
        button.onclick = setSelected;
    }

    total_base_genres = $(".categories-container").first().find(".general-genre-container").length;

    function setSelected(e) {

        var target = e.target || e.srcElement;  // IE compatible

        // if we are on the favoutites page 
        if (window.location.href.includes("favourites") && $(target).text() !== "Genres") {
            if ($("#stations").find(".page-message").length > 0)    // if any flash message left on the screen, then remove it
                $("#stations").find(".page-message").remove();
            if (current_genre === $(target).text().toLowerCase()) { // if the genre selected is the current genre
                $("#stations > div").detach();              // attach the favourites back
                $("#stations").append(favourites);
                current_genre = "Genres";                   // set back to "Genres"
            } else {        // else a specific genre was selected
                current_genre = $(target).text().toLowerCase();
                getFavouritesOf(current_genre);
                $(".radmenu li").removeClass("clicked"); // Remove all highlights
                $(target).parent('li').addClass("clicked");
            }
            // if we are on the search page, then perform a normal search
        } else if (!this.classList.contains("selected") && $(target).text() !== "Genres" && current_genre !== $(target).text().toLowerCase()) {
            // remove the current list of stations if any
            current_genre = $(target).text().toLowerCase();
            $('#stations > div').empty();
            channels_ajax_request(0);
            triggerScrollEvent();
            if (!window.location.href.includes(encodeURIComponent(current_genre)))  // if we have moved back to this page then, dont push it
                history.pushState(current_genre, null, "/#index/" + current_genre);
        }

        // change CSS of selected item
        if (this.classList.contains("selected")) {  // if centre menu item selected and has already been selected go up
            this.classList.remove("selected");
            if ($(target).text() !== "Genres" || !this.parentNode.classList.contains("radmenu")) {  // if the selected is not "Genres"              
                var genre_selector = $(".radmenu > a"); // get the Genres menu item and add selected to it
                $(genre_selector).addClass("selected");
                placeMenuItems($(genre_selector).next("ul"));   // redisplay the menu items from the perspective of Genres
            } else {    // else the selected is "Genre" so add show to it
                this.classList.add("show");
            }
        } else {    // a new menu item was selected (not the currently selected one)
            this.classList.add("selected");
            // $(target).css({ 'transform': 'rotate(-' + 0 + 'deg)' });    // move it to the center
            if (!this.parentNode.classList.contains("radmenu")) {   // if its not "Genre"
                this.parentNode.parentNode.parentNode.querySelector("a").classList.remove("selected")
            } else {    // else it is Genre, so remove the show class from it
                this.classList.remove("show");  // remove the show class from "Genre" selection
            }
            $(target).each(function (index, elem) {
                if ($(elem).next("ul").length > 0) {
                    placeMenuItems($(elem).next("ul"));
                }
            });
        }

        return false;
    }
}

/*
    detach the entire group of favourites channels
    loop through each channel in the group of favourites and return only the genres that match
    Initialize the display of the channel items
*/
function getFavouritesOf(genre) {
    $('#stations > div').detach();  // remove the current list of stations if any   
    $('#stations').append("<div class='container-fluid header-container'></div>");
    $(favourites).find(".row").each(function (index, elem) {
        if ($(elem).find(".genre").text().toLowerCase() === genre) {
            var matched_elem = $(elem).clone();
            $("#stations > div").append(matched_elem);
            initializeChannelItem(matched_elem);
        }
    });
    if ($("#stations").find(".row").length === 0)
        $("#stations").append('<p class="page-message">You currently have no ' + genre + ' favourites in your selection. Navigate to the <a onclick="page_ajax_request(\'/Home/IndexPartial\', initializeRadialMenu)">Search</a> page to begin searching for ' + genre + ' music channels</p>');
    if (!window.location.href.includes("/" + encodeURIComponent(genre)))  // if we have moved back to this page then, dont push it
        history.pushState(genre, null, "/#favourites/" + genre);
}

// list is the ul which holds the genres to be displayed
function placeMenuItems(list) {

    var items = $(list).children("li");
    var translateX = 50;    // how far from the center to move each circle
    var degree = 360 / total_base_genres;

    // for each li element, rotate and translate that item to its correct position
    $(items).each(function (index, elem) {
        rotate(elem, degree * (index + 1), translateX);
    });

}

function rotate(elem, degree, translateX) {
    var starting_point = 170;   // shifts the genres around the circle
    // style the li element
    $(elem).css({
        WebkitTransform: 'rotate(' + ((starting_point + degree) % 360) + 'deg), translateX(' + translateX + 'px)',
        '-moz-transform': 'rotate(' + ((starting_point + degree) % 360) + 'deg), translateX(' + translateX + 'px)',
        'transform': 'rotate(' + ((starting_point + degree) % 360) + 'deg) translateX(' + translateX + 'px)'
    });
    // style the a element (negate the above calculation)
    $(elem).find("a").css({ WebkitTransform: 'rotate(-' + ((starting_point + degree) % 360) + 'deg)' });
    $(elem).find("a").css({ '-moz-transform': 'rotate(-' + ((starting_point + degree) % 360) + 'deg)' });
    $(elem).find("a").css({ 'transform': 'rotate(-' + ((starting_point + degree) % 360) + 'deg)' });
}

