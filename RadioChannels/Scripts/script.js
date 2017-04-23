// create the module and name it scotchApp
var app = angular.module('app', ['ngRoute']);

app.config(function ($routeProvider) {
    $routeProvider
        .when('/', {
            templateUrl: '/Home/IndexPartial',
            controller: 'MainController'
        })
        .when('/account', {
            templateUrl: '/Account/Account',
            controller: 'MainController'
        })
        .when('/favourites', {
            templateUrl: '/Favourites',
            controller: 'MainController'
        })
        .when('/account/logout', {
            templateUrl: '/Account/Account',
            controller: 'MainController'
        })
        .when('/account/register', {
            templateUrl: '/Account/Register',
            controller: 'MainController'
        })
});

app.controller('MainController', function ($scope) { 
    $scope.default_message = 'Please hover your mouse over the search area above to begin searching for music channels';
});