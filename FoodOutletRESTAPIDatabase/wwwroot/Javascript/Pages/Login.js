import Login from "/Javascript/User/Login.js";
import Header from "/Javascript/Functions/Header.js";

Header.loginHeader();

document.getElementById('loginForm').addEventListener('submit', async function (event) {
    event.preventDefault();
    const username = document.getElementById('loginUsername').value;
    const password = document.getElementById('loginPassword').value;
    await Login.Login(username, password);
});

function registerPage() {
    window.location.href = "/Pages/Register/Register.html";
}
window.registerPage = registerPage;
