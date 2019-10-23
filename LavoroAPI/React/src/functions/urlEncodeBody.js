export function encodeBody(body){
	// 'body' is the body of data being sent in sd-client functions
    var formBody = [];
    for (var arg in body) {
	    var encodedKey = encodeURIComponent(arg);
	    var encodedValue = encodeURIComponent(body[arg]);
	    formBody.push(encodedKey + "=" + encodedValue);
    }
    formBody = formBody.join("&");
    return formBody
}