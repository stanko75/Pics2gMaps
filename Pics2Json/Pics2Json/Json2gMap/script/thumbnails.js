function zoomMap(x) {
    debugger;
}

$.getJSON("/*picsJson*/Thumbs.json", function (data) {
	var thumbs;
	thumbs = $('#thumbnails');
	data.forEach(function(file) {
        $('#thumbnails').append('<img onmouseover = "zoomMap(this)" src="' + file.FileName + '">');
	})
})