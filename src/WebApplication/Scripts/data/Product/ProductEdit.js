angular.module('productEdit', [])
	.controller('ProductEditController', ['$scope', '$http',
		function ($scope, $http) {
			$scope.IsCreate = true;
			$scope.CurrentProduct = null;

			$scope.ResetProduct = function() {
				$scope.CurrentProduct = {
					ProductId : null,
					CreatedDate : null,
					ModifiedDate : null,
					UPC : null,
					Name : null,
					Cost : null,
					PromotionCost : null,
					OverrideCost : null,
					MAPPrice: null,
					MinPrice: null,
					MaxPrice: null,
					BreakevenPrice: null,
					QuantityInCase : null,
					ASIN : null,
					SKU : null,
					ItemNumber : null,
					ManufacturerId : null,
					Length : null,
					Width : null,
					Height : null,
					Weight : null,
					IsMAP : null,
					IsDiscontinued: null,
					Notes: null
				};
			};

			// Initialize the $scope.CurrentProduct.
			$scope.ResetProduct();

			$(document).ready(function () {
				var productSearch = new Bloodhound({
					datumTokenizer: Bloodhound.tokenizers.obj.whitespace('value'),
					queryTokenizer: Bloodhound.tokenizers.whitespace,
					limit: 30,
					remote: {
						url: '../Product/Search?query=%QUERY',
						rateLimitWait: 1000,
						ttl: 1
					}
				});

				productSearch.initialize();

				$('#searchTypeahead .typeahead').typeahead(null, {
					name: 'ProductId',
					displayKey: 'Name',
					minLength: 3,
					source: productSearch.ttAdapter(),
					templates: {
						empty: 'Could not find results.',
						suggestion: Handlebars.compile('<div style="font-size: 12px;"><b>ASIN</b>: {{ASIN}} &nbsp;&nbsp;&nbsp;&nbsp; <b>SKU</b>: {{SKU}} <br>' +
														'<b>Item Number</b>: {{ItemNumber}} &nbsp;&nbsp;&nbsp;&nbsp; <b>UPC</b>: {{UPC}} <br>' +
														'<b>Name:</b> {{Name}} </div>')
					}
				}).on('typeahead:selected', function (obj, datum) {
					$scope.IsCreate = false;
					$scope.$apply();
					
					$('.typeahead').typeahead('val', '');

					$scope.GetProduct(datum.ProductId);
				});
			});

			$scope.ResetForm = function() {
				$scope.ResetProduct();
			};

			$scope.ResetAll = function () {
				$scope.IsCreate = true;

				$scope.ResetForm();

				$('.typeahead').typeahead('val', '');
			};

			$scope.GetProduct = function (selectedProductId) {
				$scope.ResetProduct();

				$http.get('../api/Product/Get?ProductId=' + selectedProductId)
					.success(function (data, status, headers, config) {
						$scope.CurrentProduct = data;
					})
					.error(function (data, status, headers, config) {
						alert('Could not get product. status: ' + status);
					});
			};

			$scope.EditProduct = function () {
				var previousProductId = $scope.CurrentProduct.ProductId;

				if ($scope.CurrentProduct.ManufacturerId === null) {
					alert('A manufacturer must be selected.');
					return;
				}

				$http({
					method: 'POST',
					url: '../api/Product/Edit',
					data: angular.toJson($scope.CurrentProduct),
					dataType: 'json',
					contentType: 'application/json'
				})
				.success(function (data, status) {
					$scope.CurrentProduct = data;

					$scope.IsCreate = false;
					
					if (previousProductId === data.ProductId) {
						alert('Saved changes successfully.');
					} else {
						alert('Created product successfully.');
					}
				}).error(function (data, status) {
					alert('Could not edit/create the Product. Status: ' + status);
				});
			};

		}]);