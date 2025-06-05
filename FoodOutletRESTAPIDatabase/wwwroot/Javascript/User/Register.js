import Login from '/Javascript/User/Login.js';

async function requestRegister(username, password){
    const response = await fetch('/api/login/register', {
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
        Login.Login(username, password);
        return true;
    } else {
        alert('Registration failed');
        return false;
    }
}

async function Register(username, password){
    const response = await requestRegister(username, password);
    if (!response) {
        console.log("Register error");
        return;
    }
}

export default {
    Register
}