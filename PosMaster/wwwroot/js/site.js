var issueListItems = [];
let currentQty;
let productCode;
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
    $("#issListRecords").val(JSON.stringify(issueListItems));
};

let customerId;
$('#customerId').change(function () {
    customerId = $('option:selected', this).attr('data-customer');
    $('#order-customer').val(customerId);
});

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
        '<tbody class="receipt-items">' +
        '<tr>' +
        '<td></td>' +
        '<td></td>' +
        '<td></td>' +
        '<td></td>' +
        '</tr>' +
        '</tbody>' +
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

function populateSelect(checked) {
    //if (!checked) {
    //	$('#customer-select')
    //		.empty()
    //		.append('<option selected="selected" value="walkIn">WALK-IN</option>');
    //} else {
    //	$('#customer-select')
    //		.empty();
    //}
}


$('#customer-select').select2({
    ajax: {
        dataType: 'json',
        delay: 250,
        url: function (params) {
            return '/Customers/Search?term=' + params.term + '&cId=' + $("#clientId").val();
        },
        processResults: function (data, params) {
            return {
                results: data.data
            };
        },
        cache: true
    },

    placeholder: 'Search by name, Id number or phone number',
    minimumInputLength: 1
});


let orderId;
$('.launch-modal').click(function (event) {
    event.preventDefault();
    let modalTitle = $(this).attr('data-order-name');
    orderId = $(this).attr('data-order-id');
    $('#modal-default .modal-title').text(modalTitle);
})
$("#fulfil-order").click(function () {
    $.post('/Orders/FulFilOrder', { id: orderId }, function (result) {
        console.log(result);
    });
    $('#modal-default').modal('hide');
});

var ul = $("#low-stock-ul");
var x = document.getElementById('revenue-chart-canvas');
if (x) {
    var salesChartCanvas = document.getElementById('revenue-chart-canvas').getContext('2d')
    $.get('/Reports/MonthlySalesReport').done(function (data) {
        let sales = new Object();
        sales = getSales(data);
        plot(sales);
    });
}

function getSales(response) {
    let sales = response['data'];
    let months = [];
    let totalSales = [];
    let expProfits = [];
    let actualProfits = [];
    for (let sale of sales) {
        months.push(sale['month']);
        totalSales.push(sale['totalSales']);
        expProfits.push(sale['expectedProfit']);
        actualProfits.push(sale['actualProfit']);
    }

    let salesObj = {
        months: months, totalSales: totalSales, expProfits: expProfits, actualProfits: actualProfits
    };
    return salesObj;
}

function plot(sales) {
    var salesChartData = {
        labels: sales['months'],
        datasets: [
            {
                label: 'Total Sales',
                backgroundColor: 'rgba(60,141,188,0.9)',
                borderColor: 'rgba(60,141,188,0.8)',
                pointRadius: false,
                pointColor: '#3b8bba',
                pointStrokeColor: 'rgba(60,141,188,1)',
                pointHighlightFill: '#fff',
                pointHighlightStroke: 'rgba(60,141,188,1)',
                data: sales['totalSales']
            },
            {
                label: 'Expected Profit',
                backgroundColor: 'rgba(210, 214, 222, 1)',
                borderColor: 'rgba(210, 214, 222, 1)',
                pointRadius: false,
                pointColor: 'rgba(210, 214, 222, 1)',
                pointStrokeColor: '#c1c7d1',
                pointHighlightFill: '#fff',
                pointHighlightStroke: 'rgba(220,220,220,1)',
                data: sales['expProfits']
            }
        ]
    }

    var salesChartOptions = {
        maintainAspectRatio: false,
        responsive: true,
        legend: {
            display: false
        },
        scales: {
            xAxes: [{
                gridLines: {
                    display: false
                }
            }],
            yAxes: [{
                gridLines: {
                    display: false
                }
            }]
        }
    }

    var salesChart = new Chart(salesChartCanvas, {
        type: 'line',
        data: salesChartData,
        options: salesChartOptions
    })
}

$('#calendar').datetimepicker({
    format: 'L',
    inline: true
})

$(".day").click(function () {
    //var date = $(this).attr("data-day").replace(/[\W_]/g, "-");
    var date = $(this).attr("data-day").split('/');
    var str = date[0] + '-' + date[1] + '-' + date[2];
    var date_ = new Date(str).toString().split(" ");
    var xstr = date_[2] + '-' + date_[1] + '-' + date_[3];
    window.location.href = '/Reports/SalesReport?dtFrom=' + xstr + '&dtTo=' + xstr + '&option=Weekly';
});

if (x) {
    $.get('/Products/LowStockProducts').done(function (data) {
        if (data.success) {
            data.data.forEach(element => {
                ul.append('<li><span class= "text">[' + element.code + ']' + element.name + ' [Av. ' + element.availableQuantity + ' ' + element.unitOfMeasure
                    + ']</span > <small class="badge badge-info">' + element.productCategory.name + '</small>  </li >');
            });
        }
    });
}

if (x) {
    var labels = [];
    var donutData = [];
    $.get('/Products/TopSellingByVolume').done(function (data) {
        if (data.success) {
            data.data.forEach(element => {
                labels.push(element.product.code + '-' + element.product.name + ' [' + element.product.productCategory.name + ']');
                donutData.push(element.volume);
            });
            plotDoughnut(labels, donutData);
        }
    });

}
function plotDoughnut(labels, data) {
    var donutChartCanvas = $('#donutChart').get(0).getContext('2d')
    var donutData = {
        labels: labels,
        datasets: [
            {
                data: data,
                backgroundColor: ['#f56954', '#00a65a', '#f39c12', '#00c0ef', '#3c8dbc', '#d2d6de'],
            }
        ]
    }
    var donutOptions = {
        maintainAspectRatio: false,
        responsive: true,
    }
    var donutChart = new Chart(donutChartCanvas, {
        type: 'doughnut',
        data: donutData,
        options: donutOptions
    })
}
$('#credit-sale').change(function () {
    if (this.checked) {
        $('#payment-mode').hide();
    } else { 
        $('#payment-mode').show();
    }
});


$('#is-service').change(function () {
    if (this.checked) {
        $('#product-related').hide();
    } else { 
        $('#product-related').show();
    }
});


$("#item-price-select").change(function () {
    var instanceId = $(this).val();
     $.get(`/Products/GetInstanceProducts/${instanceId}`).done(function (data) {
         if (data) {
            var items = data.data;
            var select = $("#items-price-dropdown").empty()
                .append($('<option>').text("-- Please select --").attr('value', ""));;
            $.each(items, function () {
                if (this.status === 1) {
                    select.append($('<option>').text(this.name).attr('value', this.id)
                        .attr('data-buy', this.buyingPrice)
                        .attr('data-sell', this.sellingPrice)
                        .attr('data-priceStart', this.priceStartDate)
                        .attr('data-priceEnd', this.priceEndDate)
                    );
                }
            });
        }
    });
})

function addProductItemToList()
{
    $('#items-price-dropdown')
}

 