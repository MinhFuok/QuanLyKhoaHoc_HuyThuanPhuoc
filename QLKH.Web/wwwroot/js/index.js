(() => {
    'use strict';

    const root = document.querySelector('.home-index');
    if (!root) return;

    root.classList.add('is-enhanced');

    const prefersReduced = window.matchMedia('(prefers-reduced-motion: reduce)').matches;

    const makeRandom = (seed) => {
        let value = seed;
        return () => {
            value = (value * 9301 + 49297) % 233280;
            return value / 233280;
        };
    };

    const buildStars = (count, seed, alphaMin = 0.72) => {
        const random = makeRandom(seed);
        const stars = [];

        for (let i = 0; i < count; i += 1) {
            const x = Math.floor(random() * 2000);
            const y = Math.floor(random() * 2000);
            const alpha = alphaMin + random() * (1 - alphaMin);
            stars.push(`${x}px ${y}px rgba(255, 255, 255, ${alpha.toFixed(2)})`);
        }

        return stars.join(', ');
    };

    if (!prefersReduced) {
        root.style.setProperty('--home-stars-small', buildStars(620, 11, 0.62));
        root.style.setProperty('--home-stars-medium', buildStars(210, 29, 0.7));
        root.style.setProperty('--home-stars-large', buildStars(90, 47, 0.78));
    }

    const scrollLinks = root.querySelectorAll('[data-home-scroll]');
    scrollLinks.forEach((link) => {
        link.addEventListener('click', (event) => {
            const targetId = link.getAttribute('href');
            if (!targetId || !targetId.startsWith('#')) return;

            const target = root.querySelector(targetId);
            if (!target) return;

            event.preventDefault();
            target.scrollIntoView({
                behavior: prefersReduced ? 'auto' : 'smooth',
                block: 'start',
            });
        });
    });

    const revealItems = root.querySelectorAll('.home-reveal');
    if (!revealItems.length) return;

    if (!('IntersectionObserver' in window) || prefersReduced) {
        revealItems.forEach((item) => item.classList.add('is-visible'));
        return;
    }

    const observer = new IntersectionObserver((entries) => {
        entries.forEach((entry) => {
            if (!entry.isIntersecting) return;

            const siblings = Array.from(entry.target.parentElement?.querySelectorAll('.home-reveal') ?? []);
            const index = Math.max(0, siblings.indexOf(entry.target));
            entry.target.style.transitionDelay = `${Math.min(index * 70, 280)}ms`;
            entry.target.classList.add('is-visible');
            entry.target.addEventListener('transitionend', () => {
                entry.target.style.transitionDelay = '';
            }, { once: true });
            observer.unobserve(entry.target);
        });
    }, {
        threshold: 0.14,
        rootMargin: '0px 0px -50px 0px',
    });

    revealItems.forEach((item) => observer.observe(item));
})();
