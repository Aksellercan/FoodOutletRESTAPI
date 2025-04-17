function clearStorage() {
    localStorage.removeItem('token');
    alert("cleared token");
    console.log("cleared token");
}

document.getElementById('loginForm').addEventListener('submit', async function (event) {
    event.preventDefault();
    const username = document.getElementById('username').value;
    const password = document.getElementById('password').value;

    console.log(username);
    console.log(password);
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
    const result = await response.json();
    if (response.ok) {
        alert('Login successful');
        localStorage.setItem('token', result.token);
        console.log('Token:', result.token);
    } else {
        alert('Login failed');
        console.log(result);
    }
});

document.getElementById('registerForm').addEventListener('submit', async function (event) {
    event.preventDefault();

    const username = document.getElementById('regUsername').value;
    const password = document.getElementById('regPassword').value;

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

    const result = await response.json();

    if (response.ok) {
        alert('Registration successful');
        console.log(result);
    } else {
        alert('Registration failed');
        console.log(result);
    }
});

document.getElementById('reviewForm').addEventListener('submit', async function (event) {
    event.preventDefault();

    const comment = document.getElementById('regComment').value;
    const score = document.getElementById('regScore').value;
    const foodoutletid = document.getElementById('regFoodoutletid').value

    if (score < 1 || score > 5) alert('Score must be between 1 and 5');

    const response = await fetch(`/api/reviews/${foodoutletid}/reviews`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            Authorization: 'Bearer ' + localStorage.getItem('token')
        },
        body: JSON.stringify({
            comment: comment,
            score: score
        })
    });
    console.log(response)
    const result = await response.text();
    if (response.ok) {
        alert('Submission successful ' + result);
    } else {
        alert('Submission failed. ' + result);
        console.log(result);
    }
});

document.getElementById('btnList').addEventListener('click', async function (event) {
    event.preventDefault();

    const response = await fetch ('/api/foodoutlet');
    const data = await response.json();

    const resultDiv = document.getElementById('outletShowList');
    resultDiv.innerHTML = "";
    for(let i = 0; i < data.length; i++)
    {
        const outlet = data[i];
        console.log(data[i])

        const outletLine = document.createElement('p');
        outletLine.textContent = ` ${outlet.name} - ${outlet.location} (${outlet.rating} stars from ${outlet.reviewCount} reviews) `;
        resultDiv.appendChild(outletLine);
    }
});