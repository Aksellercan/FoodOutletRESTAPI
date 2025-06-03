async function LogOut() {
    console.log ("Logout");
    try {
        const logoutRequest = await fetch('/api/login/logout', { 
            method: "POST",
            credentials: "include",
            path:"/Identity"
        });
        const bodyResponse = await logoutRequest.text();
        console.log(`Status code: ${logoutRequest.status} and body: ${bodyResponse}`)
        sessionStorage.clear();
        localStorage.clear();
        window.location.href = "/Pages/Login/Login.html";
    } catch(err) {
        console.log("Log out error! " + err);
        window.location.href = "/Pages/Login/Login.html";
    }
}


window.LogOut = LogOut;
export default {
    LogOut
}