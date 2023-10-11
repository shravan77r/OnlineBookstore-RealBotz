
function getQueryParam(name) {
    const urlParams = new URLSearchParams(window.location.search);
    return urlParams.get(name);
}

function fnGetBookDetails() {
    const BookId = getQueryParam('BookId');
    $(".loader-container").show();
    $.ajax({
        type: "POST",
        url: "/Home/GetBookDetails",
        dataType: "json",
        data: { BookId: BookId },
        success: function (data) {
            let html = '';
            if (data.status == 0) {
                toastr.error(data.message, data.messageType);
                html = `<div class="col-md-12 mb-4" style="text-align:center">
                            <div class="alert alert-info mt-3">
                                No details found.
                            </div>
                        </div>`;
            }
            else {
                var item = data.data;

                html += `<div class="col-md-4">
                        <img src="images/${item.image}" style="width:100%" alt="Book Image" class="${item.title}">
                    </div>
                    <div class="col-md-8">
                        <h2 id="Title">${item.title}</h2>
                        <p>Author: <span id="spn-author">${item.author}</span></p>
                        <p>Description: <span id="spn-description">${item.description}</span></p>
                        <p>ISBN: <span id="spn-isbn">${item.isbn}</span></p>
                        <p>Price: <span id="spn-price">${item.price}</span></p>
                        <button class="btn btn-primary add-to-cart" onclick="fnAddToCart(${item.id})">Add to Cart</button>
                    </div>`;

            }
            $("#book-details-Container").html(html);
            $(".loader-container").hide();
        },
        error: function (response) {
            $(".loader-container").hide();
            console.log(response);
        }
    });
}
fnGetBookDetails();

