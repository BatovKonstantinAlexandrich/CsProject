window.blazorErpClient = {
    evalJs: function (js) {
        try {
            const result = eval(js);
            if (result === undefined || result === null)
                return '';
            if (typeof result === 'object')
                return JSON.stringify(result);
            return String(result);
        } catch (e) {
            return String(e);
        }
    },
    alert: function (text) {
        window.alert(text ?? '');
        return true;
    },
    confirm: function (text) {
        return window.confirm(text ?? '');
    },
    openUrl: function (url) {
        if (!url)
            return false;
        window.open(url, '_blank', 'noopener,noreferrer');
        return true;
    },
    dispatchEvent: function (id, type, message) {
        const el = document.getElementById(id);
        if (!el)
            return false;
        const detail = message ?? null;
        el.dispatchEvent(new CustomEvent(type, { detail: detail, bubbles: true }));
        return true;
    },
    addEventListener: function (id, type, handlerId) {
        const el = document.getElementById(id);
        if (!el)
            return false;
        el.addEventListener(type, function () {
            if (window.blazorErpClient._handlers && window.blazorErpClient._handlers[handlerId])
                window.blazorErpClient._handlers[handlerId]();
        });
        return true;
    },
    _handlers: {}
};
