$(document).ready(function() {
    var imgWidth = $('.regular-image').width();
    $('.artist').width(imgWidth);
});


$(function(){
    $("#categories a").on("click", function(){
        $("#categories li").removeClass("clicked"); // Remove all highlights
        $(this).parent('li').addClass("clicked"); // Add the class only for actually clicked element
    });
});

$(function(){
    $("#navigation a").on("click", function(){
        $("#navigation li").removeClass("nav-clicked"); // Remove all highlights
        $(this).parent('li').addClass("nav-clicked"); // Add the class only for actually clicked element
    });
});