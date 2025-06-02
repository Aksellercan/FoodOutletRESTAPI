//Display outlet list
function outletListLayout(outlet, resultDiv) {
    const outletLine = document.createElement('a');
    outletLine.href = "#";
    outletLine.className = "myButton";
    outletLine.textContent = ` ${outlet.name} - ${outlet.location} (${outlet.rating} from ${outlet.reviewCount} reviews) `;
    resultDiv.appendChild(outletLine);
    return outletLine
}

export default {
    outletListLayout
};
