var CurrentBusinnessPartnerPage = 1;





var idleTime = 0;
var maxIdle = 2; // minutes

// reset idle time on events
function resetIdle() {
    idleTime = 0;
}

setInterval(function () {
    idleTime++;
   
    $.ajax({
        url: "/Account/CheckIsIdle",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify({}),
        success: function (response) {
            if (response.success) {
                window.location.href = "/Account/Login";
            }
        }
    });       
    
}, 20000); // check every minute






function callGlobalMethod() {
//        alert('majid');
}

// Run every 30 seconds
setInterval(callGlobalMethod, 15000);

function FillCity() {
    var stateId = $('#DropDownProvinceId').val();
    $.ajax({
        url: '/Common/GetCities',
        type: "Post",
        dataType: "JSON",
        data: { id: stateId },
        success: function (cities) {
            $("#DropDownCityId").html(""); // clear before appending new list 
            $("#DropDownCityId").append('<option value>' + "انتخاب شهر" + '</option>');
            for (var j = 0; j < cities.length; j++) {
                $("#DropDownCityId").append('<option value=' + cities[j].Value + '>' + cities[j].Text + '</option>');
            }
        }
    });
}
function add_commas(number) {
    return number.toString().replace(/[^0-9.]/g, '').replace(/\B(?=(\d{3})+(?!\d))/g, ",");
}
function remove_commas(number) {
    return number ? parseFloat(number.replace(/[^0-9.]/g, '')) : 0;
}

function checkBackend() {
    $.ajax({
        url: '/Home/CheckIntegrity',
        type: 'GET',
        success: function (data) {
            console.log("Backend check:", data);

            if (data.status === "false") {

                $("#GenerelModal .modal-body").html(data.message);
                $('#GenerelModal').modal({
                    backdrop: 'static',
                    keyboard: false
                }).modal('show'); // 👈 show the modal
            } else {
                
            }
        },
        error: function () {
            
        }
    });
}

//setInterval(checkBackend, 120000);


function FillProduct() {

    var token = $('input[name="__RequestVerificationToken"]').val();
    var storeId = $('#StoreId').val();
    var data = {
        __RequestVerificationToken: token,
        storeId: storeId

    }
    $.ajax({
        type: "POST",
        url: "/StoreReceipt/FillProduct",
        data: data,
        dataType: "JSON",
        success: function (cities) {
            $('#ProductId').empty();
            //            $("#ProductId").append('<option value>' + "انتخاب شهر" + '</option>');
            for (var j = 0; j < cities.length; j++) {
                var data = {
                    id: cities[j].Value,
                    text: cities[j].Text
                };
                var newOption = new Option(data.text, data.id, false, false);
                $('#ProductId').append(newOption).trigger('change');
            }
            GetReceiptNumber(storeId);
        },
        failure: function (response) {
            //            alert(response.responseText);
        },
        error: function (response) {
            //            alert(response.responseText);
        }
    });
}

function GetReceiptNumber(storeId) {
    var token = $('input[name="__RequestVerificationToken"]').val();
    var data = {
        __RequestVerificationToken: token,
        storeId: storeId

    }

    $.ajax({
        type: "POST",
        url: "/StoreReceipt/GetReceiptNumber",
        data: data,
        dataType: "JSON",
        success: function (val) {
            $('#ReceiptNumber').val(val);
            //            $("#ProductId").append('<option value>' + "انتخاب شهر" + '</option>');
        },
        failure: function (response) {
            //            alert(response.responseText);
        },
        error: function (response) {
            //            alert(response.responseText);
        }
    });

}
function FillStoreReceipt() {
    var token = $('input[name="__RequestVerificationToken"]').val();
    var storeId = $('#StoreId').val();
    var data = {
        __RequestVerificationToken: token,
        storeId: storeId

    }
    $.ajax({
        type: "POST",
        url: "/Invoice/FillStoreReceipt",
        data: data,
        dataType: "JSON",
        success: function (cities) {
            $('#StoreReceiptDetailId').empty();
            //            $("#ProductId").append('<option value>' + "انتخاب شهر" + '</option>');
            for (var j = 0; j < cities.length; j++) {
                var data = {
                    id: cities[j].Value,
                    text: cities[j].Text
                };

                var newOption = new Option(data.text, data.id, false, false);
                $('#StoreReceiptDetailId').append(newOption);


            }
            $('#StoreReceiptDetailId').trigger('change');
            GetStoreIsUsagePeriodForce(storeId);

        },
        failure: function (response) {
            //            alert(response.responseText);
        },
        error: function (response) {
            //            alert(response.responseText);
        }
    });
}
function GetStoreIsUsagePeriodForce(val) {
    var token = $('input[name="__RequestVerificationToken"]').val();
    var storeId = val;
    var data = {
        __RequestVerificationToken: token,
        storeId: storeId

    }
    $.ajax({
        type: "POST",
        url: "/Invoice/GetStoreIsUsagePeriodForce",
        data: data,
        dataType: "JSON",
        success: function (res) {

            if (res === 0) {
                $("#lblUsagePeriod").removeClass('required');
            }
            else {
                $("#lblUsagePeriod").addClass('required');
            }
            //            SetSellPrice();
            GetLastInvoiceNumber(val);
        },
        failure: function (response) {
            //            alert(response.responseText);
        },
        error: function (response) {
            //            alert(response.responseText);
        }
    });
}
function GetLastInvoiceNumber(val) {
    var token = $('input[name="__RequestVerificationToken"]').val();
    var storeId = val;
    var data = {
        __RequestVerificationToken: token,
        storeId: storeId

    }
    $.ajax({
        type: "POST",
        url: "/Invoice/GetLastInvoiceNumber",
        data: data,
        dataType: "JSON",
        success: function (res) {

            $("#FactorNumber").val(res);
            //            SetSellPrice();

        },
        failure: function (response) {
            //            alert(response.responseText);
        },
        error: function (response) {
            //            alert(response.responseText);
        }
    });
}

