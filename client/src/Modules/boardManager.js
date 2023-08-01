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