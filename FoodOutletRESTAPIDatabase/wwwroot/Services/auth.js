import UserDetails from './UserDetails.js';

const CurrentUser = new UserDetails();
const isLoggedin = false;

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
        CurrentUser.setLoggedinStatus(isLoggedin);
        CurrentUser.setUserDetails(currUser.id, currUser.username);
        
        console.log(`currUser raw text: id ${CurrentUser.getCurrentUsername()} and ${CurrentUser.getCurrentUserId}`);
        saveSession(CurrentUser);
        return CurrentUser;
    }
    return;
}

function saveSession(sessionUserData) {
    sessionStorage.setItem("id", sessionUserData.getCurrentUserId());
    sessionStorage.setItem("username", sessionUserData.getCurrentUsername());
    sessionStorage.setItem("status", sessionUserData.getisLoggedin());
}