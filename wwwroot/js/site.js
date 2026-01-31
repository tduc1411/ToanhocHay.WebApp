const chatMessages = document.getElementById('chat-messages');
const chatInput = document.querySelector('#chat-window input');
const sendBtn = document.querySelector('#chat-window button');

// Hàm gửi tin nhắn văn bản
async function sendMessage(text) {
    appendMessage(text, 'user');
    chatInput.value = '';

    const response = await fetch('/Chat/Send', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ text: text })
    });
    const data = await response.json();
    handleBotResponse(data.response);
}

// Hàm gửi Quick Reply (nút bấm)
async function sendQuickReply(replyText) {
    appendMessage(replyText, 'user');

    const response = await fetch('/Chat/QuickReply', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ reply: replyText })
    });
    const data = await response.json();
    handleBotResponse(data.response);
}

// Hàm xử lý dữ liệu trả về từ Bot
function handleBotResponse(res) {
    // 1. Hiển thị tin nhắn chính
    appendMessage(res.message, 'bot');

    // 2. Nếu có options (quick_reply), hiển thị các nút bấm
    if (res.options && res.options.length > 0) {
        const optionsContainer = document.createElement('div');
        optionsContainer.className = 'flex flex-wrap gap-2 mt-2 ml-10';
        res.options.forEach(opt => {
            const btn = document.createElement('button');
            btn.className = 'px-3 py-1.5 bg-blue-100 text-primary rounded-full text-xs font-bold hover:bg-primary hover:text-white transition-all border border-blue-200';
            btn.innerText = opt;
            btn.onclick = () => {
                optionsContainer.remove(); // Bấm xong thì xóa các lựa chọn cũ
                sendQuickReply(opt);
            };
            optionsContainer.appendChild(btn);
        });
        chatMessages.appendChild(optionsContainer);
    }

    // 3. Nếu là dạng Form, hiển thị thông báo yêu cầu (Bạn có thể mở rộng thêm UI Form ở đây)
    if (res.type === 'form') {
        console.log("Cần hiển thị Form cho các trường:", res.form_fields);
    }

    chatMessages.scrollTop = chatMessages.scrollHeight;
}

function appendMessage(text, sender) {
    const msgDiv = document.createElement('div');
    msgDiv.className = sender === 'user' ? 'flex justify-end' : 'flex gap-2';

    const contentClass = sender === 'user'
        ? 'bg-primary text-white p-3 rounded-2xl rounded-tr-none text-sm shadow-sm max-w-[80%]'
        : 'bg-white text-gray-700 p-3 rounded-2xl rounded-tl-none shadow-sm text-sm max-w-[80%]';

    const avatarHtml = sender === 'bot'
        ? `<div class="w-8 h-8 rounded-full bg-blue-100 flex items-center justify-center text-primary text-xs flex-shrink-0"><i class="fa-solid fa-robot"></i></div>`
        : '';

    msgDiv.innerHTML = `${avatarHtml}<div class="${contentClass}">${text}</div>`;
    chatMessages.appendChild(msgDiv);
    chatMessages.scrollTop = chatMessages.scrollHeight;
}

// Bắt sự kiện nút gửi
sendBtn.onclick = () => {
    if (chatInput.value.trim()) sendMessage(chatInput.value);
};

// Bắt sự kiện phím Enter
chatInput.onkeypress = (e) => {
    if (e.key === 'Enter' && chatInput.value.trim()) sendMessage(chatInput.value);
};