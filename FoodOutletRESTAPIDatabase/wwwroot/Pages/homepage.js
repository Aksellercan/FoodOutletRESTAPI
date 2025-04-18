document.addEventListener("DOMContentLoaded", async function (event) { //runs on page load
    event.preventDefault();
    let id;
    const outletList = await getOutletList();
    const resultDiv = document.getElementById('outletShowList');

    //show logged in user (for testing)
    const loggedinUser = document.getElementById('asidePanelP2');
    const testResponse = await fetch('/api/login/testPoint', {
        method: "GET",
        headers: {
            "Authorization": `Bearer ${localStorage.getItem('token')}`
        }
    });
    const currUser = await testResponse.text();
    console.log("currUser raw text:", currUser);
    loggedinUser.innerHTML = `<p id='userNameNav'>Current User: ${currUser}</p>`;

    //continue
    if (outletList.length > 0) {
        resultDiv.innerHTML = "";
        for (let i = 0; i < outletList.length; i++) {
            const outlet = outletList[i];
            const outletLine = outletListLayout(outlet, resultDiv);

            outletLine.addEventListener('click', async function (event) {
                event.preventDefault();
                if (outlet.id == id) { return; } //avoid loading same data
                const reviews = await getReviews(outlet.id);
                //<main> id="content" section
                const mainContent = document.getElementById('content');
                mainContent.innerHTML = `<h2 id="contentH2">Reviews for ${outlet.name}</h2>`;
                reviewLayout(reviews, mainContent);
                reviewFormLayout(mainContent, outlet.id);
                id = outlet.id;
            });
        }
    } else {
        resultDiv.innerHTML = "<h2 class=\"listH2\">No outlets found</h2>";
    }
});

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
async function getUserName(userId) {
    const response = await fetch(`/api/login/${userId}`);
    const userName = await response.text();
    return userName;
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
async function reviewLayout(reviews, mainContent) {
    if (reviews.length > 0) {
        for (let i = 0; i < reviews.length; i++) {
            const review = reviews[i];
            const userName = await getUserName(review.userId);
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
async function postReviewBody(comment, score, outletId){
    if (score < 1 || score > 5) alert('Score must be between 1 and 5');
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
    const result = await response.text();
    if (response.ok) {
        alert('Submission successful ' + result);
    } else {
        alert('Submission failed. ' + result);
        console.log(result);
    }
}

//Review form
function reviewFormLayout(mainContent, outletid){
    const postRating = document.createElement('input');
    postRating.id = "inputboxRating";
    postRating.type = "number";
    postRating.required;

    const postReview = document.createElement('input');
    postReview.id = "inputboxComment";
    postReview.type = "text";

    const postButton = document.createElement('button');
    postButton.type = "click";
    postButton.textContent = "Comment";

    postButton.addEventListener('click', function(event){
        event.preventDefault();
        const score = postRating.value;
        const comment = postReview.value;
        console.log("DOES THIS DO ANYTHING");
        postReviewBody(comment, score, outletid);
    });

    const breakLine = document.createElement('br');
    mainContent.appendChild(breakLine);
    mainContent.appendChild(postRating);
    mainContent.appendChild(postReview);
    mainContent.appendChild(postButton);
}

