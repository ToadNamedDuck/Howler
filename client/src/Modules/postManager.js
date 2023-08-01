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