window.deviceHelper = {
    // True for handheld phones. Combines UA sniffing with a narrow-viewport +
    // touch check so the BlackBerry UI only takes over on actual phones, not
    // desktops with small windows or large tablets.
    isMobilePhone: function () {
        const ua = (navigator.userAgent || navigator.vendor || window.opera || '').toLowerCase();
        const phoneUa = /iphone|ipod|android.*mobile|windows phone|blackberry|bb10|iemobile|opera mini|mobile.*firefox/i;
        if (phoneUa.test(ua)) return true;

        const hasTouch = ('ontouchstart' in window) || (navigator.maxTouchPoints || 0) > 0;
        const narrow = window.matchMedia && window.matchMedia('(max-width: 820px)').matches;
        const genericMobile = /android|mobile/i.test(ua);
        return hasTouch && narrow && genericMobile;
    }
};
