var issueListItems = [];
let currentQty;
let productCode
$("#selectedProduct").change(function () {
	currentQty = $('option:selected', this).attr('data-qty');
	productCode = $('option:selected', this).attr('data-pcode')
	$('#currentQuantity').val(currentQty);
	$('#productCode').val(productCode)
});

let unitOfMeasure;
$('#selectedProductItem').change(function () {
	unitOfMeasure = $('option:selected', this).attr('data-uom');
	$('#unitOfMeasure').val(unitOfMeasure);
});

let sellingPrice;
$("#productItem").change(function () {
	sellingPrice = $('option:selected', this).attr('data-price')
	$('#sellingPrice').val(sellingPrice)
	$("#quantityBought").focus();
});
$('#issBtnAdd').click(function (event) {
	event.preventDefault();
	addItemToList();
	$('#productListForm').trigger('reset');
})

var addItemToList = function () {
	var productId = $("#productItem option:selected").val();
	var itemName = $("#productItem option:selected").text();
	var quantity = $("#quantityBought").val();
	var unitPrice = $("#sellingPrice").val();
	var avQuantity = $("#productItem option:selected").attr("data-qty");
	quantity = parseFloat(quantity)
	avQuantity = parseFloat(avQuantity)
	var discount = 0;
	var taxAmount = 0;
	if (productId === "") {
		$("#issMsg").text("select an item first").addClass('text-danger');
		$("#issItemId").focus();
	}
	else if (quantity === "") {
		$("#issMsg").text("quantity is required").addClass('text-danger');
		$("#quantityBought").focus();
	}
	else if (unitPrice === "") {
		$("#issMsg").text("price is required").addClass('text-danger');
		$("#issWarehouseId").focus();
	}
	else if (quantity > avQuantity) {
		$("#issMsg").text("available quantity is " + avQuantity).addClass('text-danger');
		$("#quantityBought").focus();
	}
	else {
		$("#issMsg").text("");
		var listItem = {
			"productId": productId,
			"quantity": quantity,
			"unitPrice": unitPrice,
			"discount": discount,
			"taxAmount": taxAmount,
			"itemName": itemName
		}
		issueListItems.push(listItem);
		createIssueListTable();
		$("#issListRecords").val(JSON.stringify(issueListItems));
		$("#quantityBought").val("");
	}
};
var createIssueListTable = function () {
	$("#IssueListTable").html("");
	var tr = "";
	var total = 0;
	var index = 0;
	$.each(issueListItems, function () {
		var trTotal = this.quantity * this.unitPrice;
		total += trTotal;
		tr += '<tr><td>' + this.itemName + '</td><td>' + this.quantity + '</td><td>' + this.unitPrice + '</td><td>' + trTotal + '</td>'
			+ '<td><button class="btn btn-danger btn-sm" onclick="removeListItem(' + index + ')">Remove</button> </td > </tr > ';
		index++;
	});

	issueListItems.length > 0 ? $("#btnSumbitIss").prop("disabled", false) : $("#btnSumbitIss").prop("disabled", true);
	$("#IssueListTable").html(tr);
	$("#issTotal").text(total.toLocaleString());
};

var list = $("#issListRecords").val();
if (list) {
	issueListItems = JSON.parse(list);
	createIssueListTable();
}
function removeListItem(index) {
	issueListItems.splice(index, 1);
	createIssueListTable();
};
$('#dateTo').datetimepicker({
	format: 'DD-MMM-yyyy'
});
$('#dateFrom').datetimepicker({
	format: 'DD-MMM-yyyy'
});
$('#timepicker').datetimepicker({
	format: 'LT'
})
$('.select2').select2()
$('.select2bs4').select2({
	theme: 'bootstrap4'
})

var table = $('#receiptsdt').DataTable({
	"paging": true,
	"lengthChange": false,
	"searching": false,
	"ordering": true,
	"info": true,
	"autoWidth": false,
	"responsive": true,
});
function createRow(item) {
	return '<tr>' +
		'<td>' + item.Product.Name + '</td>' +
		'<td>' + item.Quantity + '</td>' +
		'<td>' + item.SellingPrice + '</td>' +
		'<td>' + item.Amount + '</td>'
		+ '</tr>';
}
function appendTtRow(items) {
	let total = 0;
	for (let item of items) 
		total += (item.Quantity * item.SellingPrice)

	return '<tr>' +
		'<td></td>' +
		'<td></td>' +
		'<td>Total:</td>' +
		'<td>' + total + '</td>'
		+ '</tr>';
}
function format() {
	return '<table cellpadding="5" cellspacing="0" border="0" style="padding-left:50px;">' +
		'<thead>' +
		'<tr>' +
		'<th>Product</th>' +
		'<th>Quantity</th>' +
		'<th>Price</th>' +
		'<th>Amount</th>' +
		'</tr>' +
		'</thead>' +
		'<tbody class="receipt-items">'+
		'<tr>' +
		'<td></td>' +
		'<td></td>' +
		'<td></td>' +
		'<td></td>' +
		'</tr>' +
		'</tbody>'+
		'</table>';
}

$('#receiptsdt tbody').on('click', 'td.details-control', function () {
	var tr = $(this).closest('tr');
	var row = table.row(tr);
	if (row.child.isShown()) {
		row.child.hide();
		tr.removeClass('shown');
	}
	else {
		var items = tr.data()['items'];
		var totalRow = appendTtRow(items);
		row.child(format()).show();
		for (let item of items) {
			var tableRow = createRow(item);
			$("table .receipt-items").append(tableRow);
		}
		$("table .receipt-items").append(totalRow);
		tr.addClass('shown');
	}
});

$("input[value='credit']").prop('checked', false);
populateSelect(false);
$("#credit-sale").on("click", function () {
	let checked = $("input[value='credit']").is(':checked')? true : false;
	populateSelect(checked);
});
function populateSelect(checked) {
	console.log(checked);
	if (!checked) {
		$('#customer-select')
			.empty()
			.append('<option selected="selected" value="walkIn">WALK-IN</option>');
	} else {
		$('#customer-select')
			.empty();
	}
}
