import Header from "/Javascript/Functions/Header.js";
import ProfileLayout from "/Javascript/Render/ProfileLayout.js";
import Logout from '/Javascript/User/Logout.js';

var CurrentUserName;
var CurrentUserId;

document.addEventListener("DOMContentLoaded", async function (event) { //runs on page load
    event.preventDefault();
    //show logged in user
    if (Header.getisLoggedin() === false) return;
    CurrentUserName = Header.getCurrentUsername();
    CurrentUserId = Header.getCurrentUserId();
    Header.profileHeader();
    ProfileLayout.loop(Header.getisLoggedin());
});

window.Logout = Logout.LogOut;