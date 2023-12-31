// Nav search
const searchInput = document.querySelector("#nav-search");
const searchDropdown = document.querySelector("#search-dropdown");
const searchFilter = document.querySelector("#search-filter");
const searchFilterIcon = document.querySelector("#search-filter-icon");
const searchFilterBox = document.querySelector("#search-filter-box");

// Handle Toggle Search Dropdown
const toggleSearchDropdown = () => {
  if (searchDropdown) {
    searchDropdown.classList.toggle("visible");
  }
};

// Handle Toggle Search Filter
const toggleSearchFilter = () => {
  if (searchFilterIcon) {
    const isOpen = searchFilterIcon.src.includes("close.svg");
    searchFilterIcon.src = isOpen
      ? "./img/icons/search-settings.svg"
      : "./img/icons/close.svg";
    searchFilterBox.classList.toggle("visible");
  }
};

// Handle Profile Dropdown

const profileDropdownBtn = document.querySelector(".nav-profile .profile-btn");
const profileDropdownMenu = document.querySelector(".nav-profile-dropdown");

if (profileDropdownBtn) {
  profileDropdownBtn.addEventListener("click", (e) => {
    const icon = profileDropdownBtn.querySelector(".profile-icon");

    icon.classList.toggle("rotate");

    profileDropdownMenu.classList.toggle("visible");
  });
}

// Sidebar Collapse Expand
const sidebar = document.querySelector("#sidebar");
const sidebarButton = document.querySelector("#sidebar-btn");
const sidebarIcon = document.querySelector("#sidebar-btn img");
const sidebarMenuLink = document.querySelectorAll(".sidebar-menu ul li a");
const sidebarMenuText = document.querySelectorAll(
  ".sidebar-menu ul li a .sidebar-link-txt"
);

// Handle Sidebar Toggle
const toggleSidebar = () => {
  if (!sidebar) return;

  const isExpanded = sidebarIcon.src.includes("expand.svg");
  const main = document.querySelector("main");
  const userInfobox = document.querySelector(
    ".sidebar-profile .sidebar-profile-info"
  );

  sidebarIcon.src = isExpanded
    ? "./img/icons/collapse.svg"
    : "./img/icons/expand.svg";
  sidebar.classList.toggle("expand", isExpanded);

  sidebarMenuLink.forEach((item) => item.classList.toggle("full", isExpanded));
  sidebarMenuText.forEach((item) => item.classList.toggle("show", isExpanded));
  sidebarButton.classList.toggle("expand", isExpanded);
  main.classList.toggle("sidebar-expand", isExpanded);
  userInfobox.classList.toggle("show", isExpanded);
};

// Sidebar Menu Active by url
const sidebarMenu = document.querySelectorAll(".sidebar-menu ul li a");

const activeSidebarMenu = () => {
  const url = window.location.href;

  sidebarMenu.forEach((item) => {
    if (item.href === url) {
      const iconSpan = item.querySelector(".sidebar-link-icon svg");
      const svgPaths = iconSpan.querySelectorAll("path");

      svgPaths.forEach((path) => {
        path.setAttribute("stroke", "#1da39d");
      });
      item.classList.add("active");
    } else {
      const iconSpan = item.querySelector(".sidebar-link-icon svg");
      const svgPaths = iconSpan.querySelectorAll("path");

      svgPaths.forEach((path) => {
        path.setAttribute("stroke", "#40454A");
      });
      item.classList.remove("active");
    }
  });
};

activeSidebarMenu();

// Handle Outside Clicks
const handleDocumentClick = (event) => {
  const isClickInsideSearchInput = searchInput.contains(event.target);
  const isClickInsideSearchDropdown =
    searchDropdown && searchDropdown.contains(event.target);
  const isClickFilterBtn = searchFilter.contains(event.target);
  const isClickFilterBox =
    searchFilterBox && searchFilterBox.contains(event.target);

  // Handle Outside click for search dropdown
  if (!isClickInsideSearchInput && !isClickInsideSearchDropdown) {
    searchDropdown.classList.remove("visible");
  }
  if (!isClickFilterBtn && !isClickFilterBox) {
    searchFilterIcon.src = "./img/icons/search-settings.svg";
    searchFilterBox.classList.remove("visible");
  }

  // Handle Outside click for profile dropdown
  const isClickProfileDropdown =
    profileDropdownBtn && profileDropdownBtn.contains(event.target);

  if (!isClickProfileDropdown) {
    profileDropdownMenu.classList.remove("visible");
  }
};

searchInput.addEventListener("click", toggleSearchDropdown);
searchFilter.addEventListener("click", toggleSearchFilter);
sidebarButton.addEventListener("click", toggleSidebar);
document.addEventListener("click", handleDocumentClick);
