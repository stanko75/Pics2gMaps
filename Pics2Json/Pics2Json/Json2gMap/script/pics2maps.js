(function (ns) {
	/*globals google, $*/
	"use strict";
    $.getJSON("/*picsJson*/.json", function (data) {
        data.forEach(function (file) { 

            try {
                var picsLatLng = new google.maps.LatLng(file.Latitude, file.Longitude);
                var bounds = new google.maps.LatLngBounds();

                var marker = new google.maps.Marker({
                    position: picsLatLng,
                    map: ns.map,
                    title: file.FileName,
                    url: file.FileName
                });
            }
            catch (e) {
                console.log(e);
                setTimeout(function () {
                    if (typeof google !== 'object') {
                        location.reload();
                    }
                }, 1000);
            }

			marker.addListener('click', function () {
				window.open(marker.url, "_target");
			});
				
			bounds.extend(picsLatLng);
			ns.map.fitBounds(bounds);	
			
			var zoomChangeBoundsListener = 
				google.maps.event.addListenerOnce(ns.map, 'bounds_changed', function(event) {
					if ( ns.map.getZoom() ){
						ns.map.setZoom(14);  // set zoom here
					}
				});

			setTimeout(function(){
				google.maps.event.removeListener(zoomChangeBoundsListener);
			}, 2000);			
		});
	});
})(window.milosev);