import {Component, Show} from 'solid-js';
import {Lang, UserInformation} from "./Singleton";
import {BbsUrl, Routes} from "./Configuration";


const ForumIndex: Component = () => {
    const NavBar = ()=>{
        return <>
            <div class={"flex px-5 py-1 border-y-1 border-y-base-300 flex-row-reverse w-full"}>
                <Show when={UserInformation} fallback={<a class={" link-primary"} href={Routes.firstOrDefault(i=>i.identifier === "login")?.path}>{Lang?.login}</a>}>
                    <a class={" link-primary"} href={Routes.firstOrDefault(i=>i.identifier === "home")?.path}>{UserInformation?.username}</a>
                </Show>
            </div>
        </>
    }
  return (
    <>
        <NavBar/>
        <div class={"py-10 px-20 "}>
            <div class={"px-5 grid grid-cols-2 w-full justify-between"}>
                <img src={BbsUrl + "/api/bbs/conf/ico"} class={"h-27"}/>
                <div class={"p-2 w-100 bg-base-200 ml-auto h-full"}>
                    <Show when={!UserInformation}>
                        <>
                            aa
                        </>
                    </Show>
                </div>

            </div>
        </div>
    </>
  );
};

export default ForumIndex;
