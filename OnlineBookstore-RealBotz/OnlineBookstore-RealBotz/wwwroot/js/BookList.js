const pageSize = 4;
let currentPage = 1;

var sort_column = "";
var sortDirection = $("#sortbyauthor").val();

function fnLoadList() {
    var search = $("#search").val();
    $(".loader-container").show();
    $.ajax({
        type: "POST",
        url: "/Home/GetBookList",
        dataType: "json",
        data: { PageIndex: currentPage, PageSize: pageSize, SortCol: sort_column, SortDir: sortDirection, Keyword: search },
        success: function (data) {
            let html = '';
            if (data.status == 0) {
                toastr.error(data.message, data.messageType);
                html = `<div class="col-md-12 mb-4" style="text-align:center">
                            <div class="alert alert-info mt-3">
                                No data found.
                            </div>
                        </div>`;
            }
            else {

                data.data.forEach(item => {
                    html += `<div class="book-card">
                            <a href="bookdetail.html?BookId=${item.id}"><img src="images/${item.image}" class="card-img-top" alt="${item.title}"></a>
                                <a href="bookdetail.html?BookId=${item.id}"><h6 class="card-title">${item.title}</h6></a>
                                <p class="card-text">Author: ${item.author}</p>
                                <p class="card-text">ISBN: ${item.isbn}</p>
                                <p class="card-text">Price: ${item.price}</p>
                                <button class="btn btn-primary add-to-cart" onclick="fnAddToCart(${item.id})">Add to Cart</button>
                        </div>`;
                });
            }
            var totalRecords = data.count;
            var totalPages = Math.ceil(totalRecords / pageSize);
            fnPagination(totalPages);
            $("#book-list-Container").html(html);
            $(".loader-container").hide();
        },
        error: function (response) {
            $(".loader-container").hide();
            console.log(response);
        }
    });
}
fnLoadList();

function fnPagination(totalPages) {
    var pagination = $('#pagination');
    pagination.empty();

    if (totalPages <= 1) {
        return;
    }

    if (currentPage > 1) {
        pagination.append('<li class="page-item"><a class="page-link" href="#" data-page="' + (currentPage - 1) + '">Previous</a></li>');
    }

    for (var i = 1; i <= totalPages; i++) {
        pagination.append('<li class="page-item ' + (i === currentPage ? 'active' : '') + '"><a class="page-link" href="#" data-page="' + i + '">' + i + '</a></li>');
    }

    if (currentPage < totalPages) {
        pagination.append('<li class="page-item"><a class="page-link" href="#" data-page="' + (currentPage + 1) + '">Next</a></li>');
    }

    pagination.find('.page-link').on('click', function () {
        var newPage = parseInt($(this).data('page'));
        if (!isNaN(newPage) && newPage !== currentPage) {
            currentPage = newPage;
            fnLoadList();
        }
    });
}


$("#sort").change(function () {
    sortDirection = $("#sort").val();
    sort_column = "Price";
    fnLoadList();
});

$("#sortbyauthor").change(function () {
    sortDirection = $("#sortbyauthor").val();
    sort_column = "Author";
    fnLoadList();
});

