document.addEventListener('DOMContentLoaded', function () {

    // ✅ Define AG Grid column structure
    const columnDefs = [
        { headerName: "Doctor Code", field: "doctorCode", flex: 1 },
        { headerName: "Full Name", field: "fullName", flex: 1 },
        { headerName: "Gender", field: "gender", width: 120 },
        { headerName: "Specialization", field: "specialization", flex: 1 },
        { headerName: "Experience (Years)", field: "experienceYears", width: 180 },
        { headerName: "City", field: "city", flex: 1 },
        { headerName: "Department", field: "departmentName", flex: 1 },
        {
            headerName: "Actions",
            field: "id",
            width: 160,
            cellRenderer: function (params) {
                return `
                    <button class="btn btn-sm btn-outline-primary view-docs-btn" data-id="${params.value}">
                        <i class="bi bi-eye"></i> View Documents
                    </button>`;
            }
        }
    ];

    // ✅ Grid options
    const gridOptions = {
        columnDefs: columnDefs,
        defaultColDef: {
            sortable: true,
            filter: true,
            resizable: true,
        },
        pagination: true,
        paginationPageSize: 10,
        onGridReady: function () {
            loadDoctors();
        }
    };

    // ✅ Initialize AG Grid
    const doctorGrid = document.querySelector('#doctorGrid');
    new agGrid.Grid(doctorGrid, gridOptions);

    // ✅ Fetch doctor data using AJAX
    function loadDoctors() {
        $.ajax({
            url: '/Doctor/GetAllDoctors',
            method: 'GET',
            success: function (response) {
                console.log("Doctor data received:", response);

                // Check for array
                if (Array.isArray(response)) {
                    gridOptions.api.setRowData(response);
                } else if (response && response.data && Array.isArray(response.data)) {
                    // If response is wrapped (e.g., { data: [...] })
                    gridOptions.api.setRowData(response.data);
                } else {
                    console.warn("Unexpected response format:", response);
                    gridOptions.api.setRowData([]);
                }
            },
            error: function (xhr) {
                console.error('Failed to load doctors:', xhr);
                alert('Error loading doctors.');
            }
        });
    }

   // ✅ Handle View Documents button click (Fetch documents by DoctorId)
$(document).on('click', '.view-docs-btn', function () {
    const doctorId = $(this).data('id');
    console.log("Fetching documents for Doctor ID:", doctorId);

    $.ajax({
        url: '/DoctorDocument/GetDoctorDocumentsByDoctorId',
        method: 'GET',
        data: { doctorId: doctorId },
        success: function (docs) {
            console.log("Documents received:", docs);

            const container = $('#doctorDocumentsContainer');
            container.empty();

            if (!docs || docs.length === 0) {
                container.html('<p class="text-muted text-center mb-0">No documents found for this doctor.</p>');
            } else {
                docs.forEach(doc => {
                    const fileType = (doc.fileType || '').toLowerCase();
                    let preview = '';

                    // Determine preview based on file type
                    if (fileType.includes('jpg') || fileType.includes('jpeg') || fileType.includes('png') || fileType.includes('gif')) {
                        preview = `<img src="${doc.filePath}" class="img-fluid rounded mb-2" style="height: 150px; object-fit: cover;">`;
                    } else if (fileType.includes('pdf')) {
                        preview = `<img src="/images/pdf-icon.png" class="img-fluid mb-2" style="height: 100px;">`;
                    } else {
                        preview = `<img src="/images/file-icon.png" class="img-fluid mb-2" style="height: 100px;">`;
                    }

                    const card = `
                <div class="col-md-4 col-sm-6">
                    <div class="card shadow-sm border-0 h-100">
                        <div class="card-body text-center">
                            ${preview}
                            <h6 class="fw-semibold text-truncate" title="${doc.fileName}">${doc.fileName}</h6>
                            <a href="${doc.filePath}" download class="btn btn-sm btn-outline-primary mt-2">
                                <i class="bi bi-download"></i> Download
                            </a>
                        </div>
                    </div>
                </div>`;
                    container.append(card);
                });
            }

            // Show modal after rendering
            $('#doctorDocumentsModal').modal('show');
        }

    });
});

});
