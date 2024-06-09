var isAuthenticated = document.getElementById('isAuthenticated').value === 'true';
var userType = document.getElementById('userType').value;
document.addEventListener('DOMContentLoaded', function () {
    LoadData(userType, isAuthenticated);
});

function LoadData(userType, isAuthenticated) {
    var xhr = new XMLHttpRequest();
    xhr.open('GET', '/Home/LoadHeDaoTao', true);

    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4) {
            if (xhr.status === 200) {
                var contentType = xhr.getResponseHeader("Content-Type");
                if (contentType && contentType.indexOf("application/json") !== -1) {
                    var res = JSON.parse(xhr.responseText);
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
                            html += "<div class='col-sm-6 mb-4'>";
                            html += "<div class='card feature-card'>";
                            html += "<div class='card-body'>";
                            html += "<a href='#'>";
                            html += "<img src='https://tdmu.edu.vn/hinh/Icon.png' class='feature-icon' alt='Icon' style='width: 50px;'>";
                            html += "</a>";
                            if (items[i].TenHDT == "ĐẠI HỌC") {
                                html += "<h5 class='card-title'>KHẢO SÁT CÁC BÊN LIÊN QUAN - <span style='color: red;'>DÀNH CHO HỆ ĐÀO TẠO " + items[i].TenHDT + "</span></h5>";
                            } else {
                                html += "<h5 class='card-title'>KHẢO SÁT CÁC BÊN LIÊN QUAN - <span style='color: blue;'>DÀNH CHO HỆ ĐÀO TẠO " + items[i].TenHDT + "</span></h5>";
                            }
                            var surveyUrl = '/Home/PhieuKhaoSat?id=' + items[i].MaHDT;
                            html += "<p class='card-text'>Khảo sát ý kiến, đánh giá, góp ý của Cán Bộ Viên Chức Giảng viên, Sinh viên, Cựu Sinh Viên, Doanh Nghiệp,...</p>";
                            html += "<button class='btn btn-primary' onclick='handleSurveyClick(\"" + surveyUrl + "\")'>Đi đến</button>";
                            html += "</div>";
                            html += "</div>";
                            html += "</div>";
                        }
                    }
                    if (userType === '3') {
                        // Dành cho CTDT
                        html += "<div class='col-sm-12 mb-4'>";
                        html += "<div class='card feature-card'>";
                        html += "<div class='card-body'>";
                        html += "<a href='#' class='feature-icon'>";
                        html += "<img src='https://tdmu.edu.vn/hinh/Icon.png' class='feature-icon' alt='Icon' style='width: 50px;'>";
                        html += "</a>";
                        html += "<h5 class='card-title'>KHẢO SÁT CÁC BÊN LIÊN QUAN - <span style='color: #FF00FF;'>DÀNH CHO CHƯƠNG TRÌNH ĐÀO TẠO</span></h5>";
                        html += "<p class='card-text'>Thống kê kết quả khảo sát ý kiến, đánh giá, góp ý của Cán Bộ Viên Chức Giảng viên, Sinh viên, Cựu Sinh Viên, Doanh Nghiệp theo Chương trình đào tạo ...</p>";
                        html += "<button class='btn btn-primary' onclick='window.location.href=\"/CTDT/ThongKeKhaoSat/TKSVCKS\"'>Đi đến</button>";
                        html += "</div>";
                        html += "</div>";
                        html += "</div>";
                    } else if (userType === '2') {
                        // Dành cho admin
                        html += "<div class='col-sm-12 mb-4'>";
                        html += "<div class='card feature-card'>";
                        html += "<div class='card-body'>";
                        html += "<a href='#' class='feature-icon'>";
                        html += "<img src='https://tdmu.edu.vn/hinh/Icon.png' class='feature-icon' alt='Icon' style='width: 50px;'>";
                        html += "</a>";
                        html += "<h5 class='card-title'>KHẢO SÁT CÁC BÊN LIÊN QUAN - <span style='color: #009900;'>DÀNH CHO QUẢN TRỊ VIÊN</span></h5>";
                        html += "<p class='card-text'>Thống kê, quản lý kết quả khảo sát ý kiến, đánh giá, góp ý của Cán Bộ Viên Chức Giảng viên, Sinh viên, Cựu Sinh Viên, Doanh Nghiệp thực hiện khảo sát...</p>";
                        html += "<button class='btn btn-primary' onclick='window.location.href=\"/Admin/PhieuKhaoSat/Index\"'>Đi đến</button>";
                        html += "</div>";
                        html += "</div>";
                        html += "</div>";
                    }
                    document.getElementById('showdata').innerHTML = html;
                } else {
                    console.error('Unexpected content type: ' + contentType);
                }
            } else {
                console.error('Error loading data: ' + xhr.status);
            }
        }
    };

    xhr.onerror = function () {
        console.error('Request error...');
    };

    xhr.send();
}

function handleSurveyClick(url) {
    if (isAuthenticated) {
        window.location.href = url;
    } else {
        Swal.fire({
            icon: 'warning',
            title: 'Bạn chưa đăng nhập',
            text: 'Bạn vui lòng đăng nhập để xác thực',
            confirmButtonText: 'Đăng nhập'
        }).then((result) => {
            if (result.isConfirmed) {
                loginWithGoogle();
            }
        });
    }
}
