// Column Definitions
const columnDefs = [
    { headerName: 'Patient Code', field: 'patientCode' },
    { headerName: 'Full Name', field: 'fullName' },
    { headerName: 'Gender', field: 'gender' },
    { headerName: 'DOB', field: 'dateOfBirth', valueFormatter: params => new Date(params.value).toLocaleDateString() },
    { headerName: 'Email', field: 'email' },
    { headerName: 'Contact', field: 'contactNumber' },
    { headerName: 'Address', field: 'address' },
    { headerName: 'City', field: 'city' },
    { headerName: 'State', field: 'state' },
    { headerName: 'Zip', field: 'zipCode' },
    { headerName: 'Blood Group', field: 'bloodGroup' },
    { headerName: 'Emergency Contact', field: 'emergencyContact', cellRenderer: params => `${params.data.emergencyContactName} - ${params.data.emergencyContactNumber} (${params.data.relationshipWithEmergencyContact})` },
    {
        headerName: 'Actions',
        field: 'actions',
        cellRenderer: params => `
            <button class="btn btn-sm btn-warning edit-btn" data-id="${params.data.id}">Edit</button>
            <button class="btn btn-sm btn-danger delete-btn" data-id="${params.data.id}">Delete</button>
            <button class="btn btn-sm btn-primary view-btn" data-id="${params.data.id}">View</button>
        `
    }
];

// Grid Options
const gridOptions = {
    columnDefs: columnDefs,
    rowData: [],
    pagination: true,
    paginationPageSize: 10,
    defaultColDef: { sortable: true, filter: true, resizable: true },
    onGridReady: params => params.api.sizeColumnsToFit()
};

// Initialize AG Grid
function initializeGrid() {
    const gridDiv = document.querySelector('#PatientsGrid');
    new agGrid.Grid(gridDiv, gridOptions);
}

// Fetch Patients
function fetchPatientList() {
    $.ajax({
        url: '/Patient/GetAllPatients',
        type: 'GET',
        success: data => gridOptions.api.setRowData(data),
        error: (xhr, status, err) => console.error('Error fetching patients:', err)
    });
}

// Add Patient
function addPatient() {
    const dto = {
        FullName: $('#fullName').val(),
        Gender: $('#gender').val(),
        DateOfBirth: $('#dob').val(),
        Email: $('#email').val(),
        ContactNumber: $('#contactNumber').val(),
        AlternateContactNumber: $('#alternateContactNumber').val(),
        Address: $('#address').val(),
        City: $('#city').val(),
        State: $('#state').val(),
        ZipCode: $('#zipCode').val(),
        BloodGroup: $('#bloodGroup').val(),
        EmergencyContactName: $('#emergencyContactName').val(),
        EmergencyContactNumber: $('#emergencyContactNumber').val(),
        RelationshipWithEmergencyContact: $('#relationshipWithEmergencyContact').val()
    };

    $.ajax({
        url: '/Patient/CreatePatient',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(dto),
        success: () => {
            $('#addPatientModal').modal('hide');
            fetchPatientList();
        }
    });
}

// Load Patient for Edit
function loadPatientForEdit(id) {
    $.ajax({
        url: `/Patient/GetPatientById?id=${id}`,
        type: 'GET',
        success: data => {
            $('#editId').val(data.id);
            $('#editFullName').val(data.fullName);
            $('#editGender').val(data.gender);
            $('#editDob').val(data.dateOfBirth.split('T')[0]);
            $('#editEmail').val(data.email);
            $('#editContactNumber').val(data.contactNumber);
            $('#editAlternateContactNumber').val(data.alternateContactNumber);
            $('#editAddress').val(data.address);
            $('#editCity').val(data.city);
            $('#editState').val(data.state);
            $('#editZipCode').val(data.zipCode);
            $('#editBloodGroup').val(data.bloodGroup);
            $('#editEmergencyContactName').val(data.emergencyContactName);
            $('#editEmergencyContactNumber').val(data.emergencyContactNumber);
            $('#editRelationshipWithEmergencyContact').val(data.relationshipWithEmergencyContact);
            $('#editPatientModal').removeClass('d-none');
        }
    });
}

// Update Patient
function updatePatient() {
    const dto = {
        Id: $('#editId').val(),
        FullName: $('#editFullName').val(),
        Gender: $('#editGender').val(),
        DateOfBirth: $('#editDob').val(),
        Email: $('#editEmail').val(),
        ContactNumber: $('#editContactNumber').val(),
        AlternateContactNumber: $('#editAlternateContactNumber').val(),
        Address: $('#editAddress').val(),
        City: $('#editCity').val(),
        State: $('#editState').val(),
        ZipCode: $('#editZipCode').val(),
        BloodGroup: $('#editBloodGroup').val(),
        EmergencyContactName: $('#editEmergencyContactName').val(),
        EmergencyContactNumber: $('#editEmergencyContactNumber').val(),
        RelationshipWithEmergencyContact: $('#editRelationshipWithEmergencyContact').val()
    };

    $.ajax({
        url: '/Patient/UpdatePatient',
        type: 'PUT',
        contentType: 'application/json',
        data: JSON.stringify(dto),
        success: () => {
            $('#editPatientModal').modal('hide');
            fetchPatientList();
        }
    });
}

// Delete Patient
function deletePatient(id) {
    if (confirm('Are you sure?')) {
        $.ajax({
            url: `/Patient/DeletePatient?id=${id}`,
            type: 'DELETE',
            success: fetchPatientList
        });
    }
}

// Document Ready
$(document).ready(function () {
    initializeGrid();
    fetchPatientList();

    // Modals
    // Show modal (like AG Grid popup)
    $('#addPatientBtn').on('click', function () {
        $('#addPatientModal').modal('show');
    });

    $('#savePatientBtn').click(addPatient);

    $('#updatePatientBtn').click(updatePatient);

    // Dynamic buttons (edit, delete, view)
    $(document).on('click', '.edit-btn', function () { $('#editPatientModal').modal('show'); loadPatientForEdit($(this).data('id')); });
    $(document).on('click', '.delete-btn', function () { deletePatient($(this).data('id')); });
    $(document).on('click', '.view-btn', function () { window.location.href = '/Patient/ViewPatient?id=' + $(this).data('id'); });
});


