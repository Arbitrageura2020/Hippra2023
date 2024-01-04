
document.getElementById("login").addEventListener("click", function(){
    document.querySelector(".login-1-popup").style.display = "flex";
});

document.getElementById("login-here").addEventListener("click", function(){
    document.querySelector(".login-1-popup").style.display = "flex";
});

document.getElementById("login-create-accnt").addEventListener("click", function(){
    document.querySelector(".create-acc").style.display = "flex";
});

document.getElementById("login-register").addEventListener("click", function(){
    document.querySelector(".create-acc").style.display = "flex";
});

document.getElementById('register_new_user').addEventListener('click', function(){
    document.querySelector(".login-1-popup").style.display = "none";
    document.querySelector('.create-acc').style.display = 'flex';
});

function dropDown(){
    const dropdown = document.getElementById('acc_type');
    const selectedValue = dropdown.value;
    if (selectedValue === 'phy') {
        document.querySelector('.create-acc').style.display = 'none';
        document.querySelector('.profile-phy').style.display = 'flex';
    } else if (selectedValue === 'nurse') {
        document.querySelector('.create-acc').style.display = 'none';
        document.querySelector('.profile-nurse-prac').style.display = 'flex';
    }
}


document.getElementById('back_profile').addEventListener('click', function(){
    document.querySelector('.profile-phy').style.display = 'none';
    document.querySelector('.create-acc').style.display = 'flex';
});

document.getElementById('next_profile').addEventListener('click', function(){
    document.querySelector('.profile-phy').style.display = 'none';
    document.querySelector('.contact').style.display = 'flex';
});

document.getElementById('back_profile_nurse').addEventListener('click', function(){
    document.querySelector('.profile-nurse-prac').style.display = 'none';
    document.querySelector('.create-acc').style.display = 'flex';
});

document.getElementById('next_profile_nurse').addEventListener('click', function(){
    document.querySelector('.profile-nurse-prac').style.display = 'none';
    document.querySelector('.contact').style.display = 'flex';
});

document.getElementById('back_contact').addEventListener('click', function(){
    const dropdown = document.getElementById('acc_type');
    const selectedValue = dropdown.value;
    if (selectedValue === 'phy') {
        document.querySelector('.contact').style.display = 'none';
        document.querySelector('.profile-phy').style.display = 'flex';
    } else if (selectedValue === 'nurse') {
        document.querySelector('.contact').style.display = 'none';
        document.querySelector('.profile-nurse-prac').style.display = 'flex';
    }
});

document.getElementById('sub_contact').addEventListener('click', function(){
    document.querySelector('.contact').style.display = 'none';
    document.querySelector('.welcome-popup').style.display = 'flex';
});



document.getElementById('here').addEventListener('click', function(){
    document.querySelector('.welcome-popup').style.display = 'none';
});