import {lazy} from "solid-js";
import ForumIndex from "./ForumIndex";
import Enumerable from "linq";

const BbsUrl:string = ""; // TODO For testing.

enum Language{
	auto,
	zh_CN,
	en_US
}
// DO NOT CHANGE THE IDENTIFIER. The identifier is used for the website to track the routes.
const Routes = Enumerable.from([
	{identifier: "index", path: "/", component: lazy(() => import("./ForumIndex"))},
	{identifier: "login", path: "/login", component: lazy(()=>import("./ForumLogin"))},
	{identifier: "home", path: "/home", component: lazy(()=>import("./ForumUserHome"))},
	{identifier: "board-view", path: "/board-view", component: null}
])
const DefaultLanguage : Language = Language.auto;

export {BbsUrl, Language, DefaultLanguage, Routes}