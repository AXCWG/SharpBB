import {BbsUrl, DefaultLanguage, Language} from "./Configuration";
import ILanguage from "./Languages/ILanguage";

let Lang: ILanguage | null = null;
switch(DefaultLanguage) {
    case Language.auto:
        switch(navigator.language){
            case "zh-CN":
                Lang = (await import("./Languages/zh_CN")).default as unknown as ILanguage;
                break;
            default:
            case "en-US":
                Lang = (await (import("./Languages/en_US"))).default as unknown as ILanguage;
                break;

        }
        break;
    case Language.en_US:
        Lang = (await (import("./Languages/en_US"))).default as unknown as ILanguage
        break;
    case Language.zh_CN:
        Lang = (await import("./Languages/zh_CN")).default as unknown as ILanguage;
        break;
}
const UserInformation: {
    username: string, email: string, userrole: number, uuid: string
} | null = (await fetch(BbsUrl + "/api/bbs/user/userapi", {
    credentials: "include",
})).ok ? {
    username: await (await fetch(BbsUrl + "/api/bbs/user/userapi?requestType=0", {
        credentials: "include",
    })).json(),
    email: await (await fetch(BbsUrl + "/api/bbs/user/userapi?requestType=1", {
        credentials: "include",
    })).json(),
    userrole: Number.parseInt(await (await fetch(BbsUrl + "/api/bbs/user/userapi?requestType=5",{
        credentials: "include",
    })).json()),
    uuid: await(await fetch(BbsUrl + "/api/bbs/user/userapi?requestType=7", {
        credentials: "include",
    })).json()
} : null
const ServerInformation: {
    forumName: string, enableLoginWithEmail: boolean,
} | null = await (await fetch(BbsUrl + "/api/bbs/conf")).json();
// Logging
console.log(ServerInformation);
console.log(UserInformation);


export { Lang, UserInformation, ServerInformation};
