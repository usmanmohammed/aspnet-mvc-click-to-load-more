// Write your JavaScript code.

$('#loading-animation').hide();
var page = 0;
var inCallback = false;

function loadMore(url) {
    if (page > -1 && !inCallback) {
        page++;
        inCallback = true;
        $('#loading-animation').show();
        $('#load-more').hide();
        $.ajax({
            type: 'GET',
            url: url,
            data: "pageNumber=" + page,
            success: function (data, textstatus) {
                if (data !== '') {
                    $("ul.infinite-load").append(data);
                }
                else {
                    page = -1;
                }
                inCallback = false;
                $('#loading-animation').hide();
                $('#load-more').show();
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                $('#load-more').hide();
                $('#loading-animation').hide();
            }
        });
    }
}