window.addEventListener('popstate', function (event) {
    if (event.state !== undefined && event.state !== null) {

        // genre favourties
        if (window.location.href.includes("/#favourites/")) {
            page_ajax_request('/Favourites/IndexPartial', function () {
                $(".radmenu > a").trigger("click");
                $(".radmenu a").each(function (index, elem) {   // find the genre 
                    if ($(elem).text().toLowerCase() === event.state.toLowerCase()) {
                        $(elem).trigger("click");
                        return false;
                    }
                });
                triggerMouseOverIcon();
            });
        } else if ((event.state === "favourites")) {
            $("#item-favourites").trigger("click");
            triggerMouseOverIcon();
        } else if (event.state === "index") {
            $("#item-radio-stations").trigger("click");
        } else if (event.state === "register") {
            page_ajax_request('/Account/Register', ActionOnReady);
        } else if (event.state === "login") {
            page_ajax_request('/Account/Login', ActionOnReady);
        } else if (event.state === "account") {
            page_ajax_request('/Account/Account', ActionOnReady);
        } else if (event.state === "edit") {
            page_ajax_request('/Account/Edit', ActionOnReady);
        } else if (event.state === "editpassword") {
            page_ajax_request('/Account/EditPassword', ActionOnReady);
        } else if (event.state === "forgotpassword") {
            page_ajax_request('/Account/ForgotPassword', ActionOnReady);
        } else if (event.state === "resetpassword") {
            page_ajax_request('/Account/ResetPassword', ActionOnReady);
        } else { // else its index genre
            page_ajax_request('/Home/IndexPartial', function () {
                initializeRadialMenu();
                $(".radmenu > a").trigger("click"); // select "Genre" item
                $(".radmenu a").each(function (index, elem) {   // find the Genre selected and trigger it
                    if ($(elem).text().toLowerCase() === event.state.toLowerCase()) {
                        $(elem).trigger("click");
                        return false;
                    }
                });
            });
        }
    } else {    // reinitialize it
        // return to home page
        $("#stations > div").empty();
        // remove selected genre
        $(".radmenu").find(".clicked").removeClass("clicked");
    }
});