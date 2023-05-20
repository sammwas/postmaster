"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub")
    .build();
let btnSendChat = document.getElementById("btnSendChat");
btnSendChat.setAttribute("disabled", true);

var instanceId = document.getElementById("inpChatInstanceId").value;
connection.on("SendChatMessage_" + instanceId, function (insId, sender, senderName, receiver, message, time) {
    var li = document.createElement("li");
    document.getElementById("messagesList").appendChild(li);
    var liHtml = getListDiv(senderName, receiver, message, time);
    var currUser = document.getElementById("inpChatUser").value;
    var liCls = currUser == receiver ? "left" : "right";
    li.className = liCls;
    li.innerHTML = liHtml;
    var sidebar = document.getElementById("chat-side-" + insId);
    if (sidebar) {
        sidebar.style.display = "block"
    }
    document.getElementById("newChatBadge").style.display = "block";
});

connection.start().then(function () {
    btnSendChat.removeAttribute("disabled");
}).catch(function (err) {
    return console.error(err.toString());
});


if (btnSendChat) {
    btnSendChat.addEventListener("click", function (event) {
        btnSendChat.disabled = true;
        sendXHRrequest()
        event.preventDefault();
    });
}

document.getElementById("inpChatMsg").addEventListener("keyup", function (event) {
    event = event || window.event;
    if (event.keyCode == 13) {
        sendXHRrequest()
    }
    event.preventDefault();
});

function sendXHRrequest() {
    // var token = document.getElementsByName("__RequestVerificationToken")[0].value;
    var token = "";
    var message = document.getElementById("inpChatMsg").value;
    var receiver = document.getElementById("inpChatReceiver").value;
    var params = `Receiver=${receiver}&Message=${message}&__RequestVerificationToken=${token}`
    var xhr = new XMLHttpRequest();
    xhr.open('POST', `/Messages/InAppPost`);
    xhr.setRequestHeader("Content-type", "application/x-www-form-urlencoded")
    xhr.onload = function () {
        btnSendChat.disabled = false;
    };
    xhr.send(params);
    document.getElementById("inpChatMsg").value = ""
}

function getListDiv(sender, receiver, message, time) {
    var fChar = sender.charAt(0)
    var li = '<div class=""><span class="chat-img pull-left" style="display:none">'
        + '<span class="i-circle" > ' + fChar + '</span></span> '
        + '<div class="chat-body clearfix">'
        + '<div class="header">'
        + '<strong class="primary-font">' + sender + '</strong>'
        + '<small class="pull-right text-muted"> &nbsp;<span class="fa fa-clock"></span> ' + time + '</small> </div>'
        + '<p>' + message + '</p></div></div>';
    return li;
}