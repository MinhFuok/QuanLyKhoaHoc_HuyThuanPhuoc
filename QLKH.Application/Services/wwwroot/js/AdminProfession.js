/* =============================================
   QLKH - Admin Profession shared UI
   Dung chung cho Course, ClassRoom va cac module admin cung style.
   ============================================= */

(function () {
    'use strict';

    var reduced = window.matchMedia('(prefers-reduced-motion: reduce)').matches;

    function $(sel, ctx) { return (ctx || document).querySelector(sel); }
    function $$(sel, ctx) { return Array.from((ctx || document).querySelectorAll(sel)); }

    /* =============================================
       1. INJECT GALAXY BACKGROUND
       ============================================= */
    function injectGalaxy() {
        if ($('.cp-galaxy-bg')) return;

        var bg = document.createElement('div');
        bg.className = 'cp-galaxy-bg';
        bg.setAttribute('aria-hidden', 'true');

        ['cp-stars', 'cp-stars2', 'cp-stars3'].forEach(function (id) {
            var el = document.createElement('div');
            el.id = id;
            bg.appendChild(el);
        });

        document.body.insertBefore(bg, document.body.firstChild);

        /* Shooting stars */
        if (!reduced) {
            var shootData = [
                { top: '15%', left: '10%', delay: '3s', dur: '6s', angle: '35deg' },
                { top: '40%', left: '60%', delay: '9s', dur: '5s', angle: '25deg' },
                { top: '70%', left: '25%', delay: '15s', dur: '7s', angle: '40deg' },
                { top: '25%', left: '80%', delay: '20s', dur: '5.5s', angle: '30deg' },
            ];
            shootData.forEach(function (d) {
                var s = document.createElement('div');
                s.className = 'cp-shooting-star';
                s.style.cssText =
                    'top:' + d.top + ';left:' + d.left + ';' +
                    'animation-delay:' + d.delay + ';' +
                    'animation-duration:' + d.dur + ';' +
                    'transform:rotate(' + d.angle + ');' +
                    'width:80px;height:1.5px;border-radius:1px;' +
                    'background:linear-gradient(90deg,rgba(255,255,255,0),#fff,#c084fc);';
                bg.appendChild(s);
            });
        }
    }

    injectGalaxy();

    /* =============================================
       2. INJECT RUNTIME STYLES
       ============================================= */
    var styleTag = document.createElement('style');
    styleTag.textContent = [
        /* Spinner */
        '.cp-spinner{display:inline-block;width:13px;height:13px;border:2px solid rgba(255,255,255,.3);border-top-color:#c084fc;border-radius:50%;animation:cpSpin .6s linear infinite;flex-shrink:0}',
        '@keyframes cpSpin{to{transform:rotate(360deg)}}',

        /* Twinkle keyframes for random stars */
        '@keyframes cpTwinkle{0%,100%{opacity:.3}50%{opacity:1}}',
    ].join('');
    document.head.appendChild(styleTag);

    /* =============================================
       3. STAGGER REVEALS
       ============================================= */

    /* Header */
    var header = $('.cp-header');
    if (header && !reduced) {
        header.style.cssText += 'opacity:0;transform:translateY(-12px);transition:opacity .35s ease,transform .35s ease';
        setTimeout(function () { header.style.opacity = '1'; header.style.transform = 'translateY(0)'; }, 30);
    }

    /* Cards & table */
    var surfaces = $$('.cp-form-card, .cp-detail-card, .cp-delete-card, .cp-table-wrap');
    if (!reduced) {
        surfaces.forEach(function (el, i) {
            el.style.cssText += 'opacity:0;transform:translateY(20px);transition:opacity .4s cubic-bezier(.4,0,.2,1),transform .4s cubic-bezier(.4,0,.2,1)';
            setTimeout(function () { el.style.opacity = '1'; el.style.transform = 'translateY(0)'; }, 80 + i * 60);
        });
    }

    /* Table rows stagger */
    var rows = $$('.cp-table .cp-row');
    if (rows.length && !reduced) {
        rows.forEach(function (row, i) {
            row.style.cssText += 'opacity:0;transform:translateX(-10px);transition:opacity .28s ease,transform .28s ease';
            setTimeout(function () { row.style.opacity = '1'; row.style.transform = 'translateX(0)'; }, 160 + i * 40);
        });
    }

    /* =============================================
       4. FORM ENHANCEMENTS
       ============================================= */
    function initAdminForm(form) {
        /* Focus first invalid on load */
        var firstErr = form.querySelector('.input-validation-error');
        if (firstErr) setTimeout(function () { firstErr.focus(); }, 300);

        /* Clear error on input */
        $$('.cp-input', form).forEach(function (inp) {
            inp.addEventListener('input', function () {
                inp.classList.remove('input-validation-error');
                var name = inp.id || inp.name;
                if (name) {
                    var span = form.querySelector('[data-valmsg-for="' + name + '"]');
                    if (span) { span.textContent = ''; span.classList.remove('field-validation-error'); }
                }
            });
        });

        /* Textarea auto-resize */
        $$('.cp-textarea', form).forEach(function (ta) {
            function resize() { ta.style.height = 'auto'; ta.style.height = (ta.scrollHeight + 2) + 'px'; }
            ta.addEventListener('input', resize);
            resize();
        });

        /* Submit loading state */
        form.addEventListener('submit', function () {
            var errCount = form.querySelectorAll('.input-validation-error, .field-validation-error:not(:empty)').length;
            if (errCount) return;

            var btn = form.querySelector('[type="submit"]');
            if (!btn) return;
            btn.disabled = true;
            var orig = btn.innerHTML;
            var loadingText = form.getAttribute('data-loading-text') || 'Đang lưu...';
            btn.innerHTML = '<span class="cp-spinner"></span><span>' + loadingText + '</span>';
            setTimeout(function () { btn.disabled = false; btn.innerHTML = orig; }, 9000);
        });
    }

    $$('#courseForm, #classroomForm, [data-admin-form]').forEach(initAdminForm);

    /* =============================================
       5. DELETE CONFIRM + SHAKE
       ============================================= */
    function initDeleteForm(delForm) {
        delForm.addEventListener('submit', function (e) {
            var label = delForm.getAttribute('data-delete-label') || 'mục này';
            var message = delForm.getAttribute('data-delete-message') ||
                ('Xác nhận xóa ' + label + '?\n\nHành động này không thể hoàn tác.\nNhấn OK để tiếp tục.');
            var ok = window.confirm(message);
            if (!ok) {
                e.preventDefault();
                if (!reduced) {
                    var card = $('.cp-delete-card');
                    if (!card) return;
                    var steps = [8, -7, 5, -4, 3, 0];
                    steps.forEach(function (px, i) {
                        setTimeout(function () {
                            card.style.transform = 'translateX(' + px + 'px)';
                            card.style.transition = 'transform .07s ease';
                        }, i * 60);
                    });
                }
            }
        });
    }

    $$('#deleteForm, [data-delete-form]').forEach(initDeleteForm);

    /* =============================================
       6. BADGE GLOW ON HOVER
       ============================================= */
    $$('.cp-badge').forEach(function (badge) {
        badge.addEventListener('mouseenter', function () {
            if (reduced) return;
            badge.style.boxShadow = '0 0 12px currentColor';
            badge.style.transition = 'box-shadow .2s';
        });
        badge.addEventListener('mouseleave', function () {
            badge.style.boxShadow = '';
        });
    });

    /* =============================================
       7. CODE CHIP COPY ON CLICK
       ============================================= */
    $$('.cp-code').forEach(function (chip) {
        chip.style.cursor = 'pointer';
        chip.title = 'Click để sao chép';
        chip.addEventListener('click', function () {
            var text = chip.textContent.trim();
            if (!navigator.clipboard) return;
            navigator.clipboard.writeText(text).then(function () {
                var orig = chip.textContent;
                var origStyle = chip.getAttribute('style') || '';
                chip.textContent = '✓ Sao chép';
                chip.style.background = 'rgba(16,185,129,.18)';
                chip.style.color = '#6ee7b7';
                chip.style.borderColor = 'rgba(16,185,129,.3)';
                chip.style.boxShadow = '0 0 12px rgba(16,185,129,.3)';
                setTimeout(function () {
                    chip.textContent = orig;
                    chip.setAttribute('style', origStyle);
                }, 1600);
            });
        });
    });

    /* =============================================
       8. TOOLTIP FOR TRUNCATED CELLS
       ============================================= */
    $$('.cp-name').forEach(function (el) {
        if (el.scrollWidth > el.clientWidth) el.title = el.textContent.trim();
    });

    /* =============================================
       9. RANDOM STAR TWINKLE (extra polish)
       ============================================= */
    if (!reduced) {
        var starsEl = document.getElementById('cp-stars3');
        if (starsEl) {
            /* Already animated by CSS; add subtle opacity pulse */
            starsEl.style.animation = 'cpStarFloat 150s linear infinite, cpTwinkle 4s ease-in-out infinite alternate';
        }
    }

    var sound = new Audio('/audio/click.mp3');
    sound.play();

})();
