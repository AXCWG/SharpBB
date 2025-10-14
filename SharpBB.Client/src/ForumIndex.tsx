import type { Component } from 'solid-js';
import {Lang, UserInformation} from "./Singleton";

const ForumIndex: Component = () => {
  return (
    <>
      <div>{UserInformation ? <>{UserInformation.username} - {UserInformation.userrole} - {UserInformation.email}</> : "null"}</div>
      <a href={"login"} class={"link-primary"}>{Lang?.login}</a>
    </>
  );
};

export default ForumIndex;
