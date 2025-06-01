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
        document.getElementById('loggedUserP').textContent = "Not logged in!";
    }
});

//Display username
async function getUserName() {
    const testResponse = await fetch('/api/login/CurrentUser', {
        method: "GET",
        headers: {
            "Authorization": `Bearer ${localStorage.getItem('token')}`
        }
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

async function removeUser() {
    const deleteRequest = await fetch(`/api/user/remove/${CurrentUserId}`, {
        method: "DELETE",
        headers: {
            "Authorization": `Bearer ${localStorage.getItem('token')}`
        }
    });
    localStorage.removeItem('token');
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
        headers: {
            "Authorization": `Bearer ${localStorage.getItem('token')}`,
        }
    });

    if (updateUsername.status === 200) {
        await refreshToken();
        console.log("username updated successfully");
        location.reload();
    }
}

async function refreshToken() {
    const getRefreshToken = await fetch('/api/login/refreshToken', {
        method: "GET",
        headers: {
            "Authorization": `Bearer ${localStorage.getItem('token')}`
        }
    });
    const refreshResponse = await getRefreshToken.json();
    if (getRefreshToken.status === 200) {
        localStorage.removeItem('token');
        localStorage.setItem('token', refreshResponse.token);
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
        headers: {
            "Authorization": `Bearer ${localStorage.getItem('token')}`,
        }
    });

    if (updatePassword.status === 200) {
        localStorage.removeItem('token');
        alert("Login with new password");
        window.location.href = "/index.html"; //send to the frontpage (placeholder)
    } else {
        alert("Password is same as old one.");
    }
}

document.getElementById('newPasswordForm').addEventListener('submit', async function (event) {
    event.preventDefault();
    const newPassword = document.getElementById('setNewPassword').value;
    await updatePassword(newPassword);
});