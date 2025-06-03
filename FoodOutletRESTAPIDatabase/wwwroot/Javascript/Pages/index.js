import Auth from '/Javascript/Services/Auth.js';

document.addEventListener("DOMContentLoaded", async function (event) { //runs on page load
    event.preventDefault(); 
    document.getElementById("redirect").textContent = "Redirecting";
    animation();
    await Auth.getUserName(false);
    const objectCheck = sessionStorage.getItem("object");
    if (objectCheck != null) {
        window.location.href = "/Pages/Homepage/homepage.html";
        return;
    }
    window.location.href = "/Pages/Login/Login.html";
});

async function animation() {
    let text = "Redirecting";
    let count = 0;
    const animHTML = document.getElementById("redirect");
    setInterval(function (){
        for (let i = 0; i < count; i++){
            text += ".";
        }
        animHTML.textContent = text;
        if (count === 4) {
            text = "Redirecting";
            count = 0;
            animHTML.textContent = text;
        }
        text = "Redirecting";
        count++;
    }, 1500
    );
}