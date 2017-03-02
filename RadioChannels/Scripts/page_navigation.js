window.addEventListener('popstate', function (event) {
    if (event.state !== undefined && event.state !== null) {

        // genre favourties
        if (window.location.href.includes("/#favourites/")) {
            ajax_request('/Favourites/IndexPartial', function () {
                triggerFavouritesGenreSelect();
                $(".radmenu > a").trigger("click");
                $(".radmenu a").each(function (index, elem) {
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
            ajax_request('/Account/Register', ActionOnReady);
        } else if (event.state === "login") {
            ajax_request('/Account/Login', ActionOnReady);
        } else if (event.state === "account") {
            ajax_request('/Account/Account', ActionOnReady);
        } else if (event.state === "edit") {
            //window.location.pathname = '/Account/Edit';
            ajax_request('/Account/Edit', ActionOnReady);
        } else if (event.state === "editpassword") {
            //window.location.pathname = '/Account/EditPassword';
            ajax_request('/Account/EditPassword', ActionOnReady);
        } else if (event.state === "forgotpassword") {
            // window.location.pathname = '/Account/ForgotPassword';
            ajax_request('/Account/ForgotPassword', ActionOnReady);
        } else if (event.state === "resetpassword") {
            // window.location.pathname = '/Account/ResetPassword';
            ajax_request('/Account/ResetPassword', ActionOnReady);
        } else { // else its index genre
            //$("#item-radio-stations").find("a").trigger("click");
            ajax_request('/Home/IndexPartial', function () {
                triggerGenreSelect();
                $(".radmenu > a").trigger("click");
                $(".radmenu a").each(function (index, elem) {
                    if ($(elem).text().toLowerCase() === event.state.toLowerCase()) {
                        $(elem).trigger("click");
                        return false;
                    }
                });
            });
        }
    } else {
        // return to home page
        $("#stations > div").empty();
        // remove selected genre
        $(".radmenu").find(".clicked").removeClass("clicked");
    }
});