function FillProductSubGroup() {
    var stateId = $('#DropDownProductGroup').val();

    if (stateId) {

        $.ajax({
            url: '/Product/GetProductSubGroup',
            type: "Post",
            dataType: "JSON",
            data: { id: stateId },
            success: function (productSubGroup) {
                $("#DropDownProductSubGroup").html(""); // clear before appending new list 
                $("#DropDownProductSubGroup").append('<option value>' + "انتخاب کنید" + '</option>');
                for (var j = 0; j < productSubGroup.length; j++) {
                    $("#DropDownProductSubGroup").append('<option value=' + productSubGroup[j].Value + '>' + productSubGroup[j].Text + '</option>');
                }
            }
        });
    } else {
        $("#DropDownProductSubGroup").html(""); // clear before appending new list 
        $("#DropDownProductSubGroup").append('<option value>' + "انتخاب کنید" + '</option>');

    }
}




function SearchStore(page) {

    var form = $('#StoreListFrom');
    var token = $('input[name="__RequestVerificationToken"]').val();
    var title = $('#StoreTitle').val();
    var inCharge = $('#StoreInCharge').val();
    var data = {
        __RequestVerificationToken: token,
        Title: title,
        Incharge: inCharge,
        Page: page
    }
    Swal.fire({
        title: 'درحال بارگزاری',
        allowOutsideClick: false,
        allowEscapeKey: false,
        allowEnterKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });
    $.ajax({
        type: "POST",
        url: "/Store/AjaxListStore",
        data: data,
        //        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (response) {


            $('#StoreList').html(response);
            Swal.close();

        },
        failure: function (response) {
            //            alert(response.responseText);
        },
        error: function (response) {
            //            alert(response.responseText);
        }
    });

}
function SearchStoreReceipt(page) {


    var token = $('input[name="__RequestVerificationToken"]').val();
    var factorNumber = $('#FactorNumber').val();
    var receiptNumber = $('#ReceiptNumber').val();
    var requestNumber = $('#RequestNumber').val();
    var businnessPartnerId = $('#BusinnessPartnerId').val();
    var data = {
        __RequestVerificationToken: token,
        factorNumber: factorNumber,
        receiptNumber: receiptNumber,
        requestNumber: requestNumber,
        businnessPartnerId: businnessPartnerId,
        Page: page
    }
    Swal.fire({
        title: 'درحال بارگزاری',
        allowOutsideClick: false,
        allowEscapeKey: false,
        allowEnterKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });
    $.ajax({
        type: "POST",
        url: "/StoreReceipt/AjaxListStoreReceipt",
        data: data,
        //        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (response) {


            $('#StoreReceiptList').html(response);
            Swal.close();

        },
        failure: function (response) {
            //            alert(response.responseText);
        },
        error: function (response) {
            //            alert(response.responseText);
        }
    });

} function SearchStoreReceiptReturn(page) {


    var token = $('input[name="__RequestVerificationToken"]').val();
    var FactorNumber = $('#FactorNumber').val();
    var data = {
        __RequestVerificationToken: token,
        FactorNumber: FactorNumber,
        Page: page
    }
    Swal.fire({
        title: 'درحال بارگزاری',
        allowOutsideClick: false,
        allowEscapeKey: false,
        allowEnterKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });
    $.ajax({
        type: "POST",
        url: "/StoreReceipt/AjaxListStoreReceiptReturn",
        data: data,
        //        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (response) {


            $('#StoreReceiptList').html(response);
            Swal.close();

        },
        failure: function (response) {
            //            alert(response.responseText);
        },
        error: function (response) {
            //            alert(response.responseText);
        }
    });

}
function SearchProductBalance(page) {


    var token = $('input[name="__RequestVerificationToken"]').val();
    var StoreId = $('#StoreId').val();
    var data = {
        __RequestVerificationToken: token,
        StoreId: StoreId,
        Page: page
    }
    Swal.fire({
        title: 'درحال بارگزاری',
        allowOutsideClick: false,
        allowEscapeKey: false,
        allowEnterKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });
    $.ajax({
        type: "POST",
        url: "/StoreReport/AjaxStockBalance",
        data: data,
        //        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (response) {


            $('#StockBalanceList').html(response);
            Swal.close();

        },
        failure: function (response) {
            //            alert(response.responseText);
        },
        error: function (response) {
            //            alert(response.responseText);
        }
    });

}
function SearchReceiptReport(page) {


    var token = $('input[name="__RequestVerificationToken"]').val();
    var storeId = $('#StoreId').val();
    var productId = $('#ProductId').val();
    var receiptDateFrom = $('#ReceiptDateFrom').val();
    var receiptDateTo = $('#ReceiptDateTo').val();
    var data = {
        __RequestVerificationToken: token,
        StoreId: storeId,
        productId: productId,
        receiptDateFrom: receiptDateFrom,
        receiptDateTo: receiptDateTo,
        Page: page
    }
    Swal.fire({
        title: 'درحال بارگزاری',
        allowOutsideClick: false,
        allowEscapeKey: false,
        allowEnterKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });
    $.ajax({
        type: "POST",
        url: "/ReceiptReport/AjaxReceiptReport",
        data: data,
        //        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (response) {


            $('#ReceiptReportList').html(response);
            Swal.close();

        },
        failure: function (response) {
            //            alert(response.responseText);
        },
        error: function (response) {
            //            alert(response.responseText);
        }
    });

}
function SearchInvoiceReport(page) {


    var token = $('input[name="__RequestVerificationToken"]').val();
    var storeId = $('#StoreId').val();
    var productId = $('#ProductId').val();
    var invoiceDateFrom = $('#InvoiceDateFrom').val();
    var invoiceDateTo = $('#InvoiceDateTo').val();
    var data = {
        __RequestVerificationToken: token,
        StoreId: storeId,
        productId: productId,
        invoiceDateFrom: invoiceDateFrom,
        invoiceDateTo: invoiceDateTo,
        Page: page
    }
    Swal.fire({
        title: 'درحال بارگزاری',
        allowOutsideClick: false,
        allowEscapeKey: false,
        allowEnterKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });
    $.ajax({
        type: "POST",
        url: "/InvoiceReport/AjaxInvoiceReport",
        data: data,
        //        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (response) {


            $('#InvoiceReportList').html(response);
            Swal.close();

        },
        failure: function (response) {
            //            alert(response.responseText);
        },
        error: function (response) {
            //            alert(response.responseText);
        }
    });

}
function PrintInvoiceReport(page) {


    var token = $('input[name="__RequestVerificationToken"]').val();
    var storeId = $('#StoreId').val();
    var productId = $('#ProductId').val();
    var invoiceDateFrom = $('#InvoiceDateFrom').val();
    var invoiceDateTo = $('#InvoiceDateTo').val();
    var data = {
        __RequestVerificationToken: token,
        StoreId: storeId,
        productId: productId,
        invoiceDateFrom: invoiceDateFrom,
        invoiceDateTo: invoiceDateTo,
        Page: page
    }

    window.open('/InvoiceReport/PrintInvoiceReport?InvoiceDateFrom=' + invoiceDateFrom + "&StoreId=" + storeId + "&productId=" + productId + "&invoiceDateTo=" + invoiceDateTo, '_blank');
}
function SearchDisposableProductBalance(page) {


    var token = $('input[name="__RequestVerificationToken"]').val();
    var StoreId = $('#StoreId').val();
    var DispoasableStoreId = $('#DisposableStoreId').val();
    var data = {
        __RequestVerificationToken: token,
        StoreId: StoreId,
        DispoasableStoreId: DispoasableStoreId,
        Page: page
    }
    Swal.fire({
        title: 'درحال بارگزاری',
        allowOutsideClick: false,
        allowEscapeKey: false,
        allowEnterKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });
    $.ajax({
        type: "POST",
        url: "/StoreReport/AjaxDisposableStockBalance",
        data: data,
        //        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (response) {


            $('#StockBalanceList').html(response);
            Swal.close();

        },
        failure: function (response) {
            //            alert(response.responseText);
        },
        error: function (response) {
            //            alert(response.responseText);
        }
    });

}
function SearchInvoice(page) {


    var token = $('input[name="__RequestVerificationToken"]').val();
    var factorNumber = $('#FactorNumber').val();
    var businnessPartnerId = $('#BusinnessPartnerId').val();
    var hamrahName = $('#HamrahName').val();
    var factorNumber = $('#FactorNumber').val();
    var factorDate = $('#FactorDate').val();
    var factorDateFrom = $('#FactorDateFrom').val();
    var factorDateTo = $('#FactorDateTo').val();
    var data = {
        __RequestVerificationToken: token,
        FactorNumber: factorNumber,
        businnessPartnerId: businnessPartnerId,
        hamrahName: hamrahName,
        factorDateFrom: factorDateFrom,
        factorDateTo: factorDateTo,
        Page: page
    }
    Swal.fire({
        title: 'درحال بارگزاری',
        allowOutsideClick: false,
        allowEscapeKey: false,
        allowEnterKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });
    $.ajax({
        type: "POST",
        url: "/Invoice/AjaxListInvoice",
        data: data,
        //        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (response) {


            $('#InvoiceList').html(response);
            Swal.close();

        },
        failure: function (response) {
            //            alert(response.responseText);
        },
        error: function (response) {
            //            alert(response.responseText);
        }
    });

}
function SearchLogs(page) {


    var token = $('input[name="__RequestVerificationToken"]').val();
    //var logNumber = $('#LogNumber').val();
    var creator = $('#Creator').val();
    var logTypeId = $('#LogTypeId').val();
    var ipAddress = $('#IpAddress').val();
    var logDateFrom = $('#LogDateFrom').val();
    var logDateTo = $('#LogDateTo').val();
    var logStatusId = $('#LogStatusId').val();
    var sortType = $('#SortType').val();
    var sortField = $('#SortField').val();

    
    var data = {
        __RequestVerificationToken: token,
        //LogNumber: logNumber,
        Creator: creator,
        LogTypeId: logTypeId,
        IpAddress: ipAddress,
        LogDateFrom: logDateFrom,
        logDateTo: logDateTo,
        LogStatusId: logStatusId,
        SortType: sortType,
        SortField: sortField,

        Page: page
    }
    Swal.fire({
        title: 'درحال بارگزاری',
        allowOutsideClick: false,
        allowEscapeKey: false,
        allowEnterKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });
    $.ajax({
        type: "POST",
        url: "/Log/AjaxListLog",
        data: data,
        //        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (response) {


            $('#LogList').html(response);
            Swal.close();

        },
        failure: function (response) {
            //            alert(response.responseText);
        },
        error: function (response) {
            //            alert(response.responseText);
        }
    });

}
function SearchInvoiceReturn(page) {


    var token = $('input[name="__RequestVerificationToken"]').val();
    var factorNumber = $('#FactorNumber').val();
    var data = {
        __RequestVerificationToken: token,
        FactorNumber: factorNumber,
        Page: page
    }
    Swal.fire({
        title: 'درحال بارگزاری',
        allowOutsideClick: false,
        allowEscapeKey: false,
        allowEnterKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });
    $.ajax({
        type: "POST",
        url: "/Invoice/AjaxListInvoiceReturn",
        data: data,
        //        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (response) {


            $('#InvoiceList').html(response);
            Swal.close();

        },
        failure: function (response) {
            //            alert(response.responseText);
        },
        error: function (response) {
            //            alert(response.responseText);
        }
    });

}
function SearchUser(page) {


    var token = $('input[name="__RequestVerificationToken"]').val();
    var lastName = $('#LastName').val();
    var data = {
        __RequestVerificationToken: token,
        LastName: lastName,
        Page: page
    }
    Swal.fire({
        title: 'درحال بارگزاری',
        allowOutsideClick: false,
        allowEscapeKey: false,
        allowEnterKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });
    $.ajax({
        type: "POST",
        url: "/Account/AjaxListUser",
        data: data,
        //        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (response) {


            $('#UserList').html(response);
            Swal.close();

        },
        failure: function (response) {
            //            alert(response.responseText);
        },
        error: function (response) {
            //            alert(response.responseText);
        }
    });

}
function SearchProductGroup(page) {


    var token = $('input[name="__RequestVerificationToken"]').val();
    var title = $('#ProductGroupTitle').val();
    var data = {
        __RequestVerificationToken: token,
        Title: title,
        Incharge: inCharge,
        Page: page
    }
    Swal.fire({
        title: 'درحال بارگزاری',
        allowOutsideClick: false,
        allowEscapeKey: false,
        allowEnterKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });
    $.ajax({
        type: "POST",
        url: "/ProductGroup/AjaxListProductGroup",
        data: data,
        //        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (response) {


            $('#StoreList').html(response);
            Swal.close();

        },
        failure: function (response) {
            //            alert(response.responseText);
        },
        error: function (response) {
            //            alert(response.responseText);
        }
    });

}
function SearchProductSubGroup(page) {


    var token = $('input[name="__RequestVerificationToken"]').val();
    var title = $('#ProductSubGroupTitle').val();
    var data = {
        __RequestVerificationToken: token,
        Title: title,
        Incharge: inCharge,
        Page: page
    }
    Swal.fire({
        title: 'درحال بارگزاری',
        allowOutsideClick: false,
        allowEscapeKey: false,
        allowEnterKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });
    $.ajax({
        type: "POST",
        url: "/ProductSubGroup/AjaxListStore",
        data: data,
        //        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (response) {


            $('#ProductSubGroupList').html(response);
            Swal.close();

        },
        failure: function (response) {
            //            alert(response.responseText);
        },
        error: function (response) {
            //            alert(response.responseText);
        }
    });

}

