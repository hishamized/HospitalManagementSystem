const apiBase = '/Chats';
let connection;
let currentChatRoomId = null;
let currentUserId = null;
let typingTimer;

$(function () {
    currentUserId = parseInt($('#current-user-id').val()); // Get current user ID from session
    initSignalR();
    loadUsers();
    loadRecentChats();
    setupSearch();
    setupSend();
    setupTypingDetection();
});

// ===================== SignalR =====================
function initSignalR() {
    connection = new signalR.HubConnectionBuilder()
        .withUrl('/chathub')
        .withAutomaticReconnect()
        .build();

    connection.on('UserOnline', userId => markUserOnline(userId));
    connection.on('UserOffline', userId => markUserOffline(userId));
    connection.on('ReceiveMessage', payload => onReceiveMessage(payload));
    connection.on('ChatCreated', chat => loadRecentChats());
    connection.on('MessageEdited', payload => onMessageEdited(payload));
    connection.on('MessageDeleted', payload => onMessageDeleted(payload));
    connection.on('UserTyping', (userId, isTyping) => updateTypingIndicator(userId, isTyping));

    connection.start()
        .then(() => console.log('Connected to ChatHub'))
        .catch(err => console.error('SignalR connection error:', err));
}

// ===================== Users =====================
function loadUsers(query) {
    $('#users-list').html('<div>Loading...</div>');
    $.get(apiBase + '/GetUsers', { query: query || '' })
        .done(users => renderUsers(users));
}

function renderUsers(users) {
    const container = $('#users-list').empty();

    if (!users || users.length === 0) {
        container.append('<div class="text-muted">No users</div>');
        return;
    }

    users.forEach(u => {
        if (u.id === currentUserId) return;
        const el = $(`
            <div class="user-item p-2 border-bottom d-flex justify-content-between align-items-center" data-userid="${u.id}">
                <div>
                    <div class="fw-bold">${escapeHtml(u.fullName)}</div>
                    <div class="small text-muted">
                        ${escapeHtml(u.email)} · ${escapeHtml(u.username)}
                    </div>
                     <small class="text-muted">
                         Last seen: ${u.lastSeenAt ? new Date(u.lastSeenAt).toLocaleString() : 'Never'}
                    </small>
                    <div class="d-flex align-items-center status-indicator" style="margin-top:2px;">
                        <span class="online-dot" style="display:none; width:8px; height:8px; border-radius:50%; background-color:#28a745; margin-right:5px;"></span>
                        <span class="online-text" style="display:none; font-size:0.75rem; color:#28a745;">Online</span>
                    </div>
                    <div class="last-seen small text-muted" style="font-size:0.7rem;"></div>
                    <div class="typing-indicator small text-muted" style="display:none;"></div>
                </div>
                <div>
                    <button class="btn btn-sm btn-outline-primary start-chat">Chat</button>
                </div>
            </div>
        `);

        const lastSeenEl = el.find('.last-seen');
        if (!u.IsOnline && u.LastSeenAt) {
            const lastSeen = new Date(u.LastSeenAt);
            lastSeenEl.text(`Last seen: ${lastSeen.toLocaleString()}`);
        } else {
            lastSeenEl.text('');
        }

        // Hook up the chat button click
        el.find('.start-chat').on('click', () => startChatWithUser(u));

        container.append(el);

        // Show online if user is online
        if (u.isOnline) markUserOnline(u.id);
    });
}



// ===================== Recent Chats =====================
function loadRecentChats() {
    $('#recent-chats').html('<div>Loading...</div>');
    $.get(apiBase + '/GetRecentChats')
        .done(data => {
            const container = $('#recent-chats').empty();
            if (!data || data.length === 0) {
                container.append('<div class="text-muted">No recent chats</div>');
                return;
            }

            data.forEach(c => {
                const el = $(`
                    <div class="recent-chat p-2 border-bottom d-flex justify-content-between align-items-center" data-chatid="${c.chatRoomId}">
                        <div>
                            <div class="fw-bold">${escapeHtml(c.title)}</div>
                            <div class="small text-muted">
                                ${escapeHtml(c.lastMessageSnippet)} 
                                ${c.unreadCount > 0 ? `<span class="badge bg-danger">${c.unreadCount}</span>` : ''}
                            </div>
                        </div>
                    </div>
                `);
                el.on('click', () => openChat(c.chatRoomId, c.title));
                container.append(el);
            });
        });
}

// ===================== Search =====================
function setupSearch() {
    let timer;
    $('#user-search').on('input', function () {
        clearTimeout(timer);
        const q = $(this).val();
        timer = setTimeout(() => loadUsers(q), 300);
    });

    $('#user-search').on('keypress', function (e) {
        if (e.key === 'Enter') {
            const q = $(this).val();
            if (isEmailOrUsername(q)) startChatByIdentifier(q);
            else loadUsers(q);
        }
    });
}

