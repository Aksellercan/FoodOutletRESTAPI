const path = "/Identity";

async function getReviews() {
    const response = await fetch(`/api/reviews`, {
        method: "GET",
        credentials: "include",
        path: path
    });
    const reviews = await response.json();
    return reviews;
}

async function removeUser(CurrentUserId) {
    const deleteRequest = await fetch(`/api/user/remove/${CurrentUserId}`, {
        method: "DELETE",
        credentials: "include",
        path:path
    });
    const logoutRequest = await fetch('/api/login/logout', { 
            method: "POST",
            credentials: "include",
            path:path
        });
    alert("Account Removed!");
    window.location.href = "https://localhost:7277/index.html"; //send to the frontpage (placeholder)
}

async function updatePassword(newPassword) {
    const updatePassword = await fetch(`/api/user/passch/${CurrentUserId}?newPassword=${newPassword}`,{
        method: "PUT",
        credentials: "include",
        path:path
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

async function refreshToken() {
    const getRefreshToken = await fetch('/api/login/refreshToken', {
        method: "POST",
        credentials: "include",
        path:path
    });

    const refreshResponse = await getRefreshToken.text();
    if (getRefreshToken.status === 200) {
        console.log(`Status code: ${getRefreshToken.status} and body: ${refreshResponse}`)
        return;
    }
    console.log(`Error refreshing token. Status code: ${getRefreshToken.status} and body: ${refreshResponse}`);
}

export default {
    getReviews,
    removeUser,
    updatePassword
};