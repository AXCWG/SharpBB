import {Component, createSignal, Show} from 'solid-js';
import {Lang, ServerInformation, UserInformation} from "./Singleton";
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
    const [validation, setValidation] = createSignal<boolean>(false);
    const [loginForm, setLoginForm] = createSignal<{username: string, password: string}>({
        username: "", password: ""
    });

  return (
    <>
        {/*<NavBar/>*/}
        <div class={"py-10 xl:px-90 md:px-30 sm:px-10"}>
            <div class={"px-5 sm:grid sm:gap-4 sm:grid-cols-2 w-full "}>
                <div class={"flex flex-col justify-center"}>
                    <img alt={"ForumLogo"} src={BbsUrl + "/api/bbs/conf/ico"} class={"sm:w-auto sm:h-auto max-w-70"}/>

                </div>
                <div class={"p-2 sm:block hidden max-w-100  bg-base-200 rounded-lg h-full"}>
                    <Show when={!UserInformation}>
                        <div >
                            <div class={"flex flex-col gap-4 p-5 "}>
                                <input
                                    class={"input w-full"}
                                    placeholder={ServerInformation?.enableLoginWithEmail ? Lang?.usernameOrEmail : Lang?.username}
                                    id={"login-form-username"}/>

                                <input type={"password"}
                                    class={"input w-full"} placeholder={Lang?.password} id={"login-form-username"}/>
                                <button disabled={validation()} class={"btn btn-primary"}>{Lang?.login}</button>


                            </div>
                        </div>
                    </Show>
                </div>

            </div>
        </div>
    </>
  );
};

export default ForumIndex;
