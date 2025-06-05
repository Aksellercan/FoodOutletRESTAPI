import ApiRequest from "/Javascript/Services/ApiRequest.js";

async function loop(isLoggedin){
    const listContent = document.getElementById('profileMainContent');
    const breakLine = document.createElement('br');
    listContent.innerHTML = '';
    listContent.appendChild(breakLine);
    let reviewsArray;
    if (!isLoggedin) {
        return;
    } else {
        reviewsArray = await ApiRequest.getReviews();
    }

    if (reviewsArray.length === 0) {
        console.log('no reviews');
        const noReviews = document.createElement('p');
        noReviews.className = "commentList";
        noReviews.textContent = 'No reviews';
        listContent.appendChild(noReviews);
        return;
    }
    let count = 0;
    console.log(`size ${reviewsArray.length}`);
    for (let i = 0; i < reviewsArray.length; i++) {
        const commentView = document.createElement('p');
        commentView.className = "commentList";
        commentView.textContent = `Comment: ${reviewsArray[i].comment}`;
        const scoreView = document.createElement('p');
        scoreView.className = "commentList";
        scoreView.textContent = `Score given: ${reviewsArray[i].score}`;
        const breakLine = document.createElement('br');
        count++;
        listContent.appendChild(scoreView);
        listContent.appendChild(commentView);
        listContent.appendChild(breakLine);
    }
    const hrLine = document.createElement('hr');
    listContent.appendChild(hrLine);
    const countReviews = document.createElement('p');
    countReviews.className = "commentList";
    countReviews.innerHTML = `${count} ${(count === 1) ?  ` Review` :  ` Reviews`}`;
    listContent.appendChild(countReviews);
}

export default {
    loop
}