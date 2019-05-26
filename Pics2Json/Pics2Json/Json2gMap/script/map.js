$.getJSON("/*picsJson*/", function (data) {
	var mapOptions,
        mapCanvas,
        map;

    if (typeof google !== 'object')// && typeof google.maps !== 'object')
    {
        setTimeout(function () {
            // debugger;
            location.reload();
        }, 1000);
    }

    mapOptions = {
        zoom: 6,
        mapTypeId: google.maps.MapTypeId.ROADMAP,
        center: { lat: 50.74388888888889, lng: 7.1522222222222229}
    };
		
	mapCanvas = document.getElementById('map-canvas');
    if (!map) {
        map = new google.maps.Map(mapCanvas, mapOptions);
	}
	
		
	data.forEach(function(file) {
		var myLatlng = new google.maps.LatLng(file.Latitude, file.Longitude);		
	
		var marker = new google.maps.Marker({
			position: myLatlng,
			map: map,
			title: file.FileName,
			url: file.FileName
		});
		
		marker.addListener('click', function() {
			window.open(marker.url,"_target")
        });			
    })
});