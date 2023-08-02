import { getToken } from "./authManager";

const _apiUrl = "/api/Packs"

export function getAllPacks(){
    return getToken().then(token => {
        return fetch(`${_apiUrl}/Get`, {
            method: "GET",
            headers: {
                authorization: `Bearer ${token}`
            }
        }).then(resp => resp.json())
    })
}

export function getPackById(id){
    return getToken().then(token => {
        return fetch(`${_apiUrl}/GetById/${id}`, {
            method: "GET",
            headers: {
                authorization: `Bearer ${token}`
            }
        }).then(resp => resp.json())
    })
}

export function addPack(pack){
    return getToken().then(token => {
        return fetch(`${_apiUrl}/Post`, {
            method: "POST",
            headers: {
                authorization: `Bearer ${token}`,
                "Content-Type": "application/json"
            },
            body: JSON.parse(pack)
        }).then(resp => resp.json())
    })
}

export function deletePack(id){
    return getToken().then(token => {
        return fetch(`${_apiUrl}/Delete/${id}`, {
            method: "DELETE",
            headers: {
                authorization: `Bearer ${token}`
            }
        })
    })
}

export function editPack(id, pack){
    return getToken().then(token => {
        return fetch(`${_apiUrl}/Update/${id}`, {
            method: "PUT",
            headers: {
                authorization: `Bearer ${token}`
            },
            body: JSON.stringify(pack)
        })
    })
}