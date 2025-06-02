import Auth from '/Services/Auth.js';

var CurrentUserName;
var CurrentUserId;
var isLoggedin;

document.addEventListener("DOMContentLoaded", async function (event) { //runs on page load
    event.preventDefault();
    //show logged in user
    // const currUser = await getUserName();
    CurrentUserName = sessionStorage.getItem("username");
    CurrentUserId = sessionStorage.getItem("id");
    isLoggedin = sessionStorage.getItem("status");

    if (isLoggedin) {
        document.getElementById('headerLogin').textContent = 'Log out';
        document.getElementById('loggedUserP').textContent = `${CurrentUserName}`;
        document.getElementById('updateUsernameLabelBtn').textContent = `Enter New Username (Current Username: ${CurrentUserName})`;
    } else {
        document.getElementById('headerLogin').textContent = 'Login';
        document.getElementById('headerLogin').href = '/index.html';
        document.getElementById('loggedUserP').textContent = "Not logged in!";
    }
});

async function LogOut() {
    if (isLoggedin) {
        const logoutRequest = await fetch('/api/login/logout', { 
            method: "POST",
            credentials: "include",
            path:"/Identity"
        });
        const bodyResponse = await logoutRequest.text();
        console.log(`Status code: ${logoutRequest.status} and body: ${bodyResponse}`)
        sessionStorage.clear();
        window.location.href = "https://localhost:7277/index.html";
    }
    window.location.href = "https://localhost:7277/index.html";
}
window.LogOut = LogOut;

async function removeUser() {
    const deleteRequest = await fetch(`/api/user/remove/${CurrentUserId}`, {
        method: "DELETE",
        credentials: "include",
        path:"/Identity"
    });
    const logoutRequest = await fetch('/api/login/logout', { 
            method: "POST",
            credentials: "include",
            path:"/Identity"
        });
    alert("Account Removed!");
    window.location.href = "https://localhost:7277/index.html"; //send to the frontpage (placeholder)
}


document.getElementById('deleteAccountForm').addEventListener('submit', async function (event) {
    event.preventDefault();
    const enteredName = document.getElementById('deleteAccount').value;
    if (enteredName === CurrentUserName) {
        removeUser();
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
        //location.reload();
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