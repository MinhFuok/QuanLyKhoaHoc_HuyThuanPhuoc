(() => {
    'use strict';

    const root = document.getElementById('pl-floating-contact');
    const panel = document.getElementById('pl-floating-panel');
    const toggle = document.getElementById('pl-floating-toggle');
    const scrollTopButton = document.getElementById('pl-scroll-top');

    const aiOpenButton = document.getElementById('pl-ai-support-open');
    const aiChat = document.getElementById('pl-ai-chat');
    const aiCloseButton = document.getElementById('pl-ai-chat-close');
    const aiForm = document.getElementById('pl-ai-chat-form');
    const aiInput = document.getElementById('pl-ai-chat-input');
    const aiMessages = document.getElementById('pl-ai-chat-messages');
    const aiSendButton = document.getElementById('pl-ai-chat-send');
    const aiSuggestionButtons = document.querySelectorAll('[data-ai-question]');

    if (!root || !panel || !toggle || !scrollTopButton) {
        return;
    }

    const mobileQuery = window.matchMedia('(max-width: 768px)');

    const setOpen = (open) => {
        root.classList.toggle('is-open', open);
        toggle.setAttribute('aria-expanded', String(open));
        panel.setAttribute('aria-hidden', String(!open));
    };

    const syncInitialState = () => {
        setOpen(false);
    };

    const openAiChat = () => {
        if (!aiChat || !aiOpenButton) {
            return;
        }

        aiChat.classList.add('is-open');
        aiChat.setAttribute('aria-hidden', 'false');
        aiOpenButton.setAttribute('aria-expanded', 'true');

        setOpen(false);

        setTimeout(() => {
            if (aiInput) {
                aiInput.focus();
            }
        }, 120);
    };

    const closeAiChat = () => {
        if (!aiChat || !aiOpenButton) {
            return;
        }

        aiChat.classList.remove('is-open');
        aiChat.setAttribute('aria-hidden', 'true');
        aiOpenButton.setAttribute('aria-expanded', 'false');
    };

    const normalizeAiText = (text) => {
        return (text || '')
            .replace(/\r\n/g, '\n')
            .split('\n')
            .map(line => line.trim())
            .join('\n')
            .replace(/\n{3,}/g, '\n\n')
            .trim();
    };

    const appendAiMessage = (type, text, extraClass) => {
        if (!aiMessages) {
            return null;
        }

        const row = document.createElement('div');
        row.className = `pl-ai-chat__message pl-ai-chat__message--${type}`;

        if (type === 'bot') {
            const avatar = document.createElement('div');
            avatar.className = 'pl-ai-chat__message-avatar';
            avatar.setAttribute('aria-hidden', 'true');
            avatar.innerHTML = `
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round">
                <rect x="7" y="8" width="10" height="8" rx="2"></rect>
                <path d="M12 4v3"></path>
                <path d="M9 16v2"></path>
                <path d="M15 16v2"></path>
                <path d="M7 12H5"></path>
                <path d="M19 12h-2"></path>
                <circle cx="10" cy="12" r="1"></circle>
                <circle cx="14" cy="12" r="1"></circle>
                <path d="M10 14h4"></path>
            </svg>
        `;
            row.appendChild(avatar);
        }

        const bubble = document.createElement('div');
        bubble.className = 'pl-ai-chat__bubble';

        if (extraClass) {
            bubble.classList.add(extraClass);
        }

        bubble.textContent = normalizeAiText(text);

        row.appendChild(bubble);
        aiMessages.appendChild(row);
        aiMessages.scrollTop = aiMessages.scrollHeight;

        return row;
    };

    const sendAiMessage = async (message) => {
        if (!aiChat || !aiForm || !aiInput || !aiSendButton) {
            return;
        }

        const question = (message || '').trim();

        if (!question) {
            return;
        }

        openAiChat();

        appendAiMessage('user', question);
        aiInput.value = '';

        const loadingRow = appendAiMessage('bot', 'AI đang trả lời...', 'is-loading');

        aiInput.disabled = true;
        aiSendButton.disabled = true;

        const askUrl = aiChat.getAttribute('data-ask-url') || '/AiSupport/Ask';
        const tokenInput = aiForm.querySelector('input[name="__RequestVerificationToken"]');

        try {
            const response = await fetch(askUrl, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': tokenInput ? tokenInput.value : ''
                },
                body: JSON.stringify({
                    message: question
                })
            });

            const result = await response.json();

            if (loadingRow) {
                loadingRow.remove();
            }

            if (result.success) {
                appendAiMessage('bot', result.answer || 'Mình chưa có câu trả lời phù hợp.');
            } else {
                appendAiMessage('bot', result.errorMessage || 'AI chưa phản hồi được. Vui lòng thử lại sau.');
            }
        } catch (error) {
            if (loadingRow) {
                loadingRow.remove();
            }

            appendAiMessage('bot', 'Không thể kết nối tới AI. Vui lòng thử lại sau.');
        } finally {
            aiInput.disabled = false;
            aiSendButton.disabled = false;
            aiInput.focus();
        }
    };

    syncInitialState();

    toggle.addEventListener('click', () => {
        setOpen(!root.classList.contains('is-open'));
    });

    scrollTopButton.addEventListener('click', () => {
        window.scrollTo({ top: 0, behavior: 'smooth' });
    });

    if (aiOpenButton) {
        aiOpenButton.addEventListener('click', () => {
            openAiChat();
        });
    }

    if (aiCloseButton) {
        aiCloseButton.addEventListener('click', () => {
            closeAiChat();
        });
    }

    if (aiForm) {
        aiForm.addEventListener('submit', (event) => {
            event.preventDefault();

            if (aiInput) {
                sendAiMessage(aiInput.value);
            }
        });
    }

    if (aiInput) {
        aiInput.addEventListener('keydown', (event) => {
            if (event.key === 'Enter' && !event.shiftKey) {
                event.preventDefault();
                sendAiMessage(aiInput.value);
            }
        });
    }

    aiSuggestionButtons.forEach((button) => {
        button.addEventListener('click', () => {
            const question = button.getAttribute('data-ai-question') || '';
            sendAiMessage(question);
        });
    });

    const updateScrollTopVisibility = () => {
        scrollTopButton.classList.toggle('is-visible', window.scrollY > 280);
    };

    const handleOutsideClick = (event) => {
        if (!mobileQuery.matches) {
            return;
        }

        if (!root.contains(event.target)) {
            setOpen(false);
        }
    };

    const handleEscape = (event) => {
        if (event.key !== 'Escape') {
            return;
        }

        if (mobileQuery.matches) {
            setOpen(false);
        }

        closeAiChat();
    };

    const handleBreakpointChange = (event) => {
        setOpen(!event.matches);
    };

    mobileQuery.addEventListener('change', handleBreakpointChange);

    document.addEventListener('click', handleOutsideClick);
    document.addEventListener('keydown', handleEscape);
    window.addEventListener('scroll', updateScrollTopVisibility, { passive: true });

    syncInitialState();
    updateScrollTopVisibility();
})();