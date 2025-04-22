import { http_post } from "../httpRequest";
import sign from "../signature";

const apiPath = "target/"
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


const toExport = 
{
    init, 
    updateTarget, 
};

export default  toExport
