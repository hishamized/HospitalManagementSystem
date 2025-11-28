// ---------------------------------------------
// AG GRID COLUMN DEFINITIONS
// ---------------------------------------------
const columnDefs = [
    { headerName: 'Patient Code', field: 'patientCode' },
    { headerName: 'Full Name', field: 'fullName' },
    { headerName: 'Gender', field: 'gender' },
    {
        headerName: 'DOB',
        field: 'dateOfBirth',
        valueFormatter: params => params.value ? new Date(params.value).toLocaleDateString() : ''
    },
    { headerName: 'Email', field: 'email' },
    { headerName: 'Contact', field: 'contactNumber' },
    { headerName: 'Address', field: 'address' },
    { headerName: 'City', field: 'city' },
    { headerName: 'State', field: 'state' },
    { headerName: 'Zip', field: 'zipCode' },
    { headerName: 'Blood Group', field: 'bloodGroup' },
    {
        headerName: 'Emergency Contact',
        cellRenderer: params =>
            `${params.data.emergencyContactName} - ${params.data.emergencyContactNumber} (${params.data.relationshipWithEmergencyContact})`
    },
    {
        headerName: 'Actions',
        cellRenderer: params => `
            <div class="flex gap-2">
                <button class="px-3 py-1 rounded bg-yellow-600 hover:bg-yellow-700 text-white text-sm edit-btn" data-id="${params.data.id}">Edit</button>
                <button class="px-3 py-1 rounded bg-red-600 hover:bg-red-700 text-white text-sm delete-btn" data-id="${params.data.id}">Delete</button>
                <button class="px-3 py-1 rounded bg-blue-600 hover:bg-blue-700 text-white text-sm view-btn" data-id="${params.data.id}">View</button>
            </div>
        `
    }
];

// ---------------------------------------------
// GRID OPTIONS
// ---------------------------------------------
const gridOptions = {
    columnDefs,
    rowData: [],
    pagination: true,
    paginationPageSize: 10,
    defaultColDef: {
        sortable: true,
        filter: true,
        resizable: true
    }
};

let gridApi;

// ---------------------------------------------
// INITIALIZE AG GRID (Updated for AG Grid v29+)
// ---------------------------------------------
function initializeGrid() {
    const gridDiv = document.querySelector('#PatientsGrid');
    gridApi = agGrid.createGrid(gridDiv, gridOptions);

}

// ---------------------------------------------
// FETCH PATIENT LIST
// ---------------------------------------------
function fetchPatientList() {
    $.ajax({
        url: '/Patient/GetAllPatients',
        type: 'GET',
        success: data => gridApi.setGridOption('rowData', data),
        error: err => console.error('Error fetching patients:', err)
    });
}

// ---------------------------------------------
// TAILWIND MODAL HELPERS
// ---------------------------------------------
function openAddModal() {
    document.getElementById('addPatientModal').classList.remove('hidden');
}

function closeAddModal() {
    document.getElementById('addPatientModal').classList.add('hidden');
}

function openEditModal() {
    document.getElementById('editPatientModal').classList.remove('hidden');
}

function closeEditModal() {
    document.getElementById('editPatientModal').classList.add('hidden');
}

// ---------------------------------------------
// ADD PATIENT
// ---------------------------------------------
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
        RelationshipWithEmergencyContact: $('#relationshipWithEmergencyContact').val(),
        Password: $('#password').val()
    };

    $.ajax({
        url: '/Patient/CreatePatient',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(dto),
        success: () => {
            closeAddModal();
            fetchPatientList();
        }
    });
}

// ---------------------------------------------
// LOAD PATIENT FOR EDIT
// ---------------------------------------------
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

            openEditModal();
        }
    });
}

// ---------------------------------------------
// UPDATE PATIENT
// ---------------------------------------------
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
            closeEditModal();
            fetchPatientList();
        }
    });
}

// ---------------------------------------------
// DELETE PATIENT
// ---------------------------------------------
function deletePatient(id) {
    if (confirm('Are you sure you want to delete this patient?')) {
        $.ajax({
            url: `/Patient/DeletePatient?id=${id}`,
            type: 'DELETE',
            success: () => fetchPatientList()
        });
    }
}

// ---------------------------------------------
// DOCUMENT READY
// ---------------------------------------------
$(document).ready(function () {
    initializeGrid();
    fetchPatientList();

    $('#addPatientBtn').on('click', openAddModal);
    $('#savePatientBtn').on('click', addPatient);
    $('#updatePatientBtn').on('click', updatePatient);

    $(document).on('click', '.edit-btn', function () {
        loadPatientForEdit($(this).data('id'));
    });

    $(document).on('click', '.delete-btn', function () {
        deletePatient($(this).data('id'));
    });

    $(document).on('click', '.view-btn', function () {
        window.location.href = '/Patient/ViewPatient?id=' + $(this).data('id');
    });
});
