var dataTable;

$(document).ready(function () {
    var url = window.location.search;
    if (url.includes("inprocess")) {
        loadDatatable("inprocess");
    }
    else {
        if (url.includes("completed")) {
            loadDatatable("completed");
        }
        else {
            if (url.includes("approved")) {
                loadDatatable("approved");
            }
            else {
                if (url.includes("pending")) {
                    loadDatatable("pending");
                }
                else {
                    loadDatatable("all");
                }

            }

        }
    }
});

function loadDatatable(status) {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/admin/order/getall?status='+status },       
            "columns": [
                { data: 'id',"width":"5%" },
                { data: 'name', "width": "15%" },
                { data: 'phoneNumber', "width": "15%" },
                { data: 'applicationUser.email', "width": "20%" },
                { data: 'orderStatus', "width": "10%" },
                { data: 'orderTotal', "width": "15%" },
                {
                    data: 'id',
                    "render": function (data) {
                        return `<div class =w-75 btn-group role="group">
                          <a href="/admin/order/details?orderId=${data}" class="btn btn-primary mx-2"> <i class="bi-pencil-square"></i></a>
                        </div>`
                    },

                    "width": "30%"
                }
            ]
    });
}

