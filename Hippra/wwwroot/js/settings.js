
// Handle Profile Picture Upload
const uploadButton = document.querySelector("#uploadProfilePictureBtn");
const uploadInput = document.querySelector("#uploadProfilePictureInput");
const profilePicture = document.querySelector(".settings-profile-preview img");
function HandleUploadButtonClick() {


    // Trigger Upload
    if (uploadButton) {

        uploadInput.click();


    }
}

// Handle Password Change Password
const newPassword = document.querySelector("#newPassword");
const confirmPassword = document.querySelector("#confirmPassword");

if (newPassword && confirmPassword) {
    newPassword.addEventListener("input", () => {
        if (newPassword.value !== confirmPassword.value) {
            confirmPassword.classList.add("is-invalid");
        } else {
            confirmPassword.classList.remove("is-invalid");
        }
    });

    confirmPassword.addEventListener("input", () => {
        if (newPassword.value !== confirmPassword.value) {
            confirmPassword.classList.add("is-invalid");
        } else {
            confirmPassword.classList.remove("is-invalid");
        }
    });
}

// Handle Edit Input

const accountInputs = document.querySelectorAll(".account-inputs");
const editButtons = document.querySelectorAll(".setting-edit-action");

if (accountInputs.length > 0 && editButtons.length > 0) {
    editButtons.forEach((button) => {
        button.addEventListener("click", () => {
            const input = button.getAttribute("data-button");

            const accountInputsArray = Array.from(accountInputs);

            const filteredInputs = accountInputsArray.filter(
                (accountInput) => accountInput.getAttribute("data-input") === input
            );

            filteredInputs.forEach((input) => {
                if (input.hasAttribute("disabled")) {
                    input.removeAttribute("disabled");
                    input.classList.add("editable");
                } else {
                    input.setAttribute("disabled", "disabled");
                    input.classList.remove("editable");
                }
            });
        });
    });
}

// Handle Setting Nav Active State
const settingNavLinks = document.querySelectorAll(".settings-nav-item a");

if (settingNavLinks.length > 0) {
    const url = window.location.href;

    settingNavLinks.forEach((link) => {
        if (link.href === url) {
            link.classList.add("active");
            link.parentElement.classList.add("active");
        } else {
            link.classList.remove("active");
            link.parentElement.classList.remove("active");
        }
    });
}

// Handle Notification and Subscriptions
const handleCheckboxChange = (checkbox, items) => {
    if (checkbox) {
        checkbox.addEventListener("change", () => {
            items.forEach((item) => {
                item.checked = checkbox.checked;
            });
        });
    }
};

const notificationCheckBox = document.querySelector("#emailNotification");
const subscriptionCheckBox = document.querySelector(
    "#subscriptionsNotification"
);
const notificationItems = document.querySelectorAll(
    ".settings-notification-item.notification .form-switch input"
);
const subscriptionItems = document.querySelectorAll(
    ".settings-notification-item.subscriptions .form-switch input"
);

handleCheckboxChange(notificationCheckBox, notificationItems);
handleCheckboxChange(subscriptionCheckBox, subscriptionItems);
