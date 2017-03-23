/// <reference path="html5shiv.js" />
var lat = 0.0;
var lng = 0.0;
var range = "10";
var address = "address";
$(document).ready(function () {
    navigator.geolocation.getCurrentPosition(function (pos) {
        lat = pos.coords.latitude;
        lng = pos.coords.longitude;
    });
});
function getGeolocationCoordinates1() {
    navigator.geolocation.getCurrentPosition(function (pos) {
        lat = pos.coords.latitude;
        lng = pos.coords.longitude;
    });
}
function getGeolocationCoordinatesOnLoad(obj, param) {
    navigator.geolocation.getCurrentPosition(function (pos) {
        lat = pos.coords.latitude;
        lng = pos.coords.longitude;
    });
    mapViewOnPage(lat, lng, 'You are here!');
    obj.getElementById(param + 'Latitude').text = lat;
    obj.getElementById(param + 'Longitude').text = lng;
}
function getGeolocationCoordinatesOnChange(param) {
    navigator.geolocation.getCurrentPosition(function (pos) {
        lat = pos.coords.latitude;
        lng = pos.coords.longitude;
    });
    mapViewOnPage(lat, lng, 'You are here!');
    var latitude = "#" + param + "Latitude";
    var longitude = "#" + param + "Longitude";
    $(latitude).text(lat);
    $(longitude).text(lng);
}
function getGeoLocationOnEditPage(latitude, longitude) {
    navigator.geolocation.getCurrentPosition(function (pos) {
        lat = pos.coords.latitude;
        lng = pos.coords.longitude;
    });
    if (latitude == "" && longitude == "") {
        mapViewOnPage(pos.coords.latitude, pos.coords.longitude, 'You are here!')
    }
    else {
        mapViewOnPage(latitude, longitude, 'You are here!');
    }
}
function getLatitude() {
    return lat;
}
function getLongitude() {
    return lng;
}
function editLatLngInURL(obj) {
    var hrf = $(obj).attr("href");
    hrf = hrf.replace("getLatitude", getLatitude());
    hrf = hrf.replace("getLongitude", getLongitude());
    hrf = hrf.replace("range", range);
    $(obj).attr("href", hrf);
}
function editSearchURL(obj) {
    if ($("#SearchStringRange").val() != null && $("#SearchStringRange").val() != "")
        range = $("#SearchStringRange").val();
    else
        range = range;
    if ($("#SearchStringAddress").val() != "" && $("#SearchStringAddress").val() != null)
        address = $("#SearchStringAddress").val();
    else
        address = "address";
    //var hrf = window.location.href;
    var hrf = obj.baseURI;
    hrf = hrf.replace("Range=" + GetParameterValues('Range', obj.baseURI), "Range=" + range);
    hrf = hrf.replace("Address=" + GetParameterValues('Address', obj.baseURI), "Address=" + address);
    $(obj).attr("href", hrf);
}
function editCancelURL(obj) {
    var hrf = obj.baseURI;
    var oldRangeValue = GetParameterValues('Range', obj.baseURI);
    var oldAddressValue = GetParameterValues('Address', obj.baseURI);
    var hrf = hrf.replace("Range=" + oldRangeValue, "Range=" + range);
    hrf = hrf.replace("Address=" + oldAddressValue, "Address=address");
    $(obj).attr("href", hrf);
}
function GetParameterValues(param, baseurl) {
    //var url = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
    var url = baseurl.slice(window.location.href.indexOf('?') + 1).split('&');
    for (var i = 0; i < url.length; i++) {
        var urlparam = url[i].split('=');
        if (urlparam[0] == param) {
            return urlparam[1];
        }
    }
}
function MapView(model) {
    $("#ListView").attr('style', 'display:none');
    $("#btnMapView").attr('style', 'display:none');
    $("#btnListView").removeAttr('style', 'display:none');
    $("#btnListView").attr('style', 'float:right');
    $("#MapView").removeAttr('style', 'display:none');
    $("#MapView").attr('style', 'width:100%;height:600px;');
    // Google has tweaked their interface somewhat - this tells the api to use that new UI
    google.maps.visualRefresh = true;
    var currentLoc = new google.maps.LatLng(lat, lng);
    // These are options that set initial zoom level, where the map is centered globally to start, and the type of map to show
    var mapOptions = {
        zoom: 13,
        center: currentLoc,
        mapTypeId: google.maps.MapTypeId.G_NORMAL_MAP
    };
    // This makes the div with id "map_canvas" a google map
    var map = new google.maps.Map(document.getElementById("MapView"), mapOptions);
    // This shows adding a simple pin "marker" - this happens to be the Tate Gallery in Liverpool!
    var myLatlng = new google.maps.LatLng(lat, lng);
    var currentMarker = new google.maps.Marker({
        position: myLatlng,
        map: map,
        title: 'You are here!'
    });
    // You can make markers different colors...  google it up!
    currentMarker.setIcon('http://maps.google.com/mapfiles/ms/icons/green-dot.png')
    // put in some information about each json object - in this case, the opening hours.
    var infowindow1 = new google.maps.InfoWindow({
        content: "<div class='infoDiv'><h2>" + 'You are here!' + "</h2>" + "<div>"
    });
    infowindow1.open(map, currentMarker);
    // finally hook up an "OnClick" listener to the map so it pops up out info-window when the marker-pin is clicked!
    google.maps.event.addListener(currentMarker, 'click', function () {
        infowindow1.open(map, currentMarker);
    });
    var latlngbounds = new google.maps.LatLngBounds();
    // a sample list of JSON encoded data of places to visit in Liverpool, UK
    // you can either make up a JSON list server side, or call it from a controller using JSONResult
    var data = model;
    // Using the JQuery "each" selector to iterate through the JSON list and drop marker pins
    $.each(data, function (i, item) {
        var marker = new google.maps.Marker({
            'position': new google.maps.LatLng(item.Latitude, item.Longitude),
            'map': map,
            'title': item.DisplayValue
        });
        // Make the marker-pin blue!
        marker.setIcon('http://maps.google.com/mapfiles/ms/icons/blue-dot.png')
        // put in some information about each json object - in this case, the opening hours.
        var infowindow = new google.maps.InfoWindow({
            content: "<div class='infoDiv'><h2>" + item.DisplayValue + "</h2>" + "<div>"
        });
        // finally hook up an "OnClick" listener to the map so it pops up out info-window when the marker-pin is clicked!
        infowindow.open(map, marker);
        // finally hook up an "OnClick" listener to the map so it pops up out info-window when the marker-pin is clicked!
        google.maps.event.addListener(marker, 'click', function () {
            infowindow.open(map, marker);
        });
        latlngbounds.extend(new google.maps.LatLng(item.Latitude, item.Longitude));
    })
    map.fitBounds(latlngbounds);
    var listener = google.maps.event.addListener(map, "idle", function () {
        if (map.getZoom() > 15) map.setZoom(11);
        google.maps.event.removeListener(listener);
    });
}
function ListView() {
    $('#ListView').removeAttr("style");
    $('#MapView').attr('style', 'display:none');
    $("#btnListView").attr('style', 'display:none');
    $("#btnMapView").removeAttr('style', 'display:none');
    $("#btnMapView").attr('style', 'float:right');
}
function mapViewOnPage(propLat, propLng, markerString) {
    $("#MapViewOnPage").attr('style', 'width:100%;height:200px;');
    if (propLat == null || propLng == "" || propLat == null || propLng == "") {
        markerString = 'You are here!';
        navigator.geolocation.getCurrentPosition(function (pos) {
            propLat = pos.coords.latitude;
            propLng = pos.coords.longitude;
        });
    }
    var tempLat = propLat;
    var tempLng = propLng;
    //var tempLat = propLat == null || propLat == "" ? lat : propLat;
    //var tempLng = propLng == null || propLng == "" ? lng : propLng;
    var currentlatlng = new google.maps.LatLng(tempLat, tempLng);
    var bounds = new google.maps.LatLngBounds();
    var options = {
        zoom: 15,
        center: currentlatlng,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    };
    var map = new google.maps.Map(document.getElementById("MapViewOnPage"), options);
    var marker = new google.maps.Marker({
        position: currentlatlng,
        map: map,
        title: markerString
    });
    marker.setIcon('http://maps.google.com/mapfiles/ms/icons/green-dot.png')
    //bounds.extend(marker.getPosition());
    bounds.extend(new google.maps.LatLng(tempLat, tempLng));
    var infowindow = new google.maps.InfoWindow({
        content: "<div class='infoDiv'>" + markerString + "<div>"
    });
    infowindow.open(map, marker);
    google.maps.event.addListener(marker, 'click', function () {
        infowindow.open(map, marker);
    });
    map.fitBounds(bounds);
    var listener = google.maps.event.addListener(map, "idle", function () {
        if (map.getZoom() > 15) map.setZoom(14);
        google.maps.event.removeListener(listener);
    });
    //google.maps.event.addListenerOnce(map, 'idle', function () {
    //    google.maps.event.trigger(map, 'resize');
    //});
    //$("#MapViewOnPage").on("pageshow", function (event, data) {
    //    var center = map.getCenter();
    //    google.maps.event.trigger(map, "resize");
    //    map.setCenter(center);
    //});
    //$(document).on("pageshow", function (event, data) {
    //    var center = map.getCenter();
    //    google.maps.event.trigger(map, "resize");
    //    map.setCenter(center);
    //});
}
/*
Unused methods
*/
function editSearchByRangeURL(obj) {
    if ($("#SearchStringRange").val() != null && $("#SearchStringRange").val() != "")
        range = $("#SearchStringRange").val();
    var hrf = window.location.href;
    hrf = hrf.replace(GetParameterValues('Range'), range);
    $(obj).attr("href", hrf);
    $("#SearchStringRange").val(range);
}
function editCancelByRangeURL(obj) {
    var oldRangeValue = GetParameterValues('Range');
    var hrf = window.location.href;
    hrf = hrf.replace(oldRangeValue, range);
    $(obj).attr("href", hrf);
    $("#SearchStringRange").val(range);
}
function editSearchByAddressURL(obj) {
    if ($("#SearchStringAddress").val() == "")
        return;
    var hrf = window.location.href;
    hrf = hrf.replace(GetParameterValues('Address'), $("#SearchStringAddress").val());
    $(obj).attr("href", hrf);
    $("#SearchStringAddress").val("");
}
function editLatLngInURL1(entityName, propertyName) {
    alert(entityName + "--" + lat + "--" + lng + "--" + propertyName);
    $.ajax({
        // edit to add steve's suggestion.
        //url: "/ControllerName/ActionName",
        url: '<%= Url.Action("Index", "NearByLocations",new { EntityName=' + entityName + ',Latitude=' + lat + ' ,Longitude=' + lng + ',ImagePropertyName=' + propertyName + '})) %>',
        success: function (data) {
            // your data could be a View or Json or what ever you returned in your action method 
            // parse your data here
            alert(data);
        }
    });
}
function MapView1(model) {
    $("#ListView").attr('style', 'display:none');
    $("#btnMapView").attr('style', 'display:none');
    $("#btnListView").removeAttr('style', 'display:none');
    $("#btnListView").attr('style', 'float:right');
    $("#MapView").removeAttr('style', 'display:none');
    $("#MapView").attr('style', 'width:1000px;height:600px;');
    var currentlatlng = new google.maps.LatLng(lat, lng);
    var bounds = new google.maps.LatLngBounds();
    var options = {
        zoom: 11,
        center: currentlatlng,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    };
    var map = new google.maps.Map(document.getElementById("MapView"), options);
    var infowindow = new google.maps.InfoWindow();
    var data = model;
    $.each(data, function (i, item) {
        var myLatLng = new google.maps.LatLng(item.latitude, item.Longitude);
        var contentString = "<b>" + item.DisplayValue + "</b><br/>" + item.Address;
        var marker = new google.maps.Marker({
            position: myLatLng,
            map: map,
            title: contentString
            //bounds: true
        });
        bounds.extend(marker.getPosition());
        marker.html = contentString;
        google.maps.event.addListener(marker, 'click', function () {
            infowindow.setContent(this.html);
            infowindow.open(map, this);
        });
    });
    //map.setCenter(bounds.getCenter(), map.getBoundsZoomLevel(bounds));
    map.fitBounds(bounds);
    var listener = google.maps.event.addListener(map, "idle", function () {
        if (map.getZoom() > 15) map.setZoom(11);
        google.maps.event.removeListener(listener);
    });
}