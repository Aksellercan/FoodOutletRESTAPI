import reviewsLayout from '/Javascript/Render/ReviewsLayout.js';
import Logout from '/Javascript/User/Logout.js';

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

async function loadReviews(outletId) {
    const reviews = await getOutletReviews(outletId);
    //<main> id="content" section
    reviewsLayout.reviewLayout(reviews);
    reviewsLayout.reviewFormLayout(outletId);
    return outletId;
}

//Fetch reviews
async function getOutletReviews(outletId) {
    const response = await fetch(`/api/reviews/${outletId}`);
    const reviews = await response.json();
    return reviews;
}

//Fetch outlet list
async function getOutletList() {
    const response = await fetch('/api/foodoutlet');
    const list = await response.json();
    return list;
}

//Post Review
async function postReviewBody(comment, score, outletId, mainContent) {
    try {
        if (score < 1 || score > 5) {
            throw new Error(`Score must be between 1 and 5`);
        }
        const response = await fetch(`/api/reviews/${outletId}/reviews`, {
            method: 'POST',
            credentials:"include",
            path:path,
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                comment: comment,
                score: score
            })
        });
        const result = await response.json();
        const error = await result.error;
        if (response.ok) {
            alert('Submission successful ');
            mainContent.innerHTML = "";
            await loadReviews(outletId);
        } else {
            throw new Error(`Submission failed. ${error}`);
        }
    } catch (e) {
        alert(e);
        console.log(e);
    }
}

async function removeUser(CurrentUserId) {
    const deleteRequest = await fetch(`/api/user/remove/${CurrentUserId}`, {
        method: "DELETE",
        credentials: "include",
        path:path
    });
    alert("Account Removed!");
    Logout.LogOut();
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

async function updateOutletDetails(newName, newLocation, outletid) {
    console.log(`${newName} and ${newLocation}`);
    const updateDetails = await fetch(`/api/foodoutlet/${outletid}`, {
        method: "PUT",
        credentials: "include",
        path:path,
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
                name: newName,
                location: newLocation
            })
    });
    if (updateDetails.status === 204) location.reload();
}

async function removeOutlet(id) {
    const removeRequest = await fetch (`/api/foodoutlet/${id}`, {
        method: "DELETE",
        credentials: "include",
        path:path
    });

    if (removeRequest.status === 204) location.reload();
}

async function addOutlet(name, setlocation) {
    const addRequest = await fetch(`/api/foodoutlet`, {
        method: "POST",
        credentials: "include",
        path:path, 
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            name: name,
            location: setlocation
        })
    });
    const response = await addRequest.json();
    if (addRequest.status === 201) location.reload();
}

export default {
    getReviews,
    removeUser,
    addOutlet,
    removeOutlet,
    updateOutletDetails,
    loadReviews,
    getOutletReviews,
    postReviewBody,
    updatePassword,
    getOutletList
};