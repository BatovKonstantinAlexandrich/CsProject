var interval = null;
window.registerWaitingStatusChangeCallback = (dotnetHelper, timeout) => {    
    console.log(`timeout: ${timeout}`);
    if (interval) {
        window.clearInterval(interval);
    }        
    interval = window.setInterval(function () {
        dotnetHelper.invokeMethodAsync('OnCheckout');    
    }, timeout);
}
window.startInitCallback = (dotnetHelper, timeout) => {             
    dotnetHelper.invokeMethodAsync('OnStartInit');
}

window.removeWaitingStatusChangeCallback = () =>
{
    if (interval)
    {
        window.clearInterval(interval);
        interval = null;
    }    
}

window.downloadFileFromStream = async (fileName, contentStreamReference) => {
    const arrayBuffer = await contentStreamReference.arrayBuffer();
    const blob = new Blob([arrayBuffer]);
    const url = URL.createObjectURL(blob);
    const anchorElement = document.createElement('a');
    anchorElement.href = url;
    anchorElement.download = fileName ?? '';
    anchorElement.click();
    anchorElement.remove();
    URL.revokeObjectURL(url);
}