$(document).ready(function () {

    // ----------------- Modal Setup -----------------
    $('#addEmployeeModal').modal({
        backdrop: 'static',
        keyboard: false,
        show: false
    });

    // Initialize Parsley
    $("#formAddEmployee").parsley();

    // Initialize AG Grid
    initializeGrid();

    // Fetch employees initially
    fetchEmployeeList();

    // Show modal for adding new employee
    $('#btnShowAddEmployeeModal').click(function () {
        resetForm();
        $('#addEmployeeModal').modal('show');
    });

    // Close modal manually
    $('#btnCloseModal').click(function () {
        $('#addEmployeeModal').modal('hide');
        $("#formAddEmployee").parsley().reset();
    });

    // Handle form submit (Add or Update)
    $('#formAddEmployee').submit(async function (e) {
        e.preventDefault();
        if (!$(this).parsley().isValid()) return;

        const employeeId = $(this).attr('data-id');

        if (employeeId) {
            await updateEmployee(employeeId);
        } else {
            await addEmployee();
        }

        fetchEmployeeList();
        $('#addEmployeeModal').modal('hide');
        resetForm();
    });

    // Edit button handler
    $(document).on('click', '.edit-btn', function () {
        const id = $(this).data('id');
        loadEmployeeForEdit(id);
    });

    // Delete button handler
    $(document).on('click', '.delete-btn', function () {
        const id = $(this).data('id');
        deleteEmployeeById(id);
    });

});

// ====================== AG Grid Setup ======================

const columnDefs = [
    { headerName: '#', field: 'id', filter: false, resizable: false, width: 80 },
    { headerName: 'Name', field: 'name', filter: "agTextColumnFilter" },
    { headerName: 'Description', field: 'description', filter: "agTextColumnFilter" },
    { headerName: 'Phone Number', field: 'phoneNumber', filter: "agTextColumnFilter" },
    { headerName: 'Email', field: 'email', filter: "agTextColumnFilter" },
    { headerName: 'Address', field: 'address', filter: "agTextColumnFilter" },
    {
        headerName: 'Actions',
        field: 'actions',
        cellRenderer: function (params) {
            return `
                <button class="btn btn-sm btn-primary edit-btn" data-id="${params.data.id}">
                    <i class="fas fa-edit"></i>
                </button>
                <button class="btn btn-sm btn-danger delete-btn" data-id="${params.data.id}">
                    <i class="fas fa-trash-alt"></i>
                </button>
            `;
        },
        width: 150
    }
];

const gridOptions = {
    columnDefs: columnDefs,
    rowData: [],
    animateRows: true,
    pagination: true,
    paginationPageSize: 10,
    defaultColDef: {
        resizable: true,
        filter: true,
        sortable: true,
        floatingFilter: true
    },
    onGridReady: params => {
        params.api.sizeColumnsToFit();
    }
};

function initializeGrid() {
    const gridDiv = document.querySelector('#EmployeesGrid');
    new agGrid.Grid(gridDiv, gridOptions);
}

// ====================== CRUD Operations ======================

// Fetch all employees
function fetchEmployeeList() {
    $.ajax({
        url: '/Employee/GetEmployeeList',
        method: 'GET',
        success: function (data) {
            console.log('Fetched employees:', data);
            if (Array.isArray(data)) {
                gridOptions.api.setRowData(data);
            } else {
                console.error('Invalid data format:', data);
            }
        },
        error: function (err) {
            console.error('Error fetching employee data:', err);
        }
    });
}

// Add new employee
async function addEmployee() {
    const employee = getFormData();
    try {
        await $.ajax({
            url: '/Employee/AddEmployee',
            method: 'POST',
            data: employee,
            success: function () {
                Swal.fire('Added!', 'Employee added successfully.', 'success');
            },
            error: function (err) {
                Swal.fire('Error', 'Failed to add employee.', 'error');
                console.error(err);
            }
        });
    } catch (e) {
        console.error('Add Employee Error:', e);
    }
}

// Load employee data into form for editing
function loadEmployeeForEdit(id) {
    $.ajax({
        url: `/Employee/GetEmployeeById?empIdentityCode=${id}`,
        method: 'GET',
        success: function (data) {
            $('#employeeName').val(data.name);
            $('#employeeDescription').val(data.description);
            $('#employeePhoneNumber').val(data.phoneNumber);
            $('#employeeEmail').val(data.email);
            $('#employeeAddress').val(data.address);

            $('#formAddEmployee').attr('data-id', id);
            $('#addEmployeeModalLabel').text('Edit Employee');
            $('#btnAddEmployee').html('<i class="fas fa-save"></i> Save Changes');
            $('#addEmployeeModal').modal('show');
        },
        error: function (err) {
            console.error('Error fetching employee:', err);
        }
    });
}

// Update employee
async function updateEmployee(id) {
    const employee = getFormData();
    employee.Id = id;
    try {
        await $.ajax({
            url: '/Employee/UpdateEmployee',
            method: 'POST',
            data: employee,
            success: function () {
                Swal.fire('Updated!', 'Employee details updated successfully.', 'success');
            },
            error: function (err) {
                Swal.fire('Error', 'Failed to update employee.', 'error');
                console.error(err);
            }
        });
    } catch (e) {
        console.error('Update Employee Error:', e);
    }
}

// Delete employee
function deleteEmployeeById(id) {
    Swal.fire({
        title: 'Are you sure?',
        text: 'Do you want to delete this employee?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Yes, delete it!',
        cancelButtonText: 'Cancel'
    }).then(result => {
        if (result.isConfirmed) {
            $.ajax({
                url: '/Employee/DeleteEmployeeById',
                method: 'POST',
                contentType: 'application/json',
                data: JSON.stringify({ ids: [id] }),
                success: function () {
                    fetchEmployeeList();
                    Swal.fire('Deleted!', 'Employee has been deleted.', 'success');
                },
                error: function (err) {
                    Swal.fire('Error', 'Failed to delete employee.', 'error');
                    console.error(err);
                }
            });
        }
    });
}

// Get form data
function getFormData() {
    return {
        Name: $('#employeeName').val(),
        Description: $('#employeeDescription').val(),
        PhoneNumber: $('#employeePhoneNumber').val(),
        Email: $('#employeeEmail').val(),
        Address: $('#employeeAddress').val()
    };
}

// Reset form
function resetForm() {
    $('#formAddEmployee')[0].reset();
    $("#formAddEmployee").parsley().reset();
    $('#formAddEmployee').removeAttr('data-id');
    $('#addEmployeeModalLabel').text('Add Employee');
    $('#btnAddEmployee').html('<i class="fas fa-user-plus"></i> Add Employee');
}
