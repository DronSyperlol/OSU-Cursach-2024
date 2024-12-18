export async function http_get(url, params = undefined) {
    return new Promise((resolve, reject) => {
        const xml = new XMLHttpRequest();
        xml.onreadystatechange = function () {
            if (this.readyState === XMLHttpRequest.DONE) {
                if (this.status === 200) {
                    resolve(xml.responseText);
                } 
                else {
                    reject(this);
                }
            }
            if (params !== undefined) {
                var strParams = new URLSearchParams(params).toString(); 
                xml.open("GET", url + '?' + strParams);
            }
            else {
                xml.open("GET", url, true);
            }
            xml.send(null);
        }
    });
}


export async function http_post(url, params = undefined, headers = undefined) {
    return new Promise((resolve, reject) => {
        const xml = new XMLHttpRequest();
        xml.onreadystatechange = function (response) {
            if (this.readyState === XMLHttpRequest.DONE) {
                if (this.status === 200)
                    resolve(xml.responseText);
                else 
                    reject(this);
            } 
        }
        xml.open("POST", url, true);
        xml.setRequestHeader("Accept", "application/json");
        xml.setRequestHeader("Content-Type", "application/json");
        if (headers !== undefined)
            for (const [key, value] of Object.entries(headers)) {
                xml.setRequestHeader(key, value);
            }
        xml.send(JSON.stringify(params));
    });
}