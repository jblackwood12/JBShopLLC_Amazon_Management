angular.module('unpricedProducts', [])
	.controller('UnpricedProductsController', ['$scope', '$http',
		function ($scope, $http) {
			$http.get('/api/ProductReport/UnpricedProducts')
				.success(function(data, status, headers, config) {
					$scope.unpricedProductsList = data;
				})
				.error(function(data, status, headers, config) {

				});
		}]);