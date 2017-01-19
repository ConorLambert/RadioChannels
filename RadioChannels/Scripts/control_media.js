//Mute/Unmute control clicked
$('.muted').click(function() {
    video[0].muted = !video[0].muted;
    return false;
});
 
//Volume control clicked
$('.volumeBar').on('mousedown', function(e) {
    var position = e.pageX - volume.offset().left;
    var percentage = 100 * position / volume.width();
    $('.volumeBar').css('width', percentage+'%');
    video[0].volume = percentage / 100;
});

