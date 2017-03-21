﻿/**
 * cbpAnimatedHeader.js v1.0.0
 * http://www.codrops.com
 *
 * Licensed under the MIT license.
 * http://www.opensource.org/licenses/mit-license.php
 * 
 * Copyright 2013, Codrops
 * http://www.codrops.com
 */




// var cbpAnimatedHeader = (function() {

var docElem;
var header;
var didScroll;
var changeHeaderOn;;

findHeader();

function findHeader() {
    docElem = document.documentElement,
        header = document.querySelector('.cbp-af-header'),
        didScroll = false,
        changeHeaderOn = 0;
}

function init() {
    scrollPage();
    $(".header-container").on('mouseenter', function (event) {
        classie.remove(header, 'cbp-af-header-shrink');
    });
    $(".header-container").on('mouseleave', function (event) {
        classie.add(header, 'cbp-af-header-shrink');
    });
}

function scrollPage() {
    var sy = scrollY();
    if (sy >= changeHeaderOn) {
        classie.add(header, 'cbp-af-header-shrink');
    }
    else {
        classie.remove(header, 'cbp-af-header-shrink');
    }
    didScroll = false;
}

function scrollY() {
    return window.pageYOffset || docElem.scrollTop;
}

findHeader();
init();

//})();