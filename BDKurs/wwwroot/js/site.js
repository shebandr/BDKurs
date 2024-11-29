// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.



window.addEventListener('load', function () {
    document.getElementById("buttonSendForm").addEventListener('click', function (event) {
        event.preventDefault(); 
        var boolVar = false

        for (var i = 1; i < headers.length; i++) {
            if (headers[i] == "MemberID" || headers[i] == "TrainerID" || headers[i] == "TrainingID") {
                boolVar = validateFormKey(headers[i])

            } else {
                boolVar = validateFormDefault(headers[i])
            }
            if (headers[i] == "Day") {
                boolVar = CheckDay(headers[i])
            }
            if (boolVar == false) {
                break
            }
        }


        if (boolVar) {
            document.getElementById('SendData').submit();
        }
    })
})




window.addEventListener('load', function () {
    document.getElementsByName("QueryButton")[2].addEventListener('click', function (event) {
        event.preventDefault();
        var boolVar = false

        boolVar = CheckDay2("DayInput")


        if (boolVar) {
            console.log("сабмит")
            document.getElementsByName("QueryButton")[2].removeEventListener('click', arguments.callee);
            document.getElementsByName("QueryButton")[2].click();
        }
    })
})


function validateFormDefault(currentForm) {

    var form = document.getElementById(currentForm);
    var formError = document.getElementById(currentForm + " Error")
    console.log(form)
    if (currentForm == "BirthDate") {
        var inputDate = new Date(form.value);
        var currentDate = new Date();
        if (inputDate > currentDate) {
            formError.innerText = "Дата должна быть в прошлом"
            return false
        } else {
            formError.innerText = ""
            return true
        } 
    }


    if (form.value.trim() === '') {
        formError.innerText = "Необходимы значащие символы"
        return false;
    } else {
        formError.innerText = ""; 
    }

    return true;
}
function validateFormKey(currentForm) {
    var form = document.getElementById(currentForm);
    var formError = document.getElementById(currentForm + " Error")
    var boolCheck
    switch (currentForm) {
        case "MemberID":
            for (var i = 1; i < MembersTable.length; i++) {
                if (form.value == MembersTable[i][0]) {
                    boolCheck = true
                    break
                }
            }
            
            break
        case "TrainerID":
            for (var i = 1; i < TrainersTable.length; i++) {
                if (form.value == TrainersTable[i][0]) {
                    boolCheck = true
                    break
                }
            }
            break
        case "TrainingID":
            for (var i = 1; i < TrainingsTable.length; i++) {
                if (form.value == TrainingsTable[i][0]) {
                    boolCheck = true
                    break
                }
            }
            break
    }
    // заготовка на случай, если на паре придется делать запрет на вставку уникальных значений
/*    if (CheckCopy) {
        return false
    }*/

    if (!boolCheck) {
        formError.innerText = "Такой идентификатор не существует"
        return false;
    } else {
        formError.innerText = "";
    }

    return true;
}

function CheckDay(currentForm) {
    var form = document.getElementById(currentForm);
    
    var formError = document.getElementById(currentForm + " Error")
    console.log(currentForm + " Error")
    if (form.value != "Понедельник" && form.value != "Вторник" && form.value != "Среда" && form.value != "Четверг" && form.value != "Пятница" && form.value != "Суббота" && form.value != "Воскресенье") {
        formError.innerText = "Такой день недели не существует или необходимо писать с большой буквы"
        return false;
    } else {
        formError.innerText = ""; 
        return true
    }
}

function CheckDay2(currentForm) {
    var form = document.getElementById(currentForm);

    var formError = document.getElementById(currentForm + "Error")
    console.log(currentForm + "Error")
    if (form.value != "Понедельник" && form.value != "Вторник" && form.value != "Среда" && form.value != "Четверг" && form.value != "Пятница" && form.value != "Суббота" && form.value != "Воскресенье") {
        formError.innerText = "Такой день недели не существует или необходимо писать с большой буквы"
        return false;
    } else {
        formError.innerText = "";
        return true
    }
}

function CheckCopy(headers) {
    globalError = document.getElementById("GlobalError")
    boolFlag = false
    switch (headers[0]) {
        case "MemberID":
            var fn = document.getElementById("FirstName").value
            var sn = document.getElementById("LastName").value
            var d = document.getElementById("Date").value

            for (i = 1; i < MembersTable.length; i++) {
                if (fn == MembersTable[i][1] && sn == MembersTable[i][2] && d == MembersTable[i][3]) {
                    boolFlag = true
                    break
                }
            }
            console.log(headers[0])
            break
        case "TrainerID":

            console.log(headers[0])
            break
        case "TrainingID":

            console.log(headers[0])
            break
        case "MemberTrainingID":

            console.log(headers[0])
            break
    }
    if (boolFlag) {
        globalError.innerText = "Такая запись уже существует"
        return false
    } else {
        globalError.innerText = ""
        return true
    }
}