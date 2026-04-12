(() => {
    'use strict';

    /* ── 1. Smooth scroll: [data-home-scroll] → #home-content ── */
    const scrollTrigger = document.querySelector('[data-home-scroll]');
    const scrollTarget = document.querySelector('#home-content');
    if (scrollTrigger && scrollTarget) {
        scrollTrigger.addEventListener('click', (e) => {
            e.preventDefault();
            scrollTarget.scrollIntoView({ behavior: 'smooth', block: 'start' });
        });
    }

    /* ── 2. Header: sticky shadow on scroll ── */
    const header = document.getElementById('pl-header');
    if (header) {
        let ticking = false;
        const onScroll = () => {
            if (!ticking) {
                requestAnimationFrame(() => {
                    header.classList.toggle('is-scrolled', window.scrollY > 10);
                    ticking = false;
                });
                ticking = true;
            }
        };
        window.addEventListener('scroll', onScroll, { passive: true });
        onScroll();
    }

    /* ── 3. Mobile burger menu ── */
    const burger = document.getElementById('pl-burger');
    const nav = document.getElementById('pl-nav');
    if (burger && nav) {
        burger.addEventListener('click', () => {
            const open = nav.classList.toggle('is-open');
            burger.classList.toggle('is-open', open);
            burger.setAttribute('aria-expanded', String(open));
        });

        // Close on outside click
        document.addEventListener('click', (e) => {
            if (!header.contains(e.target)) {
                nav.classList.remove('is-open');
                burger.classList.remove('is-open');
                burger.setAttribute('aria-expanded', 'false');
            }
        });

        // Close on Escape
        document.addEventListener('keydown', (e) => {
            if (e.key === 'Escape') {
                nav.classList.remove('is-open');
                burger.classList.remove('is-open');
                burger.setAttribute('aria-expanded', 'false');
            }
        });
    }

    /* ── 4. Intersection Observer: fade-up on scroll ── */
    if ('IntersectionObserver' in window) {
        const targets = document.querySelectorAll('.js-fade');
        const observer = new IntersectionObserver((entries) => {
            entries.forEach((entry) => {
                if (entry.isIntersecting) {
                    entry.target.classList.add('is-visible');
                    observer.unobserve(entry.target);
                }
            });
        }, { threshold: 0.1, rootMargin: '0px 0px -40px 0px' });

        targets.forEach((el) => observer.observe(el));
    }

})();