// Handle New Post Page Tabs

const tabButtons = document.querySelectorAll(".post-tab-btn");
const tabContents = document.querySelectorAll(".post-tab-content");
const mainLayout = document.querySelector(".main-layout");

tabButtons.forEach((item) => {
  item.addEventListener("click", (e) => {
    const contentTabName = e.target.getAttribute("data-target");

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

    e.target.classList.add("active");
    document.querySelector(`#${contentTabName}`).classList.add("active");
  });
});
