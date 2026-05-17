window.dragHelper = {
    enableDrag: function (elementId, handleId) {
        const el = document.getElementById(elementId);
        const handle = document.getElementById(handleId);
        if (!el || !handle) return;

        let dragging = false;
        let offsetX = 0, offsetY = 0;

        handle.addEventListener('mousedown', (e) => {
            dragging = true;
            const rect = el.getBoundingClientRect();
            offsetX = e.clientX - rect.left;
            offsetY = e.clientY - rect.top;
            el.style.transition = 'none';
            e.preventDefault();
        });

        document.addEventListener('mousemove', (e) => {
            if (!dragging) return;
            el.style.left = (e.clientX - offsetX) + 'px';
            el.style.top = (e.clientY - offsetY) + 'px';
        });

        document.addEventListener('mouseup', () => {
            dragging = false;
        });
    }
};
