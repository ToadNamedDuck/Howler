import { getToken } from "./authManager"

const _apiUrl = "/api/Users";

export const getById = (id) => {
   return getToken().then(token => {
        return fetch(`${_apiUrl}/GetById/${id}`,
        {
            method: "GET",
            headers: {
              Authorization: `Bearer ${token}`
            }
        }).then(resp => resp.json())
    })
}