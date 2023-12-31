// Notification

const notificationTabBtn = document.querySelectorAll(
  ".notification-tab-item button"
);

if (notificationTabBtn) {
  notificationTabBtn.forEach((btn) => {
    btn.addEventListener("click", () => {
      const tab = btn.getAttribute("data-tab");
      const notificationTabContent = document.querySelectorAll(
        ".notification-inner-content"
      );

      const notificationContents = Array.from(notificationTabContent);
      const filteredTabs = notificationContents.filter(
        (notification) => notification.getAttribute("data-content") === tab
      );

      notificationTabBtn.forEach((btn) => {
        btn.parentNode.classList.remove("active");
        btn.classList.remove("active");
      });

      btn.classList.add("active");
      btn.parentNode.classList.add("active");

      notificationTabContent.forEach((tab) => {
        tab.classList.remove("active");
      });

      filteredTabs.forEach((tab) => {
        tab.classList.add("active");
      });
    });
  });
}
