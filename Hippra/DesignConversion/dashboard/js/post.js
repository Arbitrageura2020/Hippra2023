// Handle New Post Page & Search Page Tabs

const tabButtons = document.querySelectorAll(".post-tab-btn");
const tabContents = document.querySelectorAll(".post-tab-content");
const headingAction = document.querySelectorAll(".heading-actions");
const mainLayout = document.querySelector(".main-layout");

tabButtons.forEach((item) => {
  item.addEventListener("click", (e) => {
    const contentTabName = e.target.getAttribute("data-target");
    const contentFilter = e.target.getAttribute("data-action");

    if (contentTabName === "post-ask-4-content") {
      mainLayout.classList.add("orange");
    } else {
      mainLayout.classList.remove("orange");
    }

    tabButtons.forEach((item) => {
      item.classList.remove("active");
    });

    tabContents.forEach((item) => {
      item.classList.remove("active");
    });

    headingAction.forEach((item) => {
      item.classList.remove("active");
    });

    document.querySelector(`#${contentFilter}`).classList.add("active");

    e.target.classList.add("active");
    document.querySelector(`#${contentTabName}`).classList.add("active");
  });
});