function FillHamrahInfo(parameters) {

    var token = $('input[name="__RequestVerificationToken"]').val();
    var businnessPartnerId = $('#BusinnessPartnerId').val();
    $('#ReceiverFullName').val("");
    $('#ReceiverMobile').val("");
    var data = {
        __RequestVerificationToken: token,
        businnessPartnerId: businnessPartnerId,
    }
    $.ajax({
        type: "POST",
        url: "/Invoice/GetHamrahInfo",
        data: data,
        //        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            if (response.success === true) {
                $('#ReceiverFullName').val(response.HamrahName);
                $('#ReceiverMobile').val(response.HamrahMobile);
            } else {
                $('#ReceiverFullName').val("");
                $('#ReceiverMobile').val("");
            }

            SetSellPrice();
        },
        failure: function (response) {
        },
        error: function (response) {
        }
    });
}
function SetSellPrice(parameters) {
    var token = $('input[name="__RequestVerificationToken"]').val();
    var storeReceiptDetailId = $('#StoreReceiptDetailId').val();
    var businnessPartnerId = $('#BusinnessPartnerId').val();
    $('#SellPrice').val("");
    var data = {
        __RequestVerificationToken: token,
        storeReceiptDetailId: storeReceiptDetailId,
        businnessPartnerId: businnessPartnerId
    }
    $.ajax({
        type: "POST",
        url: "/Invoice/SetSellPrice",
        data: data,
        //        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            if (response.success === true) {
                $('#SellPrice').val(response.SellPrice);
            } else {
                $('#SellPrice').val("");
            }
        },
        failure: function (response) {
        },
        error: function (response) {
        }
    });
}


