var CurrentUserName;
var CurrentUserId;
var CurrentUserRole;
var isLoggedin;

function getInfo() {
    const parsedObject = JSON.parse(sessionStorage.getItem("object"));
    if(parsedObject === null) {
        isLoggedin = false;
    } else {
        CurrentUserName = parsedObject.CurrentUsername;
        CurrentUserId = parsedObject.CurrentUserId;
        CurrentUserRole = parsedObject.CurrentUserRole
        isLoggedin = parsedObject.isLoggedin;
    }
}

function headerLayout() {
    getInfo();
    if (isLoggedin) {
        if (isAdmin()) {
            const ifAdmin = document.getElementById('headerProfile');
            ifAdmin.textContent = ''
        }
        document.getElementById('headerLogin').textContent = 'Log out';
        document.getElementById('loggedUserP').textContent = `${CurrentUserName}`;
        document.getElementById('updateUsernameLabelBtn').textContent = `Enter New Username (Current Username: ${CurrentUserName})`;
    } else {
        document.getElementById('headerLogin').textContent = 'Login';
        document.getElementById('headerLogin').href = '/index.html';
        document.getElementById('loggedUserP').textContent = "Not logged in!";
    }
}

function basicHeader() {
    getInfo();
    if (isLoggedin) {
        if (isAdmin()) {
            const adminElem = document.getElementById('linkProfile');
            adminElem.textContent = 'Control Panel |';
            adminElem.href = '/Pages/Admin/admin.html';
        } else {
            document.getElementById('linkProfile').textContent = 'Profile |';
        }
        document.getElementById('loginLink').textContent = 'Log out';
        document.getElementById('showCurrUser').textContent = `${CurrentUserName}`;
    } else {
        document.getElementById('loginLink').textContent = 'Login';
    }
}

function profileHeader() {
    getInfo();
    if (isLoggedin) {
        const settingsPage = document.getElementById('headerSettings');
        settingsPage.textContent = 'Settings |'
        settingsPage.href = '/Pages/UserSettings/userSettings.html';
        document.getElementById('headerLogin').textContent = 'Log out';
        document.getElementById('loggedUserP').textContent = `${CurrentUserName}`;
        document.getElementById('profileMainHeader').textContent = `${CurrentUserName}'s Reviews:`;
    } else {
        document.getElementById('headerLogin').textContent = 'Login';
        document.getElementById('loggedUserP').textContent = "Not logged in!";
        document.getElementById('profileMainHeader').textContent = `Login to View.`;
    }
}

function adminHeader() {
    getInfo();
    if (isAdmin()) {
        const settingsPage = document.getElementById('headerSettings');
        settingsPage.textContent = 'Settings |'
        settingsPage.href = '/Pages/UserSettings/userSettings.html';
        document.getElementById('loginLink').textContent = 'Log out';
        document.getElementById('showCurrUser').textContent = `${CurrentUserName}`;
    } else {
        window.location = "/index.html";
    }
}

function loginHeader() {
    getInfo();
    if (isLoggedin) {
    document.getElementById('loggedUserP').textContent = `Currently Logged in as ${CurrentUserName}`;
    }
}

function isAdmin() {
    if (isLoggedin && CurrentUserRole === "Admin") {
        return true;
    }
    return false;
}

function getisLoggedin() {
    return isLoggedin;
}

function getCurrentUsername() {
    return CurrentUserName;
}

function getCurrentUserId() {
    return CurrentUserId;
}

export default {
    headerLayout,
    adminHeader,
    loginHeader,
    basicHeader,
    profileHeader,
    getCurrentUserId,
    getCurrentUsername,
    getisLoggedin
};