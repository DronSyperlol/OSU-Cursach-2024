import { http_post } from "../httpRequest";
import sign from "../signature";

const apiPath = "account/"
var apiAuthData = undefined;

function init(AuthData) {
    if (apiAuthData === undefined) {
        apiAuthData = AuthData;
    }
};
async function newAccount(phoneNumber) {
    const methodName = "newAccount";
    const requestUrl = process.env.REACT_APP_BACKEND_URL+apiPath+methodName;
    var bodyData = {
        phone: phoneNumber
    };
    sign(bodyData, apiAuthData.userId, apiAuthData.sessionCode);
    var response = await http_post(requestUrl, bodyData, {
        userId: apiAuthData.userId
    });
    return JSON.parse(response);
}

async function setCode(phoneNumber, code) {
    const methodName = "setCode";
    const requestUrl = process.env.REACT_APP_BACKEND_URL+apiPath+methodName;
    var bodyData = {
        phone: phoneNumber,
        code: code
    };
    sign(bodyData, apiAuthData.userId, apiAuthData.sessionCode);
    var response = await http_post(requestUrl, bodyData, {
        userId: apiAuthData.userId
    });
    return JSON.parse(response);
}

async function setPassword(phoneNumber, password) {
    const methodName = "setPassword";
    const requestUrl = process.env.REACT_APP_BACKEND_URL+apiPath+methodName;
    var bodyData = {
        phone: phoneNumber,
        password: password
    };
    sign(bodyData, apiAuthData.userId, apiAuthData.sessionCode);
    var response = await http_post(requestUrl, bodyData, {
        userId: apiAuthData.userId
    });
    return JSON.parse(response);
}

async function getMyAccounts() {
    const methodName = "getMyAccounts";
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
    newAccount, 
    setCode, 
    setPassword, 
    getMyAccounts
};

export default toExport
