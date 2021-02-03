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
    var discount = 0;
    var taxAmount = 0;

    if (productId === "") {
        $("#issMsg").text("select an item first");
        $("#issItemId").focus();
    }
    else if (quantity === "") {
        $("#issMsg").text("quantity is required");
        $("#quantityBought").focus();
    }
    else if (unitPrice === "") {
        $("#issMsg").text("price is required");
        $("#issWarehouseId").focus();
    }
    else if (parseInt(quantity) > parseInt(avQuantity)) {
        $("#issMsg").text("available quantity is " + avQuantity);
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
//Timepicker
$('#timepicker').datetimepicker({
    format: 'LT'
})