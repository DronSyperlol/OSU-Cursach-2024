import { http_post } from "../httpRequest";
import sign from "../signature";

const apiPath = "target/"
const defaultLimit = 50;

var apiAuthData = undefined;

function init(AuthData) {
    if (apiAuthData === undefined) {
        apiAuthData = AuthData;
    }
};



// Methods:
async function updateTarget(phoneNumber, peerId, accessHash, enable) {
    const methodName = "updateTarget";
    const requestUrl = process.env.REACT_APP_BACKEND_URL+apiPath+methodName;
    var bodyData = {
        phoneNumber, 
        peerId, 
        accessHash, 
        enable
    };
    sign(bodyData, apiAuthData.userId, apiAuthData.sessionCode);
    var response = await http_post(requestUrl, bodyData, {
        userId: apiAuthData.userId
    });
    return JSON.parse(response);
}

async function getSavedLogs(phoneNumber, peerId, offsetId = 0, limit = defaultLimit) {
    const methodName = "getSavedLogs";
    const requestUrl = process.env.REACT_APP_BACKEND_URL+apiPath+methodName;
    var bodyData = {
        phoneNumber, 
        peerId, 
        offsetId, 
        limit
    };
    sign(bodyData, apiAuthData.userId, apiAuthData.sessionCode);
    var response = await http_post(requestUrl, bodyData, {
        userId: apiAuthData.userId
    });
    return JSON.parse(response);
}


const toExport = 
{
    init, 
    updateTarget, 
    getSavedLogs
};

export default  toExport
