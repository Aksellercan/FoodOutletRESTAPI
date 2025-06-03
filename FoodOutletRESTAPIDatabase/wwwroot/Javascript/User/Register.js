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
    } else {
        alert('Registration failed');
    }
}

async function Register(){
    const username = document.getElementById('regUsername').value;
    const password = document.getElementById('regPassword').value;
    await requestRegister(username, password);
}

export default {
    Register
}