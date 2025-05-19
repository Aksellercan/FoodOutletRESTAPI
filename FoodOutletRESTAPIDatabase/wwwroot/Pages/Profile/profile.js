var CurrentUserName;
var isLoggedin = false;

document.addEventListener("DOMContentLoaded", async function (event) { //runs on page load
    event.preventDefault();

    //show logged in user
    const currUser = await getUserName();
    if (isLoggedin) {
        document.getElementById('headerLogin').textContent = 'Log out';
        document.getElementById('loggedUserP').textContent = `${currUser}`;
        document.getElementById('profileMainHeader').textContent = `${currUser}'s Reviews:`;
        loop();
    } else {
        document.getElementById('headerLogin').textContent = 'Login';
        document.getElementById('loggedUserP').textContent = "Not logged in!";
        document.getElementById('profileMainHeader').textContent = `Login to View.`;
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
    const currUser = await testResponse.text();
    console.log("currUser raw text:" + currUser);
    console.log(`Response Status Code: ${testResponse.status}`);
    if (testResponse.status === 200) {
        isLoggedin = true;
        return currUser;
    }
    return;
}

async function getReviews() {
    const response = await fetch(`/api/reviews`, {
        method: "GET",
        headers: {
            "Authorization": `Bearer ${localStorage.getItem('token')}`
        }
    });
    const reviews = await response.json();
    return reviews;
}

async function loop(){
    const listContent = document.getElementById('profileMainContent');
    const breakLine = document.createElement('br');
    listContent.innerHTML = '';
    listContent.appendChild(breakLine);
    let reviewsArray;
    if (isLoggedin) {
        reviewsArray = await getReviews();
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
        commentView.textContent = `> ${reviewsArray[i].comment}`;
        const scoreView = document.createElement('p');
        scoreView.className = "commentList";
        scoreView.textContent = `Score given: ${reviewsArray[i].score}`;
        const breakLine = document.createElement('hr');
        count++;
        listContent.appendChild(scoreView);
        listContent.appendChild(commentView);
        listContent.appendChild(breakLine);
    }
    const countReviews = document.createElement('p');
    countReviews.className = "commentList";
    countReviews.innerHTML = `${count} ${(count === 1) ?  ` Review` :  ` Reviews`}`;
    listContent.appendChild(countReviews);
}

function clearStorage() {
    if (localStorage.getItem('token') == null){return;}
    localStorage.removeItem('token');
    alert("cleared token");
    console.log("cleared token");
    location.reload();
}