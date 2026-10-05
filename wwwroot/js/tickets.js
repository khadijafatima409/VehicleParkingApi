// Park a vehicle, print the ticket, exit, and pay.

let currentTicketId = null;   // the ticket we are working with right now

async function parkVehicle() {
    const vehicleNumber = document.getElementById("vehicle-number").value;
    const vehicleType = document.getElementById("vehicle-type").value;

    const response = await callApi("/api/tickets/entry", "POST", { vehicleNumber: vehicleNumber, vehicleType: vehicleType });

    if (!response.ok) {
        showMessage("park-message", await readError(response), true);
        return;
    }

    const ticket = await response.json();
    currentTicketId = ticket.id;

    document.getElementById("t-number").textContent = "TKT-" + String(ticket.id).padStart(5, "0");
    document.getElementById("t-vehicle").textContent = ticket.vehicleNumber;
    document.getElementById("t-type").textContent = ticket.vehicleType;
    document.getElementById("t-slot").textContent = ticket.parkingSlot ? ticket.parkingSlot.slotNumber : "-";
    document.getElementById("t-entry").textContent = new Date(ticket.entryTime).toLocaleString();

    showMessage("park-message", "", false);
    hide("park-form");
    show("ticket");
    show("ticket-actions");
    show("exit-button");

    loadDashboard();
}

async function exitVehicle() {
    const response = await callApi("/api/tickets/" + currentTicketId + "/exit", "PUT");

    if (!response.ok) {
        showMessage("ticket-message", await readError(response), true);
        return;
    }

    const ticket = await response.json();
    document.getElementById("t-exit").textContent = new Date(ticket.exitTime).toLocaleString();
    document.getElementById("t-fee").textContent = "Rs. " + ticket.fee;

    show("exit-rows");
    hide("exit-button");
    show("payment-box");

    loadDashboard();
}

async function payTicket() {
    const method = document.getElementById("payment-method").value;

    const response = await callApi("/api/tickets/" + currentTicketId + "/payment", "POST", { method: method });

    if (!response.ok) {
        showMessage("ticket-message", await readError(response), true);
        return;
    }

    const payment = await response.json();
    document.getElementById("t-paid").textContent = payment.method;

    show("paid-row");
    hide("payment-box");
    show("new-ticket-button");
    showMessage("ticket-message", "Payment of Rs. " + payment.amount + " received.", false);
}

// Clears the ticket so the next vehicle can be parked
function resetTicket() {
    currentTicketId = null;
    document.getElementById("vehicle-number").value = "";
    document.getElementById("park-message").textContent = "";
    document.getElementById("ticket-message").textContent = "";

    show("park-form");
    hide("ticket");
    hide("ticket-actions");
    hide("exit-rows");
    hide("paid-row");
    hide("payment-box");
    hide("new-ticket-button");
}
