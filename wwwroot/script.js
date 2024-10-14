let socket;

document.addEventListener("DOMContentLoaded", function () {
  initializeWebSocket();
});

function initializeWebSocket() {
  socket = new WebSocket("ws://localhost:5000/ws");

  socket.onopen = function () {
    console.log("Соединение установлено");
  };

  socket.onmessage = function (event) {
    let response = JSON.parse(event.data);
    if (response.action === "generateKey") {
      document.getElementById("key").value = response.response;
    }
    else if (response.action === "hack") {
      document.getElementById("key").value = response.response;
    }
    else {
      document.getElementById("output").textContent = response.response;
    }
  };

  socket.onclose = function () {
    console.log("Соединение закрыто, повторное подключение...");
    setTimeout(initializeWebSocket, 100);
  };

  socket.onerror = function (error) {
    console.error("Ошибка WebSocket:", error);
  };
}

function sendMessage(message) {
  if (socket.readyState === WebSocket.OPEN) {
    console.log("Sending message:", JSON.stringify(message));
    socket.send(JSON.stringify(message));
  } else {
    console.error("WebSocket не в состоянии OPEN. Текущее состояние:", socket.readyState);
  }
}

function encrypt() {
  let text = document.getElementById("inputText").value;
  let key = document.getElementById("key").value;
  if (key === "") {
    generateKey();
  }

  let message = { action: "encrypt", text: text, key: key };
  sendMessage(message);
}

function decrypt() {
  let text = document.getElementById("inputText").value;
  let key = document.getElementById("key").value;
  let message = { action: "decrypt", text: text, key: key };
  sendMessage(message);
}

function hack() {
  let text = document.getElementById("inputText").value;
  text = text.toLowerCase();
  let message = { action: "hack", text: text };
  sendMessage(message);

  // Показать div с id swap
  document.getElementById("swap").style.display = "flex";
}

function generateKey() {
  let message = { action: "generateKey" };
  sendMessage(message);
}

function swapLetters() {
  var text = document.getElementById("key").value;
  var letter1 = document.getElementById("letter1").value;
  var letter2 = document.getElementById("letter2").value;

  if (letter1.length !== 1 || letter2.length !== 1) {
    alert("Please enter exactly one character for each letter.");
    return;
  }

  var tempChar = '\u0000';

  text = text.split(letter1).join(tempChar);
  text = text.split(letter2).join(letter1);
  text = text.split(tempChar).join(letter2);

  document.getElementById("key").value = text;

  document.getElementById("letter1").value = "";
  document.getElementById("letter2").value = "";

  encrypt();
}