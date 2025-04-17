document.addEventListener("DOMContentLoaded", async function (event) {
    event.preventDefault();

    const response = await fetch ('/api/foodoutlet');
    const data = await response.json();

    const resultDiv = document.getElementById('outletShowList');
    resultDiv.innerHTML = "";
    for(let i = 0; i < data.length; i++)
    {
        const outlet = data[i];
        console.log(data[i])

        id = outlet.id;
        console.log(id);

        const outletLine = document.createElement('a');
        outletLine.href = "#";
        outletLine.className = "myButton";

        console.log(`${outletLine.id} ${outletLine.className} ${outletLine.type}`);
        outletLine.textContent = ` ${outlet.name}: ${outlet.id} - ${outlet.location} (${outlet.rating} from ${outlet.reviewCount} reviews) `;
        resultDiv.appendChild(outletLine);

        outletLine.addEventListener('click', async function (event) {
            event.preventDefault();
            console.log(`${outlet.id} clicked`);
            const response = await fetch(`/api/reviews/${outlet.id}`);
            const reviews = await response.json();

            const mainContent = document.getElementById('content');
            mainContent.innerHTML = `<h2 id="contentH2">Reviews for ${outlet.name}</h2>`;
            if(reviews.length > 0){
            reviews.forEach(review => {
                const breakLine = document.createElement('hr');
                const ratingLine = document.createElement('p');
                ratingLine.id = "reviewP"
                ratingLine.textContent = `Rating: ${review.score}`;
                const reviewElem = document.createElement('p');
                reviewElem.id = "reviewP"

                const comment = `userId${review.userId}: ${review.comment}`;
                reviewElem.textContent = comment;

                mainContent.appendChild(breakLine);
                mainContent.appendChild(ratingLine);
                mainContent.appendChild(reviewElem);
            }
        );
        } else {
            const breakLine = document.createElement('br');
            const reviewElem = document.createElement('p');
            reviewElem.id = "reviewP"
            reviewElem.textContent = 'No Reviews';

            mainContent.appendChild(breakLine);
            mainContent.appendChild(reviewElem);
        }
        const postReview = document.createElement('input');
        postReview.id = "inputboxP";
        postReview.type = "text";

        const breakLine = document.createElement('br');
        mainContent.appendChild(breakLine);
        mainContent.appendChild(postReview);
        });
    }
});

