﻿var id = document.getElementById('viewbagid').value;
$(document).ready(function () {
    $.ajax({
        url: '/Survey/AnswerSurvey/' + id,
        type: 'GET',
        success: function (data) {
            renderSurveyData(data);
        },
        error: function () {
            alert("Đã có lỗi xảy ra. Vui lòng thử lại sau.");
        }
    });
});

function renderSurveyData(data) {
    var tableHtml = '<table class="table table-bordered table-hover">\n';
    tableHtml += '<thead>\n';
    tableHtml += '<tr>\n';
    tableHtml += '<th scope="col">#</th>\n';
    tableHtml += '<th scope="col">Câu hỏi</th>\n';
    tableHtml += '<th scope="col">Câu trả lời</th>\n';
    tableHtml += '</tr>\n';
    tableHtml += '</thead>\n';
    tableHtml += '<tbody>\n';

    var index = 1;
    var pages = data[0].pages;
    pages.forEach(function (page) {
        var elements = page.elements;
        elements.forEach(function (element) {
            tableHtml += '<tr>\n';
            tableHtml += '<th scope="row" class="text-center">' + index + '</th>\n';
            tableHtml += '<td>' + element.title + '</td>\n';

            if (element.type === "text" || element.type === "comment") {
                var textValue = element.response ? element.response.text : "Không có câu trả lời";
                tableHtml += '<td><b>' + textValue + '</b></td>\n';
            } else if (element.type === "radiogroup") {
                var radioValue = element.response ? element.response.text : "Không có câu trả lời";
                tableHtml += '<td><b>' + radioValue + '</b></td>\n';
            } else if (element.type === "checkbox") {
                var checkboxValues = element.response ? element.response.text : [];
                var otherValue = element.response ? element.response.other : null;
                var checkboxValueString = checkboxValues.join(', ');

                if (otherValue) {
                    checkboxValueString += ' ' + otherValue;
                }

                tableHtml += '<td><b>' + checkboxValueString + '</b></td>\n';
            } else {
                tableHtml += '<td>Không có câu trả lời</td>\n';
            }

            tableHtml += '</tr>\n';
            index++;
        });
    });

    tableHtml += '</tbody>\n';
    tableHtml += '</table>\n';

    $('#surveyContainer').html(tableHtml);
}