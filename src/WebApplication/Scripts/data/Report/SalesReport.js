$(document).ready(function () {
	PlotSales();
});

function PlotSales() {
	$.ajax({
		url: "/api/statistics/GetSalesByDay",
		type: "GET",
		contentType: "application/json",
		dataType: "json",
		success: function(data) {

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
					content: function(label) {
						return label + ": %y <br> Date: %x";
					}
				}
			};

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

			$.plot($("#SalesChart"), ds, salesOptions);
		}
	});
}

