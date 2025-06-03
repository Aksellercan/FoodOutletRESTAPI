import Auth from '/Javascript/Services/Auth.js';
import Logout from '/Javascript/User/Logout.js';
import ApiRequest from '/Javascript/Services/ApiRequest.js';
import Header from '/Javascript/Functions/Header.js';

var CurrentUserName;
var CurrentUserId;

document.addEventListener("DOMContentLoaded", async function (event) { //runs on page load
    event.preventDefault();
    Header.headerLayout();
    if (Header.getisLoggedin() === false) return;
    CurrentUserName = Header.getCurrentUsername();
    CurrentUserId = Header.getCurrentUserId();
});

window.Logout = Logout.LogOut;

document.getElementById('deleteAccountForm').addEventListener('submit', async function (event) {
    event.preventDefault();
    const enteredName = document.getElementById('deleteAccount').value;
    if (enteredName === CurrentUserName) {
        ApiRequest.removeUser(CurrentUserId);
    }
});

async function updateUsername(newUsername) {
    const updateUsername = await fetch(`/api/user/namech/${CurrentUserId}?newUsername=${newUsername}`,{
        method: "PUT",
        credentials: "include",
        path:"/Identity"
    });

    if (updateUsername.status === 200) {
        await refreshToken();
        Auth.getUserName(true);
        console.log("username updated successfully");
    }
}

async function refreshToken() {
    const getRefreshToken = await fetch('/api/login/refreshToken', {
        method: "POST",
        credentials: "include",
        path:"/Identity"
    });

    const refreshResponse = await getRefreshToken.text();
    if (getRefreshToken.status === 200) {
        console.log(`Status code: ${getRefreshToken.status} and body: ${refreshResponse}`)
        return;
    }
    console.log('Error refreshing token');
}

document.getElementById('newUsernameForm').addEventListener('submit', async function (event) {
    event.preventDefault();
    const newUsername = document.getElementById('setNewUsername').value;
    updateUsername(newUsername)
});

async function updatePassword(newPassword) {
    const updatePassword = await fetch(`/api/user/passch/${CurrentUserId}?newPassword=${newPassword}`,{
        method: "PUT",
        credentials: "include",
        path:"/Identity"
    });

    if (updatePassword.status === 200) {
        refreshToken();
        Auth.getUserName(true);
        alert("Password Updated");
    } else if (updatePassword.status === 400) {
        alert("Password is same as old one.");
    } else {
        alert("Unknown Error");
    }
}

document.getElementById('newPasswordForm').addEventListener('submit', async function (event) {
    event.preventDefault();
    const newPassword = document.getElementById('setNewPassword').value;
    await updatePassword(newPassword);
});