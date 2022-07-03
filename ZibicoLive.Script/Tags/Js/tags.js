var zbclive = function () {
    setTimeout(function () {
        $("body").prepend('<iframe frameborder="0" scrolling="no" src="{{url}}" id="livechat-frame-html" style="outline: none !important; visibility: visible !important; resize: none !important; box-shadow: none !important; overflow: visible !important; background: none transparent !important; opacity: 1 !important; top: auto !important; right: 24px !important; bottom: 0 !important; left: auto !important; border: 0px !important; min-height: 475px !important; min-width: auto !important; max-height: none !important; max-width: none !important; padding: 0px !important; margin: 0px !important; transition-property: none !important; transform: none !important; width: 350px !important; z-index: 999999 !important; cursor: auto !important; float: none !important; border-radius: unset !important; pointer-events: auto !important; display: block !important; height: 475px !important;position: fixed!important;font-size:12px!important;"></iframe>');

    }, 1500);
};
zbclive();