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


// Sidebar Menu Active by url
const sidebarMenu = document.querySelectorAll(".sidebar-menu ul li a");

const activeSidebarMenu = () => {
    const url = window.location.href;

    sidebarMenu.forEach((item) => {
        const iconSpan = item.querySelector(".sidebar-link-icon svg");
        const svgPaths = iconSpan.querySelectorAll("path");

        if (item.href === url) {
            svgPaths.forEach((path) => {
                path.setAttribute("stroke", "#1da39d");
                if (iconSpan.dataset.type === "new-post") {
                    path.setAttribute("fill", "#1da39d");
                }
            });
            item.classList.add("active");
        } else if (url.match(/account-password|\/notifications-subscriptions/)) {
            const settings = document.querySelector(".sidebar-settings");
            const settingsSvgPaths = settings.querySelectorAll(
                ".sidebar-settings svg path"
            );
            if (settings) {
                // Remove all sidebar menu item active status
                sidebarMenu.forEach((item) => {
                    item.classList.remove("active");
                });

                svgPaths.forEach((path) => {
                    path.setAttribute("stroke", "#40454A");
                });

                settingsSvgPaths.forEach((path) => {
                    path.setAttribute("stroke", "#1da39d");
                });
                settings.classList.add("active");
            }
        } else {
            svgPaths.forEach((path) => {
                path.setAttribute("stroke", "#40454A");
                if (iconSpan.dataset.type === "new-post") {
                    path.setAttribute("fill", "#40454A");
                }
            });
            item.classList.remove("active");
        }
    });
};

activeSidebarMenu();

// Handle Filter Checkboxes
const handleFilterCheckBoxes = (checkboxes) => {
    checkboxes.forEach((checkbox) => {
        checkbox.addEventListener("change", () => {
            checkboxes.forEach((otherCheckbox) => {
                if (otherCheckbox !== checkbox) {
                    otherCheckbox.checked = false;
                }
            });
        });
    });
};

handleFilterCheckBoxes(document.querySelectorAll(".filter-checkbox"));

handleFilterCheckBoxes(
    document.querySelectorAll('[data-type="clinical"].filter-checkbox-search')
);
handleFilterCheckBoxes(
    document.querySelectorAll('[data-type="ask4"].filter-checkbox-search')
);



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

//searchInput.addEventListener("click", toggleSearchDropdown);
//searchFilter.addEventListener("click", toggleSearchFilter);
//sidebarBot.addEventListener("click", toggleSidebar);
//document.addEventListener("click", handleDocumentClick);

// Handle Scrollble
const body = document.querySelector("body");

if (body.classList.contains("single")) {
    body.style.overflow = "hidden";
}

const handleScrollable = () => {
    const scrollableSections = document.querySelectorAll(".inside-scroll");

    scrollableSections.forEach((scrollableSection) => {
        const rect = scrollableSection.getBoundingClientRect();
        const distanceFromTopToDashboard = rect.top + window.scrollY;

        scrollableSection.style.maxHeight =
            window.innerHeight - distanceFromTopToDashboard + "px";
        scrollableSection.style.paddingBottom = "20px";
    });
};

 function handlePageScrollable() {
    handleScrollable();

    window.addEventListener("resize", handleScrollable);
}

// Handle Case Page Tabs

const caseTabButtons = document.querySelectorAll(".case-tab-btn");
const caseTabContents = document.querySelectorAll(".case-tab-content");

function InitCasePage() {
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
}
