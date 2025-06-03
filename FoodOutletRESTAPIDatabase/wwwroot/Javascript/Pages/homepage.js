let id;
const mainContent = document.getElementById('content');
var isLoggedin = false;

document.addEventListener("DOMContentLoaded", async function (event) { //runs on page load
    event.preventDefault();
    const outletList = await getOutletList();
    const resultDiv = document.getElementById('outletShowList');

    //show logged in user (for testing)
    const testResponse = await fetch('/api/login/CurrentUser', {
        method: "GET",
        headers: {
            "Authorization": `Bearer ${localStorage.getItem('token')}`
        }
    });
    const currUser = await testResponse.json();
    if (testResponse.status === 200) {
        isLoggedin = true;
    }
    console.log("currUser raw text:", currUser.username);
    if (isLoggedin) {
        document.getElementById('loginLink').textContent = 'Log out';
        document.getElementById('showCurrUser').textContent = `${currUser.username}`;
    } else {
        document.getElementById('loginLink').textContent = 'Login';
    }

    //continue
    if (outletList.length > 0) {
        resultDiv.innerHTML = "";
        for (let i = 0; i < outletList.length; i++) {
            const outlet = outletList[i];
            const outletLine = outletListLayout(outlet, resultDiv);

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
                reviewLayout(reviews);
                reviewFormLayout(outletId);
                id = outletId;
}

//Display outlet list
function outletListLayout(outlet, resultDiv) {
    const outletLine = document.createElement('a');
    outletLine.href = "#";
    outletLine.className = "myButton";
    outletLine.textContent = ` ${outlet.name} - ${outlet.location} (${outlet.rating} from ${outlet.reviewCount} reviews) `;
    resultDiv.appendChild(outletLine);
    return outletLine
}

//Display username
async function getUserName(response) {
    const userName = await response.text();
    console.log(`Username ${userName} and status code ${response.status}`);
    return userName;
}

function doesUserExist(response) {
    if (response.status === 404) {
        return false;
    } else {
        return true
    }
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

//Display reviews
async function reviewLayout(reviews) {
    if (reviews.length > 0) {
        for (let i = 0; i < reviews.length; i++) {
            const review = reviews[i];
            const response = await fetch(`/api/login/${review.userId}`);
            if (!doesUserExist(response)) {
                continue;
            }
            const userName = await getUserName(response);
            const breakLine = document.createElement('hr');
            const ratingLine = document.createElement('p');
            ratingLine.id = "reviewP";
            ratingLine.textContent = `Rating: ${review.score}`;
            const reviewElem = document.createElement('p');
            reviewElem.id = "reviewP";

            const comment = `${userName}: ${review.comment}`;
            reviewElem.textContent = comment;
            const createdAt = document.createElement('p');
            createdAt.textContent = `${review.createdAt}`;
            createdAt.id = "reviewP";
            //append elements
            mainContent.appendChild(breakLine);
            mainContent.appendChild(ratingLine);
            mainContent.appendChild(reviewElem);
            mainContent.appendChild(createdAt);
        }
    } else {
        const breakLine = document.createElement('br');
        const reviewElem = document.createElement('p');
        reviewElem.id = "reviewP";
        reviewElem.textContent = 'No Reviews';

        mainContent.appendChild(breakLine);
        mainContent.appendChild(reviewElem);
    }
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

//Review form
function reviewFormLayout(outletid){
    const postRating = document.createElement('input');
    postRating.id = "inputboxRating";
    postRating.type = "number";
    postRating.style.height = "25px";
    postRating.style.fontSize = "14pt";
    postRating.max = 5;
    postRating.min = 1;
    postRating.required;

    const postReview = document.createElement('input');
    postReview.id = "inputboxComment";
    postReview.type = "text";
    postReview.style.height = "25px";
    postReview.style.fontSize = "14pt";

    // const postButton = document.createElement('button');
    // postButton.type = "click";
    // postButton.textContent = "Comment";

    const postButton = document.createElement('a');
    postButton.href = "#";
    postButton.className = "myButton";
    postButton.textContent = "Submit";

    postButton.addEventListener('click', function(event){
        event.preventDefault();
        const score = postRating.value;
        const comment = postReview.value;
        postReviewBody(comment, score, outletid);
    });

    const breakLine = document.createElement('br');
    mainContent.appendChild(breakLine);
    mainContent.appendChild(postRating);
    mainContent.appendChild(postReview);
    mainContent.appendChild(postButton);
}

//logout (for testing)
function clearStorage() {
    if (!isLoggedin) {window.location.href = './index.html';}
    if (localStorage.getItem('token') == null){return;}
    localStorage.removeItem('token');
    alert("cleared token");
    console.log("cleared token");
    location.reload();
}