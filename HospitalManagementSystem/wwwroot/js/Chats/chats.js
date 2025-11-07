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
    currentChatRoomId = chatRoomId;
    $('#chat-header').text(title);
    $('#message-input').prop('disabled', false);
    $('#send-btn').prop('disabled', false);
    $('#messages').html('<div>Loading messages...</div>');

    $('.typing-indicator').hide().text('');

    connection.invoke('JoinChat', chatRoomId).catch(err => console.error(err));

    $.get(apiBase + '/GetMessages', { chatRoomId })
        .done(messages => {
            console.log(messages);
            renderMessages(messages);
            markMessagesAsSeen(chatRoomId);
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
        const editedLabel = m.isEdited ? '<small class="text-muted ms-1">(edited)</small>' : '';
        const deletedContent = m.isDeletedForEveryone ? '<em class="text-muted">Message deleted</em>' : escapeHtml(m.content);

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
        return '<span class="text-muted" title="Sending...">⏱️</span>';
    }

    if (recipientStatus.isSeen) {
        const seenTime = recipientStatus.seenAt ? new Date(recipientStatus.seenAt).toLocaleString() : '';
        return `<span class="message-seen" title="Seen${seenTime ? ' at ' + seenTime : ''}">✓✓</span>`;
    }

    if (recipientStatus.isDelivered) {
        return '<span class="text-muted" title="Delivered">✓✓</span>';
    }

    return '<span class="text-muted" title="Sent">✓</span>';
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


// ===================== Receive messages =====================
function onReceiveMessage(payload) {
    if (payload.chatRoomId === currentChatRoomId) {
        const alignment = payload.senderId === currentUserId ? 'message-right' : 'message-left';
        const statusIcon = payload.senderId === currentUserId ? `<div class="message-status"><span class="text-muted" title="Sent">✓</span></div>` : '';
        const editedLabel = payload.isEdited ? '<small class="text-muted ms-1">(edited)</small>' : '';

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
        el.find('.message-content').html(`${escapeHtml(payload.content)} <small class="text-muted ms-1">(edited)</small>`);
    }
}

function onMessageDeleted(payload) {
    const el = $(`[data-messageid="${payload.id}"]`);
    if (el.length) {
        if (payload.isDeletedForEveryone) {
            el.find('.message-content').html('<em class="text-muted">Message deleted</em>');
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

        // Create context menu
        const contextMenu = $(`
            <div id="message-context-menu" style="z-index: 10; position: absolute; top: ${e.pageY}px; left: ${e.pageX}px; background: white; border: 1px solid #ccc; border-radius: 4px; box-shadow: 0 2px 10px rgba(0,0,0,0.2); z-index: 9999; min-width: 150px;">
                <div class="context-menu-item" data-action="edit" style="padding: 10px 15px; cursor: pointer; border-bottom: 1px solid #eee;">
                    <i class="bi bi-pencil"></i> Edit
                </div>
                <div class="context-menu-item" data-action="delete" style="padding: 10px 15px; cursor: pointer; color: #dc3545;">
                    <i class="bi bi-trash"></i> Delete for Everyone
                </div>
            </div>
        `);

        $('body').append(contextMenu);

        // Hover effect
        contextMenu.find('.context-menu-item').on('mouseenter', function () {
            $(this).css('background-color', '#f0f0f0');
        }).on('mouseleave', function () {
            $(this).css('background-color', 'white');
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

    // Create modal
    const modal = $(`
        <div class="modal fade" id="edit-message-modal" tabindex="-1" aria-labelledby="editMessageModalLabel" aria-hidden="true">
            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="editMessageModalLabel">Edit Message</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body">
                        <form id="edit-message-form">
                            <div class="mb-3">
                                <label for="edit-message-content" class="form-label">Message Content</label>
                                <textarea class="form-control" id="edit-message-content" rows="4" required>${currentContent}</textarea>
                            </div>
                        </form>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                        <button type="button" class="btn btn-primary" id="save-edit-btn">Save Changes</button>
                    </div>
                </div>
            </div>
        </div>
    `);

    $('body').append(modal);

    // Show modal
    const bsModal = new bootstrap.Modal(document.getElementById('edit-message-modal'));
    bsModal.show();

    // Handle save button
    $('#save-edit-btn').on('click', function () {
        const newContent = $('#edit-message-content').val().trim();

        if (!newContent) {
            alert('Message content cannot be empty');
            return;
        }

        if (newContent === currentContent) {
            bsModal.hide();
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
                bsModal.hide();
                // The SignalR event will update the UI
            },
            error: function (xhr, status, error) {
                console.error('Failed to edit message:', error);
                alert('Failed to edit message. Please try again.');
            }
        });
    });

    // Clean up modal after hiding
    $('#edit-message-modal').on('hidden.bs.modal', function () {
        $(this).remove();
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