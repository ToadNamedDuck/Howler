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

export function deleteBoard(id){
    return getToken().then(token => {
        return fetch(`${_apiUrl}/Delete/${id}`, {
            method: "DELETE",
            headers: {
                authorization: `Bearer ${token}`
            }
        })
    })
}

export function editBoard(id, board){
    return getToken().then(token => {
        return fetch(`${_apiUrl}/Update/${id}`, {
            method: "PUT",
            headers: {
                authorization: `Bearer ${token}`,
                "Content-Type": "application/json"
            },
            body: JSON.stringify(board)
        })
    }).then(resp => {
        if(!resp.ok){
            resp.json()
        }
    })
}

export function addBoard(board){
    return getToken().then(token => {
        return fetch(`${_apiUrl}/Post`, {
            method: "POST",
            headers: {
                authorization: `Bearer ${token}`,
                "Content-Type": "application/json"
            },
            body: JSON.stringify(board)
        })
    }).then(resp => resp.json())
}