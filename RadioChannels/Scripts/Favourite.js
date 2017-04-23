angular.module('app').controller('FavouritesController', ['$scope', '$http', function ($scope, $http) {
    $scope.default_message = 'You currently have no favourites in your selection. Navigate to the <a onclick="page_ajax_request(\'/Home/IndexPartial\', initializeRadialMenu)">Search</a> page to begin searching for music channels';
}]);