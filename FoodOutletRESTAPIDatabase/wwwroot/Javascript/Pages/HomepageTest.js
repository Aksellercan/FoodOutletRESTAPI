import outletLayout from '/Javascript/Render/OutletListLayout.js';
import reviewsLayout from '/Javascript/Render/ReviewsLayout.js';
import ApiRequest from '/Javascript/Services/ApiRequest.js';
import Header from '/Javascript/Functions/Header.js';
import Logout from '/Javascript/User/Logout.js';


let id;
const mainContent = document.getElementById('content');
var CurrentUserName;
var CurrentUserId;

document.addEventListener("DOMContentLoaded", async function (event) { //runs on page load
    event.preventDefault();
    reviewsLayout.setMainContent(mainContent);
    const outletList = await ApiRequest.getOutletList();
    const resultDiv = document.getElementById('outletShowList');
    Header.basicHeader();
    if (Header.getisLoggedin() === false) return;
    CurrentUserName = Header.getCurrentUsername();
    CurrentUserId = Header.getCurrentUserId();

    //continue
    if (outletList.length > 0) {
        resultDiv.innerHTML = "";
        for (let i = 0; i < outletList.length; i++) {
            const outlet = outletList[i];
            const outletLine = outletLayout.outletListLayout(outlet, resultDiv);

            outletLine.addEventListener('click', async function (event) {
                event.preventDefault();
                if (outlet.id == id) { return; } //avoid loading same data
                loadReviews(outlet.id);
                mainContent.innerHTML = "";
                document.getElementById('titleHeader').textContent = `Reviews for ${outlet.name}`;
                id = outlet.id;
            });
        }
    } else {
        resultDiv.innerHTML = "<h2 class=\"listH2\">No outlets found</h2>";
    }
});

window.Logout = Logout.LogOut;

async function loadReviews(outletId) {
    const reviews = await ApiRequest.getOutletReviews(outletId);
                //<main> id="content" section
                reviewsLayout.reviewLayout(reviews);
                reviewsLayout.reviewFormLayout(outletId);
                id = outletId;
}