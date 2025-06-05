import Register from "/Javascript/User/Register.js";
import Header from "/Javascript/Functions/Header.js";

Header.loginHeader();

document.getElementById('registerForm').addEventListener('submit', async function (event) {
    event.preventDefault();
    const username = document.getElementById('registerUsername').value;
    const password = document.getElementById('registerPassword').value;
    await Register.Register(username, password);
});

function loginPage() {
    window.location.href = "/Pages/Login/Login.html";
}
window.loginPage = loginPage;