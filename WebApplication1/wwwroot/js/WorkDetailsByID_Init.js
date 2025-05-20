function initGetWrokDetailsByID(componentID,AAID) {
    var table = $('#tblWorkOrderDetails');
    $('#tblWorkOrderDetails').DataTable().destroy();
    $.ajax({
        type: "GET",
        data: {
            HeadCodeID: componentID,
            AdminApprovalID: AAID,
        },
        url: "/WorkOrder/GetWrokDetailsByID",
        success: function (data) {//start of ajax success
            var oTable = table.dataTable({//start of datatable
                "language": {
                    "aria": {
                        "sortAscending": ": activate to sort column ascending",
                        "sortDescending": ": activate to sort column descending"
                    },
                    "emptyTable": "No data available in table",
                    "info": "Showing _START_ to _END_ of _TOTAL_ entries",
                    "infoEmpty": "No entries found",
                    "infoFiltered": "(filtered1 from _MAX_ total entries)",
                    "lengthMenu": "Show records: _MENU_",
                    "search": "Search:",
                    "zeroRecords": "No matching records found",
                    'loadingRecords': '&nbsp;',
                    'processing': '<div class="spinner"></div>'
                },
                ordering: false,
                searching: false,
                paging: false,
                info: false,
                data: data,
                columns: [
                    {
                        'data': null,
                        "render": function (data, type, row, meta) {
                            return meta.row + 1;
                        }, className: "text-center"
                    },//0
                    {
                        'data': 'workDocumentFileName', 'render': function (data, type, row, meta) {
                            if (row['workDocumentFileName'] == '' || row['workDocumentFileName'] == null) {
                                return '';
                            }
                            else {
                                var a = "DisplayPdfFile('" + row["workDocumentFileName"] + "')"; //Local For Server                   
                                return '<a href="#" style="color:red !important;" onclick="' + a + '"><div class="btn btn-icon btn-sm btn-light"><i class="far fa-file"></i></div></a>';

                            }
                        }, className: "text-center"
                    }, //1
                    //{
                    //    data: 'workDocumentFileName',
                    //    render: function (data, type, row, meta) {
                    //        return row.workDocumentFileName;
                    //    }
                    //},//1
                    {
                        data: 'headNameEnglish',
                        render: function (data, type, row, meta) {
                            if (data === null || data === undefined) {
                                return '-';
                            } else {
                                return data;
                            }
                        }
                    },//2
                    {
                        data: 'fyName',
                        render: function (data, type, row, meta) {
                            if (data === null || data === undefined) {
                                return '-';
                            } else {
                                return data;
                            }
                        }
                    },//3
                    {
                        data: 'workName',
                        render: function (data, type, row, meta) {
                            if (data === null || data === undefined) {
                                return '-';
                            } else {
                                return data ;
                            }
                        }
                    },//4
                    {
                        data: 'adminApprovalNo',
                        render: function (data, type, row, meta) {
                            if (data === null || data === undefined) {
                                return '-';
                            } else {
                                return data;
                            }
                        }
                    },//5
                    {
                        data: 'adminApprovalDate',
                        render: function (data, type, row, meta) {
                            if (data === null || data === undefined) {
                                return '-';
                            } else {
                                return data;
                            }
                        }
                    },//6
                    {
                        data: 'workOrderNo',
                        render: function (data, type, row, meta) {
                            if (data === null || data === undefined) {
                                return '-';
                            } else {
                                return data;
                            }
                        }
                    },//5
                    {
                        data: 'workOrderDate',
                        render: function (data, type, row, meta) {
                            if (data === null || data === undefined) {
                                return '-';
                            } else {
                                return data;
                            }
                        }
                    },//5
                    {
                        data: 'workOrderEndDate',
                        render: function (data, type, row, meta) {
                            if (data === null || data === undefined) {
                                return '-';
                            } else {
                                return data;
                            }
                        }
                    },//5
                    {
                        data: 'contracterDetails',
                        render: function (data, type, row, meta) {
                            if (data === null || data === undefined) {
                                return '-';
                            } else {
                                return data;
                            }
                        }
                    },//5
                    {
                        data: 'panNo',
                        render: function (data, type, row, meta) {
                            if (data === null || data === undefined) {
                                return '-';
                            } else {
                                return data;
                            }
                        }
                    },//5
                    {
                        'data': 'aaAmount', 'render': function (data, type, row, meta) {
                            return parseFloat(row.aaAmount).toLocaleString('en-IN', {
                                maximumFractionDigits: 2,
                                style: 'currency',
                                currency: 'INR'
                            });
                        }, className: "text-right"
                    },//10
                    {
                        'data': 'payableAmount', 'render': function (data, type, row, meta) {
                            return parseFloat(row.payableAmount).toLocaleString('en-IN', {
                                maximumFractionDigits: 2,
                                style: 'currency',
                                currency: 'INR'
                            });
                        }, className: "text-right"
                    },//10
                    {
                        'data': 'totalPaidAmount', 'render': function (data, type, row, meta) {
                            return parseFloat(row.totalPaidAmount).toLocaleString('en-IN', {
                                maximumFractionDigits: 2,
                                style: 'currency',
                                currency: 'INR'
                            });
                        }, className: "text-right"
                    },//10
                    {
                        'data': 'totalPending', 'render': function (data, type, row, meta) {
                            return parseFloat(row.totalPending).toLocaleString('en-IN', {
                                maximumFractionDigits: 2,
                                style: 'currency',
                                currency: 'INR'
                            });
                        }, className: "text-right"
                    },//10
                    {

                        data: 'billSequenceID',
                        render: function (data, type, row, meta) {
                            if (data === null || data === undefined) {
                                return '-';
                            } else {
                                return data;
                            }
                        }
                    },//5
                   
                ],
                "lengthMenu": [
                    [-1],
                    ["All"] // change per page values here
                ],
            }); //end of datatable

        } //end of ajax success
    }); //end of ajax
    table.show();
} //end of function initBillTable