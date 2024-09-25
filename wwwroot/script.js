let socket;

document.addEventListener("DOMContentLoaded", function () {

  socket = new WebSocket("ws://localhost:5000/ws");
  
  socket.onopen = function () {
    console.log("Соединение установлено");
  };

});

function encrypt() {
  let text = document.getElementById("inputText").value;
  let key = document.getElementById("key").value;
  let message = { action: "encrypt", text: text, key: key };
  console.log("Sending message:", JSON.stringify(message));
  socket.send(JSON.stringify(message));

  socket.onmessage = function (event) {
    document.getElementById("output").textContent = event.data;
  };
}

function decrypt() {
  let text = document.getElementById("inputText").value;
  let key = document.getElementById("key").value;
  let message = { action: "decrypt", text: text, key: key };
  console.log("Sending message:", JSON.stringify(message));
  socket.send(JSON.stringify(message));

  socket.onmessage = function (event) {
    document.getElementById("output").textContent = event.data;
  };
}

function hack() {
  let text = document.getElementById("inputText").value;
  text = text.toLowerCase();
  let message = { action: "hack", text: text };
  console.log("Sending message:", JSON.stringify(message));
  socket.send(JSON.stringify(message));

  socket.onmessage = function (event) {
    document.getElementById("output").textContent = event.data;
  };
}

function generateKey() {
  let message = { action: "generateKey" };
  console.log("Sending message:", JSON.stringify(message));
  socket.send(JSON.stringify(message));

  socket.onmessage = function (event) {
    document.getElementById("key").value = event.data;
  };
}