function SearchBusinnessPartnerGroup(page) {


    var token = $('input[name="__RequestVerificationToken"]').val();
    var title = $('#BusinnessPartnerGroupTitle').val();

    var data = {
        __RequestVerificationToken: token,
        Title: title,
        Page: page
    }
    Swal.fire({
        title: 'درحال بارگزاری',
        allowOutsideClick: false,
        allowEscapeKey: false,
        allowEnterKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });
    $.ajax({
        type: "POST",
        url: "/BusinnessPartnerGroup/AjaxListBusinnessPartnerGroup",
        data: data,
        //        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (response) {


            $('#BusinnessPartnerGroupList').html(response);
            Swal.close();

        },
        failure: function (response) {
            alert(response.responseText);
        },
        error: function (response) {
            alert(response.responseText);
        }
    });

}
function SearchBusinnessPartner(page) {


    var token = $('input[name="__RequestVerificationToken"]').val();
    var companyTitle = $('#companyTitle').val();
    var lastName = $('#LastName').val();
    var mobile = $('#Mobile').val();
    var businnessGroupId = $('#BusinnessGroupId').val();
    CurrentBusinnessPartnerPage = page;
    var data = {
        __RequestVerificationToken: token,
        CompanyTitle: companyTitle,
        LastName: lastName,
        Mobile: mobile,
        BusinnessGroupId: businnessGroupId,
        Page: page
    }
    Swal.fire({
        title: 'درحال بارگزاری',
        allowOutsideClick: false,
        allowEscapeKey: false,
        allowEnterKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });
    $.ajax({
        type: "POST",
        url: "/BusinnessPartner/AjaxListBusinnessPartner",
        data: data,
        //        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (response) {


            $('#BusinnessPartnerList').html(response);
            Swal.close();

        },
        failure: function (response) {
            alert(response.responseText);
        },
        error: function (response) {
            alert(response.responseText);
        }
    });

}
function SearchManufacture(page) {


    var token = $('input[name="__RequestVerificationToken"]').val();
    var title = $('#ManufactureTitle').val();
    var data = {
        __RequestVerificationToken: token,
        Title: title,
        Page: page
    }
    Swal.fire({
        title: 'درحال بارگزاری',
        allowOutsideClick: false,
        allowEscapeKey: false,
        allowEnterKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });
    $.ajax({
        type: "POST",
        url: "/Manufacture/AjaxListManufacture",
        data: data,
        //        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (response) {


            $('#ManufactureList').html(response);
            Swal.close();

        },
        failure: function (response) {
            //            alert(response.responseText);
        },
        error: function (response) {
            //            alert(response.responseText);
        }
    });

}
function SearchProduct(page) {


    var token = $('input[name="__RequestVerificationToken"]').val();
    var title = $('#ProductTitle').val();
    var genericCode = $('#GenericCode').val();
    var productGroupId = $('#DropDownProductGroup').val();
    var productSubGroupId = $('#DropDownProductSubGroup').val();
    var data = {
        __RequestVerificationToken: token,
        Title: title,
        GenericCode: genericCode,
        ProductGroupId: productGroupId,
        ProductSubGroupId: productSubGroupId,
        Page: page
    }
    Swal.fire({
        title: 'درحال بارگزاری',
        allowOutsideClick: false,
        allowEscapeKey: false,
        allowEnterKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });
    $.ajax({
        type: "POST",
        url: "/Product/AjaxListProduct",
        data: data,
        //        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (response) {


            $('#ProductList').html(response);
            Swal.close();

        },
        failure: function (response) {
            //            alert(response.responseText);
        },
        error: function (response) {
            //            alert(response.responseText);
        }
    });

}

