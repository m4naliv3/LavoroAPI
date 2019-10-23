import { encodeBody } from "./functions/urlEncodeBody";

export async function commsClient(path, method = 'GET', args = null, noResponse = null){
    var BaseUrl = 'https://lavorochatapp.azurewebsites.net/api';
    var url = `${BaseUrl}/${path}`

    if(method === 'GET'){
        var data = await fetch(url, {mode: 'cors',
        headers: {'Access-Control-Allow-Origin':'*'}}).then(response =>{
            return response.json();
        })
        return data;
    }
    else if(method === 'POST'){
        console.log('posting to comms', args)
        const headers = new Headers({
            'Access-Control-Allow-Origin':'*',
            "Content-Type": "application/x-www-form-urlencoded",
            "Access-Control-Request-Method": "POST"
        })
        var body = encodeBody(args);
        var gotFromPost = await fetch(url, {
            mode: 'cors',
            method: 'POST', 
            headers:headers, 
            body: body
        }).then(response =>{
            if(noResponse === null){return response.text();}
        }).then(json => {
            if(noResponse === null){return JSON.parse(json);}
        })
        return gotFromPost;
    }
}