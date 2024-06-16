var id = document.getElementById('viewbagid').value;
var user = document.getElementById('userid').value;
$(document).ready(function () {
    LoadData();
});

function LoadData() {
    $.ajax({
        url: '/Survey/LoadListAnswerSurvey?id=' + id + '&iduser=' + user,
        type: 'GET',
        success: function (res) {
            var items = res.data;
            var html = "";

            if (items.length === 0) {
                html += "<tr><td colspan='6' class='text-center'>Không có dữ liệu.</td></tr>";
            } else {
                for (let i = 0; i < items.length; i++) {
                    var index = i + 1;
                    var formattedDate1 = unixTimestampToDate(items[i].ThoiGianKhaoSat);
                    if (items[i].MSSV !== null && items[i].TenCTDT !== null) {
                        html += "<tr>";
                        html += "    <td>" + index + "</td>";
                        html += "    <td>";
                        html += "        <div class='d-flex align-items-center'>";
                        html += "            <div class='ms-3'>";
                        html += "                <p class='fw-bold mb-1'>" + items[i].TenCTDT + "</p>";
                        html += "            </div>";
                        html += "        </div>";
                        html += "    </td>";
                        html += "    <td>";
                        html += "        <p class='fw-normal mb-1'>" + items[i].TenSV + "</p>";
                        html += "    </td>";
                        html += "    <td>";
                        html += "        <span class='badge badge-success rounded-pill d-inline'>" + items[i].MSSV + "</span>";
                        html += "    </td>";
                        html += "    <td>" + formattedDate1 + "</td>";
                        html += "    <td>";
                        html += "        <button onclick=\"window.location.href='/Survey/AnswerPKS?id=" + items[i].MaPhieu + "'\" type='button' class='btn btn-link btn-sm btn-rounded'>";
                        html += "            Xem chi tiết câu hỏi";
                        html += "        </button>";
                        html += "    </td>";
                        html += "</tr>";
                    } else if (items[i].MSSV === null && items[i].TenCTDT !== null) {
                        html += "<tr>";
                        html += "    <td>" + index + "</td>";
                        html += "    <td>";
                        html += "        <div class='d-flex align-items-center'>";
                        html += "            <div class='ms-3'>";
                        html += "                <p class='fw-bold mb-1'>" + items[i].TenCTDT + "</p>";
                        html += "            </div>";
                        html += "        </div>";
                        html += "    </td>";
                        html += "    <td>";
                        html += "        <p class='fw-normal mb-1'>" + items[i].Khoa + "</p>";
                        html += "    </td>";
                        html += "    <td>" + formattedDate1 + "</td>";
                        html += "    <td>";
                        html += "        <button onclick=\"window.location.href='/Survey/AnswerPKS?id=" + items[i].MaPhieu + "'\" type='button' class='btn btn-link btn-sm btn-rounded'>";
                        html += "            Xem chi tiết câu hỏi";
                        html += "        </button>";
                        html += "    </td>";
                        html += "</tr>";
                    } else if (items[i].CBVC !== null && items[i].Donvi !== null) {
                        html += "<tr>";
                        html += "    <td>" + index + "</td>";
                        html += "    <td>";
                        html += "        <div class='d-flex align-items-center'>";
                        html += "            <div class='ms-3'>";
                        html += "                <p class='fw-bold mb-1'>" + items[i].CBVC + "</p>";
                        html += "            </div>";
                        html += "        </div>";
                        html += "    </td>";
                        html += "    <td>";
                        html += "        <p class='fw-normal mb-1'>" + items[i].Donvi + "</p>";
                        html += "    </td>";
                        html += "    <td>" + formattedDate1 + "</td>";
                        html += "    <td>";
                        html += "        <button onclick=\"window.location.href='/Survey/AnswerPKS?id=" + items[i].MaPhieu + "'\" type='button' class='btn btn-link btn-sm btn-rounded'>";
                        html += "            Xem chi tiết câu hỏi";
                        html += "        </button>";
                        html += "    </td>";
                        html += "</tr>";
                    } else {
                        html += "<h2>Không có dữ liệu</h2>";
                    }
                    
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

function unixTimestampToDate(unixTimestamp) {
    var date = new Date(unixTimestamp * 1000);

    var weekdays = ['Chủ Nhật', 'Thứ 2', 'Thứ 3', 'Thứ 4', 'Thứ 5', 'Thứ 6', 'Thứ 7'];

    var dayOfWeek = weekdays[date.getDay()];

    var month = ("0" + (date.getMonth() + 1)).slice(-2);
    var day = ("0" + date.getDate()).slice(-2);
    var year = date.getFullYear();
    var hours = ("0" + date.getHours()).slice(-2);
    var minutes = ("0" + date.getMinutes()).slice(-2);
    var seconds = ("0" + date.getSeconds()).slice(-2);
    var formattedDate = dayOfWeek + ', ' + day + "-" + month + "-" + year + " " + hours + ":" + minutes + ":" + seconds;
    return formattedDate;
}