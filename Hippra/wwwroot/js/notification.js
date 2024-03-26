// Notification Tabs
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

// Fill in the notification dot
const getCookie = (name) => {
  const cookies = document.cookie
    .split("; ")
    .map((cookie) => cookie.split("="));
  const cookie = cookies.find(([cookieName]) => cookieName === name);
  return cookie ? cookie[1] : null;
};

const setCookie = (name, value, days) => {
  const expirationDate = new Date();
  expirationDate.setDate(expirationDate.getDate() + days);
  document.cookie = `${name}=${value}; expires=${expirationDate.toUTCString()}; path=/`;
};

const removeCookie = (name) =>
  (document.cookie = `${name}=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;`);

const updateNotificationDots = (element, checked, color, active) => {
  console.log(active);
  const fill = checked ? color ?? "#DA5810" : "#ffffff";
  const stroke = checked ? color ?? "#DA5810" : active ? "#1da39d" : "#40454A";
  element.style.fill = fill;
  element.style.stroke = stroke;
};

const notificationDot = document.querySelector(".nav-notification a svg .dot");
const sidebarNotificationDot = document.querySelector(
  ".sidebar-link-icon svg .dot"
);
const notificationSwitch = document.querySelector(
  ".notification-switch .form-switch input"
);
const parentElementNotification = document.querySelector(
  ".sidebar-link-icon.notification"
);
const isActiveSidebarNotification =
  parentElementNotification.parentNode.classList.contains("active");

if (notificationDot || sidebarNotificationDot) {
  const notificationStatus = getCookie("notifications");

  if (notificationStatus === "true") {
    updateNotificationDots(notificationDot, true);
    updateNotificationDots(sidebarNotificationDot, true, "#1da39d");
    if (notificationSwitch) notificationSwitch.checked = true;
  } else {
    updateNotificationDots(notificationDot, false);
    updateNotificationDots(
      sidebarNotificationDot,
      false,
      "#1da39d",
      isActiveSidebarNotification
    );

    if (notificationSwitch) notificationSwitch.checked = false;
  }

  if (notificationSwitch) {
    notificationSwitch.addEventListener("change", () => {
      updateNotificationDots(notificationDot, notificationSwitch.checked);
      updateNotificationDots(
        sidebarNotificationDot,
        notificationSwitch.checked,
        "#1da39d",
        isActiveSidebarNotification
      );
      if (notificationSwitch.checked) {
        setCookie("notifications", true, 30);
      } else {
        removeCookie("notifications");
      }
    });
  }
}
