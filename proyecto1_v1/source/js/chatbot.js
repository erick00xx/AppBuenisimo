document.addEventListener('DOMContentLoaded', function () {
    const chatBubble = document.getElementById('chatBubble');
    const chatWindow = document.getElementById('chatWindow');
    const chatClose = document.getElementById('chatClose');
    const chatInput = document.getElementById('chatInput');
    const chatSend = document.getElementById('chatSend');
    const chatBody = document.getElementById('chatBody');

    // Respuestas predefinidas para el bot
    const botResponses = {
        "hola": "¡Hola! ¿En qué puedo ayudarte?",
        "menu": "Puedes ver nuestra carta completa en la sección 'Carta' del menú lateral.",
        "horario": "Nuestro horario de atención es de lunes a domingo de 11:00 AM a 10:00 PM.",
        "reserva": "Para hacer una reserva, por favor proporciona la fecha, hora y número de personas.",
        "ubicacion": "Estamos ubicados en Av. Principal 123, Zona Centro.",
        "contacto": "Puedes contactarnos al teléfono (123) 456-7890 o por email a info@buenisimo.com",
        "default": "Lo siento, no entiendo tu consulta. ¿Podrías ser más específico?"
    };

    // Función para mostrar u ocultar el chat
    chatBubble.addEventListener('click', function () {
        chatWindow.classList.toggle('active');
    });

    // Cerrar el chat
    chatClose.addEventListener('click', function () {
        chatWindow.classList.remove('active');
    });

    // Función para enviar mensaje
    function sendMessage() {
        const message = chatInput.value.trim();
        if (message === '') return;

        // Añadir mensaje del usuario
        addMessage(message, 'user');
        chatInput.value = '';

        // Simular respuesta del bot después de un breve retraso
        setTimeout(function () {
            const botReply = getBotResponse(message.toLowerCase());
            addMessage(botReply, 'bot');
        }, 500);
    }

    // Enviar mensaje con el botón
    chatSend.addEventListener('click', sendMessage);

    // Enviar mensaje con Enter
    chatInput.addEventListener('keypress', function (e) {
        if (e.key === 'Enter') {
            sendMessage();
        }
    });

    // Función para añadir un mensaje al chat
    function addMessage(text, sender) {
        const messageDiv = document.createElement('div');
        messageDiv.classList.add('chat-message', sender);

        const now = new Date();
        const timeStr = now.getHours() + ':' + (now.getMinutes() < 10 ? '0' : '') + now.getMinutes();

        messageDiv.innerHTML = `
                <div class="message-content">${text}</div>
                <div class="message-time">${timeStr}</div>
            `;

        chatBody.appendChild(messageDiv);
        chatBody.scrollTop = chatBody.scrollHeight;
    }

    // Función para obtener respuestas del bot
    function getBotResponse(message) {
        // Buscar palabras clave en el mensaje
        for (const key in botResponses) {
            if (message.includes(key)) {
                return botResponses[key];
            }
        }

        // Respuestas específicas basadas en patrones comunes
        if (message.includes('pedido') || message.includes('orden')) {
            return "Puedes realizar o consultar tu pedido en la sección 'Pedidos' del menú lateral.";
        } else if (message.includes('gracias')) {
            return "¡De nada! Estoy para ayudarte.";
        } else if (message.includes('precio') || message.includes('costo')) {
            return "Los precios de nuestros productos están disponibles en la carta. ¿Buscas algo específico?";
        }

        // Respuesta por defecto
        return botResponses.default;
    }
});