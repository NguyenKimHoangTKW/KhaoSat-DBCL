var ids = document.getElementById('surveyIds').value;
$(document).ready(function () {
    LoadData();
});

function LoadData() {
    $.ajax({
        url: '/Survey/LoadSurveyForm?ids=' + ids,
        type: 'GET',
        success: function (res) {
            var items = res.data;
            var html = "";

            if (items.length === 0) {
                html += "<div class='col-lg-12 survey-item'>";
                html += "<div class='card survey-card border-0'>";
                html += "<div class='card-body'>";
                html += "<h5 class='card-title text-center'>Không có dữ liệu.</h5>";
                html += "</div>";
                html += "</div>";
                html += "</div>";
            } else {
                for (let i = 0; i < items.length; i++) {
                    var maxChars = 400;
                    var truncatedText = items[i].MoTaPhieu.length > maxChars ? items[i].MoTaPhieu.substring(0, maxChars) + '...' : items[i].MoTaPhieu;
                    html += "<div class='col-sm-6 mb-4'>";
                    html += "<div class='card feature-card'>";
                    html += "<div class='card-body'>";
                    html += "<a href='#' class='feature-icon'>";
                    html += "<i class='fa fa-check-square-o' aria-hidden='true'></i>";
                    html += "</a>";
                    html += "<h5 class='card-title'>" + items[i].TieuDePhieu + "</h5>";
                    html += "<p class='card-text'>" + truncatedText + "</p>";
                    html += "<button onclick=\"window.location.href='/Survey/ListAnswerSurvey?id=" + items[i].MaPhieu + "'\" class='btn btn-primary'>Xem chi tiết</button>";
                    html += "</div>";
                    html += "</div>";
                    html += "</div>";
                }
            }
            $('#showdata').html(html);
        },
        error: function (err) {
            console.error(err);
            alert("Đã có lỗi xảy ra. Vui lòng thử lại sau.");
        }
    });
}