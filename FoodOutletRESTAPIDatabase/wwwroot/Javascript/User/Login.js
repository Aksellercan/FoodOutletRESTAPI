import Auth from '/Javascript/Services/Auth.js';

async function requestLogin(username, password){
    const response = await fetch('/api/login/login', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({
            username: username,
            password: password
        })
    });
    if (response.ok) {
        sessionStorage.clear();
        alert('Login successful');
        await Auth.getUserName(false);
        return true;
    } else {
        alert('Login failed');
        return false;
    }
}

async function Login(username,password){
    const response = await requestLogin(username, password);
    if (!response) {
        console.log("Login error");
        return;
    }
    window.location.href = "/Pages/Homepage/homepage.html";
}

export default {
    Login
}