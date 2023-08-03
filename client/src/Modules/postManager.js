import { getToken } from "./authManager";

const _apiUrl = "/api/Posts"

export function GetWithComments(id){
    return getToken().then(token => {
        return fetch(`${_apiUrl}/GetWithComments/${id}`, {
            method: "GET",
            headers:{
                authorization: `Bearer ${token}`
            }
        })
    })
}

export function addPost(post){
    return getToken().then(token => {
        return fetch(`${_apiUrl}/Post`, {
            method: "POST",
            headers:{
                authorization: `Bearer ${token}`,
                "Content-Type": "application/json"
            },
            body: JSON.stringify(post)
        })
    }).then(resp => resp.json())
}

export function deletePost(postId){
    return getToken().then(token => {
        return fetch(`${_apiUrl}/Delete/${postId}`, {
            method: "DELETE",
            headers: {
                authorization: `Bearer ${token}`
            }
        })
    })
}

export function editPost(postId, post){
    return getToken().then(token => {
        return fetch(`${_apiUrl}/Update/${postId}`, {
            method: "PUT",
            headers: {
                authorization: `Bearer ${token}`,
                "Content-Type": "application/json"
            },
            body: JSON.stringify(post)
        })
    })
}