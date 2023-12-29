const url = window.location.href;
const urlObject = new URL(url);
const fragmentIdentifier = urlObject.hash;

if (fragmentIdentifier) {
  const elementId = fragmentIdentifier.slice(1);

  const contentTab = document.getElementById(elementId);

  if (contentTab) {
    // remove all active all tabs
    const tabButtons = document.querySelectorAll(".post-tab-btn");
    tabButtons.forEach((item) => {
      item.classList.remove("active");
    });

    // remove all active all tabs
    const tabContents = document.querySelectorAll(".post-tab-content");
    tabContents.forEach((item) => {
      item.classList.remove("active");
    });

    // add active class to tab button
    const tabButton = document.querySelector(`[data-target="${elementId}"]`);
    tabButton.classList.add("active");

    // add active class to tab content
    contentTab.classList.add("active");

    // if the tab is the ask for content tab then add orange class to the main layout
    if (elementId === "post-ask-4-content") {
      const mainLayout = document.querySelector(".main-layout");
      mainLayout.classList.add("orange");
    } else {
      const mainLayout = document.querySelector(".main-layout");
      mainLayout.classList.remove("orange");
    }
  }
}
