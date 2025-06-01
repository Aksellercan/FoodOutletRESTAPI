var CurrentUserName;
var CurrentUserId;
var isLoggedin = false;

document.addEventListener("DOMContentLoaded", async function (event) { //runs on page load
    event.preventDefault();
    //show logged in user
    const currUser = await getUserName();
    if (isLoggedin) {
        document.getElementById('headerLogin').textContent = 'Log out';
        document.getElementById('loggedUserP').textContent = `${currUser.username}`;
        document.getElementById('updateUsernameLabelBtn').textContent = `Enter New Username (Current Username: ${currUser.username})`;
    } else {
        document.getElementById('headerLogin').textContent = 'Login';
        document.getElementById('headerLogin').href = '/index.html';
        document.getElementById('loggedUserP').textContent = "Not logged in!";
    }
});

//Display username
async function getUserName() {
    const testResponse = await fetch('/api/login/CurrentUser', {
        method: "GET",
        credentials: "include",
        path: "/jwtTokenTest"
    });
    const currUser = await testResponse.json();
    console.log(`Response Status Code: ${testResponse.status}`);
    if (testResponse.status === 200) {
        isLoggedin = true;
        CurrentUserName = currUser.username;
        CurrentUserId = currUser.id;
        console.log(`currUser raw text: id ${CurrentUserId} and ${CurrentUserName}`);
        return currUser;
    }
    return;
}

function clearStorage() {
    if (!isLoggedin) {window.location.href = './index.html';}
    if (localStorage.getItem('token') == null){return;}
    localStorage.removeItem('token');
    alert("cleared token");
    console.log("cleared token");
    location.reload();
}

async function LogOut() {
    if (isLoggedin) {
        const logoutRequest = await fetch('/api/login/logout', { 
            method: "POST",
            credentials: "include",
            path:"/jwtTokenTest"
        });
        const bodyResponse = await logoutRequest.text();
        console.log(`Status code: ${logoutRequest.status} and body: ${bodyResponse}`)
        window.location.href = "https://localhost:7277/index.html";
    }
    window.location.href = "https://localhost:7277/index.html";
}

async function removeUser() {
    const deleteRequest = await fetch(`/api/user/remove/${CurrentUserId}`, {
        method: "DELETE",
        credentials: "include",
        path:"/jwtTokenTest"
    });
    const logoutRequest = await fetch('/api/login/logout', { 
            method: "POST",
            credentials: "include",
            path:"/jwtTokenTest"
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
        path:"/jwtTokenTest"
    });

    if (updateUsername.status === 200) {
        await refreshToken();
        location.reload();
        console.log("username updated successfully");
    }
}

async function refreshToken() {
    const getRefreshToken = await fetch('/api/login/refreshToken', {
        method: "POST",
        credentials: "include",
        path:"/jwtTokenTest"
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
        path:"/jwtTokenTest"
    });

    if (updatePassword.status === 200) {
        refreshToken();
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