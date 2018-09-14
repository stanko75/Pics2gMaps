$.getJSON("myJosn.json", function (data) {
	var mapOptions,
        mapCanvas,
        map;

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
			title: file.FileName
		});
    })
        //document.write("tesrt");
});