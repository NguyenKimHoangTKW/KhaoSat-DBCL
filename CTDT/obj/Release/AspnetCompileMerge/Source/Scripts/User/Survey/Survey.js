var surveyid = document.getElementById('idSurvey').value;
$(document).ready(function () {
    $('#submit_phieu').click(function () {
        saveFormData();
        checkVisibility();
    });

    $('.inputVal2, input[type="text"], input[type="number"], input[type="email"], textarea').blur(function () {
        if ($(this).hasClass('required-ques') && $(this).val().trim() === '') {
            $(this).closest('.item-ques').find('[id^="alert_"]').text("Trường này là bắt buộc.").show();
        } else {
            $(this).closest('.item-ques').find('[id^="alert_"]').hide();
        }
    });
});

var timeout;
function startTimer() {
    timeout = setTimeout(function () {
        Swal.fire({
            title: "Quá thời gian thực hiện khảo sát!",
            text: "Bạn sẽ được chuyển hướng về trang chủ.",
            icon: "warning",
            confirmButtonColor: "#3085d6",
            cancelButtonColor: "#d33",
            confirmButtonText: "Đồng ý"
        }).then((result) => {
            if (result.isConfirmed) {
                fetch('/Home/ClearSession', { method: 'POST' })
                    .then(() => {
                        window.location.href = '/Home/Index';
                    })
                    .catch(() => {
                        window.location.href = '/Home/Index';
                    });
            }
        });
    }, 3000000);
}

function resetTimer() {
    clearTimeout(timeout);
    startTimer();
}

document.addEventListener('DOMContentLoaded', (event) => {
    startTimer();
});

document.addEventListener('mousemove', resetTimer);
document.addEventListener('keypress', resetTimer);
document.addEventListener('click', resetTimer);

function saveFormData() {
    var formData = {
        pages: []
    };
    var valid = true;

    $('.item-ques[name^="pages_"]').each(function () {
        var pageName = $(this).attr('name');
        var pageTitle = $(this).find('.ques-header strong').text().trim();

        var pageData = {
            name: pageName.replace(/pages_/g, ''),
            title: pageTitle,
            elements: []
        };

        $(this).nextUntil('.item-ques[name^="pages_"]').each(function () {
            var questionName = $(this).attr('name');
            if (!questionName) return;

            var questionType = $(this).find('input[type="radio"]').length > 0 ? 'radiogroup' : $(this).find('input[type="checkbox"]').length > 0 ? 'checkbox' : $(this).find('textarea').length > 0 ? 'comment' : 'text';

            var questionShortName = questionName.replace(/questions_/g, '');
            var questionTitle = $(this).find('.ques-text strong').text().trim();

            var questionData = {
                type: questionType,
                name: questionShortName,
                title: questionTitle,
                response: {}
            };

            var isVisible = $(this).is(':visible');
            var isRequired = isVisible && $(this).find('.required-ques').length > 0;
            var alertElement = $(this).find('[id^="alert_"]');

            switch (questionType) {
                case 'radiogroup':
                    var selectedRadio = $(this).find('input[type="radio"]:checked');
                    var selectedValue = selectedRadio.val();
                    var otherInput = $(this).find('input[type="text"]').val();
                    var selectedText = selectedRadio.closest('label').text().trim();
                    if (isRequired && !selectedValue) {
                        valid = false;
                        alertElement.text("Trường " + questionTitle + " là bắt buộc.").show();
                    } else {
                        alertElement.hide();
                    }
                    questionData.response = {
                        "name": selectedValue,
                        "text": selectedText,
                        "other": otherInput ? otherInput : null
                    };
                    break;

                case 'checkbox':
                    var checkedValues = [];
                    var checkedTexts = [];
                    $(this).find('input[type="checkbox"]:checked').each(function () {
                        checkedValues.push($(this).val());
                        checkedTexts.push($(this).closest('label').text().trim());
                    });
                    var otherInput = $(this).find('input[type="text"]').val();
                    if (isRequired && checkedValues.length === 0) {
                        valid = false;
                        alertElement.text(questionTitle + " là bắt buộc.").show();
                    } else {
                        alertElement.hide();
                    }
                    questionData.response = {
                        "name": checkedValues,
                        "text": checkedTexts,
                        "other": otherInput ? otherInput : null
                    };
                    break;

                case 'comment':
                    var inputValue = $(this).find('textarea').val();
                    if (isRequired && !inputValue) {
                        valid = false;
                        alertElement.text(questionTitle + " là bắt buộc.").show();
                    } else {
                        alertElement.hide();
                    }
                    questionData.response = {
                        "text": inputValue
                    };
                    break;

                default:
                    var inputValue = $(this).find('input[type="text"], input[type="number"], input[type="email"]').val();
                    if (isRequired && !inputValue) {
                        valid = false;
                        alertElement.text(questionTitle + " là bắt buộc.").show();
                    } else {
                        alertElement.hide();
                    }
                    questionData.response = {
                        "text": inputValue
                    };
                    break;
            }

            pageData.elements.push(questionData);
        });

        formData.pages.push(pageData);
    });

    if (valid) {
        if (!$(this).data('saved')) {
            $.ajax({
                url: '/Survey/AddAnswer',
                type: 'POST',
                dataType: 'JSON',
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify({
                    surveyID: surveyid,
                    json_answer: JSON.stringify(formData),
                }),
                success: function (response) {
                    Swal.fire({
                        icon: 'success',
                        title: response.status,
                        showConfirmButton: false,
                        timer: 2000
                    }).then(() => {
                        window.location.href = '@Url.Action("Index", "Home")';
                    });
                },
                error: function (response) {
                    alert(response.status);
                },
            });
            $(this).data('saved', true);
        }
    } else {
        Swal.fire({
            icon: 'error',
            title: 'Vui lòng hoàn thành tất cả các trường bắt buộc',
            showConfirmButton: true
        });
    }
}

function checkVisibility() {
    $('.item-ques').each(function () {
        var visibleIf = $(this).data('visibleif');
        var isRequiredField = $(this).find('.inputVal2, input[type="text"], input[type="number"], input[type="email"], textarea').hasClass('required-ques');

        if (visibleIf) {
            var isVisible = false;
            var conditions = visibleIf.split(',');
            for (var i = 0; i < conditions.length; i++) {
                var condition = conditions[i].trim();
                if ($('input[name=' + condition.split('_')[0] + ']:checked').val() === condition) {
                    isVisible = true;
                    break;
                }
            }
            if (isVisible) {
                $(this).show();
                if (isRequiredField) {
                    $(this).find('.inputVal2, input[type="text"], input[type="number"], input[type="email"], textarea').addClass('required-ques');
                }
            } else {
                $(this).hide();
                if (isRequiredField) {
                    $(this).find('.inputVal2, input[type="text"], input[type="number"], input[type="email"], textarea').removeClass('required-ques');
                }
            }
        } else {
            $(this).show();
        }
    });
}

$('input[type=radio], input[type=checkbox]').change(checkVisibility);