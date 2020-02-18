$(document).ready(function() {
	LoadTotalInventoryValue();
});

function LoadTotalInventoryValue() {
	$.ajax({
		url: "/api/statistics/getinventoryvalue",
		type: "GET",
		contentType: "application/json",
		dataType: "json",
		success: function(data) {
			$('#totalInventoryValue').html(accounting.formatMoney(data, "$", 2, ",", "."));
		}
	});
}