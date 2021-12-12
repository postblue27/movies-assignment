userStatuses = [];

window.onload = function () {
    var path = window.location.pathname;
    var page = path.split("/").pop();
    console.log(page);
    if(page == 'my-aw-list.html') {
        getMoviesStatus('AW')
    }
    if(page == 'my-mw-list.html') {
        getMoviesStatus('MW')
    }
    
    if(!getCurrentPage()) {
        localStorage.setItem("currentWatchListPage", 1)
    }
    // getMoviesStatus('AW')
}

window.on

function getMoviesStatus(status) {
    var url = "http://localhost:5000/api/main/get-movies-by-status";
    let data = {
        userNumber: localStorage.getItem("userNumber"),
        status: status
    }
    jQuery.ajax({
        type: "POST",
        url: url,
        dataType: "json",
        data: data,
        success: function (data) {
            console.log(data);
            userStatuses = data;

            let currentPage = getCurrentPage();
            console.log(currentPage);
            pagesCount = Math.ceil(userStatuses.length / 6);

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
    for ( let i = 1;i <= pagesCount;i++)
    {
        let pageitem = document.createElement('div')
        pageitem.innerHTML = i;
        pageitem.classList.add('page_item');
        if(localStorage.getItem("currentWatchListPage") == i) {
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
    localStorage.setItem("currentWatchListPage", pageNumber);
}

function getCurrentPage() {
    return localStorage.getItem("currentWatchListPage");
}

function displayCurrentPage() {
    var main = document.querySelector('main')
    removeAllChildNodes(main);
    currentPage = getCurrentPage();
    for (i = (+currentPage - 1) * 6; i < (+currentPage) * 6; ++i) {
        var newDiv = document.createElement('div');
        var newImg = document.createElement('img');
        if(!!userStatuses[i]) {
            newImg.src = 'https://image.tmdb.org/t/p/w500' + userStatuses[i].posterPath;
            newImg.style.width='40%';
            newDiv.appendChild(newImg);
            newDiv.style.color = 'white';
            newDiv.style.border = '2px solid #f63c12';
            newDiv.style.margin = '4px';
            newDiv.style.width = '45%';

            let movieTitle = document.createElement('h4');
            movieTitle.textContent = userStatuses[i].originalTitle;
            // movieTitle.classList.add('movie-detail-poster');
            newDiv.appendChild(movieTitle);

            let movieOverview = document.createElement('p');
            movieOverview.textContent = userStatuses[i].overview;
            // movieOverview.classList.add('movie-detail-poster');
            newDiv.appendChild(movieOverview);

            main.appendChild(newDiv);
        }
    }
}
