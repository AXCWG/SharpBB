const BbsUrl:string = "http://localhost:5071"; // TODO For testing.

enum Language{
	auto,
	zh_CN,
	en_US
}
const DefaultLanguage : Language = Language.auto;

export {BbsUrl, Language, DefaultLanguage}