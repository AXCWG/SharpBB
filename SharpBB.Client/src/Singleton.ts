import {DefaultLanguage, Language} from "./Configuration";
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
    case zh_CN:
        Lang = zh_CN;
        break; 
}

export { Lang };