function isNumber(n) {
    return !isNaN(parseFloat(n)) && isFinite(n);
}
function RegisterProductInReceipt() {

    var token = $('input[name="__RequestVerificationToken"]').val();

    var productId = $('#ProductId').val();
    var manufactureId = $('#ManufactureId').val();
    var storeReceipId = $('#Id').val();
    var storeId = $('#StoreId').val();
    var dateType = $('#DateType').val();
    var buyPrice = remove_commas($('#BuyPrice').val());
    var sellPrice = remove_commas($('#SellPrice').val());
    var expireDateMiladi = $('#ExpireDateMiladi').val();
    var expireDateShamsi = $('#ExpireDateShamsi').val();
    var batchNumber = $('#BatchNumber').val();
    var id = $('#StoreReceiptDetailId').val();
    var count = $('#Count').val();
    if (!productId || !storeReceipId || !buyPrice || !count || !sellPrice || !batchNumber || !manufactureId) {
        alert("لطفا موارد ستاره دار را وارد نمائید");
        return;
    }
    if (!isNumber(count) || !isNumber(sellPrice) || !isNumber(buyPrice)) {
        alert("لطفا مقادیر عددی را بصورت صحیح وارد نمائید");
        return;
    }
    var data = {
        __RequestVerificationToken: token,
        Id: id,
        ProductId: productId,
        ManufactureId: manufactureId,
        DateType: dateType,
        BuyPrice: buyPrice,
        SellPrice: sellPrice,
        ExpireDateMiladi: expireDateMiladi,
        ExpireDateShamsi: expireDateShamsi,
        BatchNumber: batchNumber,
        Count: count,
        StoreReceiptId: storeReceipId,
        StoreId: storeId
    }
    //    alert(JSON.stringify(data));
    Swal.fire({
        title: 'درحال بارگزاری',
        allowOutsideClick: false,
        allowEscapeKey: false,
        allowEnterKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });
    $.ajax({
        type: "POST",
        url: "/StoreReceipt/AjaxCreateDetail",
        data: data,
        //        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (response) {


            $('#productList').html(response);
            $("#StoreReceiptDetailId").val("");
            $("#Count").val("");
            $("#SellPrice").val("");
            $("#BuyPrice").val("");
            $("#BatchNumber").val("");
            $("#btnRegProduct").html("ثبت جدید");
            Swal.close();

        },
        failure: function (response) {
            Swal.close();
            Swal.fire({
                title: 'خطا',
                text: response.responseText,
                icon: 'error',
                confirmButtonText: 'باشه'
            });
        },
        error: function (response) {
            Swal.close();

            Swal.fire({
                title: 'خطا',
                text: response.responseText,
                icon: 'error',
                confirmButtonText: 'باشه'
            });        }
    });
}
function RegisterReturnInReceipt() {

    var token = $('input[name="__RequestVerificationToken"]').val();


    var StoreReceiptDetailId = $('#StoreReceiptDetailId').val();
    var StoreReceiptReturnId = $('#Id').val();
    var count = $('#Count').val();
    if (!count) {
        alert("لطفا موارد ستاره دار را وارد نمائید");
        return;
    }
    if (!isNumber(count)) {
        alert("لطفا مقادیر عددی را بصورت صحیح وارد نمائید");
        return;

    }
    var data = {
        __RequestVerificationToken: token,
        storeReceiptDetailId: StoreReceiptDetailId,
        storeReceiptReturnId: StoreReceiptReturnId,
        count: count
    }
    Swal.fire({
        title: 'درحال بارگزاری',
        allowOutsideClick: false,
        allowEscapeKey: false,
        allowEnterKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });
    $.ajax({
        type: "POST",
        url: "/StoreReceipt/AjaxReturnStoreDetail",
        data: data,
        //        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (response) {


            $('#productList').html(response);
            $("#Count").val("");
            $("#btnRegProduct").hide();
            Swal.close();

        },
        failure: function (response) {
            Swal.close();
        },
        error: function (response) {
            Swal.close();
        }
    });
}
function RegisterReturnInInvoice() {

    var token = $('input[name="__RequestVerificationToken"]').val();


    var invoiceDetailId = $('#InvoiceDetailId').val();
    var invoiceReturnId = $('#InvoiceReturnId').val();
    var count = $('#Count').val();
    if (!count) {
        alert("لطفا موارد ستاره دار را وارد نمائید");
        return;
    }
    if (!isNumber(count)) {
        alert("لطفا مقادیر عددی را بصورت صحیح وارد نمائید");
        return;

    }
    var data = {
        __RequestVerificationToken: token,
        invoiceDetailId: invoiceDetailId,
        invoiceReturnId: invoiceReturnId,
        count: count
    }
    Swal.fire({
        title: 'درحال بارگزاری',
        allowOutsideClick: false,
        allowEscapeKey: false,
        allowEnterKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });
    $.ajax({
        type: "POST",
        url: "/Invoice/AjaxReturnInvoiceDetail",
        data: data,
        //        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (response) {


            $('#productList').html(response);
            $("#Count").val("");
            $("#btnRegProduct").hide();
            Swal.close();

        },
        failure: function (response) {
            Swal.close();
        },
        error: function (response) {
            Swal.close();
        }
    });
}
function RegisterProductInInvoice() {

    var token = $('input[name="__RequestVerificationToken"]').val();

    var storeReceiptDetailId = $('#StoreReceiptDetailId').val();
    var BusinnessPartnerId = $('#BusinnessPartnerId').val();
    var invoiceId = $('#Id').val();
    var sellPrice = remove_commas($('#SellPrice').val());
    var storeId = $('#StoreId').val();

    var id = $('#InvoiceDetailId').val();
    var count = $('#Count').val();
    if (!count || !sellPrice || !storeReceiptDetailId) {
        alert("لطفا موارد ستاره دار را وارد نمائید");
        return;
    }
    if (!isNumber(count) || !isNumber(sellPrice)) {
        alert("لطفا مقادیر عددی را بصورت صحیح وارد نمائید");
        return;

    }
    var data = {
        __RequestVerificationToken: token,
        Id: id,
        StoreReceiptDetailId: storeReceiptDetailId,
        BusinnessPartnerId: BusinnessPartnerId,
        SellPrice: sellPrice,
        StoreId: storeId,
        Count: count,
        InvoiceId: invoiceId
    }
    Swal.fire({
        title: 'درحال بارگزاری',
        allowOutsideClick: false,
        allowEscapeKey: false,
        allowEnterKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });
    $.ajax({
        type: "POST",
        url: "/Invoice/AjaxCreateDetail",
        data: data,
        //        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (response) {


            $('#productList').html(response);
            $("#InvoiceDetailId").val("");

            $("#Count").val("");
            //            $("#SellPrice").val("");

            $("#btnRegProduct").html("ثبت جدید");
            Swal.close();

        },
        failure: function (response) {
            Swal.close();
            alert(response.responseText);
        },
        error: function (response) {
            Swal.close();
            alert(response.responseText);
        }
    });
}
function UpdateStoreReceiptDetail(val) {

    var token = $('input[name="__RequestVerificationToken"]').val();
    var storeReceiptId = $('#Id').val();
    var data = {
        __RequestVerificationToken: token,
        id: val,
        storeReceiptId: storeReceiptId
    }
    Swal.fire({
        title: 'درحال بارگزاری',
        allowOutsideClick: false,
        allowEscapeKey: false,
        allowEnterKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });
    $.ajax({
        type: "POST",
        url: "/StoreReceipt/GetStoreRceiptDetailAjax",
        data: data,
        //        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (response) {
            //            alert(JSON.stringify(response));
            const obj = JSON.parse(response);
            //            $("#ProductId").val(obj.ProductTitle).trigger('change');;
            //            $('#ProductId').select2("val", "5426fd7c-d06a-71e0-fc1e-3a0e9e88ee43");



            $('#ProductId').val(obj.ProductId); // Select the option with a value of '1'
            $('#ProductId').trigger('change');




            $("#Count").val(obj.Count);
            $("#SellPrice").val(obj.SellPrice);
            $("#BuyPrice").val(obj.BuyPrice);
            $("#BatchNumber").val(obj.BatchNumber);
            $("#ExpireDateShamsi").val(obj.ExpireDateShamsi);
            $("#ExpireDateMiladi").val(obj.ExpireDateMiladi);
            //            alert(obj.Id);
            $("#StoreReceiptDetailId").val(obj.Id);
            $("#btnRegProduct").html("ذخیره تغییرات");
            Swal.close();

        },
        failure: function (response) {
            Swal.close();
            alert(response.responseText);
        },
        error: function (response) {
            Swal.close();
            alert(response.responseText);
        }
    });
}
function DeleteStoreReceiptDetail(val) {

    var token = $('input[name="__RequestVerificationToken"]').val();
    var storeReceiptId = $('#Id').val();
    var data = {
        __RequestVerificationToken: token,
        Id: val,
        storeReceiptId: storeReceiptId
    }
    if (confirm('آیا مطمئن هستید؟')) {
        Swal.fire({
            title: 'درحال بارگزاری',
            allowOutsideClick: false,
            allowEscapeKey: false,
            allowEnterKey: false,
            didOpen: () => {
                Swal.showLoading();
            }
        });
        $.ajax({
            type: "POST",
            url: "/StoreReceipt/DeleteStoreRceiptDetailAjax",
            data: data,
            //        contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (response) {
                $('#productList').html(response);
                Swal.close();

            },
            failure: function (response) {
                Swal.close();
                alert(response.responseText);
            },
            error: function (response) {
                Swal.close();
                alert(response.responseText);
            }
        });
    }
}
function ReturnStoreReceiptDetail(val) {

    var token = $('input[name="__RequestVerificationToken"]').val();
    var storeReceiptId = $('#StoreReceiptId').val();
    $("#StoreReceiptDetailId").val(val);
    var data = {
        __RequestVerificationToken: token,
        Id: val,
        storeReceiptId: storeReceiptId
    }
    Swal.fire({
        title: 'درحال بارگزاری',
        allowOutsideClick: false,
        allowEscapeKey: false,
        allowEnterKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });
    $.ajax({
        type: "POST",
        url: "/StoreReceipt/ReturnStoreRceiptDetailAjax",
        data: data,
        //        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            //                alert(JSON.stringify(response));

            $("#Count").val(response.Count);
            $('#ProductId').val(response.ProductId); // Select the option with a value of '1'
            $('#ProductId').trigger('change');

            $("#BatchNumber").val(response.BatchNumber);

            $("#ProductId").attr("disabled", "disabled");
            $("#BatchNumber").attr("disabled", "disabled");




            $("#btnRegProduct").html("ثبت");
            //                $("#btnRegProduct").css("display", "none");
            $("#btnRegProduct").css("display", "block");
            Swal.close();

        },
        failure: function (response) {
            Swal.close();
        },
        error: function (response) {
            Swal.close();
        }
    });

}
function ReturnInvoiceDetail(val) {


    var token = $('input[name="__RequestVerificationToken"]').val();
    var invoiceId = $("#InvoiceId").val();
    var invoiceReturnId = $("#InvoiceReturnId").val();
    $("#InvoiceDetailId").val(val);
    var data = {
        __RequestVerificationToken: token,
        Id: val,
        InvoiceId: invoiceId,
        invoiceReturnId: invoiceReturnId
    }
    Swal.fire({
        title: 'درحال بارگزاری',
        allowOutsideClick: false,
        allowEscapeKey: false,
        allowEnterKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });
    $.ajax({
        type: "POST",
        url: "/Invoice/ReturnInvoiceDetailAjax",
        data: data,
        //        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            //            alert(JSON.stringify(response));

            $("#Count").val(response.Count);


            $("#productTitle").val(response.ProductTitle + ' - ' + response.BatchNumber);

            $("#productTitle").attr("disabled", "disabled");





            $("#btnRegProduct").html("ذخیره");
            //                $("#btnRegProduct").css("display", "none");
            $("#btnRegProduct").css("display", "block");
            Swal.close();

        },
        failure: function (response) {
            Swal.close();
        },
        error: function (response) {
            Swal.close();
        }
    });

}
function UpdateInvoiceDetail(val) {

    var token = $('input[name="__RequestVerificationToken"]').val();
    var invoiceId = $('#Id').val();
    var data = {
        __RequestVerificationToken: token,
        Id: val,
        invoiceId: invoiceId
    }
    Swal.fire({
        title: 'درحال بارگزاری',
        allowOutsideClick: false,
        allowEscapeKey: false,
        allowEnterKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });
    $.ajax({
        type: "POST",
        url: "/Invoice/GetInvoiceDetailAjax",
        data: data,
        //        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (response) {
//            alert(JSON.stringify(response));
            const obj = JSON.parse(response);
            //            $("#ProductId").val(obj.ProductTitle).trigger('change');;
            //            $('#ProductId').select2("val", "5426fd7c-d06a-71e0-fc1e-3a0e9e88ee43");



            $('#StoreReceiptDetailId').val(obj.StoreReceiptDetailId); // Select the option with a value of '1'
            $('#StoreReceiptDetailId').trigger('change');




            $("#Count").val(obj.Count);
            $("#SellPrice").val(obj.SellPrice);
            $("#InvoiceDetailId").val(obj.Id);
            $("#btnRegProduct").html("ذخیره تغییرات");
            Swal.close();

        },
        failure: function (response) {
            Swal.close();
            alert(response.responseText);
        },
        error: function (response) {
            Swal.close();
            alert(response.responseText);
        }
    });
}
function DeleteInvoiceDetail(val) {

    var token = $('input[name="__RequestVerificationToken"]').val();
    var invoiceId = $('#Id').val();
    var data = {
        __RequestVerificationToken: token,
        Id: val,
        InvoiceId: invoiceId
    }
    if (confirm('آیا مطمئن هستید؟')) {
        Swal.fire({
            title: 'درحال بارگزاری',
            allowOutsideClick: false,
            allowEscapeKey: false,
            allowEnterKey: false,
            didOpen: () => {
                Swal.showLoading();
            }
        });
        $.ajax({
            type: "POST",
            url: "/Invoice/DeleteInvoiceDetailAjax",
            data: data,
            //        contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (response) {
                $('#productList').html(response);
                Swal.close();

            },
            failure: function (response) {
                Swal.close();
                alert(response.responseText);
            },
            error: function (response) {
                Swal.close();
                alert(response.responseText);
            }
        });
    }
}

