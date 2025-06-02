import outletLayout from '/Javascript/Render/OutletListLayout.js';
import reviewsLayout from '/Javascript/Render/ReviewsLayout.js';

let id;
const mainContent = document.getElementById('content');
var isLoggedin = false;
reviewsLayout.setMainContent(mainContent);

document.addEventListener("DOMContentLoaded", async function (event) { //runs on page load
    event.preventDefault();
    isLoggedin = sessionStorage.getItem("status");
    const outletList = await getOutletList();
    const resultDiv = document.getElementById('outletShowList');
    if (isLoggedin) {
        document.getElementById('loginLink').textContent = 'Log out';
        document.getElementById('showCurrUser').textContent = `${sessionStorage.getItem("username")}`;
    } else {
        document.getElementById('loginLink').textContent = 'Login';
    }

    //continue
    if (outletList.length > 0) {
        resultDiv.innerHTML = "";
        for (let i = 0; i < outletList.length; i++) {
            const outlet = outletList[i];
            const outletLine = outletLayout.outletListLayout(outlet, resultDiv);

            outletLine.addEventListener('click', async function (event) {
                event.preventDefault();
                if (outlet.id == id) { return; } //avoid loading same data
                loadReviews(outlet.id);
                mainContent.innerHTML = "";
                document.getElementById('titleHeader').textContent = `Reviews for ${outlet.name}`;
                id = outlet.id;
            });
        }
    } else {
        resultDiv.innerHTML = "<h2 class=\"listH2\">No outlets found</h2>";
    }
});

async function loadReviews(outletId) {
    const reviews = await getReviews(outletId);
                //<main> id="content" section
                reviewsLayout.reviewLayout(reviews);
                reviewsLayout.reviewFormLayout(outletId);
                id = outletId;
}

//Fetch outlet list
async function getOutletList() {
    const response = await fetch('/api/foodoutlet');
    const list = await response.json();
    return list;
}

//Fetch reviews
async function getReviews(outletId) {
    const response = await fetch(`/api/reviews/${outletId}`);
    const reviews = await response.json();
    return reviews;
}

//Post Review
async function postReviewBody(comment, score, outletId) {
    try {
        if (score < 1 || score > 5) {
            throw new Error(`Score must be between 1 and 5`);
        }
        const response = await fetch(`/api/reviews/${outletId}/reviews`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                Authorization: 'Bearer ' + localStorage.getItem('token')
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