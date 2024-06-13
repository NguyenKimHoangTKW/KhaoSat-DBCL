var id = document.getElementById('viewbagid').value;
$(document).ready(function () {
    var isKhoaSelected = false;

    $("#btnXacThucCTDT").click(function () {
        if (!isKhoaSelected) {
            Swal.fire({
                icon: 'error',
                title: 'Oops...',
                text: 'Vui lòng chọn khoa trước khi xác thực!'
            });
            return;
        }

        var ctdt = $("#select2-xacthucctdt-ctdt").val();
        $.ajax({
            type: "POST",
            url: "/Home/SaveDataXacThucCTDTWithoutSV",
            data: { ctdt: ctdt, surveyid: id },
            success: function (response) {
                if (response.success) {
                    Swal.fire({
                        icon: 'success',
                        title: 'Xác thực thành công!',
                        showConfirmButton: false,
                        timer: 2000
                    }).then(function () {
                        window.location.href = "/Survey/Survey?id=" + id;
                    });
                } else {
                    Swal.fire({
                        icon: 'error',
                        title: 'Oops...',
                        text: response.message
                    });
                }
            },
            error: function () {
                Swal.fire({
                    icon: 'error',
                    title: 'Oops...',
                    text: 'Có lỗi xảy ra khi gửi dữ liệu lên server!'
                });
            }
        });
    });

    $("#select2-xacthucctdt-khoa").change(function () {
        if ($(this).val() == "Chọn khoa") {
            isKhoaSelected = false;
            $("#select2-xacthucctdt-ctdt").empty();
            $("#select2-xacthucctdt-ctdt").append("<option selected>Chọn chương trình đào tạo</option>");
            return;
        }
        isKhoaSelected = true;
        $.get("/Home/LoadCTDTByKhoa", { khoaId: $(this).val() }, function (data) {
            $("#select2-xacthucctdt-ctdt").empty();
            $.each(data, function (index, row) {
                $("#select2-xacthucctdt-ctdt").append("<option value='" + row.id_ctdt + "'>" + row.ten_ctdt + "</option>");
            });
        });
    });
});

function activaTab(tabID) {
    $('.nav-tabs a[href="#' + tabID + '"]').tab('show');
}