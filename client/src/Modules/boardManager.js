import { getToken } from "./authManager";

const _apiUrl = "/api/Boards";

export function getAllBoards(){
    return getToken().then(token => {
        return fetch(`${_apiUrl}/Get`, {
            method: "GET",
            headers: {
                authorization: `Bearer ${token}`
            }
        }).then(resp => resp.json())
    })
}
export function getWithPosts(id){
    return getToken().then(token => {
        return fetch(`${_apiUrl}/GetWithPosts/${id}`, {
            method: "GET",
            headers: {
                authorization: `Bearer ${token}`
            }
        })
    })
}

export function getById(id){
    return getToken().then(token => {
        return fetch(`${_apiUrl}/GetById/${id}`,
        {
            method: "GET",
            headers: {
                authorization: `Bearer ${token}`
            }
        })
    }).then(resp => resp.json());
}