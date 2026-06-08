window.faviconHelper = {
    setInitials: function (initials) {
        const size = 32;
        const canvas = document.createElement('canvas');
        canvas.width = size;
        canvas.height = size;
        const ctx = canvas.getContext('2d');

        ctx.fillStyle = '#008080';
        ctx.fillRect(0, 0, size, size);

        ctx.fillStyle = '#ffffff';
        ctx.font = 'bold 13px Arial';
        ctx.textAlign = 'center';
        ctx.textBaseline = 'middle';
        ctx.fillText(initials, size / 2, size / 2);

        const link = document.querySelector("link[rel~='icon']");
        if (link) link.href = canvas.toDataURL('image/png');
    }
};
