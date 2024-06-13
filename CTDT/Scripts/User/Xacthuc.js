var id = document.getElementById('viewbagid').value;
$(document).ready(function () {
    var isKhoaSelected = false;
    var isCTDTSelected = false;
    var isLopSelected = false;
    var isSVSelected = false;

    $("#btnCheckMSSV").click(function () {
        var mssv = $("#mssv").val();
        $.ajax({
            type: "POST",
            url: "/Home/GetSvIdByMssv",
            data: { mssv: mssv, surveyid: id },
            success: function (response) {
                if (response.success) {
                    var idsv = response.svId;
                    var idctdt = response.ctdt;
                    $.ajax({
                        type: "POST",
                        url: "/Home/SaveDataXacThucCTDT",
                        data: { ctdt: idctdt, sv: idsv , surveyid: id},
                        beforeSend: function () {
                            Swal.fire({
                                title: 'Đang tải bộ câu hỏi...',
                                allowOutsideClick: false,
                                onBeforeOpen: () => {
                                    Swal.showLoading();
                                }
                            });
                        },
                        success: function (response) {
                            if (response.success) {
                                Swal.fire({
                                    icon: 'success',
                                    title: 'Xác thực thành công!',
                                    width: 600,
                                    showConfirmButton: false,
                                    timer: 2000
                                }).then(function () {
                                    window.location.href = "/Survey/Survey?id=" + id;
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
                } else {
                    Swal.fire({
                        icon: 'error',
                        title: 'Oops...',
                        width: 600,
                        text: response.message
                    });
                }
            },
            error: function () {
                alert("Có lỗi xảy ra khi gửi dữ liệu lên server!");
            }
        });
    });

    $("#forget_mssv_button").click(function () {
        if (!isKhoaSelected) {
            Swal.fire({
                icon: 'error',
                title: 'Oops...',
                width: 600,
                text: 'Vui lòng chọn khoa trước khi xác thực!'
            });
            return;
        }
        else if (!isCTDTSelected) {
            Swal.fire({
                icon: 'error',
                title: 'Oops...',
                width: 600,
                text: 'Vui lòng chọn chọn chương trình đào tạo trước khi xác thực!'
            });
            return;
        }
        else if (!isLopSelected) {
            Swal.fire({
                icon: 'error',
                title: 'Oops...',
                width: 600,
                text: 'Vui lòng chọn chọn lớp trước khi xác thực!'
            });
            return;
        }
        else if (!isSVSelected) {
            Swal.fire({
                icon: 'error',
                title: 'Oops...',
                width: 600,
                text: 'Vui lòng chọn chọn tên của bạn trước khi xác thực!'
            });
            return;
        }
        var ctdt = $("#select2-ctdt").val();
        var sv = $("#select2-sv").val();
        $.ajax({
            type: "POST",
            url: "/Home/SaveDataXacThucCTDT",
            data: { ctdt: ctdt, sv: sv, surveyid : id},
            beforeSend: function () {
                Swal.fire({
                    title: 'Đang tải bộ câu hỏi...',
                    allowOutsideClick: false,
                    onBeforeOpen: () => {
                        Swal.showLoading();
                    }
                });
            },
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

    $("#select2-khoa").change(function () {
        if ($(this).val() == "Chọn khoa của bạn") {
            isKhoaSelected = false;
            $("#select2-ctdt").empty();
            $("#select2-ctdt").append("<option selected>Chọn chương trình đào tạo của bạn</option>");
            return;
        }
        isKhoaSelected = true;
        $.get("/Home/LoadCTDTByKhoa", { khoaId: $(this).val() }, function (data) {
            $("#select2-ctdt").empty();
            $("#select2-ctdt").append("<option selected>Chọn chương trình đào tạo của bạn</option>");
            $.each(data, function (index, row) {
                $("#select2-ctdt").append("<option value='" + row.id_ctdt + "'>" + row.ten_ctdt + "</option>");
            });
        });
    });

    $("#select2-ctdt").change(function () {
        if ($(this).val() == "Chọn chương trình đào tạo của bạn") {
            isCTDTSelected = false;
            $("#select2-lop").empty();
            $("#select2-lop").append("<option selected>Chọn lớp của bạn</option>");
            return;
        }
        isCTDTSelected = true;
        $.get("/Home/LoadLopByCTDT", { CTDTID: $(this).val() }, function (data) {
            $("#select2-lop").empty();
            $("#select2-lop").append("<option selected>Chọn lớp của bạn</option>");
            $.each(data, function (index, row) {
                $("#select2-lop").append("<option value='" + row.id_lop + "'>" + row.ma_lop + "</option>");
            });
        });
    });

    $("#select2-lop").change(function () {
        if ($(this).val() == "Chọn lớp của bạn") {
            isLopSelected = false;
            $("#select2-sv").empty();
            $("#select2-sv").append("<option selected>Chọn tên của bạn</option>");
            return;
        }
        isLopSelected = true;
        $.get("/Home/LoadSVByLop", { LopID: $(this).val() }, function (data) {
            $("#select2-sv").empty();
            $("#select2-sv").append("<option selected>Chọn tên của bạn</option>");
            $.each(data, function (index, row) {
                var ngaySinh = new Date(parseInt(row.ngaysinh.substr(6)));
                var formattedNgaySinh = ngaySinh.toLocaleDateString("en-US");

                $("#select2-sv").append("<option value='" + row.id_sv + "'>" + row.hovaten + " - " + row.ma_sv + " - " + formattedNgaySinh + "</option>");
            });
        });
    });

    $("#select2-sv").change(function () {
        if ($(this).val() == "Chọn tên của bạn") {
            isSVSelected = false;
            $("#select2-sv").empty();
            $("#select2-sv").append("<option selected>Chọn tên của bạn</option>");
            return;
        }
        else {
            isSVSelected = true;
        }
    });
});

function activaTab(tabID) {
    $('.nav-tabs a[href="#' + tabID + '"]').tab('show');
}
