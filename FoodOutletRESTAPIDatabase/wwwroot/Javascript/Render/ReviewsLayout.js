import ApiRequest from "/Javascript/Services/ApiRequest.js";

var mainContent = "";

function setMainContent(setmainContent) {
    mainContent = setmainContent;
}

//Display reviews
async function reviewLayout(reviews) {
    if (reviews.length > 0) {
        for (let i = 0; i < reviews.length; i++) {
            const review = reviews[i];
            const response = await fetch(`/api/user/${review.userId}`);
            const userName = await response.text();
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
        ApiRequest.postReviewBody(comment, score, outletid, mainContent);
    });

    const breakLine = document.createElement('br');
    mainContent.appendChild(breakLine);
    mainContent.appendChild(postRating);
    mainContent.appendChild(postReview);
    mainContent.appendChild(postButton);
}

export default {
    reviewLayout,
    reviewFormLayout,
    setMainContent
};