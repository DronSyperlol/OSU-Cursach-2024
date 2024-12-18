import { http_post } from "../httpRequest";

const apiPath = "auth/"

async function logIn() {
    const methodName = "logIn";
    const requestUrl = process.env.REACT_APP_BACKEND_URL+apiPath+methodName;
    const requestData = {
        signature: "later..."
    };
    if (window.Telegram.WebApp.initData === "")
        requestData["initData"] = process.env.REACT_APP_DEV_INITDATA;
    else
        requestData["initData"] = window.Telegram.WebApp.initData;
    var response = await http_post(requestUrl, requestData, {userId: -1});
    return JSON.parse(response);
} 

async function ping(apiAuthData) {
    const methodName = "ping";
    const requestUrl = process.env.REACT_APP_BACKEND_URL+apiPath+methodName;
    var response = await http_post(requestUrl, undefined, {
        userId: apiAuthData.userId
    });
    return JSON.parse(response);
}

export default {logIn, ping} 