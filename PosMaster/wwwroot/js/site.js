﻿var item = {};
var issueListItems = [];
$("#selectedProductAdj").change(function () {
    item = {};
    item = $(this).select2('data')[0];
    var productCode = item.code;
    $('.currentQuantity').val(item.quantity);
    $('.currentPrice').val(item.buyingPrice);
    $('.currSellingPrice').val(item.sellingPrice);
    $('#inpPriceDateFrom').val(item.priceStartDate);
    $('#inpPriceDateTo').val(item.priceEndDate);
    $('#productCode').val(productCode)
});

let unitOfMeasure;
$('#selectedProductItem').change(function () {
    unitOfMeasure = $('option:selected', this).attr('data-uom');
    $('#unitOfMeasure').val(unitOfMeasure);
});

let sellingPrice;
$("#product-select").change(function () {
    item = {};
    item = $(this).select2('data')[0];
    var allowDisc = item.allowDiscount;
    $('#unitPrice').val(item.sellingPrice)
    $('#unitPrice').attr("readonly", !allowDisc);
    $("#quantityBought").focus();
});

$('#issBtnAdd').click(function (event) {
    item = {};
    event.preventDefault();
    addItemToList();
})

$(".posForm-number").keyup(function (evt) {
    evt = evt || window.event;
    if (evt.keyCode == 13) {
        addItemToList();
    }
});

