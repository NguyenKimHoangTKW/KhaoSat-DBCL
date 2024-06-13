var isAuthenticated = document.getElementById('isAuthenticated').value;
var viewbag = document.getElementById('viewbagid').value;
$(document).ready(function () {
    LoadData();
});

function LoadData() {
    $.ajax({
        url: '/Home/LoadPhieuKhaoSat?id=' + viewbag,
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
                    html += "<img src='https://tdmu.edu.vn/hinh/Icon.png' class='feature-icon' alt='Icon' style='width: 50px;'>";
                    html += "</a>";
                    html += "<h5 class='card-title'>" + items[i].TenPKS + "</h5>";
                    html += "<p class='card-text'>" + truncatedText + "</p>";
                    if (items[i].TenLoaiKhaoSat == "DOANH NGHIỆP") {
                        html += "<button class='btn btn-primary' onclick='window.location=\"/Home/XacThucCTDT?id=" + items[i].MaPhieu + "\"'>Thực hiện khảo sát</button>";
                    } else if (items[i].TenLoaiKhaoSat == "CỰU SINH VIÊN") {
                        html += "<button class='btn btn-primary' onclick='window.location=\"/Home/XacThuc?id=" + items[i].MaPhieu + "\"'>Thực hiện khảo sát</button>";
                    }
                    html += "</div>";
                    html += "</div>";
                    html += "</div>";
                }
            }
            $('#showdata').html(html);
        }
    });
}