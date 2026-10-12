// Shared values and helper functions used by all the other JS files.
// This file must load first.

let token = null;      // the JWT, kept only in memory (refresh = log in again)
let userRole = null;
let userName = null;

// .NET puts the role and name in the token under these long claim names
const ROLE_CLAIM = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
const NAME_CLAIM = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";

function show(id) {
    document.getElementById(id).classList.remove("hidden");
}

function hide(id) {
    document.getElementById(id).classList.add("hidden");
}

function showMessage(id, text, isError) {
    const element = document.getElementById(id);
    element.textContent = text;
    element.className = isError ? "message error" : "message success";
}

// Every call to the backend goes through this one function,
// so the token is always attached in the same way.
async function callApi(url, method, body) {
    const options = { method: method, headers: {} };

    if (body) {
        options.headers["Content-Type"] = "application/json";
        options.body = JSON.stringify(body);
    }

    if (token) {
        options.headers["Authorization"] = "Bearer " + token;
    }

    const response = await fetch(url, options);

    // We had a token but the server said 401: it expired, so go back to login
    if (response.status === 401 && token) {
        logout();
        showMessage("login-message", "Your session expired. Please log in again.", true);
    }

    return response;
}

// Turns an error response into readable text.
// Works for plain text errors and for validation errors (JSON).
async function readError(response) {
    const text = await response.text();

    if (!text) {
        return "Request failed (" + response.status + ")";
    }

    try {
        const data = JSON.parse(text);
        if (data.errors) {
            return Object.values(data.errors).flat().join(" ");
        }
        return data.message || data.title || text;
    } catch {
        return text;
    }
}
