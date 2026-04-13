(function () {
    'use strict';

    // ── 1. Starfield layers ──────────────────────────────────────
    var wrapper = document.createElement('div');
    wrapper.className = 'stars-wrapper';

    ['stars', 'stars2', 'stars3'].forEach(function (id) {
        var el = document.createElement('div');
        el.id = id;
        wrapper.appendChild(el);
    });

    document.body.prepend(wrapper);

    // ── 2. Shooting stars overlay ────────────────────────────────
    var bgStars = document.createElement('div');
    bgStars.className = 'bg-stars';

    for (var i = 0; i < 4; i++) {
        var s = document.createElement('div');
        s.className = 'star-shoot';
        bgStars.appendChild(s);
    }

    document.body.prepend(bgStars);

    // ── 3. Input placeholder fade on focus ──────────────────────
    document.querySelectorAll('.card.login-card .form-control').forEach(function (input) {
        input.addEventListener('focus', function () {
            this.style.transition = 'border-color 0.2s, box-shadow 0.2s';
        });
    });

    // ── 4. Button click ripple feedback ─────────────────────────
    var btn = document.querySelector('.btn-login');
    if (btn) {
        btn.addEventListener('click', function () {
            btn.style.transform = 'scale(0.97)';
            setTimeout(function () {
                btn.style.transform = 'scale(1)';
            }, 150);
        });
    }

})();