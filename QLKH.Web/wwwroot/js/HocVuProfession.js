(function (window) {
    'use strict';

    if (!window.AdminCore || !window.AdminPages) {
        return;
    }

    window.AdminCore.onReady(function () {
        window.AdminPages.mountHocVu();
    });
})(window);
