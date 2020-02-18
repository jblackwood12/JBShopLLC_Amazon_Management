angular.module('repricingEdit', [])
	.controller('RepricingEditController', ['$scope', '$http',
		function ($scope, $http) {
			$scope.IsCreate = true;
			$scope.CurrentRepricingInformation = null;

			$scope.ResetRepricingInformation = function () {
				$scope.CurrentRepricingInformation = {
					RepricingInformationId: null,
					SKU: null,
					ASIN: null,
					MinimumPrice: null,
					ProductName: null,
					IsCurrent: true
				};
			};

			// Initialize the $scope.CurrentRepricingInformation.
			$scope.ResetRepricingInformation();

			$(document).ready(function () {
				var repricingInformationSearch = new Bloodhound({
					datumTokenizer: Bloodhound.tokenizers.obj.whitespace('value'),
					queryTokenizer: Bloodhound.tokenizers.whitespace,
					limit: 30,
					remote: {
						url: '../api/RepricingInformation/Search?query=%QUERY',
						rateLimitWait: 1000,
						ttl: 1
					}
				});

				repricingInformationSearch.initialize();

				$('#searchTypeahead .typeahead').typeahead(null, {
					name: 'RepricingInformationId',
					displayKey: 'ProductName',
					minLength: 3,
					source: repricingInformationSearch.ttAdapter(),
					templates: {
						empty: 'Could not find results.',
						suggestion: Handlebars.compile('<div style="font-size: 12px;"><b>ASIN</b>: {{ASIN}} &nbsp;&nbsp;&nbsp;&nbsp; <b>SKU</b>: {{SKU}} <br>' +
														'<b>Name:</b> {{ProductName}}<br>' +
														'<b>Minimum Price </b>: {{MinimumPrice}}</div>')
					}
				}).on('typeahead:selected', function (obj, datum) {
					$scope.IsCreate = false;
					$scope.$apply();

					$('.typeahead').typeahead('val', '');

					$scope.GetRepricingInformation(datum.RepricingInformationId);
				});
			});

			$scope.ResetAll = function () {
				$scope.IsCreate = true;

				$scope.ResetRepricingInformation();

				$('.typeahead').typeahead('val', '');
			};

			$scope.GetRepricingInformation = function (selectedRepricingInformationId) {
				$scope.ResetRepricingInformation();

				$http.get('../api/RepricingInformation/Get?RepricingInformationId=' + selectedRepricingInformationId)
					.success(function (data) {
						$scope.CurrentRepricingInformation = data;
					})
					.error(function (data, status) {
						alert('Could not get Repricing Information. status: ' + status);
					});
			};

			$scope.EditRepricingInformation = function () {
				var previousRepricingInformationId = $scope.CurrentRepricingInformation.RepricingInformationId;

				if ($scope.CurrentRepricingInformation.ASIN === null) {
					alert('An ASIN must be defined.');
					return;
				}
				
				if ($scope.CurrentRepricingInformation.SKU === null) {
					alert('A SKU must be defined.');
					return;
				}

				$http({
					method: 'POST',
					url: '../api/RepricingInformation/Edit',
					data: angular.toJson($scope.CurrentRepricingInformation),
					dataType: 'json',
					contentType: 'application/json'
				})
				.success(function (data) {
					$scope.CurrentRepricingInformation = data;

					$scope.IsCreate = false;

					if (previousRepricingInformationId === data.RepricingInformationId) {
						alert('Saved changes successfully.');
					} else {
						alert('Created Repricing Information successfully.');
					}
				}).error(function (data, status) {
					alert('Could not edit/create the Repricing Information. Status: ' + status);
				});
			};

		}]);