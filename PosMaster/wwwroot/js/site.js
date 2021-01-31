// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function() {
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
});
