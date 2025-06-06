const listContent = document.getElementById('scopeListShow');
const mainContent = document.getElementById('buttonDiv');
const scopesArray = [];
addList();

document.getElementById('redirectForm').addEventListener('submit',function(event) {
    event.preventDefault();
    var state = generateRandomString(16);
    window.open(url, '_blank').focus();
});

function addList(){
    const addListBtn = document.createElement('button');
        addListBtn.href = "#";
        addListBtn.className = "buttonClassWider";
        addListBtn.id = "submitBtn";
        addListBtn.textContent = "Add Scope";
    addListBtn.addEventListener('click', function(event){
        event.preventDefault();
        var setScope = document.getElementById('setScopes').value;
        if (typeof setScope === "string" && setScope.trim().length === 0) {
            return;
        }
        scopesArray.push(setScope);
        loop();
        document.getElementById('setScopes').value = '';
    });
    mainContent.appendChild(addListBtn)
}

function loop(){
    listContent.innerHTML = '';
    for (let i = 0; i < scopesArray.length; i++) {
        const listView = document.createElement('li');
        listView.className = "jsList";
        listView.textContent = " " + scopesArray[i];
        listContent.appendChild(listView);
    }
}

function generateRandomString(length) {
    const characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
    let result = '';
    for (let i = 0; i < length; i++) {
        const randomIndex = Math.floor(Math.random() * characters.length);
        result += characters.charAt(randomIndex);
    }
    return result;
}