var addItemToList = function () {
    item = {};
    var isCards = $("#inpShowCards").val();
    if (isCards)
        item = JSON.parse(atob($("#inpProductEncoded").val()));
    else
        item = $('#product-select').select2('data')[0];
    var productId = item.id;
    var itemName = item.name;
    var quantity = $("#quantityBought").val();
    var unitPrice = $("#unitPrice").val();
    var sellingPrice = item.sellingPrice;;
    var avQuantity = item.quantity;
    var taxAmount = item.tax;
    quantity = parseFloat(quantity)
    avQuantity = parseFloat(avQuantity)
    var discount = (sellingPrice - unitPrice) * quantity;
    var fa = '<i class="fa fa-times-circle"></i> ';
    $("#issMsg").html('');
    if (productId === "") {
        $("#issMsg").html(fa + " Select an item first")
            .addClass('text-danger breadcrumb');
        $("#issItemId").focus();
        return
    }
    else if (!quantity || quantity <= 0) {
        $("#issMsg").html(fa + " Quantity is required")
            .addClass('text-danger breadcrumb');
        $("#quantityBought").focus();
        return
    }
    else if (unitPrice === "") {
        $("#issMsg").html(fa + " Price is required")
            .addClass('text-danger breadcrumb');
        $("#issWarehouseId").focus();
        return
    }
    else if (quantity > avQuantity) {
        $("#issMsg").html(fa + " Available quantity is " + avQuantity)
            .addClass('text-danger breadcrumb');
        $("#quantityBought").focus();
        return
    }
    else {
        $("#issMsg").text("");
        $("#issMsg").removeClass("breadcrumb");
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
        $('#product-select').empty();
        $("#quantityBought").val("");
        $("#unitPrice").val("");
        $('#productListForm').trigger('reset');
        if (isCards) {
            $("#inpSearchProduct").val('');
            $("#selectProductModal").modal('hide');
        } else {
            $('#product-select').select2('open');;
        }

    }
};
var createIssueListTable = function () {
    $("#IssueListTable").html("");
    var tr = "";
    var total = 0;
    var totalTax = 0;
    var totalDiscount = 0;
    var index = 0;
    $.each(issueListItems, function () {
        var trTotal = this.quantity * this.unitPrice;
        var lineTax = (trTotal * this.taxAmount * 100) / (this.taxAmount * 100 + 100);
        total += trTotal;
        totalDiscount += this.discount;
        totalTax += lineTax;
        tr += '<tr><td>' + this.itemName + '</td><td>' + this.quantity + '</td><td>' + this.unitPrice + '</td><td>' + trTotal + '</td>'
            + '<td><button class="btn btn-danger btn-sm" onclick="removeListItem(' + index + ')">Remove</button> </td > </tr > ';
        index++;
    });

    if (issueListItems.length > 0) {
        $("#btnSumbitIss").prop("disabled", false)
    } else $("#btnSumbitIss").prop("disabled", true);
    if (totalDiscount < 0)
        totalDiscount = 0;
    $("#IssueListTable").html(tr);
    $("#issTotal").text((total).toLocaleString(2));
    $("#issDiscount").text(totalDiscount.toLocaleString(2));
    $("#issTax").text(totalTax.toLocaleString(2));
    var isCredit = $('#credit-sale-check').val();
    if (isCredit == true)
        $("#posAmountRcvd").val(0);
    else
        $("#posAmountRcvd").val(total);
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

$('#customer-select').change(function () {
    var customer = $('#customer-select').select2('data')[0];
    $('#order-customer').val(customer.id);
    $('#refNoVal').val(customer.pin);
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

$("#dataTable").DataTable();

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
        '<td>' + item.UnitPrice + '</td>' +
        '<td>' + item.Amount + '</td>'
        + '</tr>';
}
function appendTtRow(items) {
    let total = 0;
    for (let item of items)
        total += (item.Quantity * item.UnitPrice)

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

var isSupplier = false;
var x = $("#inpIsSupplier").val();
if (x) isSupplier = true;
$('#customer-select').select2({
    ajax: {
        dataType: 'json',
        delay: 250,
        url: function (params) {
            return '/Customers/Search?term=' + params.term + '&isSupplier=' + isSupplier;
        },
        processResults: function (data, params) {
            return {
                results: $.map(data.data, function (item) {
                    return {
                        text: item.text,
                        id: item.id,
                        code: item.code,
                        pin: item.pinNo
                    }
                })
            };
        },
        cache: true
    },

    placeholder: 'Search by name, Id number or phone number',
    minimumInputLength: 1
});


$('.launch-modal').click(function (event) {
    event.preventDefault();
    let modalTitle = $(this).attr('data-order-name');
    $('#modal-default .modal-title').text(modalTitle);
})

$("#fulfil-order").click(function () {
    $.post('/Orders/FulFilOrder', { id: orderId }, function (result) {
    });
    $('#modal-default').modal('hide');
});

var ul = $("#low-stock-ul");
var x = document.getElementById('revenue-chart-canvas');
if (x) {
    var dFrom = $("#inpDateFrom").val();
    var dTo = $("#inpDateTo").val();
    var inId = $("#inpInstanceId").val();
    var salesChartCanvas = document.getElementById('revenue-chart-canvas').getContext('2d')
    $.get('/Reports/MonthlySalesReport?dtFrom=&dtTo=' + dTo + '&instanceId=' + inId).done(function (data) {
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
    //window.location.href = '/Reports/SalesReport?dtFrom=' + xstr + '&dtTo=' + xstr + '&option=Weekly';
});

if (x) {
    var inId = $("#inpInstanceId").val();
    $.get('/Products/LowStockProducts?inId=' + inId).done(function (data) {
        if (data.success) {
            data.data.forEach(element => {
                ul.append('<li><span class= "text">[' + element.code + ']' + element.name + ' [Qty ' + element.availableQuantity + ' ' + element.uom
                    + ']</span > <small class="badge badge-info">' + element.productCategory.name + '</small>  </li >');
            });
        } else {
            ul.append('<li><span class= "text"> No products below re-order level </span>'
                + '<small class="badge badge-info">' + data.message + '</small>  </li> ');
        }
    });
}

var donut = document.getElementById('donutChart');
if (donut) {
    var labels = [];
    var donutData = [];
    var dFrom = $("#inpDateFrom").val();
    var dTo = $("#inpDateTo").val();
    var inId = $("#inpInstanceId").val();
    $.get('/Products/TopSellingByVolume?dtFrom=' + dFrom + '&dtTo=' + dTo + '&inId=' + inId).done(function (data) {
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

$(".hide-this").hide();

$('#credit-sale-check').change(function () {
    $('#payment-mode').attr("required", !this.checked);
    if (this.checked) {
        $('.cash-sale').hide();
    } else {
        $('.cash-sale').show();
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


$("#inpRcptCode").blur(function () {
    var code = $(this).val();
    $.get(`/PointOfSale/ReceiptByCode?code=${code}`).done(function (data) {
        if (data.success) {
            $("#inpRcptAmt").val(data.data.amount);
        }
    });
})

removeGradingSchemeRow = function (id) {
    document.getElementById("tr_" + id).remove();
};

$("#btnSearchProduct").click(function (e) {
    e.preventDefault();
    getPosCardProducts();
});

$("#inpSearchProduct").keyup(function (e) {
    e.preventDefault();
    getPosCardProducts();
});

var addCardProduct;
var getPosCardProducts = function () {
    var code = $("#inpSearchProduct").val();
    var isPos = $("#inpIsPos").val();
    var div = $("#divPosCard");
    if (code === '') {
        $("#inpSearchProduct").focus();
        div.html('');
    } else {
        div.html('<p class="alert alert-info"><i class="fa fa-spinner fa-spin"></i> searching ' + code + '. please wait ...</p>')
        $.get('/Products/Search?isPos=' + isPos + '&term=' + code + '&cardView=True', function (data) {
            var message = data.message;
            if (!data.success) {
                var p = '<p class="alert alert-warning">' + message + '</p>';
                div.html(p);
            } else {
                var cardBody = '';
                $.each(data.data, function () {
                    var product = {
                        quantity: this.availableQuantity,
                        sellingPrice: this.sellingPrice,
                        buyingPrice: this.buyingPrice,
                        id: this.id,
                        tax: this.taxRate,
                        name: this.name,
                        allowDiscount: this.allowDiscount,
                        code: this.code,
                    };
                    var card = '<div class="col-md-6" id="modal-product-' + this.id + '" data="' + btoa(JSON.stringify(product)) + '"'
                        + 'onclick = "addCardProduct(`' + this.id + '`)" style="cursor: pointer"> <div class="small-box bg-info">'
                        + '<div class="inner"><h4>' + this.code + '</h4>  <p>' + this.name + ' (' + this.availableQuantity + ' ' + this.uom + ')</p></div>'
                        + '<div class="icon"><i class="fa fa-shopping-cart"></i> </div>'
                        + '<p class="small-box-footer">Price: ' + this.sellingPrice.toFixed(2) + '</p></div></div>';
                    cardBody += card;
                });
                div.html(cardBody);
            }
        });
    }
};

addCardProduct = function (id) {
    $("#inpProductEncoded").val("")
    $("#unitPrice").val("")
    $("#quantityBought").val("")
    $("#hProductTitle").text("Product")
    var productEncoded = $("#modal-product-" + id).attr("data");
    var product = JSON.parse(atob(productEncoded))
    var price = product.sellingPrice;
    var allowDisc = product.allowDiscount;
    $("#unitPrice").val(price);
    $("#inpProductEncoded").val(productEncoded)
    $('#unitPrice').attr("readonly", !allowDisc);
    $("#hProductTitle").text(product.code + ' - ' + product.name);
    $("#selectProductModal").modal('show');
};

$("#btnAddModalProduct").click(function (e) {
    e.preventDefault();
    addItemToList();
});

$('.product-select-search').select2({
    ajax: {
        dataType: 'json',
        delay: 250,
        url: function (params) {
            var isPos = $("#inpIsPos").val();
            return '/Products/Search?isPos=' + isPos + '&term=' + params.term;
        },
        processResults: function (data, params) {
            return {
                results: $.map(data.data, function (item) {
                    return {
                        text: item.code + ' - ' + item.name + ' (' + item.availableQuantity + ' ' + item.uom + ' )',
                        quantity: item.availableQuantity,
                        sellingPrice: item.sellingPrice,
                        buyingPrice: item.buyingPrice,
                        id: item.id,
                        tax: item.taxRate,
                        name: item.name,
                        allowDiscount: item.allowDiscount,
                        code: item.code,
                        priceStartDate: item.priceStartDateStr,
                        priceEndDate: item.priceEndDateStr
                    }
                })
            };
        },
        cache: true
    },

    placeholder: 'Search by product code or name',
    minimumInputLength: 1
});

var printDiv = function (divName) {
    var printContents = document.getElementById(divName).innerHTML;
    document.body.style.cssText = '#printReceiptDiv {  color: red }';
    document.body.innerHTML = printContents;
    window.print();
    location.reload();
};

function printElem(elem) {
    var mywindow = window.open('', 'PRINT', 'height=400,width=600');
    mywindow.document.write('<html><head><title>' + document.title + '</title>');
    mywindow.document.write("<link rel=\"stylesheet\" href=\"~/dist/css/adminlte.min.css\" type=\"text/css\"/>");
    mywindow.document.write('</head><body >');
    mywindow.document.write('<h1>' + document.title + '</h1>');
    mywindow.document.write(document.getElementById(elem).innerHTML);
    mywindow.document.write('</body></html>');

    mywindow.document.close(); // necessary for IE >= 10
    mywindow.focus(); // necessary for IE >= 10*/

    mywindow.print();
    mywindow.close();

    return true;
}


$("#btnPrint").click(function (e) {
    e.preventDefault();
    window.print();
    location.reload();
});

$("#btnPrintReceipt").click(function () {
    var isPrinted = $(this).attr("data-printed");
    var id = $(this).attr("data-id");
    $.get("/PointOfSale/PrintReceipt/" + id, function (data) {

    });
    window.print();
    location.reload();
});

function addPoItem() {
    var product = $('#selectedProductAdj').select2('data')[0];
    var itemName = product.name;
    var productIdStr = '"' + product.id + '"';
    var length = document.getElementById("tablePurchaseItems").tBodies[0].rows.length;
    $("#purchaseOrderItemList").append("<tr id='tr_" + product.id + "'>"
        + "<td><input class=''  value=" + product.id + " name='PurchaseOrderItems[" + length + "].ProductId' type='hidden'/>" +
        "<span class='' id='product-name' value=" + itemName + " name= 'PurchaseOrderItems[" + length + "].ProductName'>" + itemName + "</span></td>"
        + "<td><input class='product-event numbers-only' id='product-quantity' name= 'PurchaseOrderItems[" + length + "].Quantity' placeholder='0.00'/></td>"
        + "<td><input class='product-event numbers-only' id='product-price' name= 'PurchaseOrderItems[" + length + "].UnitPrice' placeholder='0.00'/></td>"
        + "<td><strong class='poTotalItems' name= 'PurchaseOrderItems[" + length + "].Amount'>---</strong></td>"
        + "<td><button type='button' class='btn bg-red btn-sm' onclick='removePoItemRow(" + productIdStr + ")'>Remove</button></td>"
        + "</tr>"
    );

    $('#selectedProductAdj').empty();
}

$('#purchaseOrderItemList').on('change', 'input.product-event', function () {
    let amount = 0.00;
    let quantity = $(this).closest('tr').find('#product-quantity').val();
    let price = $(this).closest('tr').find('#product-price').val();
    if (quantity == '' || price == '') {
        $(this).closest('tr').find('.poTotalItems').val(parseFloat(amount));
    }
    amount = price * quantity;
    $(this).closest('tr').find('.poTotalItems').text(parseFloat(amount));

    updateTotal();
})

function updateTotal() {
    let total = 0;
    $('.poTotalItems').each((i, el) => total += parseFloat(el.textContent.trim() || 0));
    $('#poTotalAmount').text(total.toFixed(2));
}

removePoItemRow = function (id) {
    document.getElementById("tr_" + id).remove();
};

$("#selRcptUserType").change(function () {
    $("#spUserType").text($(this).val());

});

$(".selRcptUser").change(function () {
    var type = $("#selRcptUserType").val();
    var userId = $("#customer-select").val();
    $.get('/Customers/Balance?id=' + userId + '&type=' + type).done(function (data) {
        $("#spAvailCredit").text(data.data.availableCredit);
        $("#spExpAmount").text(data.data.expectedAmount);
        $("#inpTotalCredit").val(data.data.creditAmount);
        $("#inpTotalDebit").val(data.data.debitAmount);
    });
});
