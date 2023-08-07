import { getToken } from "./authManager"

const _apiUrl = "/api/Search"

export function Search(q){
    return getToken().then(token => {
        return fetch(`${_apiUrl}?q=${q}&latestFirst=true`, {
            method: "GET",
            headers: {
                authorization: `Bearer ${token}`
            }
        })
    })
}