function DateTypeChange(val) {
    if (val === "shamsi") {
        $('#shamsiExpireDate').show();
        $('#miladiExpireDate').hide();
    } else {
        $('#shamsiExpireDate').hide();
        $('#miladiExpireDate').show();
    }
}

function confirmDeleteModalForPermission(itemId) {
    // Set the item ID in the modal body
    $("#DeleteConfirmationModal .modal-body").text("آیا مطمئن هستید؟");

    // Set the delete button's click event handler
    $("#confirmDelete").off("click").on("click", function () {
        // Perform the deletion logic here
        window.location.href = '/Account/DeletePermission/' + itemId;
    });
}
function confirmDeleteBlackListIp(itemId) {
    // Set the item ID in the modal body
    $("#DeleteConfirmationModal .modal-body").text("آیا مطمئن هستید؟");

    // Set the delete button's click event handler
    $("#confirmDelete").off("click").on("click", function () {
        // Perform the deletion logic here
        window.location.href = '/Account/DeleteBlackListIp/' + itemId;
    });
}
function confirmDeleteModalRole(itemId) {
    // Set the item ID in the modal body
    $("#DeleteConfirmationModal .modal-body").text("آیا مطمئن هستید؟");

    // Set the delete button's click event handler
    $("#confirmDelete").off("click").on("click", function () {
        // Perform the deletion logic here
        window.location.href = '/Account/DeleteRole/' + itemId;
    });
}
function confirmDeleteModalForUserStore(itemId) {
    // Set the item ID in the modal body
    $("#DeleteConfirmationModal .modal-body").text("آیا مطمئن هستید؟");

    // Set the delete button's click event handler
    $("#confirmDelete").off("click").on("click", function () {
        // Perform the deletion logic here
        window.location.href = '/Account/DeleteUserStore/' + itemId;
    });
}
function confirmDeleteModalForUser(itemId) {
    // Set the item ID in the modal body
    $("#DeleteConfirmationModal .modal-body").text("آیا مطمئن هستید؟");

    // Set the delete button's click event handler
    $("#confirmDelete").off("click").on("click", function () {
        // Perform the deletion logic here
        window.location.href = '/Account/DeleteUser/' + itemId;
    });
}
function confirmUnActiveModalForUser(itemId) {
    // Set the item ID in the modal body
    $("#DeleteConfirmationModal .modal-body").text("آیا مطمئن هستید؟");

    // Set the delete button's click event handler
    $("#confirmDelete").off("click").on("click", function () {
        // Perform the deletion logic here
        window.location.href = '/Account/UnActiveUser/' + itemId;
    });
}
function confirmLogOutModalForUser(itemId) {
    // Set the item ID in the modal body
    $("#DeleteConfirmationModal .modal-body").text("آیا مطمئن هستید؟");

    // Set the delete button's click event handler
    $("#confirmDelete").off("click").on("click", function () {
        // Perform the deletion logic here
        window.location.href = '/Account/LogOutUser/' + itemId;
    });
}
function confirmActiveModalForUser(itemId) {
    // Set the item ID in the modal body
    $("#DeleteConfirmationModal .modal-body").text("آیا مطمئن هستید؟");

    // Set the delete button's click event handler
    $("#confirmDelete").off("click").on("click", function () {
        // Perform the deletion logic here
        window.location.href = '/Account/ActiveUser/' + itemId;
    });
}


