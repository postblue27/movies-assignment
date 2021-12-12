var movies = [];
var pagesCount;

document.addEventListener("DOMContentLoaded", () => {
    
});

window.onload = function () {
    if(!getCurrentPage()) {
        localStorage.setItem("currentPage", 1)
    }
    getMovies();
}
  
function getMovies() {
    var url = "http://localhost:5000/api/main";
    jQuery.ajax({
        type: "GET",
        url: url,
        dataType: "json",
        success: function (data) {
            movies = data;
            console.log(movies);
            console.log(movies.length);
            let currentPage = getCurrentPage();
            console.log(currentPage);
            

            Paging();
            displayCurrentPage();
        }
    });
}



function removeAllChildNodes(parent) {
    while (parent.firstChild) parent.removeChild(parent.firstChild);
}
  
function Paging() {
    var pagination = document.querySelector('.pagination')
    removeAllChildNodes(pagination);
    pagesCount = Math.ceil(movies.length / 20);
    for ( let i = 1;i <= pagesCount;i++)
    {
        let pageitem = document.createElement('div')
        pageitem.innerHTML = i;
        pageitem.classList.add('page_item');
        if(localStorage.getItem("currentPage") == i) {
            pageitem.classList.add('active-page');
        }
        pageitem.onclick = function() {
            var activePage = document.querySelector('.active-page')
            activePage.classList.remove('active-page')
            changeCurrentPage(pageitem.innerHTML);
            displayCurrentPage();
            pageitem.classList.add('active-page');
        }
        pagination.appendChild(pageitem);
    }
}

function changeCurrentPage(pageNumber) {
    localStorage.setItem("currentPage", pageNumber);
}

function getCurrentPage() {
    return localStorage.getItem("currentPage");
}

function displayCurrentPage() {
    var main = document.querySelector('main')
    removeAllChildNodes(main);
    currentPage = getCurrentPage();
    for (i = (+currentPage - 1) * 20; i < (+currentPage) * 20; ++i) {
        if(!!movies[i]) {
            var newDiv = document.createElement('div');
            var newImg = document.createElement('img');
            newImg.src = 'https://image.tmdb.org/t/p/w500' + movies[i].posterPath;
            newImg.onclick = imageClickHandler(movies[i], i);
            newDiv.appendChild(newImg);
            main.appendChild(newDiv);
        }
    }
}

function imageClickHandler(movie, movieIndex) {
    return function() {
        localStorage.setItem("currentMovieIndex", movieIndex);
        $('#movieModal').modal();
        var notifyDiv = document.querySelector('.notify-div');
        if(!!notifyDiv) {
            notifyDiv.remove();
        }
        getUserStatus();
        var modalBody = document.querySelector('.movie-modal-body');
        modalBody.removeChild(modalBody.firstChild)
        let p = document.createElement('p');
        let currentMovieImg = document.createElement('img');
        currentMovieImg.src = 'https://image.tmdb.org/t/p/w500' + movie.posterPath;
        currentMovieImg.classList.add('movie-detail-poster');

        p.appendChild(currentMovieImg);
        appendMovieInfo(p, 'IMDB: ', movie.imdbId);
        appendMovieInfo(p, 'TITLE: ', movie.originalTitle);
        appendMovieInfo(p, 'YEAR RELEASE: ', movie.releaseDate);
        appendMovieInfo(p, 'RUNTIME: ', movie.runtime);
        appendMovieInfo(p, 'GENRES: ', movie.genres);
        appendMovieInfo(p, 'OVERVIEW: ', movie.overview);
        modalBody.prepend(p);
    }
}

function getUserStatus() {
    var url = "http://localhost:5000/api/main/get-user-status";
    let data = {
        userNumber: localStorage.getItem("userNumber"),
        movieNumber: movies[+localStorage.getItem("currentMovieIndex")].number
    }
    jQuery.ajax({
        type: "POST",
        url: url,
        dataType: "json",
        data,
        success: function (data) {
            console.log(data);
            var modalContent = document.querySelector('.movieModalContent');
            var notifyDiv = document.createElement('div');
            if(data.status == 'MW'){
                notifyDiv.textContent = 'This movie is in the Must Watch list';
            }
            if(data.status == 'AW') {
                notifyDiv.textContent = 'This movie is in the Already Watch list';
            }
            notifyDiv.style.width = '100%';
            notifyDiv.style.backgroundColor = '#f63c12';
            notifyDiv.style.color = 'white';
            notifyDiv.style.textAlign = 'center';
            notifyDiv.classList.add('notify-div');
            modalContent.appendChild(notifyDiv);
        }
    });
}

