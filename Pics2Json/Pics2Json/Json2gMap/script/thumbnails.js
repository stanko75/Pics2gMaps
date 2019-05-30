$.getJSON("/*picsJson*/Thumbs.json", function (data) {
	var thumbs;
	thumbs = $('#thumbnails');
    data.forEach(function (val, key) {
        thumbs.append('<img id="' + key + '"' + ' src=' + val.FileName + ">");
        thumbs.on('mouseover', '#' + key, function () {
            window.milosev.map.setZoom(20);
        })
        thumbs.on('mouseout', '#' + key, function () {
            window.milosev.map.setZoom(15);
        })		
	})
})