function confirmDeleteModal(itemId) {
    // Set the item ID in the modal body
    $("#DeleteConfirmationModal .modal-body").text("آیا مطمئن هستید؟");

    // Set the delete button's click event handler
    $("#confirmDelete").off("click").on("click", function () {
        // Perform the deletion logic here
        window.location.href = 'delete/' + itemId;
    });
}
function ArchiveFileModal(itemId) {
    // Set the item ID in the modal body

    // Set the delete button's click event handler
    $("#ConfirmArchive").off("click").on("click", function () {
        // Perform the deletion logic here
        var token = $('input[name="__RequestVerificationToken"]').val();
        var businnessPartnerId = itemId;
        var archiveTypeId = $('#ArchiveTypeId').val();
        var data = {
            __RequestVerificationToken: token,
            businnessPartnerId: businnessPartnerId,
            archiveTypeId: archiveTypeId
        }
        Swal.fire({
            title: 'درحال بارگزاری',
            allowOutsideClick: false,
            allowEscapeKey: false,
            allowEnterKey: false,
            didOpen: () => {
                Swal.showLoading();
            }
        });
        $.ajax({
            type: "POST",
            url: "/BusinnessPartner/ArchiveFile",
            data: data,
            //        contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                Swal.close();
                var obj = (response);
                if (obj.success === true) {
                    Swal.fire({
                        title: 'موفق',
                        text: obj.message,
                        icon: 'success',
                        confirmButtonText: 'باشه'
                    }).then(function () {
                        SearchBusinnessPartner(CurrentBusinnessPartnerPage);
                        //                    $("#ConfirmArchive .close").click();
                        $('#ArchiveModal').modal('hide');
                        $('body').removeClass('modal-open');
                        $('.modal-backdrop').remove();
                    });
                } else {
                    Swal.fire({
                        title: 'نا موفق',
                        text: obj.message,
                        icon: 'error',
                        confirmButtonText: 'باشه'
                    });

                }

                //                SearchBusinnessPartner(1);
                //                $('#ConfirmArchive').modal('toggle');


            },
            failure: function (response) {
                var obj = (response);


                //                Swal.close();
                Swal.fire({
                    title: 'نا موفق',
                    text: obj.message,
                    icon: 'error',
                    confirmButtonText: 'باشه'
                });
            },
            error: function (response) {
                var obj = (response);


                //                Swal.close();
                Swal.fire({
                    title: 'نا موفق',
                    text: obj.message,
                    icon: 'error',
                    confirmButtonText: 'باشه'
                });

            }
        });
    });
}
function UnArchiveFileModal(itemId) {
    // Set the item ID in the modal body

    // Set the delete button's click event handler
    $("#UnConfirmArchive").off("click").on("click", function () {
        // Perform the deletion logic here
        var token = $('input[name="__RequestVerificationToken"]').val();
        var businnessPartnerId = itemId;
        var data = {
            __RequestVerificationToken: token,
            businnessPartnerId: businnessPartnerId
        }
        Swal.fire({
            title: 'درحال بارگزاری',
            allowOutsideClick: false,
            allowEscapeKey: false,
            allowEnterKey: false,
            didOpen: () => {
                Swal.showLoading();
            }
        });
        $.ajax({
            type: "POST",
            url: "/BusinnessPartner/UnArchiveFile",
            data: data,
            //        contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                Swal.close();
                var obj = (response);
                if (obj.success === true) {
                    Swal.fire({
                        title: 'موفق',
                        text: obj.message,
                        icon: 'success',
                        confirmButtonText: 'باشه'
                    }).then(function () {
                        SearchBusinnessPartner(CurrentBusinnessPartnerPage);
                        //                    $("#ConfirmArchive .close").click();
                        $('#UnArchiveModal').modal('hide');
                        $('body').removeClass('modal-open');
                        $('.modal-backdrop').remove();
                    });
                } else {
                    Swal.fire({
                        title: 'نا موفق',
                        text: obj.message,
                        icon: 'error',
                        confirmButtonText: 'باشه'
                    });

                }

                //                SearchBusinnessPartner(1);
                //                $('#ConfirmArchive').modal('toggle');


            },
            failure: function (response) {
                var obj = (response);


                //                Swal.close();
                Swal.fire({
                    title: 'نا موفق',
                    text: obj.message,
                    icon: 'error',
                    confirmButtonText: 'باشه'
                });
            },
            error: function (response) {
                var obj = (response);


                //                Swal.close();
                Swal.fire({
                    title: 'نا موفق',
                    text: obj.message,
                    icon: 'error',
                    confirmButtonText: 'باشه'
                });

            }
        });
    });
}

