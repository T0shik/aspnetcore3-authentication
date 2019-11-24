var extractTokens = function (address) {
    var returnValue = address.split('#')[1];
    var values = returnValue.split('&');

    for (var i = 0; i < values.length; i++) {
        var v = values[i];
        var kvPair = v.split('=');
        localStorage.setItem(kvPair[0], kvPair[1]);
    }

    window.location.href = '/home/index';
}


extractTokens(window.location.href);