$(function(){
    $('.dropdown').click(function(){
        $('.dropdown-content').toggleClass('visible');
    });
});

function appendMovieInfo(p, bTextContent, spanTextContent) {
    let b = document.createElement('b');
    b.textContent = bTextContent;
    let span = document.createElement('span');
    span.textContent = spanTextContent;
    p.appendChild(b);
    p.appendChild(span);
    p.appendChild(document.createElement('br'));
}

function changeMovieStatus(status) {
    var url = "http://localhost:5000/api/main/change-movie-status";
    let data = {
        userNumber: localStorage.getItem("userNumber"),
        movieNumber: movies[+localStorage.getItem("currentMovieIndex")].number,
        status: status,
        posterPath: movies[+localStorage.getItem("currentMovieIndex")].posterPath,
        originalTitle: movies[+localStorage.getItem("currentMovieIndex")].originalTitle,
        overview: movies[+localStorage.getItem("currentMovieIndex")].overview
    }
    jQuery.ajax({
        type: "POST",
        url: url,
        dataType: "json",
        data: data,
        success: function (data) {
            if(status == 'MW') {
                showToast('successToast', 'Must Watch list successfully updated');
            }
            if(status == 'AW') {
                showToast('successToast', 'Already Watch list successfully updated');
            }
            if(status == '') {
                showToast('successToast', 'Movie cleared from the watch lists');
            }
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

function aboutModalOnClick() {
    var url = "http://localhost:5000/api/main/get-user-status-info/" + localStorage.getItem("userNumber");
    jQuery.ajax({
        type: "GET",
        url: url,
        dataType: "json",
        success: function (data) {
            console.log(data);
            // return data;
            let modalBody = document.querySelector('.aboutMeModalBody');
            let name = document.createElement('h2');
            name.textContent = localStorage.getItem("name");
            modalBody.appendChild(name);
            modalBody.appendChild(document.createElement('br'));
            appendMovieInfo(modalBody, "Total movies you've tracked: ", data.allRecords);
            appendMovieInfo(modalBody, "Total movies you've tagged as Must Watch: ", data.mwRecords);
            appendMovieInfo(modalBody, "Total movies you've tagged as Already Watch: ", data.awRecords);
        },
        error: function (XMLHttpRequest) {
          showToast('errorToast', XMLHttpRequest.responseText);
        }
     });
}


function search() {
    var inputText = document.getElementById('searchInput').value; 
    var filteredMovies = movies.filter(m => m.originalTitle.includes(inputText));
    var span = document.getElementById('searchedMoviesCount');
    span.textContent = filteredMovies.length + 'records found';
    // console.log(filteredMovies.length);
}
function searchClick() {
    var input = document.getElementById('searchInput'); 
    input.addEventListener('keyup', search);
}

function submitSearchOnclick() {
    
    var inputText = document.getElementById('searchInput').value;
    var filteredMovies = movies.filter(m => m.originalTitle.includes(inputText));
    movies = filteredMovies;
    Paging();
    localStorage.setItem("currentPage", 1);
    displayCurrentPage();
    // console.log('s');
    
}

// function displayFilteredMovies() {
//     var main = document.querySelector('main')
//     removeAllChildNodes(main);
//     currentPage = getCurrentPage();
//     for (i = (+currentPage - 1) * 20; i < (+currentPage) * 20; ++i) {
//         if(!!movies[i]) {
//             var newDiv = document.createElement('div');
//             var newImg = document.createElement('img');
//             newImg.src = 'https://image.tmdb.org/t/p/w500' + movies[i].posterPath;
//             newImg.onclick = imageClickHandler(movies[i], i);
//             newDiv.appendChild(newImg);
//             main.appendChild(newDiv);
//         }
//     }
// }

if (!String.prototype.includes) {
    String.prototype.includes = function(search, start) {
      'use strict';
      if (typeof start !== 'number') {
        start = 0;
      }
  
      if (start + search.length > this.length) {
        return false;
      } else {
        return this.indexOf(search, start) !== -1;
      }
    };
  }