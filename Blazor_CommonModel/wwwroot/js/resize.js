window.registerViewportChangeCallback = (dotnetHelper) => {
    window.addEventListener('load', () => {
        dotnetHelper.invokeMethodAsync('OnResize', window.innerWidth, window.innerHeight);
    });
    window.addEventListener('resize', () => {
        dotnetHelper.invokeMethodAsync('OnResize', window.innerWidth, window.innerHeight);
    });

    dotnetHelper.invokeMethodAsync('OnResize', window.innerWidth, window.innerHeight);
}