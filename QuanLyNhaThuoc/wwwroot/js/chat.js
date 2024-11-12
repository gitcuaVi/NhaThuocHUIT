document.addEventListener("DOMContentLoaded", function () {
    const chatBox = document.getElementById("chatBox");
    const chatContent = document.getElementById("chatContent");
    const userInput = document.getElementById("user-input");

    // Hàm để mở/đóng hộp chat
    window.openChat = function () {
        chatBox.style.display = (chatBox.style.display === "none" || chatBox.style.display === "") ? "block" : "none";
    };

    // Hàm gửi tin nhắn
    window.sendMessage = function () {
        const message = userInput.value.trim();
        if (message !== "") {
            addMessageToChat("Bạn", message, "user-message");  // Thêm tin nhắn của người dùng vào chat
            fetchAnswerFromServer(message);    // Gửi câu hỏi đến server và lấy câu trả lời
            userInput.value = "";              // Xóa nội dung input sau khi gửi
        }
    };

    // Hàm thêm tin nhắn vào chat
    function addMessageToChat(sender, message, className) {
        const p = document.createElement("p");
        p.textContent = `${sender}: ${message}`;
        p.classList.add(className);  // Thêm lớp CSS tương ứng (user-message hoặc bot-message)
        chatContent.appendChild(p);
        chatContent.scrollTop = chatContent.scrollHeight;  // Cuộn xuống dưới khi có tin nhắn mới
    }

    // Hàm gửi câu hỏi đến server và nhận câu trả lời
    async function fetchAnswerFromServer(question) {
        try {
            // Gửi câu hỏi đến server và nhận câu trả lời
            const response = await fetch("/KhachHang/Chat/GetFaqAnswer", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(question)  // Gửi câu hỏi đến controller
            });

            if (response.ok) {
                const data = await response.json();

                // Kiểm tra nếu có câu trả lời từ server
                if (data.success) {
                    addMessageToChat("Nhà Thuốc An Khang", data.responseMessage, "bot-message");
                } else {
                    addMessageToChat("Nhà Thuốc An Khang", "Không thể xử lý câu hỏi của bạn. Vui lòng thử lại sau.", "bot-message");
                }
            } else {
                console.error("Lỗi khi gọi API:", response.status);
                addMessageToChat("Nhà Thuốc An Khang", "Đã xảy ra lỗi khi gửi câu hỏi. Vui lòng thử lại.", "bot-message");
            }
        } catch (error) {
            console.error("Lỗi khi gửi câu hỏi:", error);
            addMessageToChat("Nhà Thuốc An Khang", "Đã xảy ra lỗi. Vui lòng thử lại.", "bot-message");
        }
    }

    // Xử lý sự kiện khi người dùng nhấn Enter để gửi tin nhắn
    userInput.addEventListener("keypress", function (event) {
        if (event.key === "Enter") {
            event.preventDefault();  // Ngăn chặn hành động mặc định (đưa con trỏ xuống dòng)
            sendMessage();           // Gửi tin nhắn
        }
    });
});
