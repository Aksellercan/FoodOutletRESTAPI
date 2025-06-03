var CurrentUserName;
var CurrentUserId;
var isLoggedin;

function getInfo() {
    const parsedObject = JSON.parse(sessionStorage.getItem("object"));
    if(parsedObject === null) {
        isLoggedin = false;
    } else {
        CurrentUserName = parsedObject.CurrentUsername;
        CurrentUserId = parsedObject.CurrentUserId;
        isLoggedin = parsedObject.isLoggedin;
    }
}

function headerLayout() {
    getInfo();
    if (isLoggedin) {
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
        settingsPage.textContent = 'Settings'
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
    basicHeader,
    profileHeader,
    getCurrentUserId,
    getCurrentUsername,
    getisLoggedin
};