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
    setupTypingDetection();
});

document.addEventListener("DOMContentLoaded", function () {
    markMessagesAsDelivered();
    setupContextMenu();
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
    connection.on('MessageStatusUpdated', payload => onMessageStatusUpdated(payload));

    connection.start()
        .then(() => console.log('Connected to ChatHub'))
        .catch(err => console.error('SignalR connection error:', err));
}

// ===================== Users =====================
function loadUsers(query) {
    $('#users-list').html('<div class="text-slate-400 text-sm p-2">Loading...</div>');
    $.get(apiBase + '/GetUsers', { query: query || '' })
        .done(users => renderUsers(users));
}

function renderUsers(users) {
    const container = $('#users-list').empty();

    if (!users || users.length === 0) {
        container.append('<div class="text-slate-400 text-sm p-2">No users</div>');
        return;
    }

    users.forEach(u => {
        if (u.id === currentUserId) return;
        const el = $(`
            <div class="user-item bg-dark-card rounded-lg p-3 border border-dark-border transition-all hover:bg-slate-700" data-userid="${u.id}">
                <div class="flex justify-between items-start">
                    <div class="flex-1">
                        <div class="font-bold text-slate-200">${escapeHtml(u.fullName)}</div>
                        <div class="text-xs text-slate-400 mt-1">
                            ${escapeHtml(u.email)} · ${escapeHtml(u.username)}
                        </div>
                        <small class="text-xs text-slate-400">
                            Last seen: ${u.lastSeenAt ? new Date(u.lastSeenAt).toLocaleString() : 'Never'}
                        </small>
                        <div class="flex items-center status-indicator mt-1">
                            <span class="online-dot" style="display:none;"></span>
                            <span class="online-text" style="display:none;">Online</span>
                        </div>
                        <div class="last-seen text-xs text-slate-400"></div>
                        <div class="typing-indicator text-xs" style="display:none;"></div>
                    </div>
                    <div>
                       <button class="start-chat bg-green-600 hover:bg-green-700 text-white text-sm px-4 py-2 rounded-lg transition-all focus:outline-none focus:ring-2 focus:ring-green-400">
                            Chat
                        </button>
                    </div>
                </div>
            </div>
        `);

        const lastSeenEl = el.find('.last-seen');
        if (!u.isOnline && u.lastSeenAt) {
            const lastSeen = new Date(u.lastSeenAt);
            lastSeenEl.text(`Last seen: ${lastSeen.toLocaleString()}`);
        } else {
            lastSeenEl.text('');
        }

        // Hook up the chat button click
        el.find('.start-chat').on('click', (e) => {
            e.stopPropagation();
            startChatWithUser(u);
        });

        container.append(el);

        // Show online if user is online
        if (u.isOnline) markUserOnline(u.id);
    });
}



// ===================== Recent Chats =====================
function loadRecentChats() {
    $('#recent-chats').html('<div class="text-slate-400 text-sm p-2">Loading...</div>');
    $.get(apiBase + '/GetRecentChats')
        .done(data => {
            const container = $('#recent-chats').empty();
            if (!data || data.length === 0) {
                container.append('<div class="text-slate-400 text-sm p-2">No recent chats</div>');
                return;
            }

            data.forEach(c => {
                const el = $(`
                    <div class="recent-chat bg-dark-card rounded-lg p-3 border border-dark-border transition-all hover:bg-slate-700 cursor-pointer" data-chatid="${c.chatRoomId}">
                        <div>
                            <div class="font-bold text-slate-200">${escapeHtml(c.title)}</div>
                            <div class="text-xs text-slate-400 mt-1 flex items-center justify-between">
                                <span>${escapeHtml(c.lastMessageSnippet)}</span>
                                ${c.unreadCount > 0 ? `<span class="bg-red-500 text-white text-xs px-2 py-1 rounded-full ml-2">${c.unreadCount}</span>` : ''}
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
    console.log('Starting chat with user:', user);
    $.post(apiBase + '/StartChat', { userId: user.id })
        .done(chat => {
            console.log('Chat started:', chat);
            openChat(chat.chatRoomId, chat.title);
        })
        .fail(xhr => {
            console.error('Failed to start chat:', xhr);
            alert(xhr.responseText || 'Failed to start chat');
        });
}

function startChatByIdentifier(identifier) {
    $.post(apiBase + '/StartChat', { identifier })
        .done(chat => openChat(chat.chatRoomId, chat.title))
        .fail(xhr => alert(xhr.responseText || 'User not found'));
}

function markMessagesAsSeen(chatRoomId) {
    $.get(apiBase + '/GetUnseenMessages', { chatRoomId })
        .done(messages => {
            const ids = messages.map(m => m.id);
            if (ids.length > 0) {
                connection.invoke("MessagesSeen", ids)
                    .catch(err => console.error('Seen update failed:', err));
            }
        });
}

function openChat(chatRoomId, title) {
    console.log('Opening chat:', chatRoomId, title);
    currentChatRoomId = chatRoomId;
    $('#chat-header').html(`
        <h3 class="text-lg font-semibold text-purple-secondary flex items-center">
            <svg class="mr-2 h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 12h.01M12 12h.01M16 12h.01M21 12c0 4.418-4.03 8-9 8a9.863 9.863 0 01-4.255-.949L3 20l1.395-3.72C3.512 15.042 3 13.574 3 12c0-4.418 4.03-8 9-8s9 3.582 9 8z"></path>
            </svg>
            ${escapeHtml(title)}
        </h3>
    `);
    $('#message-input').prop('disabled', false);
    $('#send-btn').prop('disabled', false);
    $('#messages').html('<div class="text-slate-400 text-center p-4">Loading messages...</div>');

    $('.typing-indicator').hide().text('');

    connection.invoke('JoinChat', chatRoomId).catch(err => console.error(err));

    $.get(apiBase + '/GetMessages', { chatRoomId })
        .done(messages => {
            console.log('Messages loaded:', messages);
            renderMessages(messages);
            markMessagesAsSeen(chatRoomId);
        })
        .fail(err => {
            console.error('Failed to load messages:', err);
            $('#messages').html('<div class="text-red-400 text-center p-4">Failed to load messages</div>');
        });
}

function renderMessages(messages) {
    const container = $('#messages').empty();

    // Group messages by message ID to handle multiple recipients
    const groupedMessages = {};
    messages.forEach(m => {
        if (!groupedMessages[m.id]) {
            groupedMessages[m.id] = m;
            groupedMessages[m.id].statuses = [];
        }
        if (m.userId) {
            groupedMessages[m.id].statuses.push({
                userId: m.userId,
                isDelivered: m.isDelivered,
                isSeen: m.isSeen,
                seenAt: m.seenAt
            });
        }
    });

    Object.values(groupedMessages).forEach(m => {
        const alignment = m.senderId === currentUserId ? 'message-right' : 'message-left';
        const statusIcon = getMessageStatusIcon(m, currentUserId);
        const editedLabel = m.isEdited ? '<small class="text-slate-400 ml-1">(edited)</small>' : '';
        const deletedContent = m.isDeletedForEveryone ? '<em class="text-slate-400">Message deleted</em>' : escapeHtml(m.content);

        const el = $(`
            <div class="message ${alignment}" data-messageid="${m.id}" data-senderid="${m.senderId}" data-content="${escapeHtml(m.content)}">
                <div class="message-meta">
                    ${escapeHtml(m.senderUsername)} · ${new Date(m.sentAt).toLocaleString()}
                </div>
                <div class="message-content">
                    ${deletedContent}${editedLabel}
                </div>
                ${m.senderId === currentUserId ? `<div class="message-status">${statusIcon}</div>` : ''}
            </div>
        `);
        container.append(el);
    });

    container.scrollTop(container.prop('scrollHeight'));
}

function getMessageStatusIcon(message, currentUserId) {
    // Only show status for messages sent by current user
    if (message.senderId !== currentUserId) return '';

    // Check status for the recipient
    const recipientStatus = message.statuses.find(s => s.userId !== currentUserId);
    console.log(message.statuses)

    if (!recipientStatus) {
        return '<span class="text-slate-400" title="Sending...">⏱️</span>';
    }

    if (recipientStatus.isSeen) {
        const seenTime = recipientStatus.seenAt ? new Date(recipientStatus.seenAt).toLocaleString() : '';
        return `<span class="message-seen" title="Seen${seenTime ? ' at ' + seenTime : ''}">✓✓</span>`;
    }

    if (recipientStatus.isDelivered) {
        return '<span class="text-slate-400" title="Delivered">✓✓</span>';
    }

    return '<span class="text-slate-400" title="Sent">✓</span>';
}

function onMessageStatusUpdated(payload) {
    const messageEl = $(`[data-messageid="${payload.messageId}"]`);
    if (messageEl.length && payload.senderId === currentUserId) {
        const statusIcon = getMessageStatusIcon({
            senderId: payload.senderId,
            statuses: [{
                userId: payload.userId,
                isDelivered: payload.isDelivered,
                isSeen: payload.isSeen,
                seenAt: payload.seenAt
            }]
        }, currentUserId);

        messageEl.find('.message-status').html(statusIcon);
    }
}

// ===================== Sending messages =====================
$('#send-btn').on('click', async () => {
    const text = $('#message-input').val();
    if (!text || !currentChatRoomId) return;

    try {
        await connection.invoke("SendMessage", currentChatRoomId, text);
        $('#message-input').val('');
    } catch (err) {
        console.error(err);
        alert('Failed to send message');
    }
});

// Allow Enter key to send message
$('#message-input').on('keypress', function (e) {
    if (e.key === 'Enter' && !e.shiftKey) {
        e.preventDefault();
        $('#send-btn').click();
    }
});


// ===================== Receive messages =====================
function onReceiveMessage(payload) {
    if (payload.chatRoomId === currentChatRoomId) {
        const alignment = payload.senderId === currentUserId ? 'message-right' : 'message-left';
        const statusIcon = payload.senderId === currentUserId ? `<div class="message-status"><span class="text-slate-400" title="Sent">✓</span></div>` : '';
        const editedLabel = payload.isEdited ? '<small class="text-slate-400 ml-1">(edited)</small>' : '';

        $('#messages').append(`
            <div class="message ${alignment}" data-messageid="${payload.id}" data-senderid="${payload.senderId}" data-content="${escapeHtml(payload.content)}">
                <div class="message-meta">${escapeHtml(payload.senderUsername)} · ${new Date(payload.sentAt).toLocaleString()}</div>
                <div class="message-content">${escapeHtml(payload.content)}${editedLabel}</div>
                ${statusIcon}
            </div>
        `);
        $('#messages').scrollTop($('#messages')[0].scrollHeight);

        // If message is from another user, mark it as seen
        if (payload.senderId !== currentUserId) {
            markMessagesAsSeen(currentChatRoomId);
        }
    }
    loadRecentChats();
}

// ===================== Message edit/delete =====================
function onMessageEdited(payload) {
    const el = $(`[data-messageid="${payload.id}"]`);
    if (el.length) {
        el.attr('data-content', escapeHtml(payload.content));
        el.find('.message-content').html(`${escapeHtml(payload.content)} <small class="text-slate-400 ml-1">(edited)</small>`);
    }
}

function onMessageDeleted(payload) {
    const el = $(`[data-messageid="${payload.id}"]`);
    if (el.length) {
        if (payload.isDeletedForEveryone) {
            el.find('.message-content').html('<em class="text-slate-400">Message deleted</em>');
        }
    }
}

// ===================== Context Menu for Messages =====================
function setupContextMenu() {
    // Hide context menu when clicking elsewhere
    $(document).on('click', function () {
        $('#message-context-menu').remove();
    });

    // Right-click on messages
    $(document).on('contextmenu', '.message', function (e) {
        const messageEl = $(this);
        const senderId = parseInt(messageEl.data('senderid'));

        // Only show context menu for messages sent by current user
        if (senderId !== currentUserId) {
            return; // Allow default context menu
        }

        e.preventDefault();

        const messageId = messageEl.data('messageid');
        const messageContent = messageEl.data('content');

        // Remove existing context menu
        $('#message-context-menu').remove();

        // Create context menu with dark theme
        const contextMenu = $(`
            <div id="message-context-menu" style="position: absolute; top: ${e.pageY}px; left: ${e.pageX}px; background: #1e293b; border: 1px solid #334155; border-radius: 8px; box-shadow: 0 4px 12px rgba(0,0,0,0.5); z-index: 9999; min-width: 150px;">
                <div class="context-menu-item" data-action="edit" style="padding: 10px 15px; cursor: pointer; border-bottom: 1px solid #334155; color: #e2e8f0;">
                    <i class="bi bi-pencil"></i> Edit
                </div>
                <div class="context-menu-item" data-action="delete" style="padding: 10px 15px; cursor: pointer; color: #ef4444;">
                    <i class="bi bi-trash"></i> Delete for Everyone
                </div>
            </div>
        `);

        $('body').append(contextMenu);

        // Hover effect
        contextMenu.find('.context-menu-item').on('mouseenter', function () {
            $(this).css('background-color', '#334155');
        }).on('mouseleave', function () {
            $(this).css('background-color', '#1e293b');
        });

        // Handle menu item clicks
        contextMenu.find('.context-menu-item').on('click', function (e) {
            e.stopPropagation();
            const action = $(this).data('action');

            if (action === 'edit') {
                openEditMessageModal(messageId, messageContent);
            } else if (action === 'delete') {
                deleteMessageForEveryone(messageId);
            }

            contextMenu.remove();
        });
    });
}

function openEditMessageModal(messageId, currentContent) {
    // Remove existing modal if any
    $('#edit-message-modal').remove();

    // Create modal with Tailwind CSS (no Bootstrap)
    const modal = $(`
        <div id="edit-message-modal" class="fixed inset-0 z-50 flex items-center justify-center" style="background-color: rgba(0, 0, 0, 0.75);">
            <div class="bg-dark-card rounded-lg shadow-xl w-full max-w-md mx-4" style="background-color: #1e293b; border: 1px solid #334155;">
                <div class="px-6 py-4 border-b" style="border-color: #334155;">
                    <div class="flex items-center justify-between">
                        <h5 class="text-lg font-semibold" style="color: #a78bfa;">Edit Message</h5>
                        <button type="button" class="text-slate-400 hover:text-slate-200 close-modal">
                            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
                            </svg>
                        </button>
                    </div>
                </div>
                <div class="px-6 py-4">
                    <form id="edit-message-form">
                        <div class="mb-3">
                            <label for="edit-message-content" class="block text-sm font-medium text-slate-300 mb-2">Message Content</label>
                            <textarea class="w-full px-3 py-2 rounded-lg text-slate-200 focus:outline-none focus:ring-2 focus:ring-purple-primary" id="edit-message-content" rows="4" required style="background-color: #0f172a; border: 1px solid #334155;">${escapeHtml(currentContent)}</textarea>
                        </div>
                    </form>
                </div>
                <div class="px-6 py-4 border-t flex justify-end gap-2" style="border-color: #334155;">
                    <button type="button" class="px-4 py-2 rounded-lg text-white close-modal" style="background-color: #475569;">Cancel</button>
                    <button type="button" class="px-4 py-2 rounded-lg text-white" id="save-edit-btn" style="background: linear-gradient(135deg, #8b5cf6 0%, #a78bfa 100%);">Save Changes</button>
                </div>
            </div>
        </div>
    `);

    $('body').append(modal);

    // Close modal handlers
    modal.find('.close-modal').on('click', function () {
        modal.remove();
    });

    // Close on backdrop click
    modal.on('click', function (e) {
        if (e.target === this) {
            modal.remove();
        }
    });

    // Handle save button
    $('#save-edit-btn').on('click', function () {
        const newContent = $('#edit-message-content').val().trim();

        if (!newContent) {
            alert('Message content cannot be empty');
            return;
        }

        if (newContent === currentContent) {
            modal.remove();
            return;
        }

        // Make AJAX call to edit message
        $.ajax({
            url: apiBase + '/EditMessage',
            method: 'POST',
            data: {
                messageId: messageId,
                newContent: newContent
            },
            success: function (response) {
                console.log('Message edited successfully:', response);
                modal.remove();
            },
            error: function (xhr, status, error) {
                console.error('Failed to edit message:', error);
                alert('Failed to edit message. Please try again.');
            }
        });
    });
}

function deleteMessageForEveryone(messageId) {
    if (!confirm('Are you sure you want to delete this message for everyone?')) {
        return;
    }

    $.ajax({
        url: apiBase + '/DeleteMessage',
        method: 'POST',
        data: {
            messageId: messageId
        },
        success: function (response) {
            console.log('Message deleted successfully:', response);
            // The SignalR event will update the UI
        },
        error: function (xhr, status, error) {
            console.error('Failed to delete message:', error);
            alert('Failed to delete message. Please try again.');
        }
    });
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

// Add this function after the escapeHtml function

function markMessagesAsDelivered() {
    $.post(apiBase + '/MarkMessagesDelivered')
        .done(result => {
            console.log(`${result.count} messages marked as delivered`);
        })
        .fail(err => console.error('Failed to mark messages as delivered:', err));
}