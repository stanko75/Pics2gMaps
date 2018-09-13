$.getJSON("myJosn.json", function (data) {
        console.log("test")
    }).done(function() {
    console.log( "second success" );
  })
  .fail(function( jqxhr, textStatus, error) {
       var err = textStatus + ", " + error;
    console.log( "Request Failed: " + err );
  })
  .always(function() {
    console.log( "complete" );
  });