// Slot tiles, the summary numbers and the admin buttons.

function loadDashboard() {
    loadSlots();
    loadSummary();
}

async function loadSummary() {
    const response = await callApi("/api/parkingslots/summary", "GET");
    if (!response.ok) {
        return;
    }

    const data = await response.json();
    document.getElementById("total-slots").textContent = data.totalSlots;
    document.getElementById("free-slots").textContent = data.availableSlots;
}

async function loadSlots() {
    // onlyFree=false so the page shows occupied slots too
    const response = await callApi("/api/parkingslots/availability?onlyFree=false", "GET");
    if (!response.ok) {
        return;
    }

    const slots = await response.json();
    const list = document.getElementById("slot-list");
    list.innerHTML = "";

    for (const slot of slots) {
        const box = document.createElement("div");
        box.className = slot.isOccupied ? "slot taken" : "slot free";

        let html = "<strong>" + slot.slotNumber + "</strong>" +
                   "<span>" + slot.vehicleType + "</span>" +
                   "<em>" + (slot.isOccupied ? "Occupied" : "Free") + "</em>";

        // Only for convenience. The server still checks the Admin role.
        if (userRole === "Admin") {
            if (slot.isOccupied) {
                html += "<button onclick=\"changeSlot(" + slot.id + ", 'release')\">Mark free</button>";
            } else {
                html += "<button onclick=\"changeSlot(" + slot.id + ", 'occupy')\">Mark occupied</button>";
            }
        }

        box.innerHTML = html;
        list.appendChild(box);
    }
}

async function changeSlot(id, action) {
    const response = await callApi("/api/parkingslots/" + id + "/" + action, "PUT");

    if (!response.ok) {
        alert(await readError(response));
    }

    loadDashboard();
}
