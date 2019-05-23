$.getJSON("travelBuddiesThumbs.json", function (data) {
	var thumbs;
	thumbs = $('#thumbnails');
	data.forEach(function(file) {
		$('#thumbnails').append('<img src="' + file.FileName + '">');
	})
})