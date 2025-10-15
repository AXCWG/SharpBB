import {createSignal, Show} from "solid-js";
import {Lang, ServerInformation, UserInformation} from "./Singleton";
import {BiRegularLeftArrowAlt} from "solid-icons/bi";
import {BbsUrl} from "./Configuration";

const ForumLogin = ()=>{
	document.title = ServerInformation?.forumName ?? "null"
	enum PageStatus {
		FIRST_LOAD, LOGGING, ERROR, SUCCESS
	}
	const [form, setForm] = createSignal<{
		username: string,
		password: string
	}>({
		username: "", password: ""
	});
	const [pageStatus, setPageStatus] = createSignal<[PageStatus, string]>([PageStatus.FIRST_LOAD, ""]);
	return <>
		<div class={"hero  bg-base-200 min-h-screen"}>
			<div class={"hero-content sm:min-w-auto min-w-full"}>
				<div class={"flex flex-col gap-5 min-w-full"}>
					<h1 class={"text-5xl font-bold"}>{Lang?.login}</h1>
					<div class={"card bg-base-100 w-full max-w-sm sm:min-w-100 min-w-full shrink-0 shadow-2xl"}>
						<div class={"card-body"}>
							<fieldset class={"fieldset"}>

								<Show when={pageStatus()[0] === PageStatus.SUCCESS}
									  fallback={
									<Show when={pageStatus()[0] === PageStatus.ERROR}>
										  <div role="alert" class="alert alert-error">
											  <svg xmlns="http://www.w3.org/2000/svg"
												   class="h-6 w-6 shrink-0 stroke-current" fill="none"
												   viewBox="0 0 24 24">
												  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
														d="M10 14l2-2m0 0l2-2m-2 2l-2-2m2 2l2 2m7-2a9 9 0 11-18 0 9 9 0 0118 0z"/>
											  </svg>
											  <span>{Lang?.loginFailed}{pageStatus()[1]}</span>
										  </div>
									  </Show>
								}>
									<div role="alert" class="alert alert-success">
										<svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6 shrink-0 stroke-current"
											 fill="none" viewBox="0 0 24 24">
											<path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
												  d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"/>
										</svg>
										<span>{Lang?.loginSucceed}</span>
									</div>
								</Show>


								<a class={"btn btn-ghost btn-square"} onClick={() => {
									history.back()
								}}>
									<BiRegularLeftArrowAlt size={"2rem"}/>

								</a>
								<label class={"label"}>{ServerInformation?.enableLoginWithEmail ? Lang?.usernameOrEmail : Lang?.username}</label>
								<input class={"input min-w-full"} onChange={(e) => {
									setForm({
										...form(), username: e.target.value
									})
								}} value={form().username} placeholder={ServerInformation?.enableLoginWithEmail ? Lang?.usernameOrEmail :  Lang?.username }/>
								<label class={"label"}>{Lang?.password}</label>
								<input onKeyDown={(e)=>{
									if(e.key === "Enter"){
										document.getElementById("login-button")!.click()
									}

								}} onInput={(e)=>{
									console.log("changed")
									setForm({
										...form(), password: e.currentTarget.value
									})
								}}  value={form().password} class={"input min-w-full"} placeholder={Lang?.password}
									   type="password"/>
								<button id={"login-button"} class={"btn btn-primary mt-3"} disabled={pageStatus()[0] === PageStatus.LOGGING}
										onClick={async () => {
											setPageStatus([PageStatus.LOGGING, pageStatus()[1]]);
											let response = await fetch(BbsUrl + "/api/bbs/user/login", {
												method: "POST",
												body: JSON.stringify({
													username: form().username,
													password: form().password
												}),
												headers: {
													"Content-Type": "application/json"
												},
												credentials: "include"
											})
											if (response.ok) {
												setPageStatus([PageStatus.SUCCESS, pageStatus()[1]])
												setTimeout(() => {
													window.location.replace("/")
												}, 1000)


											} else if (response.status === 409){
												alert("Having two accounts with same email. ")
											}else{
												setPageStatus([PageStatus.ERROR, response.statusText])
											}

										}}>{pageStatus()[0] === PageStatus.LOGGING ?
									<span class="loading loading-spinner loading-md"></span>
									: Lang?.login}</button>
							</fieldset>
						</div>
					</div>
				</div>

			</div>
		</div>
	</>
}

export default ForumLogin;