function confirmConfirmationModal(itemId) {
    // Set the item ID in the modal body
    $("#DeleteConfirmationModal .modal-body").text("آیا مطمئن هستید؟");

    // Set the delete button's click event handler
    $("#confirmDelete").off("click").on("click",
        function () {
            // Perform the deletion logic here
            window.location.href = 'Confirm/' + itemId;
        });
}
function GetLastInvoices() {
    // Set the item ID in the modal body
    $("#MyModal .modal-body").text("");
    var businnessPartnerId = $('#BusinnessPartnerId').val();
    var token = $('input[name="__RequestVerificationToken"]').val();
    var data = {
        __RequestVerificationToken: token,
        businnessPartnerId: businnessPartnerId
    }
    if (businnessPartnerId) {

        $.ajax({
            url: '/Invoice/GetLastInvoice',
            type: "Post",
            dataType: "html",
            data: data,
            success: function (result) {
                $("#MyModal .modal-body").html(result);

            }
        });
    }


}
function confirmDisposalModal(itemId) {
    // Set the item ID in the modal body
    $("#DeleteConfirmationModal .modal-body").text("آیا مطمئن هستید؟");

    // Set the delete button's click event handler
    $("#confirmDelete").off("click").on("click",
        function () {
            // Perform the deletion logic here
            window.location.href = 'ConfirmDisposal/' + itemId;
        });
}
function confirmReturnConfirmationModal(itemId) {
    // Set the item ID in the modal body
    $("#DeleteConfirmationModal .modal-body").text("آیا مطمئن هستید؟");

    // Set the delete button's click event handler
    $("#confirmDelete").off("click").on("click",
        function () {
            // Perform the deletion logic here
            window.location.href = 'ConfirmReturnInvoice/' + itemId;
        });
}

function confirmStoreReceiptReturnModal(itemId) {
    // Set the item ID in the modal body
    $("#DeleteConfirmationModal .modal-body").text("آیا مطمئن هستید؟");

    // Set the delete button's click event handler
    $("#confirmDelete").off("click").on("click",
        function () {
            // Perform the deletion logic here
            window.location.href = 'ConfirmReturn/' + itemId;
        });
}

function confirmAccoutingConfirmationModal(itemId) {
    // Set the item ID in the modal body
    $("#DeleteConfirmationModal .modal-body").text("آیا مطمئن هستید؟");

    // Set the delete button's click event handler
    $("#confirmDelete").off("click").on("click",
        function () {
            // Perform the deletion logic here
            window.location.href = 'ConfirmAccounting/' + itemId;
        });
}

