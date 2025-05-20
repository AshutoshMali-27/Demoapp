$(window).on('load', function () {
    $('input.form-control').attr('autocomplete', 'off');
})

$(document).ready(function () {
    $('#ddl_ComponentName').change(function () {
        var selectedValue = $(this).val();
        if (selectedValue == "1") {
            $('#SerachBylabel').text('Beneficiary Search By');
            $('#lbl_Search').text('Enter AADHAR Number');
            $('#lblBeneficiary').text('Beneficiary Name');
            $('#txtVendorName').attr('placeholder', 'Beneficiary Name');
            $('#ddlSearch').val('3').prop('disabled', true);
        } else {
            $('#lbl_Search').text('Enter PAN Number');
            $('#SerachBylabel').text('Vendor Search By');
            $('#lblBeneficiary').text('Vendor Name');
            $('#txtVendorName').attr('placeholder', 'Vendor Name');
            $('#ddlSearch').val('1').prop('disabled', false);
        }
    });
    $('#txtVendorName').on('focus click', function (e) {
        e.preventDefault(); 
        $(this).blur();     
    });
    currentFyYear();
    $('input.form-control').attr('autocomplete', 'off');
    $('#ddl_SchemeName').change(function () {
        var schemeid = parseInt($(this).val());
        if (schemeid != 0) {
            GetComponent(schemeid);
        }
        $('#txtInsertHere').val('');
        $('#txtVendorName').val('');
        $('#Party_ID').val('');
        $("#tblVendorList tbody").empty();
        $('#div_tblVendorList').hide();
        $('#div_tblVendor').hide();
    });
    $('#ddl_ComponentName').change(function () {
        var Componentid = parseInt($(this).val());
        var schemeid = parseInt($('#ddl_SchemeName').val());
        if (Componentid != 0 && schemeid != 0) {
            Get_AdminApprovalNo(Componentid, schemeid);
        }
        if (parseInt(Componentid) != 0) {
            $('#DivAdminApprovalNo').show();
        } else {
            $('#DivAdminApprovalNo').hide();
        }
        $('#txtInsertHere').val('');
        $('#txtVendorName').val('');
        $('#Party_ID').val('');
        $("#tblVendorList tbody").empty();
        $('#div_tblVendorList').hide();
        $('#div_tblVendor').hide();
    });
    $('#ddl_AdminApprovalNo').change(function () {
        var AdminApprovalNo = parseInt($(this).val());
        if (AdminApprovalNo != 0) {
            CheckAAUpdateApproval(AdminApprovalNo);
            var Componentid = parseInt($('#ddl_ComponentName').val());
            initGetWrokDetailsByID(Componentid, AdminApprovalNo)
            $('#WorkOrderNumber').prop('disabled', false);
            $('#div_WorkOrderDetails').show();
            $('#DivWorkName').show();
        }
        else {
            $('#div_WorkOrderDetails').show();
            $('#DivWorkName').show();
            $('#WorkOrderNumber').prop('disabled', true);
            $('#WorkOrderNumber').val('');
            swal('Error', 'Please select an admin approval number ', 'error');
        }
    });
    $('#ddlSearch').change(function () {
        $('#txtInsertHere').val('');
        $('#div_tblVendor').hide();
        $('#div_tblVendorList').hide();
    });
    $('#txtInsertHere').on('input', function () {
        const txtInput = $(this).val().trim();
        if (txtInput === '') {
            $('#div_tblVendor').hide();
            $('#div_tblVendorList').hide();
        }
    });
    $('#txtInsertHere').blur(function () {
        var input = $(this).val();
        var SchemeID = $('#ddl_SchemeName').val();
        var ComponentID = $('#ddl_ComponentName').val();
        if (input != '' && parseInt(SchemeID)!=0 && parseInt(ComponentID)!=0) {
            var searchby = parseInt($('#ddlSearch').val());
            $('#div_tblVendor').hide();
            SearchVendor(searchby, input, SchemeID, ComponentID);
        }
        else {
            $('#div_tblVendor').hide();
            $('#div_tblVendorList').hide();
        }
    });
    $('#txtApprovedTenderAmount').change(function () {
       // debugger;
        var balanceAAAmount = $('#td_balanceAAAmount').text();
        balanceAAAmount = parseFloat(balanceAAAmount);
        var ApprovedTenderAmount = $(this).val();
        if (ApprovedTenderAmount > balanceAAAmount) {
            swal('Error', 'Tender amount should be less than or equal to admin approval amount ', 'error');
            $('#txtApprovedTenderAmount').val('');
        }
        CalculateRemaingAmount();
    });
    $('#txtPaidAmount').change(function () {
        CalculateRemaingAmount();
    });
    $('#WorkOrderNumber').change(function () {
        var workOrderNumber = $(this).val();
        var adminApprovalNumber = $('#ddl_AdminApprovalNo').val();
        if (workOrderNumber != '')
            checkWorkisExist(workOrderNumber, adminApprovalNumber);

        if (parseInt(workOrderNumber) != 0) {
            $('#DivLastRANo').show();
           
        } else {
            $('#DivLastRANo').hide();
          
        }
    });
    
    $('#file_WorkOrderCopy').change(function () {
        try {
            var fileName = $(this).val().split('\\').pop();
            if (this.files[0].type != 'application/pdf') {
                removeWorkOrderCopy();
                swal('File Format Issue.', 'Only PDF File Accepted', 'error');
                return false;
            }
            if (this.files[0].size > 10485760) {
                removeWorkOrderCopy();
                swal('File Size Issue.', 'Please select file less than 10 MB', 'error');
                return false;
            }
            if (fileName !== '') {
                var fileInput = $("#file_WorkOrderCopy")[0];
                var file = fileInput.files[0];
                var formData = new FormData();
                formData.append("file", file);
                $.ajax({
                    url: '/Home/UploadFile',
                    type: 'POST',
                    data: formData,
                    dataType: 'json',
                    contentType: false,
                    processData: false,
                    success: function (result) {
                        if (result.message == "File uploaded successfully") {
                            $('#removeWorkOrderCopy').show();
                            $('#lbl_WorkOrderCopy').text(result.filename);
                            swal('File Upload Done.', 'File Upload successfully', 'success');
                        }
                        else {
                            removeWorkOrderCopy();
                            swal('File Upload Issue.', result.message, 'error');
                            return false;
                        }
                    },
                    error: function (error) {
                        console.log(error);
                        swal('File Upload Issue.', 'Error in File Upload', 'error');
                    }
                });
            }
            else {
                $('#removeWorkOrderCopy').hide();
            }
        }
        catch (err) {
            removeWorkOrderCopy();
        }
    });

    $('#WorkOrderDate').change(function () {
        //debugger;
        const WorkOrderDate = $('#WorkOrderDate').val();
        const WorkEndDate = $('#WorkEndDate').val();
        const AAdate = getAADatesFromTable();
        if (Date.parse(WorkOrderDate) > Date.parse(WorkEndDate)) {
            swal('Error', 'Work order date should be before the work end date', 'error');
            $('#WorkOrderDate').val("");
            $('#WorkOrderDate').focus();
            return;
        }
        if (Date.parse(AAdate) > Date.parse(WorkOrderDate)) {
            swal('Error', 'Work order date should be greater than or equal to A.A details date.', 'error');
            $('#WorkOrderDate').val("");
            $('#WorkOrderDate').focus();
        }

        
    });

    function getAADatesFromTable() {
        var date;
        $('#tbody_AdminApprovalDetails tr').each(function () {
            date = $(this).find('td:eq(0)').text();  // Get the date from the first cell
        });
        var parts = date.split('-');
        var convertedDate = parts[2] + '-' + parts[1] + '-' + parts[0];
        return convertedDate;
    }

    $('#WorkEndDate').change(function () {
        // debugger;
        const WorkOrderDate = $('#WorkOrderDate').val();
        const WorkEndDate = $('#WorkEndDate').val();
        if (Date.parse(WorkOrderDate) > Date.parse(WorkEndDate)) {
            swal('Error', 'The end date of the work should be after the work order date', 'error');
            $('#WorkEndDate').val("");
            $('#WorkEndDate').focus();
        };
        if (parseInt(WorkEndDate) != 0) {
            $('#DivApprovedTenderAmount').show();
            $('#DivCostPutToTender').show();
            $('#btnSave').show();
        }
        else {
            $('#DivApprovedTenderAmount').hide();
            $('#DivCostPutToTender').hide();
            $('#btnSave').hide();
        }


    });
    $('#btnSave').click(function (e) {
        e.preventDefault();
        $('#btnBack').prop("disabled", true);
        $('#UserDropdown').prop("disabled", true);
        $('#DivImportantNote').prop("disabled", true);
        $('#divNotification').prop("disabled", true);
        $('#btnClear').prop("disabled", true);


        if (validate()) {
            $('#btnSave').prop("disabled", true);
            swal({
                title: 'Are you sure?',
                text: 'Once Save, you will able to see this Work Details Entry on Work Details Inbox !',
                icon: 'warning',
                buttons: true,
                dangerMode: true,
            })
                .then((willDelete) => {
                    if (willDelete) {
                        saveData(function (workDetailsUID) {
                            $('#btnSave').prop("disabled", false); +
                                swal(`Work UID: ${workDetailsUID } Work Details save successfully !`, {
                                icon: 'success',
                            });
                        });
                    } else {
                        $('#btnSave').prop("disabled", false);
                        swal('Your Data Pending for Save..!');
                    }

                    $('#btnSave').prop("disabled", false);
                    $('#btnBack').prop("disabled", false);
                    $('#UserDropdown').prop("disabled", false);
                    $('#DivImportantNote').prop("disabled", false);
                    $('#divNotification').prop("disabled", false);
                    $('#btnClear').prop("disabled", false);
                }); // Prevent form submission

        }
    });
    $('#btnClear').click(function () {
        clear();
    });
})
//--------------------------    Functions  ------------------------------
function numericOnly(elementRef) {
    var keyCodeEntered = (event.which) ? event.which : (window.event.keyCode) ? window.event.keyCode : -1;
    //if ((keyCodeEntered >= 48) && (keyCodeEntered <= 57) && elementRef.value.split('.').length === 2) {
    if ((keyCodeEntered >= 48) && (keyCodeEntered <= 57) && elementRef.value.split('.').length <= 2) {
        return true;
    }
    // '.' decimal point...
    else if (keyCodeEntered == 46) {
        // Allow only 1 decimal point ('.')...
        if ((elementRef.value) && (elementRef.value.indexOf('.') >= 0))
            return false;
        else
            return true;
    }
    return false;
}
function GetComponent(schemeid) {
    $.ajax({
        type: "GET",
        url: "/WorkOrder/GetComponentBySchemeId",
        data: {
            schemeId: schemeid
        },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            console.log(data);
            var componentDropdown = $('#ddl_ComponentName');
            componentDropdown.empty();
            componentDropdown.append('<option value="0" selected="selected">--Select Component Name--</option>');
            if (data.length > 0) {
                $.each(data, function (index, item) {
                    var id = parseInt(item.id);
                    componentDropdown.append($('<option></option>').attr('value', id).text(item.headNameEnglish)); // Use lowercase 'headNameEnglish' here
                });
                $("#ddl_ComponentName").val(0);
            }
            else {
                $("#ddl_ComponentName").empty();
                $("#ddl_ComponentName").val(0);
            }
        },
        error: function (err) {
            swal('Something Went Wrong!.', 'Please correct the data', 'error');
            console.log(err);
            return false;
        }
    });
}
function Get_AdminApprovalNo(componentid, schemeid) {
    $.ajax({
        type: "GET",
        url: "/WorkOrder/Get_AdminApprovalNumber",
        data: {
            Componentid: componentid,
            SchemeId: schemeid
        },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            console.log(data);
            var componentDropdown = $('#ddl_AdminApprovalNo');
            componentDropdown.empty();
            componentDropdown.append('<option value="0" selected="selected">--Select AdminApprovalNo--</option>');
            if (data.length > 0) {
                $.each(data, function (index, item) {
                    var id = parseInt(item.id);
                    componentDropdown.append($('<option></option>').attr('value', id).text(item.adminApprovalNo)); // Use lowercase 'headNameEnglish' here
                });
                $("#ddl_AdminApprovalNo").val(0);
            }
            else {
                swal('Warning', 'For this Component Administrative Approval is not created', 'warning');
                $('#div_tblAdminApprovalDetails').hide();
                $("#ddl_AdminApprovalNo").empty();
                $("#ddl_AdminApprovalNo").val(0);
            }
        },
        error: function (err) {
            swal('Something Went Wrong!.', 'Please correct the data', 'error');
            console.log(err);
            return false;
        }
    });
}
function CheckAAUpdateApproval(AdminApprovalNo) {
    $.ajax({
        type: "GET",
        url: "/WorkOrder/CheckAAUpdateApproval",
        data: {
            AdminApprovalNo: AdminApprovalNo
        },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            console.log(data);
            if (data == 'true') {
                getAdminApproveNumberDetails(AdminApprovalNo);
            }
            else if (data == false) {
                swal('Error', 'Please select an admin approval number ', 'error');
            }
        },
        error: function (err) {
            swal('Something Went Wrong!.', 'Please correct the data', 'error');
            console.log(err);
            return false;
        }
    });
}
function getAdminApproveNumberDetails(AdminApprovalNo) {
    var Componentid = parseInt($('#ddl_ComponentName').val());
    if (AdminApprovalNo != 0 && Componentid != 0) {
        $.ajax({
            type: "GET",
            url: "/WorkOrder/Get_AdminApproveNumberDetails",
            data: {
                Componentid: Componentid,
                AdminApprovalNo: AdminApprovalNo
            },
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                console.log(data);
                if (data.length > 0) {
                    $('#div_tblAdminApprovalDetails').show();
                    $("#tbody_AdminApprovalDetails tr").remove();
                    var tbody = $("#tbody_AdminApprovalDetails");
                 //   debugger;
                    for (var i = 0; i < data.length; i++) {
                        //var approvalNoSdate = data[i].approvalNoSdate;
                        var dateTime = new Date(data[i].approvalNoSdate);
                        var formattedDate = ("0" + dateTime.getDate()).slice(-2) + "-" + ("0" + (dateTime.getMonth() + 1)).slice(-2) + "-" + dateTime.getFullYear();
                        var row = "<tr>" +
                            "<td>" + formattedDate + "</td>" +
                            "<td>" + data[i].aaamount + "</td>" +
                            "<td>" + data[i].workOrderSumAmount + "</td>" +
                            "<td id='td_balanceAAAmount'>" + data[i].balanceAAAmount + "</td>" +
                            "<td>" + data[i].approverByName + "</td>" +
                            "</tr>";
                        tbody.append(row);
                    }
                    if (data[0].isWorkNoFullNfinal == 1) {
                        swal('Full and final work created under this A.A.No.');
                        $('#btnSave').prop('disabled', true);
                    }
                    else {
                        $('#btnSave').prop('disabled', false);
                    }
                }
                else {
                    $('#div_tblAdminApprovalDetails').hide();
                }
            },
            error: function (err) {
                swal('Something Went Wrong!.', 'Please correct the data', 'error');
                console.log(err);
                return false;
            }
        });
    }
}
function SearchVendor(searchBy, input, schemeID, componentID) {
    switch (searchBy) {
        case 1:    /*Search by PAN */
            input = input.toUpperCase();
            if (!/^[A-Z]{5}[0-9]{4}[A-Z]{1}$/.test(input)) {
                swal('Error', 'PAN Code should be in appropriate format', 'error');
                $('#txtInsertHere').val('');
                $('#txtInsertHere').focus();
                return false;
            }
            else {
                GetVendorInformation('PAN', input, schemeID, componentID)
            }
            break;
        case 2: /*Search by TAN */
            input = input.toUpperCase();
            if (!/^[A-Z]{4}[0-9]{5}[A-Z]{1}$/.test(input)) {
                swal('Error', 'TAN Code should be in appropriate format', 'error');
                $('#txtInsertHere').val('');
                $('#txtInsertHere').focus();
                return false;
            }
            else {
                GetVendorInformation('TAN', input, schemeID, componentID)
            }
            break;
        case 3: /*Search by AADHAR */
            if (!/^[0-9]{12}$/.test(input)) {
                swal('Error', 'Aadhar Number should be in appropriate format', 'error');
                $('#txtInsertHere').val('');
                $('#txtInsertHere').focus();
                return false;
            }
            else {
                GetVendorInformation('AADHAR', input, schemeID, componentID)
            }
            break;
        case 4: /*Search by Name */
            GetVendorInformation('NAME', input, schemeID, componentID)
            break;
    }
}
function GetVendorInformation(searchBy, input, schemeID, componentID) {
    if (searchBy != '' && input != '') {
        $.ajax({
            type: "GET",
            url: "/WorkOrder/Get_VendorInformation",
            data: {
                SearchBy: searchBy,
                Input: input,
                SchemeID: schemeID,
                ComponentID: componentID
            },
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                console.log(data);
                if (data.length > 0) {
                    // $('#div_tblVendor').hide();
                    // $('#tblVendor tbody').empty();

                    $('#div_tblVendorList').show();
                    $('#tblVendorList tbody').empty();
                    // Iterate through the data and append rows to the table
                    $.each(data, function (index, item) {
                        var row = '<tr>' +
                            //'<td id="' + item.id + '" style="color:red" onclick="GetVendor(\'' + item.id + '\', \'' + item.partyName + '\', \'' + item.bankAcctNo + '\', \'' + item.ifsc + '\', \'' + item.bankName + '\', \'' + item.bankBranch + '\', \'' + item.panNo + '\');">Select</td>' +
                            '<td><button id="' + item.id + '" class="btn btn-primary rounded-pill btn-sm" style="color:white;padding: 0.25rem 0.5rem; font-size: 0.875rem;" onclick="GetVendor(\'' + item.id + '\', \'' + item.partyName + '\', \'' + item.bankAcctNo + '\', \'' + item.ifsc + '\', \'' + item.bankName + '\', \'' + item.bankBranch + '\', \'' + item.micrcode + '\', \'' + item.panNo + '\');">Select</button></td>' +
                            '<td>' + item.partyName + '</td>' +
                            '<td>' + item.bankAcctNo + '</td>' +
                            '<td>' + item.ifsc + '</td>' +
                            '<td>' + item.bankName + '</td>' +
                            '<td>' + item.bankBranch + '</td>' +
                            '<td>' + item.micrcode + '</td>' +
                            '<td>' + item.panNo + '</td>' +
                            '</tr>';
                        $('#tblVendorList tbody').append(row);
                    });
                }
                else {
                    swal({
                        title: '',
                        text: 'Are You Sure You Want To Create New Vendor?',
                        icon: 'warning',
                        buttons: true,
                        dangerMode: true,
                        buttons: {
                            cancel: {
                                text: "No",
                                value: null,
                                visible: true,
                                className: "",
                                closeModal: true,
                            },
                            confirm: {
                                text: "Yes",
                                value: true,
                                visible: true,
                                className: "",
                                closeModal: true, 
                            }
                        },
                        dangerMode: true,
                    })
                        .then((willDelete) => {
                            if (willDelete) {
                                window.location.href = '/VendorCreation/VendorCreation'
                            } else {
                                $('#txtInsertHere').val('');
                                $('#txtVendorName').val('');
                                $('#Party_ID').val('');
                                $("#tblVendorList tbody").empty();
                                $('#div_tblVendorList').hide();
                                $('#div_tblVendor').hide();
                            }
                        });
                }
            },
            error: function (err) {
                swal('Something Went Wrong!.', 'Please correct the data', 'error');
                console.log(err);
                return false;
            }
        });
    }
}
function GetVendor(id, partyName, bankAcctNo, ifsc, bankName, bankBranch, micrcode, panNo) {
    //debugger;
    $('#tblVendorList tbody').empty();
    $('#div_tblVendorList').hide();
    $('#div_tblVendor').show();
    $('#tblVendor').show();

    $('#td_Id').text(id);
    $('#td_PartyName').text(partyName);
    $('#td_AccountNo').text(bankAcctNo);
    $('#td_IFSCCode').text(ifsc);
    $('#td_BankName').text(bankName);
    $('#td_BranchName').text(bankBranch);
    $('#td_MicrCode').text(micrcode);
    $('#td_PANNO').text(panNo);

    $('#Party_ID').val(id);
    $('#txtVendorName').val(partyName);
}
function CalculateRemaingAmount() {
    // debugger;
    var ApprovedAmount = ($('#txtApprovedTenderAmount').val().trim() === '') ? 0 : parseFloat($('#txtApprovedTenderAmount').val());
    var PaidAmount = ($('#txtPaidAmount').val().trim() === '') ? 0 : parseFloat($('#txtPaidAmount').val());
    if (PaidAmount > ApprovedAmount) {
        swal('Error', 'Please check paid amount ', 'error');
        $('#txtPaidAmount').val('');
        PaidAmount = 0;
    }
    var RemaingAmount = parseFloat(ApprovedAmount - PaidAmount);
    $('#txtRemaingAmount').val(RemaingAmount);
}
function removeWorkOrderCopy() {
    $('#file_WorkOrderCopy').val('');
    $('#lbl_WorkOrderCopy').text('');
    $('#removeWorkOrderCopy').hide();
}
function validate() {
    if ($('#ddl_FinancialYear').val() < 1) {
        swal('Error', 'Please select financial year', 'error');
        $('#ddl_FinancialYear').focus();
        return false;
    }
    if ($('#ddl_SchemeName').val() < 1) {
        swal('Error', 'Please select scheme', 'error');
        return false;
    }
    if ($('#ddl_AdminApprovalNo').val() < 1) {
        swal('Error', 'Please Select Admin Approval Number', 'error');
        $('#ddl_AdminApprovalNo').focus();
        return false;
    }
    if ($('#txtVendorName').val() == '') {
        swal('Error', 'Please Select Vendor', 'error');
        $('#txtInsertHere').focus();
        return false;
    }
    if ($('#WorkName').val() == '') {
        swal('Error', 'Please Enter Work Name', 'error');
        $('#WorkName').focus();
        return false;
    }
    if ($('#ddlStatus').val() < 1) {
        swal('Error', 'Please Select Work Status', 'error');
        $('#ddlStatus').focus();
        return false;
    }
    if ($('#WorkOrderNumber').val() == '') {
        swal('Error', 'Please Enter Work Order Number', 'error');
        $('#WorkOrderNumber').focus();
        return false;
    }
    if ($('#WorkOrderDate').val() == '') {
        swal('Error', 'Please Select Work Order Start Date', 'error');
        $('#WorkOrderDate').focus();
        return false;
    }
    if ($('#WorkEndDate').val() == '') {
        swal('Error', 'Please Select Work Order End Date', 'error');
        $('#WorkEndDate').focus();
        return false;
    }
    if ($('#txtApprovedTenderAmount').val() == '' || parseFloat($('#txtApprovedTenderAmount').val()) == 0) {
        swal('Error', 'Please Enter Approved Tender Amount', 'error');
        $('#txtApprovedTenderAmount').focus();
        return false;
    }
    if ($('#txtPaidAmount').val() == '') {
        swal('Error', 'Please Enter Paid Amount', 'error');
        $('#txtPaidAmount').focus();
        return false;
    }
    var files = $('#file_WorkOrderCopy')[0].files;
    if (files.length == 0) {
        swal('Error', 'Please Select Work Details Copy.', 'error');
        return false;
    }
    return true;
}
function saveData(callback) {
    // debugger;
    var FinancialYear = $('#ddlFinancialYear option:selected').text();
    var FyId = $("#ddlFinancialYear").val();
    var CashbookId = $("#ddl_SchemeName").val();
    var HeadCodeId = $("#ddl_ComponentName").val();
    var AdminApprovalNo = $("#ddl_AdminApprovalNo").val();
    var PartyId = $("#Party_ID").val();
    var VendorName = $("#txtVendorName").val();
    var WorkName = $("#WorkName").val();
    var WorkStatus = $("#ddlStatus").val();
    var WorkOrderNumber = $("#WorkOrderNumber").val();
    var LastRANo = $("#ddlLastRA").val();
    var WorkStartDate = $("#WorkOrderDate").val();
    var WorkEndDate = $("#WorkEndDate").val();
    var ApprovedAmount = ($('#txtApprovedTenderAmount').val().trim() === '') ? 0 : parseFloat($('#txtApprovedTenderAmount').val());
    var PaidAmount = ($('#txtPaidAmount').val().trim() === '') ? 0 : parseFloat($('#txtPaidAmount').val());
    var RemaingAmount = ($('#txtRemaingAmount').val().trim() === '') ? 0 : parseFloat($('#txtRemaingAmount').val());
    var CostPutToTenderAmount = ($('#txtCostPutToTender').val().trim() === '') ? 0 : parseFloat($('#txtCostPutToTender').val());
    var WorkOrderCopy = $('#lbl_WorkOrderCopy').text();
    if ($('#chk_IsWorkFullAndFinal').prop('checked')) {
        var IsWorkFullAndFinal = 1;
    } else {
        var IsWorkFullAndFinal = 0;
    }
    var formData = {
        WfinancialYearId: $('#ddlFinancialYear').val(),
        ComponetId: $('#ddl_SchemeName').val(),
        HeadCodeId: $('#ddl_ComponentName').val(),
        AdminApprovalNo: $('#ddl_AdminApprovalNo option:selected').text(),
        AdminApprovalId: $('#ddl_AdminApprovalNo').val(),
        PartyId: $('#Party_ID').val(),
        ContracterDetails: $('#txtVendorName').val(),
        DocumentTypeID: $('#ddlSearch').val(),
        DocumentDetails: $('#txtInsertHere').val(),
        WorkName: $('#WorkName').val(),
        WorkStatus: $('#ddlStatus').val(),
        WorkOrderNo: $('#WorkOrderNumber').val(),
        BillSequenceId: $('#ddlLastRA option:selected').text(),
        WorkOrderDate: $('#WorkOrderDate').val(),
        WorkOrderEndDate: $('#WorkEndDate').val(),
        PayableAmount: parseFloat($('#txtApprovedTenderAmount').val()) || 0,
        TotalPaidAmount: parseFloat($('#txtPaidAmount').val()) || 0,
        TotalPending: parseFloat($('#txtRemaingAmount').val()) || 0,
        CostPutToTender: parseFloat($('#txtCostPutToTender').val()) || 0,
        WorkDocumentFileName: $('#lbl_WorkOrderCopy').text(),
        IsBillFullNfinal: $('#chk_IsWorkFullAndFinal').prop('checked') ? true : false
    };
    var token = $('input[name="__RequestVerificationToken"]').val();
    $.ajax({
        url: "/WorkOrder/SaveWorkDetails",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(formData),
        headers: {
            'RequestVerificationToken': token
        },
        success: function (response) {
            var workDetailsUID = response.workDetailsUID;
            if (typeof callback === 'function') {
                callback(workDetailsUID);
            }
            clear();
        },
        error: function (xhr, status, error) {
            if (xhr.status === 400) {
                var errorResponse = JSON.parse(xhr.responseText);
                swal('Error', errorResponse.message, 'error');
            } else {
                console.error("Error:", xhr.responseText);
                swal('Error', 'An error occurred while saving data. Please try again later.', 'error');
            }
        }
    });

}
function currentFyYear() {
    //debugger;
    $('#ddlFinancialYear option').filter(function () {
        return $(this).text() === GetCurrentFinancialYear();
    }).prop('selected', true);
}
function clear() {
    $("input[type=text], textarea").val("");
    $("input[type=date]").val("");
    $("input[type=checkbox]").prop('checked', false);
    $("select").val("0");
    $("#ddlSearch").val("1");
    $("#tblAdminApprovalDetails tbody").empty();
    $("#tblVendorList tbody").empty();
    $('.TClass').hide();
    removeWorkOrderCopy();
    $('#td_Id').val('');
    $('#td_PartyName').val('');
    $('#td_AccountNo').val('');
    $('#td_IFSCCode').val('');
    $('#td_BankName').val('');
    $('#td_BranchName').val('');
    $('#td_MicrCode').val('');
    $('#td_PANNO').val('');
    currentFyYear();
    $('#DivAdminApprovalNo').hide();
    $('#div_tblAdminApprovalDetails').hide();
    $('#div_tblVendorList').hide();
    $('#div_tblVendor').hide();
    $('#DivWorkName').hide();
    $('#DivLastRANo').hide();
    $('#DivApprovedTenderAmount').hide();
    $('#DivCostPutToTender').hide();
    $('#btnSave').hide();
    $('#div_WorkOrderDetails').hide();
    $('#btnSave').prop("disabled", false);


}
function checkWorkisExist(workOrderNumber, adminApprovalNumber) {
    if (workOrderNumber != '') {
        $.ajax({
            type: "GET",
            url: "/WorkOrder/CheckWorkisExist",
            data: {
                WorkOrderNumber: workOrderNumber,
                AdminApprovalNumber: adminApprovalNumber
            },
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
               // debugger;
                if (data.message == true) {
                    swal('Error', 'This work number is already used!', 'error').then(() => {
                        $('#WorkOrderNumber').val('');
                        return false;
                    });
                }
            },
            error: function (err) {
                swal('Something Went Wrong!.', 'Please correct the data', 'error');
                console.log(err);
                return false;
            }
        });
    }
}