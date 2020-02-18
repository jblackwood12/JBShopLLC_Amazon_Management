$(document).ready(function () {
	var asin = $('#ASINValue').val();

	// Clear out all of the containers.
	$("#ProductSalesGraphContainer").empty();
	$("#ProductQuantityGraphContainer").empty();
	$("#PriceHistoryGraphContainer").empty();

	PlotSalesByDay(asin);
	PlotInventoryByDay(asin);
	PlotPriceHistory(asin);
});

function PlotSalesByDay(asin) {
	$.ajax({
		url: "/api/ProductReport/GetProductSalesByDay",
		type: "GET",
		data: { asin: asin },
		dataType: "json",
		success: function (data) {

			var salesOptions = {
				series: {
					stack: true
				},
				xaxis: { mode: "time", timeformat: "%m/%d", minTickSize: [1, "day"] },
				yaxis: {
					tickFormatter: function numberWithCommas(x) {
						return x.toFixed(2).toString().replace(/\B(?=(?:\d{3})+(?!\d))/g, ",");
					}
				},
				grid: { hoverable: true },
				legend: { show: true },
				tooltip: true,
				tooltipOpts: {
					content: function (label) {
						return label + ": %y <br> Date: %x";
					}
				}
			};

			if (data.CombinedSeries.length > 0) {
				var ds = new Array();

				var barData = {
					label: data.CombinedSeries[0].Label,
					data: data.CombinedSeries[0].Data,
					bars: {
						show: true,
						barWidth: 43200000,
						align: 'center',
						fill: 1.0,
						lineWidth: 0
					}
				};

				ds.push(barData);

				var lineData = {
					label: data.MovingAverageSeries.Label,
					data: data.MovingAverageSeries.Data,
					color: '#1E90FF',
					lines: {
						show: true,
						align: 'center'
					},
					shadowSize: 0
				};

				ds.push(lineData);

				$.plot($("#ProductSalesGraphContainer"), ds, salesOptions);
			}
		}
	});
}

function PlotInventoryByDay(asin) {
	$.ajax({
		url: "/api/ProductReport/GetProductInventoryByDay",
		type: "GET",
		data: { asin: asin },
		dataType: "json",
		success: function (data) {

			var salesOptions = {
				series: {
					stack: true
				},
				xaxis: { mode: "time", timeformat: "%m/%d", minTickSize: [1, "day"] },
				yaxis: {
					tickFormatter: function numberWithCommas(x) {
						return x.toFixed(2).toString().replace(/\B(?=(?:\d{3})+(?!\d))/g, ",");
					}
				},
				grid: { hoverable: true },
				legend: { show: true },
				tooltip: true,
				tooltipOpts: {
					content: function (label) {
						return label + ": %y <br> Date: %x";
					}
				}
			};

			if (data.CombinedSeries.length > 0) {
				var ds = new Array();

				var lineData = {
					label: data.CombinedSeries[0].Label,
					data: data.CombinedSeries[0].Data,
					lines: {
						show: true,
						align: 'center'
					},
					shadowSize: 0
				};

				ds.push(lineData);

				$.plot($("#ProductQuantityGraphContainer"), ds, salesOptions);
			}
		}
	});
}

function PlotPriceHistory(asin) {
	$.ajax({
		url: "/api/ProductReport/GetPriceHistory",
		type: "GET",
		data: { asin: asin },
		dataType: "json",
		success: function (data) {

			var salesOptions = {
				series: {
					stack: true
				},
				xaxis: { mode: "time", timeformat: "%m/%d", minTickSize: [1, "day"] },
				yaxis: {
					tickFormatter: function numberWithCommas(x) {
						return '$' + x.toFixed(2).toString().replace(/\B(?=(?:\d{3})+(?!\d))/g, ",");
					}
				},
				grid: { hoverable: true },
				legend: { show: true },
				tooltip: true,
				tooltipOpts: {
					content: function (label) {
						return label + ": %y <br> Date: %x";
					}
				}
			};

			if (data.CombinedSeries.length > 0) {
				var ds = new Array();

				var lineData = {
					label: data.CombinedSeries[0].Label,
					data: data.CombinedSeries[0].Data,
					lines: {
						show: true,
						align: 'center'
					},
					shadowSize: 0
				};

				ds.push(lineData);

				$.plot($("#PriceHistoryGraphContainer"), ds, salesOptions);
			}
		}
	});
}
