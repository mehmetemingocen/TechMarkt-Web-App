// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function startListening() {
        if (!('webkitSpeechRecognition' in window)) {
            alert("Tarayıcınız sesli yazmayı desteklemiyor.");
            return;
        }

        const recognition = new webkitSpeechRecognition();
        recognition.lang = "tr-TR"; // Türkçe için
        recognition.continuous = false;
        recognition.interimResults = false;

        recognition.onstart = function () {
            console.log("Dinleniyor...");
        };

        recognition.onresult = function (event) {
            const transcript = event.results[0][0].transcript;
            document.getElementById("searchInput").value = transcript;
        };

        recognition.onerror = function (event) {
            console.error("Hata:", event.error);
        };

        recognition.start();
    }

    