document.addEventListener("DOMContentLoaded", function () {
  document.querySelectorAll(".button-container .btn").forEach((button) => {
    button.addEventListener("click", function () {
      this.classList.toggle("clicked");
    });
  });
});
