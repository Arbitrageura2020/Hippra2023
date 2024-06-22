// Handle Case Page Tabs

const caseTabButtons = document.querySelectorAll(".case-tab-btn");
const caseTabContents = document.querySelectorAll(".case-tab-content");

caseTabButtons.forEach((item) => {
    item.addEventListener("click", (e) => {
        const contentTabName = e.target.getAttribute("data-target");

        const upperDivWithCaseContent = item.closest(".case-layout.case-1");
        if (upperDivWithCaseContent) {
            mainLayout.classList.remove("orange");
        }

        caseTabButtons.forEach((item) => {
            item.classList.remove("active");
        });

        caseTabContents.forEach((item) => {
            item.classList.remove("active");
        });

        e.target.classList.add("active");
        document.querySelector(`.${contentTabName}`).classList.add("active");
    });
});


// Handle Comments File Upload
//const uploadButton = document.querySelector("#uploadFileButton");
//const uploadInput = document.querySelector("#uploadProfilePictureInput");
function HandleCommentUploadButtonClick() {

    var upldBtn = document.querySelector("#uploadFileButton");
    const uplInput = document.querySelector("#uploadFileInput");
    // Trigger Upload
    if (upldBtn) {

        uplInput.click();


    }
}
