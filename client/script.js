$(function(){
  $('.dropdown').click(function(){
    $('.dropdown-content').toggleClass('visible');
  });

  $('main>div>img').click(function(){
    $('#movieModal').modal();
  });
});


document.addEventListener("DOMContentLoaded", () => {
  let span = document.getElementById("nameSpan");
  span.innerHTML = localStorage.getItem("name");
});

// $(document).ready(function(){
//   $('.toast').toast('show');
// });

function signIn() {
  var url = "http://localhost:5000/api/user/sign-in";
  var data = {
    username: $('#usernameInput').val(),
    password: $('#passwordInput').val()
  }

  jQuery.ajax({
    type: "POST",
    url: url,
    dataType: "json",
    data: data,
    success: function (data) {
      console.log(data);
      localStorage.setItem("userNumber", data.number);
      localStorage.setItem("name", data.name);
      localStorage.setItem("username", data.username);
      document.location.href = "movies.html"
      
    },
    error: function (XMLHttpRequest) {
      showToast('errorToast', XMLHttpRequest.responseText);
    }
 });
}

function register() {
  let username = $('#usernameRegisterInput').val();
  let name = $('#nameRegisterInput').val();
  let password = $('#passwordRegisterInput').val();
  let confirmPassword = $('#confirmPasswordRegisterInput').val();
  if(password !== confirmPassword) {
    showToast('errorToast', "Passwords don't match");
    return;
  }

  var url = "http://localhost:5000/api/user/register";
  var data = {
    name: name,
    username: username,
    password: password
  }

  jQuery.ajax({
    type: "POST",
    url: url,
    dataType: "json",
    data: data,
    async: false, 
    success: function (XMLHttpRequest) {
      showToast('successToast', 'User registered');
    },
    error: function (XMLHttpRequest) {
      showToast('errorToast', XMLHttpRequest.responseText);
    }
  });
}

function showToast(toastClassName, message) {
  $('.' + toastClassName).toast('show');
  text = document.getElementsByClassName(toastClassName)[0].lastElementChild;
  text.innerHTML = message;
}



