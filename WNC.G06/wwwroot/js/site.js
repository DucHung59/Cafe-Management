var btnLogin = document.getElementById("btnLogin");
var btnRegister = document.getElementById("btnRegister");

var loginForm = document.querySelector(".login-form");
var registerForm = document.querySelector(".register-form"); // Sửa lại chính tả từ 'registerFrom' thành 'registerForm'

console.log(btnLogin, btnRegister, loginForm, registerForm);

btnLogin.addEventListener("click", function () {
    btnRegister.classList.remove("active");
    btnLogin.classList.add("active");
    loginForm.classList.remove("hidden");
    registerForm.classList.add("hidden");
})

btnRegister.addEventListener("click", function () {
    btnLogin.classList.remove("active");
    btnRegister.classList.add("active");
    registerForm.classList.remove("hidden");
    loginForm.classList.add("hidden");
});
