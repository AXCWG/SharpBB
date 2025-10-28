import {Component, createEffect, createResource, createSignal, For, Show, Suspense} from 'solid-js';
import {Lang, ServerInformation, UserInformation} from "./Singleton";
import {BbsUrl, Routes} from "./Configuration";
import {  createMediaQuery } from '@solid-primitives/media';
import BoardGroupReturnResult from "./Types/BoardGroupReturnResult";

const Announces =()=>{
    return <>
        <div class={"h-50 bg-base-200 rounded-lg"}></div>
    </>;
}
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
	const [data, {mutate, refetch}] = createResource<BoardGroupReturnResult[]>(async ()=>{
		let res=  await fetch(BbsUrl + "/api/bbs/boardgroup/get")
		if(res.ok){
			let json =  (await res.json()) as BoardGroupReturnResult[];
			for (let boardGroupReturnResult of json) {
				for (let board of boardGroupReturnResult.boards) {
					board.latestActivity.by = await (await fetch(BbsUrl + "/api/bbs/user/getnamefast?uuid=" + board.latestActivity.by)).text()
				}
			}

			return json
		}else{
			throw Error(`Unable to fetch boards ${res.status}`);
		}
	})
	const isDesktop = createMediaQuery("(min-width: 640px)");

  return (
	  <>
		  <Show when={isDesktop()}>
			  <>
				  <img src={BbsUrl+"/api/bbs/conf/ico"} class={"w-100"}/>
				  <Suspense fallback={<div><span class={"loading loading-spinner"}></span></div>}>
					  <For each={data()}>
						  {(item, index)=>{
							  return <>
								  <table class={"table table-fixed"}>
									  <thead>
									  <tr>
										  <td colspan={4} class={"text-center"}>
											  {item.title} - {item.description}
										  </td>
									  </tr>

									  </thead>
									  <tbody>
									  <For each={item.boards}>
										  {(item, index)=>{
											  return <>
												  <tr>
													  <td><span class={"text-xl"}>{item.name}</span><br/>{item.description}</td>
													  <td>{item.topicCount} Topics</td>
													  <td>{item.repliesCount} Replies</td>
													  <td>Latest activity in: {item.latestActivity.title}<br/>By: {item.latestActivity.by}</td>
												  </tr>
											  </>
										  }}
									  </For>

									  </tbody>
								  </table>
							  </>
						  }}
					  </For>

				  </Suspense>

			  </>
		  </Show>
		  <Show when={!isDesktop()}>
			  <>mobile</>
		  </Show>
	  </>

  );
};

export default ForumIndex;
