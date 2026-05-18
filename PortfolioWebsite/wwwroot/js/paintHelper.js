window.paintHelper = {
    _canvas: null,
    _ctx: null,
    _tool: 'draw',
    _color: '#000000',
    _size: 3,
    _drawing: false,
    _lastX: 0,
    _lastY: 0,

    init: function (canvasId, imageUrl) {
        const canvas = document.getElementById(canvasId);
        if (!canvas || canvas.dataset.paintInit) return;
        canvas.dataset.paintInit = 'true';
        this._canvas = canvas;
        this._ctx = canvas.getContext('2d');

        // White background
        this._ctx.fillStyle = '#ffffff';
        this._ctx.fillRect(0, 0, canvas.width, canvas.height);

        // Load pre-drawn image
        if (imageUrl) {
            const img = new Image();
            img.crossOrigin = 'anonymous';
            img.onload = () => {
                this._ctx.drawImage(img, 0, 0, canvas.width, canvas.height);
            };
            img.src = imageUrl;
        }

        canvas.addEventListener('mousedown', (e) => {
            e.preventDefault();
            const pos = this._getPos(e);
            if (this._tool === 'fill') {
                this._floodFill(Math.round(pos.x), Math.round(pos.y));
                return;
            }
            this._drawing = true;
            this._lastX = pos.x;
            this._lastY = pos.y;
            // Draw initial dot
            const ctx = this._ctx;
            ctx.beginPath();
            ctx.arc(pos.x, pos.y, this._effectiveSize() / 2, 0, Math.PI * 2);
            ctx.fillStyle = this._drawColor();
            ctx.fill();
        });

        canvas.addEventListener('mousemove', (e) => {
            if (!this._drawing) return;
            const pos = this._getPos(e);
            const ctx = this._ctx;
            ctx.beginPath();
            ctx.moveTo(this._lastX, this._lastY);
            ctx.lineTo(pos.x, pos.y);
            ctx.strokeStyle = this._drawColor();
            ctx.lineWidth = this._effectiveSize();
            ctx.lineCap = 'round';
            ctx.lineJoin = 'round';
            ctx.stroke();
            this._lastX = pos.x;
            this._lastY = pos.y;
        });

        canvas.addEventListener('mouseup', () => { this._drawing = false; });
        canvas.addEventListener('mouseleave', () => { this._drawing = false; });
    },

    setTool:  function (tool)  { this._tool = tool; },
    setColor: function (color) { this._color = color; },
    setSize:  function (size)  { this._size = parseInt(size); },

    loadImage: function (canvasId, url) {
        const canvas = document.getElementById(canvasId);
        if (!canvas) return;
        const ctx = canvas.getContext('2d');
        ctx.fillStyle = '#ffffff';
        ctx.fillRect(0, 0, canvas.width, canvas.height);
        if (url) {
            const img = new Image();
            img.crossOrigin = 'anonymous';
            img.onload = () => { ctx.drawImage(img, 0, 0, canvas.width, canvas.height); };
            img.src = url;
        }
    },

    _effectiveSize: function () {
        return this._tool === 'erase' ? this._size * 5 : this._size;
    },

    _drawColor: function () {
        return this._tool === 'erase' ? '#ffffff' : this._color;
    },

    _getPos: function (e) {
        const r = this._canvas.getBoundingClientRect();
        return {
            x: (e.clientX - r.left) * (this._canvas.width  / r.width),
            y: (e.clientY - r.top)  * (this._canvas.height / r.height)
        };
    },

    _floodFill: function (startX, startY) {
        const canvas = this._canvas;
        const ctx    = this._ctx;
        const w = canvas.width, h = canvas.height;
        if (w === 0 || h === 0) return;
        startX = Math.max(0, Math.min(w - 1, Math.round(startX)));
        startY = Math.max(0, Math.min(h - 1, Math.round(startY)));
        const imageData = ctx.getImageData(0, 0, w, h);
        const data = imageData.data;
        const fc   = this._hexToRgb(this._color);

        const base = (startY * w + startX) * 4;
        const tr = data[base], tg = data[base+1], tb = data[base+2], ta = data[base+3];

        if (tr === fc.r && tg === fc.g && tb === fc.b && ta === 255) return;

        const tolerance = 32;
        const matchesTarget = (pi) =>
            Math.abs(data[pi]   - tr) <= tolerance &&
            Math.abs(data[pi+1] - tg) <= tolerance &&
            Math.abs(data[pi+2] - tb) <= tolerance &&
            Math.abs(data[pi+3] - ta) <= tolerance;

        const visited = new Uint8Array(w * h);
        const stack   = [startX + startY * w];

        while (stack.length) {
            const p  = stack.pop();
            if (visited[p]) continue;
            const pi = p * 4;
            if (!matchesTarget(pi)) continue;
            visited[p] = 1;
            data[pi] = fc.r; data[pi+1] = fc.g; data[pi+2] = fc.b; data[pi+3] = 255;
            const x = p % w, y = Math.floor(p / w);
            if (x > 0)     stack.push(p - 1);
            if (x < w - 1) stack.push(p + 1);
            if (y > 0)     stack.push(p - w);
            if (y < h - 1) stack.push(p + w);
        }
        ctx.putImageData(imageData, 0, 0);
    },

    _hexToRgb: function (hex) {
        return {
            r: parseInt(hex.slice(1, 3), 16),
            g: parseInt(hex.slice(3, 5), 16),
            b: parseInt(hex.slice(5, 7), 16)
        };
    }
};
