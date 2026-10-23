window.keyboardHandler = {

    _dotNetRef: null,
    _keydown_handler: null,
    _keypressed_handler: null,
    _keyup_handler: null,

    register: function (dotNetRef) {


        document.onkeypress = function (event) {
            dotNetRef.invokeMethodAsync('OnKeyPress', event.key, event.code);
        };
        //this._dotNetRef = dotNetRef;

        //this._keydown_handler = function (event) {
        //    dotNetRef.invokeMethodAsync('OnKeyDown', event.key, event.code);
        //};
        //this._keypressed_handler = function (event) {
        //    dotNetRef.invokeMethodAsync('OnKeyPress', event.key, event.code);
        //};
        //this._keyup_handler = function (event) {
        //    dotNetRef.invokeMethodAsync('OnKeyUp', event.key, event.code);
        //};

        
        //document.addEventListener('keydown', this._keydown_handler, true);
        //document.addEventListener('keypress', this._keypressed_handler, true);
        //document.addEventListener('keyup', this._keyup_handler, true);
    },

    unregister: function () {
        //if (this._keydown_handler) {
        //    document.removeEventListener('keydown', this._keydown_handler, true);
        //    this._keydown_handler = null;
        //}
        //if (this._keypressed_handler) {
        //    document.removeEventListener('keypress', this._keypressed_handler, true);
        //    this._keypressed_handler = null;
        //}
        //if (this._keyup_handler) {
        //    document.removeEventListener('keyup', this._keyup_handler, true);
        //    this._keyup_handler = null;
        //}
        //this._dotNetRef = null;
    }
};