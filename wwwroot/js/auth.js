// Login, register and logout.

// The middle part of a JWT is the payload. It is only Base64 encoded,
// so the browser can read the user's name and role from it.
function readToken(jwt) {
    const payload = jwt.split(".")[1];
    const json = atob(payload.replace(/-/g, "+").replace(/_/g, "/"));
    return JSON.parse(json);
}

async function login() {
    const username = document.getElementById("login-username").value;
    const password = document.getElementById("login-password").value;

    const response = await callApi("/api/auth/login", "POST", { username: username, password: password });

    if (!response.ok) {
        showMessage("login-message", await readError(response), true);
        return;
    }

    const data = await response.json();
    token = data.token;

    const payload = readToken(token);
    userRole = payload[ROLE_CLAIM];
    userName = payload[NAME_CLAIM];

    document.getElementById("user-info").textContent = userName + " (" + userRole + ")";
    document.getElementById("login-password").value = "";
    showMessage("login-message", "", false);

    hide("login-page");
    show("dashboard");
    show("user-bar");

    loadDashboard();
}

async function register() {
    const username = document.getElementById("register-username").value;
    const password = document.getElementById("register-password").value;

    const response = await callApi("/api/auth/register", "POST", { username: username, password: password });

    if (!response.ok) {
        showMessage("register-message", await readError(response), true);
        return;
    }

    showMessage("register-message", "Account created. You can log in now.", false);
}

function logout() {
    token = null;
    userRole = null;
    userName = null;

    hide("dashboard");
    hide("user-bar");
    show("login-page");

    resetTicket();
}
