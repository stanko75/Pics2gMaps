$.getJSON("/*picsJson*/Thumbs.json", function (data) {
	var thumbs;
	thumbs = $('#thumbnails');
    data.forEach(function (val, key) {
        thumbs.append('<img id="' + key + '"' + ' src=' + val.FileName + ">");
        thumbs.on('mouseover', '#' + key, function () {
            if (!clicked) {
                $('#' + key).css("border", "14px solid #333");
                window.milosev.map.setZoom(20);
                window.milosev.map.setCenter(new google.maps.LatLng(val.Latitude, val.Longitude), 13);
            }
        });
        thumbs.on('mouseout', '#' + key, function () {
            if (!clicked) {
                $('#' + key).css("border", "");
                window.milosev.map.setZoom(15);
            }
        });
        thumbs.on('click', '#' + key, function () {
            clicked = !clicked;
        });
	})
})