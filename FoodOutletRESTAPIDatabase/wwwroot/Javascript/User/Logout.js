async function LogOut() {
    console.log ("Logout");
    const parsedObject = JSON.parse(localStorage.getItem("object"));
    var isLoggedin = parsedObject.isLoggedin;
    if (isLoggedin) {
        const logoutRequest = await fetch('/api/login/logout', { 
            method: "POST",
            credentials: "include",
            path:"/Identity"
        });
        const bodyResponse = await logoutRequest.text();
        console.log(`Status code: ${logoutRequest.status} and body: ${bodyResponse}`)
        sessionStorage.clear();
        localStorage.clear();
    }
    window.location.href = "https://localhost:7277/index.html";
}

window.LogOut = LogOut;
export default {
    LogOut
}