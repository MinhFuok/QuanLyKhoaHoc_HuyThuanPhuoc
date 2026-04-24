(() => {
    'use strict';

    const root = document.getElementById('pl-floating-contact');
    const panel = document.getElementById('pl-floating-panel');
    const toggle = document.getElementById('pl-floating-toggle');
    const scrollTopButton = document.getElementById('pl-scroll-top');

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

    syncInitialState();

    toggle.addEventListener('click', () => {
        setOpen(!root.classList.contains('is-open'));
    });

    scrollTopButton.addEventListener('click', () => {
        window.scrollTo({ top: 0, behavior: 'smooth' });
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
        if (event.key === 'Escape' && mobileQuery.matches) {
            setOpen(false);
        }
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
