import ApiRequest from "/Javascript/Services/ApiRequest.js";

var mainContent;
var outlet;

function setMainContent(setmainContent) {
    mainContent = setmainContent;
}

function setOutlet(setoutlet) {
    outlet = setoutlet;
}

function loadUI() {
    const formElem = outletFormLayout();
    inputFields(formElem);
}

function outletFormLayout() {
    const outletForm = document.createElement('form');
    outletForm.id = 'outletForm';
    mainContent.appendChild(outletForm);
    return document.getElementById('outletForm');
}

function inputFields(formElem) {
    const nameDiv = document.createElement('div');
    nameDiv.id = 'nameDiv';
    const nameLabel = document.createElement('label');
    nameLabel.id = 'nameLabel';
    nameLabel.for = 'newName';
    nameLabel.textContent = `Current Name: ${outlet.name}`;
    const nameInput = document.createElement('input');
    nameInput.id = 'setNewName';
    nameInput.type = 'text';
    nameInput.name = 'newName';

    const locationDiv = document.createElement('div');
    locationDiv.id = 'locationDiv';
    const locationLabel = document.createElement('label');
    locationLabel.id = 'locationLabel';
    locationLabel.for = 'newLocation';
    locationLabel.textContent = `Current location: ${outlet.location}`;
    const locationInput = document.createElement('input');
    locationInput.id = 'setNewLocation';
    locationInput.type = 'text';
    locationInput.name = 'newLocation';

    const breakLine = document.createElement('br');
    const setButton = buttons('Submit');

    setButton.addEventListener('click', async function(event){
        event.preventDefault();
        var newName = document.getElementById('setNewName').value;
        if (newName === '') {
            newName = outlet.name;
        }
        var newLocation = document.getElementById('setNewLocation').value;
        if (newLocation === '') {
            newLocation = outlet.location;
        }
        console.log("placeholder" + `${newName} and ${newLocation}`);
        await ApiRequest.updateOutletDetails(newName,newLocation, outlet.id);
    });

    const removeButton = buttons('Remove Outlet');
    const asidePanel = document.getElementById('adminRemove');

    asidePanel.innerHTML = '';
    formElem.appendChild(nameLabel);
    formElem.appendChild(nameDiv);
    nameDiv.appendChild(nameInput);
    formElem.appendChild(breakLine);
    formElem.appendChild(locationLabel);
    formElem.appendChild(locationDiv);
    locationDiv.appendChild(locationInput);
    formElem.appendChild(setButton);
    document.getElementById('asidePanelContent').textContent = `Remove Outlet: ${outlet.name}`;
    asidePanel.appendChild(removeButton);

    removeButton.addEventListener('click', async function(event) {
        event.preventDefault();
        await ApiRequest.removeOutlet(outlet.id);
    });
}

function buttons(textContent) {
    const setButton = document.createElement('a');
    setButton.href = "#";
    setButton.className = "myButton";
    setButton.textContent = textContent;
    return setButton;
}

export default {
    setMainContent,
    setOutlet,
    loadUI
}