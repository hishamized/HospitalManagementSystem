document.addEventListener("DOMContentLoaded", function () {

    /* ------------------------------
       AG GRID COLUMN DEFINITIONS
    ------------------------------ */
    const columnDefs = [
        { field: "id", headerName: "ID", width: 90 },
        { field: "reportingTime", headerName: "Reporting Time", flex: 1 },
        { field: "leavingTime", headerName: "Leaving Time", flex: 1 },

        {
            field: "daysOfWeek",
            headerName: "Days",
            flex: 1,
            valueFormatter: (params) => {
                if (!params.value) return 'None';

                const days = [];
                const dayNames = [
                    { bit: 1, name: 'Sun' },
                    { bit: 2, name: 'Mon' },
                    { bit: 4, name: 'Tue' },
                    { bit: 8, name: 'Wed' },
                    { bit: 16, name: 'Thu' },
                    { bit: 32, name: 'Fri' },
                    { bit: 64, name: 'Sat' }
                ];

                dayNames.forEach(day => {
                    if (params.value & day.bit) days.push(day.name);
                });

                return days.length > 0 ? days.join(', ') : 'None';
            }
        },

        {
            headerName: "Actions",
            width: 180,
            cellRenderer: (params) => {
                const id = params.data.id;

                // IMPORTANT: store row data in HTML (for edit)
                const rowJson = encodeURIComponent(JSON.stringify(params.data));

                return `
                    <div class="flex gap-2">
                        <button 
                            class="px-3 py-1 text-sm rounded-lg bg-purple-600 hover:bg-purple-700 text-white transition edit-btn"
                            data-row="${rowJson}">
                            <i class="fas fa-edit"></i> Edit
                        </button>

                        <button 
                            class="px-3 py-1 text-sm rounded-lg bg-red-600 hover:bg-red-700 text-white transition delete-btn"
                            data-id="${id}">
                            <i class="fas fa-trash"></i> Delete
                        </button>
                    </div>
                `;
            }
        }
    ];

    /* ------------------------------
       GRID INITIALIZATION
    ------------------------------ */
    const gridOptions = {
        columnDefs,
        rowHeight: 50,
        animateRows: true,
        defaultColDef: {
            sortable: true,
            filter: true,
            resizable: true
        },
        onGridReady: (p) => p.api.sizeColumnsToFit(),
        onGridSizeChanged: (p) => p.api.sizeColumnsToFit()
    };

    new agGrid.Grid(document.querySelector("#slotsGrid"), gridOptions);
    loadSlots();


    function loadSlots() {
        fetch("/Slot/GetAllSlots")
            .then(res => res.json())
            .then(response => {
                const slots = response.data || [];
                gridOptions.api.setRowData(slots);
            })
            .catch(err => {
                console.error("Error loading slots:", err);
                gridOptions.api.setRowData([]);
            });
    }


    /* ------------------------------
       ADD SLOT
    ------------------------------ */
    document.getElementById("btnAddSlot")?.addEventListener("click", () => {
        document.getElementById("addSlotForm").reset();
        window.showModal("addSlotModal");
    });

    document.getElementById("addSlotForm").addEventListener("submit", function (e) {
        e.preventDefault();

        let daysOfWeek = 0;
        document.querySelectorAll(".day-checkbox:checked").forEach(cb => {
            daysOfWeek += parseInt(cb.value);
        });

        const formData = {
            ReportingTime: document.getElementById("ReportingTime").value,
            LeavingTime: document.getElementById("LeavingTime").value,
            DaysOfWeek: daysOfWeek
        };

        fetch("/Slot/Add", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(formData)
        })
            .then(r => r.json())
            .then(res => {
                if (res.success) {
                    window.hideModal("addSlotModal");
                    loadSlots();
                    toastr.success("Slot created!");
                } else toastr.error(res.message);
            })
            .catch(() => toastr.error("Error creating slot"));
    });


    /* ------------------------------
       EDIT + DELETE (delegation)
    ------------------------------ */
    document.addEventListener("click", function (e) {

        /* --------- EDIT --------- */
        const editBtn = e.target.closest(".edit-btn");
        if (editBtn) {
            e.stopPropagation();  // Prevent AG Grid interference

            const rowData = JSON.parse(decodeURIComponent(editBtn.dataset.row));

            // Populate modal
            document.getElementById("editSlotId").value = rowData.id;
            document.getElementById("editReportingTime").value = rowData.reportingTime;
            document.getElementById("editLeavingTime").value = rowData.leavingTime;

            document.querySelectorAll(".edit-day-checkbox").forEach(cb => {
                cb.checked = (rowData.daysOfWeek & parseInt(cb.value)) !== 0;
            });

            window.showModal("editSlotModal");
            return;
        }


        /* --------- DELETE --------- */
        const delBtn = e.target.closest(".delete-btn");
        if (delBtn) {
            e.stopPropagation();

            const id = parseInt(delBtn.dataset.id);
            console.log(id);

            if (!confirm("Are you sure you want to delete this slot?")) return;

            $.ajax({
                url: "/Slot/Delete",
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify(id),
                success: function (res) {
                    if (res.success) {
                        loadSlots();
                        toastr.success("Slot deleted!");
                    } else {
                        toastr.error(res.message);
                    }
                },
                error: function () {
                    toastr.error("Error deleting slot");
                }
            });

            return;
        }

    });


    /* ------------------------------
       EDIT SLOT (SUBMIT)
    ------------------------------ */
    document.getElementById("editSlotForm").addEventListener("submit", function (e) {
        e.preventDefault();

        let daysOfWeek = 0;
        document.querySelectorAll(".edit-day-checkbox:checked").forEach(cb => {
            daysOfWeek += parseInt(cb.value);
        });

        const formData = {
            Id: parseInt(document.getElementById("editSlotId").value),
            ReportingTime: document.getElementById("editReportingTime").value,
            LeavingTime: document.getElementById("editLeavingTime").value,
            DaysOfWeek: daysOfWeek
        };

        fetch("/Slot/EditSlot", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(formData)
        })
            .then(r => r.json())
            .then(res => {
                if (res.success) {
                    window.hideModal("editSlotModal");
                    loadSlots();
                    toastr.success("Slot updated!");
                } else toastr.error(res.message);
            })
            .catch(() => toastr.error("Update failed"));
    });

});
