(function (ns) {
    /*globals google, $*/
    "use strict";
    $.getJSON("/*picsJson*/Thumbs.json", function (data) {
        var thumbs,
            clicked;
        clicked = false;
        thumbs = $('#thumbnails');
        data.forEach(function (val, key) {
            thumbs.append('<img id="' + key + '"' + ' src=' + val.FileName + ">");
            thumbs.on('mouseover', '#' + key, function () {
                if (!clicked) {
                    $('#' + key).css("border", "14px solid #333");
                    ns.map.setZoom(20);
                    ns.map.setCenter(new google.maps.LatLng(val.Latitude, val.Longitude), 13);
                }
            });
            thumbs.on('mouseout', '#' + key, function () {
                if (!clicked) {
                    $('#' + key).css("border", "");
                    ns.map.setZoom(15);
                }
            });
            thumbs.on('click', '#' + key, function () {
                clicked = !clicked;
            });
        });
        ns.map.setZoom(15);
    });
})(window.milosev);