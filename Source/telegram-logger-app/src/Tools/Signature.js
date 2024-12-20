import CryptoJS from "crypto-js";

export default function sign(jsonData, userId, sessionCode) {
  var result = {};
  transformParams(jsonData, "", result);
  var sortedString = toSortedString(result);
  jsonData["ts"] = Date.now();
  var stringToSign = `userId:${userId}_ts:${jsonData["ts"]}_${sortedString}`;
  var hash = CryptoJS.HmacSHA256(stringToSign, sessionCode);
  console.log(stringToSign, sessionCode, hash.toString(CryptoJS.enc.Hex));
  jsonData["signature"] = hash.toString(CryptoJS.enc.Hex);
}

function transformParams(jsonData, parentKey = "", resultObject) {
  for (let key in jsonData) {
    let element = jsonData[key];
    let currentKey = parentKey !== "" ? `${parentKey}.${key}` : key;
    if (typeof element === "object")
      transformParams(element, currentKey, resultObject);
    else
      resultObject[currentKey] = element;
  }
}


function toSortedString(sourceObject) {
  var keys = Object.keys(sourceObject); 

  keys.sort();
  let resultString = keys.map((key) => `data.${key}=${sourceObject[key]}`).join('&');
  
  return resultString;
}
