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
        }).then(resp => {
            if(resp.ok){
                return resp.json()
            }
        })
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
            body: JSON.stringify(pack)
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
                authorization: `Bearer ${token}`,
                "Content-Type": "application/json"
            },
            body: JSON.stringify(pack)
        })
    })
}

export function getPackMembers(packId){
    return getToken().then(token => {
        return fetch(`/api/Users/GetByPackId/?packId=${packId}`, {
            method: "GET",
            headers: {
                authorization: `Bearer ${token}`
            }
        }).then(resp => {
            if(resp.ok){
             return resp.json()
            }
        })
    })
}

export function joinPack(loggedInUser, packId){
    const userToSend = {...loggedInUser}
    userToSend.packId = packId;

    return getToken().then(token => {
        return fetch(`/api/Users/Update/${loggedInUser.id}`, {
            method: "PUT",
            headers: {
                authorization: `Bearer ${token}`,
                "Content-Type": "application/json"
            },
            body: JSON.stringify(userToSend)
        })
    })
}

export function leavePack(loggedInUser){
    const userToSend = {...loggedInUser}
    userToSend.packId = null;

    return getToken().then(token => {
        return fetch(`/api/Users/Update/${loggedInUser.id}`, {
            method: "PUT",
            headers: {
                authorization: `Bearer ${token}`,
                "Content-Type": "application/json"
            },
            body: JSON.stringify(userToSend)
        })
    })
}