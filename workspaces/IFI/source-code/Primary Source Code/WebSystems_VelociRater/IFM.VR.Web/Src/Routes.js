



//Angular
var ifmApp = angular.module('ifmApp', ['ngRoute', 'ngSanitize']);


ifmApp.config(function ($routeProvider) {

    $routeProvider.
   when('/Home', {
       templateUrl: 'Src/MyVr/controls/Home/Home.html',
       reloadOnSearch: false
   }).
    when('/Quote', {
        templateUrl: 'Src/MyVr/controls/Quote/QuoteSearch.html',        
        reloadOnSearch: false
    }).
         when('/Endorsements', {
             templateUrl: 'Src/MyVr/controls/Endorsements/EndorsementMaster.html',
             reloadOnSearch: false
         }).
    otherwise({
        redirectTo: '/Home'
    });


});


ifmApp.factory('serviceInfo', function () {
    var factory = {};
    factory.baseUrl = baseUrl;
    return factory;
});


var spinnerOpts = {
    lines: 13 // The number of lines to draw
, length: 28 // The length of each line
, width: 14 // The line thickness
, radius: 34 // The radius of the inner circle
, scale: 1 // Scales overall size of the spinner
, corners: 1 // Corner roundness (0..1)
, color: '#000' // #rgb or #rrggbb or array of colors
, opacity: 0 // Opacity of the lines
, rotate: 0 // The rotation offset
, direction: 1 // 1: clockwise, -1: counterclockwise
, speed: 1 // Rounds per second
, trail: 60 // Afterglow percentage
, fps: 20 // Frames per second when using setTimeout() as a fallback for CSS
, zIndex: 2e9 // The z-index (defaults to 2000000000)
, className: 'spinner' // The CSS class to assign to the spinner
, top: '50%' // Top position relative to parent
, left: '50%' // Left position relative to parent
, shadow: false // Whether to render a shadow
, hwaccel: false // Whether to use hardware acceleration
, position: 'fixed' // Element positioning
};


var mainSpinner = new Spinner(spinnerOpts);