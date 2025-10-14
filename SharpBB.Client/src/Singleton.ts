import {BbsUrl, DefaultLanguage, Language} from "./Configuration";
import ILanguage from "./Languages/ILanguage";
import zh_CN from "./Languages/zh_CN";
import en_US from "./Languages/en_US";

let Lang: ILanguage | null = null;
switch(DefaultLanguage) {
    case Language.auto:
        switch(navigator.language){
            case "zh-CN":
                Lang = zh_CN;
                break;
            case "en-US":
                Lang = en_US;
        }
        break;
    case Language.en_US:
        Lang = en_US
        break;
    case Language.zh_CN:
        Lang = zh_CN;
        break; 
}
const UserInformation: {
    username: string, email: string, userrole: number
} | null = (await fetch(BbsUrl + "/api/bbs/userapi")).ok ? {
    username: await (await fetch(BbsUrl + "/api/bbs/userapi?requestType=0")).text(),
    email: await (await fetch(BbsUrl + "/api/bbs/userapi?requestType=1")).text(),
    userrole: Number.parseInt(await (await fetch(BbsUrl + "/api/bbs/userapi?requestType=5")).text())
} : null
const ServerInformation: {
    bbsName: string, allowEmailLogin: boolean,
} | null = null;
export { Lang, UserInformation, ServerInformation};
