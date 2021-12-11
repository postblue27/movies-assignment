

$(function(){
  $('.dropdown').click(function(){
    $('.dropdown-content').toggleClass('visible');
  });

  $('main>div>img').click(function(){
    $('#movieModal').modal();
  });
});

function redirectClick() {
  // console.log(document.location.ancestorOrigins);
  // document.location.ancestorOrigins
  document.location.replace('movies.html');
}

