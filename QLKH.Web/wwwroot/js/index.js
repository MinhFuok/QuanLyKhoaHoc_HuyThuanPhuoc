/* ============================================================
   QLKH — Landing Page JS  (index.js)
   Quy tắc:
   · Không inject <style> bằng JS
   · Không can thiệp animation class trực tiếp bằng inline style
   · Tôn trọng prefers-reduced-motion (đọc từ CSS media query)
   ============================================================ */

(() => {
    'use strict';

    /* ── 1. Smooth scroll khi click [data-lp-scroll] ─────────── */
    const scrollTrigger = document.querySelector('[data-lp-scroll]');
    const scrollTarget = document.getElementById('lp-intro');

    if (scrollTrigger && scrollTarget) {
        scrollTrigger.addEventListener('click', (e) => {
            e.preventDefault();
            scrollTarget.scrollIntoView({ behavior: 'smooth', block: 'start' });
        });
    }

    /* ── 2. Fade-in cards khi scroll vào viewport ─────────────
       Dùng class .lp-animate → .lp-animate--visible
       CSS đã khai báo sẵn transition; JS chỉ thêm/xóa class
       prefers-reduced-motion: CSS tắt hẳn transition rồi,
       nhưng JS vẫn add class để element visible ngay
    ────────────────────────────────────────────────────────── */
    const prefersReduced = window.matchMedia('(prefers-reduced-motion: reduce)').matches;

    const items = document.querySelectorAll('.lp-animate');

    if (!items.length) return;

    if (!('IntersectionObserver' in window) || prefersReduced) {
        // Fallback: hiện tất cả ngay lập tức
        items.forEach(el => el.classList.add('lp-animate--visible'));
        return;
    }

    const observer = new IntersectionObserver((entries) => {
        entries.forEach((entry) => {
            if (!entry.isIntersecting) return;

            // Tính stagger index dựa trên vị trí trong grid/list cha
            const parent = entry.target.parentElement;
            const siblings = parent ? Array.from(parent.querySelectorAll('.lp-animate')) : [];
            const idx = siblings.indexOf(entry.target);
            const delay = Math.max(0, idx) * 70; // 70 ms mỗi card

            entry.target.style.transitionDelay = `${delay}ms`;
            entry.target.classList.add('lp-animate--visible');

            // Reset delay sau khi animation chạy xong để không ảnh hưởng hover
            entry.target.addEventListener('transitionend', () => {
                entry.target.style.transitionDelay = '';
            }, { once: true });

            observer.unobserve(entry.target);
        });
    }, {
        threshold: 0.1,
        rootMargin: '0px 0px -40px 0px',
    });

    items.forEach(el => observer.observe(el));

})();
