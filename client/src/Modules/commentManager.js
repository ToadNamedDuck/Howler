import { getToken } from "./authManager";

const _apiUrl = "/api/Comments"

export function postComment(comment) {
    return getToken().then(token => {
        return fetch(`${_apiUrl}/Post`, {
            method: "POST",
            headers: {
                authorization: `Bearer ${token}`,
                "Content-Type": "application/json"
            },
            body: JSON.stringify(comment),
        })
    }
    ).then(resp => resp.json())
}

export function deleteComment(commentId){
    return getToken().then(token => {
        return fetch(`${_apiUrl}/Delete/${commentId}`, {
            method: "DELETE",
            headers: {
                authorization: `Bearer ${token}`
            }
        })
    })
}

export function editComment(id, comment){
    return getToken().then(token => {
        return fetch(`${_apiUrl}/Update/${id}`, {
            method: "PUT",
            headers: {
                authorization: `Bearer ${token}`,
                "Content-Type": "application/json"
            },
            body: JSON.stringify(comment)
        })
    }).then(resp => {
        if(!resp.ok){
            resp.json()
        }
    })
}