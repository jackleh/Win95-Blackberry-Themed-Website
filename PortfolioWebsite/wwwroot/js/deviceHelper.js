window.deviceHelper = {
    isMobilePhone: function () {
        return window.innerWidth <= 820;
    },
    onViewportChange: function (dotnetRef, methodName, breakpoint) {
        const mq = window.matchMedia('(max-width: ' + breakpoint + 'px)');
        mq.addEventListener('change', function (e) {
            dotnetRef.invokeMethodAsync(methodName, e.matches);
        });
    }
};
