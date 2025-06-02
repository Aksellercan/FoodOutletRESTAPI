import UserDetails from './UserDetails.js';

const CurrentUser = new UserDetails();
var isLoggedin = false;

async function getUserName(reload) {
    const testResponse = await fetch('/api/login/CurrentUser', {
        method: "GET",
        credentials: "include",
        path: "/Identity"
    });
    const currUser = await testResponse.json();
    console.log(`Response Status Code: ${testResponse.status}`);
    if (testResponse.status === 200) {
        isLoggedin = true;
        CurrentUser.setLoggedinStatus(isLoggedin);
        CurrentUser.setUserDetails(currUser.id, currUser.username);
        
        console.log(`currUser raw text: Name ${CurrentUser.getCurrentUsername()} and id ${CurrentUser.getCurrentUserId()}`);
        saveSession(CurrentUser);
        if (reload) {
            location.reload();
        }
        return CurrentUser;
    }
    return;
}

function saveSession(sessionUserData) {
    sessionStorage.setItem("id", sessionUserData.getCurrentUserId());
    sessionStorage.setItem("username", sessionUserData.getCurrentUsername());
    sessionStorage.setItem("status", sessionUserData.getisLoggedin());
}

export default {
    getUserName,
    saveSession
};
