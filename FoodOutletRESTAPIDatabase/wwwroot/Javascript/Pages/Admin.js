import Header from "/Javascript/Functions/Header.js";
import ApiRequest from "/Javascript/Services/ApiRequest.js";
import OutletListLayout from "/Javascript/Render/OutletListLayout.js";
import AdminFormLayout from "/Javascript/Render/AdminFormLayout.js";

let id;
const mainContent = document.getElementById('content');
document.addEventListener("DOMContentLoaded", async function (event) { //runs on page load
    event.preventDefault();
    Header.adminHeader();
    AdminFormLayout.setMainContent(mainContent);
    const outletList = await ApiRequest.getOutletList();
    const resultDiv = document.getElementById('outletShowList');

    if (outletList.length > 0) {
            resultDiv.innerHTML = "";
            for (let i = 0; i < outletList.length; i++) {
                const outlet = outletList[i];
                const outletLine = OutletListLayout.outletListLayout(outlet, resultDiv);

                outletLine.addEventListener('click', async function (event) {
                                event.preventDefault();
                                if (outlet.id == id) { return; } //avoid loading same data
                                id = ApiRequest.loadReviews(outlet.id);
                                mainContent.innerHTML = "";
                                document.getElementById('titleHeader').textContent = `Details for ${outlet.name}`;
                                AdminFormLayout.outletFormLayout();
                                id = outlet.id;
                });
            }
        } else {
        resultDiv.innerHTML = "<h2 class=\"listH2\">No outlets found</h2>";
    }
});