function isEmailOrUsername(q) {
    return q && (q.includes('@') || /^[a-zA-Z0-9_.-]+$/.test(q));
}

// ===================== Chat operations =====================
function startChatWithUser(user) {
    $.post(apiBase + '/StartChat', { userId: user.id })
        .done(chat => openChat(chat.chatRoomId, chat.title));
}

function startChatByIdentifier(identifier) {
    $.post(apiBase + '/StartChat', { identifier })
        .done(chat => openChat(chat.chatRoomId, chat.title))
        .fail(xhr => alert(xhr.responseText || 'User not found'));
}

function openChat(chatRoomId, title) {
    currentChatRoomId = chatRoomId;
    $('#chat-header').text(title);
    $('#message-input').prop('disabled', false);
    $('#send-btn').prop('disabled', false);
    $('#messages').html('<div>Loading messages...</div>');

    $('.typing-indicator').hide().text('');

    connection.invoke('JoinChat', chatRoomId).catch(err => console.error(err));

    $.get(apiBase + '/GetMessages', { chatRoomId })
        .done(messages => renderMessages(messages));
}

function renderMessages(messages) {
    const container = $('#messages').empty();
    messages.forEach(m => {
        const alignment = m.senderId === currentUserId ? 'message-right' : 'message-left';
        const el = $(`
            <div class="message ${alignment}" data-messageid="${m.id}">
                <div class="message-meta">${escapeHtml(m.senderUsername)} · ${new Date(m.sentAt).toLocaleString()}</div>
                <div>${escapeHtml(m.content)} ${m.isEdited ? '<small class="text-muted">(edited)</small>' : ''}</div>
            </div>
        `);
        container.append(el);
    });
    container.scrollTop(container.prop('scrollHeight'));
}

// ===================== Sending messages =====================
function setupSend() {
    $('#send-btn').on('click', async () => {
        const text = $('#message-input').val();
        if (!text || !currentChatRoomId) return;

        const payload = { chatRoomId: currentChatRoomId, content: text };

        try {
            await $.post(apiBase + '/SendMessage', payload);
            $('#message-input').val('');
        } catch (err) {
            console.error(err);
            alert('Failed to send message');
        }
    });
}

// ===================== Receive messages =====================
function onReceiveMessage(payload) {
    if (payload.chatRoomId === currentChatRoomId) {
        const alignment = payload.senderId === currentUserId ? 'message-right' : 'message-left';
        $('#messages').append(`
            <div class="message ${alignment}" data-messageid="${payload.id}">
                <div class="message-meta">${escapeHtml(payload.senderUsername)} · ${new Date(payload.sentAt).toLocaleString()}</div>
                <div>${escapeHtml(payload.content)}</div>
            </div>
        `);
        $('#messages').scrollTop($('#messages')[0].scrollHeight);
    }
    loadRecentChats();
}

// ===================== Message edit/delete =====================
function onMessageEdited(payload) {
    const el = $(`[data-messageid="${payload.id}"]`);
    if (el.length) el.find('div').last().html(`${escapeHtml(payload.content)}${payload.isEdited ? ' <small class="text-muted">(edited)</small>' : ''}`);
}

function onMessageDeleted(payload) {
    const el = $(`[data-messageid="${payload.id}"]`);
    if (el.length) el.find('div').last().text(payload.isDeletedForEveryone ? 'Message deleted' : '');
}

// ===================== Online status =====================
function markUserOnline(userId) {
    const el = $(`[data-userid="${userId}"] .status-indicator`);
    if (!el.length) return;
    el.find('.online-dot, .online-text').show();
}

function markUserOffline(userId) {
    const el = $(`[data-userid="${userId}"] .status-indicator`);
    if (!el.length) return;
    el.find('.online-dot, .online-text').hide();
}

// ===================== Typing =====================
function setupTypingDetection() {
    $('#message-input').on('input', function () {
        if (!currentChatRoomId) return;

        clearTimeout(typingTimer);
        connection.invoke('Typing', currentChatRoomId, currentUserId, true);

        typingTimer = setTimeout(() => {
            connection.invoke('Typing', currentChatRoomId, currentUserId, false);
        }, 1000);
    });
}

function updateTypingIndicator(userId, isTyping) {
    const el = $(`[data-userid="${userId}"] .typing-indicator`);
    if (!el.length) return;

    el.toggle(isTyping);
    el.text(isTyping ? 'Typing...' : '');
}

// ===================== Utility =====================
function escapeHtml(text) {
    return text ? $('<div/>').text(text).html() : '';
}
