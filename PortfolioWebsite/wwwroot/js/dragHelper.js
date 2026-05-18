window.dragHelper = {
    scrollToBottom: function (elementId) {
        const el = document.getElementById(elementId);
        if (el) el.scrollTop = el.scrollHeight;
    },

    videoPause:     function (el) { if (el) el.pause(); },
    videoPlay:      function (el) { if (el) el.play();  },
    videoSetVolume: function (el, v) { if (el) el.volume = v; },

    enableResize: function (elementId) {
        const el = document.getElementById(elementId);
        if (!el || el.dataset.resizeEnabled) return;
        el.dataset.resizeEnabled = 'true';

        const EDGE = 6;   // px from window edge that activates a resize zone
        const FALLBACK_W = 150;
        const FALLBACK_H = 80;

        function getDir(e) {
            if (el.classList.contains('maximized')) return '';
            const r = el.getBoundingClientRect();
            const x = e.clientX - r.left;
            const y = e.clientY - r.top;
            const n = y <= EDGE, s = y >= r.height - EDGE;
            const east = x >= r.width - EDGE, west = x <= EDGE;
            if (n && west)  return 'nw-resize';
            if (n && east)  return 'ne-resize';
            if (s && west)  return 'sw-resize';
            if (s && east)  return 'se-resize';
            if (n)    return 'n-resize';
            if (s)    return 's-resize';
            if (east) return 'e-resize';
            if (west) return 'w-resize';
            return '';
        }

        // Override cursor globally (beats titlebar cursor:move, video cursor:default, etc.)
        function setCursor(c) {
            if (c) document.documentElement.style.setProperty('cursor', c, 'important');
            else   document.documentElement.style.removeProperty('cursor');
        }

        let resizing = false, dir = '';
        let startX, startY, startW, startH, startL, startT;
        let moveHandler, upHandler;

        el.addEventListener('mousemove', e => {
            if (!resizing) setCursor(getDir(e) || null);
        });

        el.addEventListener('mouseleave', () => {
            if (!resizing) setCursor(null);
        });

        el.addEventListener('mousedown', e => {
            dir = getDir(e);
            if (!dir) return;
            resizing = true;
            const r = el.getBoundingClientRect();
            startX = e.clientX; startY = e.clientY;
            startW = r.width;   startH = r.height;
            // Resolve any CSS calc/transform to explicit px before resizing
            el.style.transform = 'none';
            startL = r.left;    startT = r.top;
            el.style.left = startL + 'px';
            el.style.top  = startT + 'px';
            setCursor(dir);
            e.preventDefault();
            e.stopPropagation();

            // Attach document listeners only during active resize
            moveHandler = e => {
                if (!resizing) return;
                const cs   = getComputedStyle(el);
                const minW = Math.max(FALLBACK_W, parseInt(cs.minWidth)  || 0);
                const minH = Math.max(FALLBACK_H, parseInt(cs.minHeight) || 0);
                const dx = e.clientX - startX;
                const dy = e.clientY - startY;
                if (dir.includes('e')) el.style.width  = Math.max(minW, startW + dx) + 'px';
                if (dir.includes('s')) el.style.height = Math.max(minH, startH + dy) + 'px';
                if (dir.includes('w')) {
                    const w = Math.max(minW, startW - dx);
                    el.style.width = w + 'px';
                    el.style.left  = (startL + startW - w) + 'px';
                }
                if (dir.includes('n')) {
                    const h = Math.max(minH, startH - dy);
                    el.style.height = h + 'px';
                    el.style.top    = (startT + startH - h) + 'px';
                }
            };

            upHandler = () => {
                if (resizing) {
                    resizing = false;
                    setCursor(null);
                    // Remove document listeners after resize completes
                    document.removeEventListener('mousemove', moveHandler);
                    document.removeEventListener('mouseup', upHandler);
                }
            };

            document.addEventListener('mousemove', moveHandler);
            document.addEventListener('mouseup', upHandler);
        });
    },

    enableDrag: function (elementId, handleId) {
        const el = document.getElementById(elementId);
        const handle = document.getElementById(handleId);
        if (!el || !handle) return;
        // Prevent duplicate listeners on the same handle element
        if (handle.dataset.dragEnabled) return;
        handle.dataset.dragEnabled = 'true';

        let dragging = false;
        let offsetX = 0, offsetY = 0;
        let moveHandler, upHandler;

        handle.addEventListener('mousedown', (e) => {
            if (el.classList.contains('maximized')) return;
            dragging = true;
            // Use getBoundingClientRect so CSS transform/calc positioning is accounted for,
            // then lock in explicit left/top and clear any transform so dragging works correctly.
            const rect = el.getBoundingClientRect();
            el.style.transform = 'none';
            el.style.left = rect.left + 'px';
            el.style.top  = rect.top  + 'px';
            offsetX = e.clientX - rect.left;
            offsetY = e.clientY - rect.top;
            e.preventDefault();

            // Attach document listeners only during active drag
            moveHandler = e => {
                if (!dragging) return;
                el.style.left = (e.clientX - offsetX) + 'px';
                el.style.top  = (e.clientY - offsetY) + 'px';
            };

            upHandler = () => {
                dragging = false;
                // Remove document listeners after drag completes
                document.removeEventListener('mousemove', moveHandler);
                document.removeEventListener('mouseup', upHandler);
            };

            document.addEventListener('mousemove', moveHandler);
            document.addEventListener('mouseup', upHandler);
        });
    }
};
