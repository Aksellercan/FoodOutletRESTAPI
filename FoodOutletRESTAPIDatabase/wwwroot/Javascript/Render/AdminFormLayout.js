var mainContent;

function setMainContent(setmainContent) {
    mainContent = setmainContent;
}

function outletFormLayout() {
    const test = document.createElement('form');
    mainContent.appendChild(test);
}

export default {
    setMainContent,
    outletFormLayout
}