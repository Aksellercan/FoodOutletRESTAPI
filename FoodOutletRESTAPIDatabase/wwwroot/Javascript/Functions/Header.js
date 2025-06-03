var CurrentUserName;
var CurrentUserId;
var isLoggedin;

function headerLayout() {
    const parsedObject = JSON.parse(localStorage.getItem("object"));
    if(parsedObject === null) {
        isLoggedin = false;
    } else {
        CurrentUserName = parsedObject.CurrentUsername;
        CurrentUserId = parsedObject.CurrentUserId;
        isLoggedin = parsedObject.isLoggedin;
    }
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
    getCurrentUserId,
    getCurrentUsername,
    getisLoggedin
};