$(document).ready(function () {

    // ✅ Initialize AG Grid configuration
    const gridOptions = {
        columnDefs: [
            {
                headerName: "ID",
                field: "documentId",
                width: 90,
                filter: 'agNumberColumnFilter'
            },
            {
                headerName: "Doctor Name",
                field: "fullName",
                flex: 1,
                filter: 'agTextColumnFilter'
            },
            {
                headerName: "Specialization",
                field: "specialization",
                flex: 1,
                filter: 'agTextColumnFilter'
            },
            {
                headerName: "File Name",
                field: "fileName",
                flex: 1.2,
                filter: 'agTextColumnFilter',
                cellRenderer: params => {
                    const name = params.value || 'Untitled';
                    return `<span class="text-truncate" title="${name}">${name}</span>`;
                }
            },
            {
                headerName: "Type",
                field: "fileType",
                width: 120,
                filter: 'agTextColumnFilter'
            },
            {
                headerName: "Size (KB)",
                field: "fileSize",
                width: 130,
                filter: 'agNumberColumnFilter',
                valueFormatter: params => {
                    const size = params.value;
                    if (!size || size <= 0) return "0";
                    return (size / 1024).toFixed(1);
                }
            },
            {
                headerName: "Uploaded At",
                field: "uploadedAt",
                width: 180,
                filter: 'agDateColumnFilter',
                filterParams: {
                    comparator: function (filterLocalDateAtMidnight, cellValue) {
                        if (!cellValue) return -1;
                        const cellDate = new Date(cellValue);
                        if (cellDate < filterLocalDateAtMidnight) return -1;
                        if (cellDate > filterLocalDateAtMidnight) return 1;
                        return 0;
                    }
                },
                valueFormatter: params => {
                    if (!params.value) return "-";
                    const date = new Date(params.value);
                    return date.toLocaleString();
                }
            },
            {
                headerName: "Uploaded By",
                field: "uploadedBy",
                width: 150,
                filter: 'agTextColumnFilter'
            },
            {
                headerName: "Document",
                field: "filePath",
                width: 230,
                cellRenderer: (params) => {
                    const url = params.data?.filePath;
                    if (!url) return `<span class="text-muted fst-italic">No File</span>`;
                    const name = params.data.fileName || "View File";
                    return `
                <div class="d-flex align-items-center justify-content-between">
                    <span class="text-truncate" title="${name}" style="max-width:130px;">${name}</span>
                    <button class="btn btn-sm btn-outline-secondary view-btn ms-2" data-url="${url}" title="Open Document">
                        <i class="bi bi-box-arrow-up-right"></i> View
                    </button>
                </div>
            `;
                }
            },
            {
                headerName: "Actions",
                width: 150,
                cellRenderer: params => `
            <div class="d-flex justify-content-center">
                <button class="btn btn-sm btn-info me-2 edit-btn" data-id="${params.data.documentId}" title="Edit Document">
                   Edit
                </button>
                <button class="btn btn-sm btn-danger delete-btn" data-id="${params.data.documentId}" title="Delete Document">
                  Delete  
                </button>
            </div>
        `
            }
        ],


        defaultColDef: {
            sortable: true,
            filter: true,
            floatingFilter: true,
            resizable: true
        },

        pagination: true,
        paginationPageSize: 10,
        animateRows: true,
        domLayout: 'normal',
        suppressHorizontalScroll: false, 

        onGridReady: loadDocuments,

        getContextMenuItems: (params) => [
            {
                name: '🔍 View Document',
                action: () => params.node.data.filePath && window.open(params.node.data.filePath, '_blank')
            },
            {
                name: '✏️ Edit Document',
                action: () => openEditModal(params.node.data)
            },
            {
                name: '🗑 Delete Document',
                action: () => deleteDocument(params.node.data.id)
            },
            'separator', 'copy', 'export'
        ]
    };

    // ✅ Initialize the grid
    const gridDiv = document.querySelector('#doctorDocumentsGrid');
    new agGrid.Grid(gridDiv, gridOptions);

    // ✅ Load document data from server
    function loadDocuments() {
        $.ajax({
            url: '/DoctorDocument/GetAllDoctorDocuments',
            method: 'GET',
            success: function (response) {
                console.log(response);
                if (response.success && response.data?.length) {
                    gridOptions.api.setRowData(response.data);
                } else {
                    gridOptions.api.setRowData([]);
                    console.warn(response.message || "No documents found.");
                }
            },
            error: function (xhr) {
                console.error("Error loading documents:", xhr.responseText);
                gridOptions.api.setRowData([]);
            }
        });
    }

    // ✅ Handle "View" button clicks
    $(document).on('click', '.view-btn', function () {
        const fileUrl = $(this).data('url');
        if (fileUrl) {
            window.open(fileUrl, '_blank');
        } else {
            alert('File not found.');
        }
    });

    // ✅ Edit document button click

    $(document).on('click', '.edit-btn', function () {
        const id = $(this).data('id');

        // Get selected row data directly from AG Grid
        let selectedRowData = null;
        gridOptions.api.forEachNode(node => {
            if (node.data.documentId == id) {   // or node.data.documentId depending on your field name
                selectedRowData = node.data;
            }
        });

        if (!selectedRowData) {
            alert("Document data not found in grid.");
            return;
        }

        // Populate modal fields
        openEditDocumentModal(selectedRowData);
    });

    function openEditDocumentModal(doc) {
        // Hidden fields
        $('#editDocumentId').val(doc.documentId);     // fix here, previously doc.id
        $('#editDoctorId').val(doc.doctorId);
        $('#editOldFilePath').val(doc.filePath);
        $('#editOldFileName').val(doc.fileName);

        // Read-only doctor name
        $('#editDoctorName').val(doc.fullName);       // fix here, previously doc.doctorName

        // Clear any previous file input
        $('#editFileUpload').val('');

        // Open modal
        $('#editDocumentModal').modal('show');
    }



    // ✅ Handle Edit Form Submit
    $('#editDocumentForm').on('submit', function (e) {
        e.preventDefault();

        const form = this;
        const formData = new FormData(form);

        $.ajax({
            url: '/DoctorDocument/EditDocument',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            beforeSend: function () {
                // Optional: disable button and show loader
                $('#editDocumentForm button[type="submit"]').prop('disabled', true).text('Updating...');
            },
            success: function (result) {
                $('#editDocumentForm button[type="submit"]').prop('disabled', false).text('Update Document');

                if (result.success) {
                    loadDocuments();
                    $('#editDocumentModal').modal('hide');
                    Swal.fire({
                        icon: 'success',
                        title: 'Updated!',
                        text: 'Document updated successfully.',
                        timer: 2000,
                        showConfirmButton: false
                    });

                    // Refresh AG Grid or table
                    if (typeof refreshDoctorDocumentsGrid === 'function') {
                        refreshDoctorDocumentsGrid();
                    }
                } else {
                    Swal.fire({
                        icon: 'error',
                        title: 'Update Failed',
                        text: 'Unable to update document. Please try again.'
                    });
                }
            },
            error: function () {
                $('#editDocumentForm button[type="submit"]').prop('disabled', false).text('Update Document');
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: 'An error occurred while updating the document.'
                });
            }
        });
    });

    // ✅ Delete document
    $(document).on('click', '.delete-btn', function () {
        const id = $(this).data('id');

        // ✅ Find the matching row data directly from the grid
        let filePath = null;
        gridOptions.api.forEachNode(node => {
            if (node.data.documentId == id) {
                filePath = node.data.filePath;
            }
        });


        Swal.fire({
            title: "Are you sure?",
            text: "This will permanently delete the document!",
            icon: "warning",
            showCancelButton: true,
            confirmButtonText: "Yes, delete it!",
            cancelButtonText: "Cancel",
            confirmButtonColor: "#d33",
            cancelButtonColor: "#3085d6"
        }).then((result) => {
            if (result.isConfirmed) {

                // Read AntiForgeryToken
                const token = $('input[name="__RequestVerificationToken"]').val();

                $.ajax({
                    url: '/DoctorDocument/DeleteDocument',
                    type: 'POST',
                    data: {
                        __RequestVerificationToken: token,
                        id: id,
                        filePath: filePath // ✅ send it
                    },
                    success: function (response) {
                        if (response.success) {
                            Swal.fire({
                                icon: 'success',
                                title: 'Deleted!',
                                text: response.message || 'Document deleted successfully.',
                                timer: 2000,
                                showConfirmButton: false
                            });
                            loadDocuments();
                        } else {
                            Swal.fire({
                                icon: 'error',
                                title: 'Failed!',
                                text: response.message || 'Unable to delete document.'
                            });
                        }
                    },
                    error: function (xhr) {
                        Swal.fire({
                            icon: 'error',
                            title: 'Error!',
                            text: xhr.responseText || 'An error occurred while deleting.'
                        });
                    }
                });

            }
        });
    });


    // ✅ Add 
    $('#documentForm').on('submit', function (e) {
        e.preventDefault();

        const formData = new FormData(this);
        const id = $('#documentId').val();
        const url = id ? '/DoctorDocument/Update' : '/DoctorDocument/UploadDocument';

        $.ajax({
            url,
            type: 'POST',
            data: formData,
            contentType: false,
            processData: false,
            success: function (response) {
                alert(response.message || 'Saved successfully.');
                $('#addDocumentModal').modal('hide');
                loadDocuments();
            },
            error: function () {
                alert('An error occurred while saving.');
            }
        });
    });

    // ✅ Load doctor dropdown
    function loadDoctorsDropdown() {
        $.ajax({
            url: '/Doctor/GetAllDoctors',
            type: 'GET',
            success: function (response) {
                const $select = $('#doctorSelect');
                $select.empty().append('<option value="">-- Select Doctor --</option>');
                if (response.success && response.data?.length) {
                    response.data.forEach(doc => {
                        const text = `${doc.fullName} (${doc.specialization})`;
                        $select.append(`<option value="${doc.id}">${text}</option>`);
                    });
                }
            },
            error: function (xhr) {
                console.error('Error loading doctors:', xhr.responseText);
            }
        });
    }

    // ✅ Add button opens modal
    $('#btnAddDocument').on('click', function () {
        resetForm();
        loadDoctorsDropdown();
        $('#addDocumentModal').modal('show');
    });

    // ✅ Global search
    $('#quickFilter').on('input', function () {
        gridOptions.api.setQuickFilter(this.value);
    });

    // ✅ Utility
    function resetForm() {
        $('#documentForm')[0].reset();
        $('#documentId').val('');
    }
});
