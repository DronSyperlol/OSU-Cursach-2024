import { http_post } from "../httpRequest";
import sign from "../signature";

const apiPath = "auth/"
var apiAuthData = undefined;

function init(AuthData) {
    if (apiAuthData === undefined) {
        apiAuthData = AuthData;
    }
};


// Methods:
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

async function ping() {
    const methodName = "ping";
    const requestUrl = process.env.REACT_APP_BACKEND_URL+apiPath+methodName;
    var bodyData = {};
    sign(bodyData, apiAuthData.userId, apiAuthData.sessionCode);
    var response = await http_post(requestUrl, bodyData, {
        userId: apiAuthData.userId
    });
    return JSON.parse(response);
}
const toExport = 
{
    init, 
    logIn, 
    ping
};
export default  toExport