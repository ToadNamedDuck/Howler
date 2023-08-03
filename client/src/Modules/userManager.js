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

export function getUserWithPosts(id){
  return getToken().then(token => {
    return fetch(`${_apiUrl}/GetByIdWithPosts/${id}`, {
      method: "GET",
      headers:{
        Authorization: `Bearer ${token}`
      }
    })
  }).then(resp => resp.json())
}

export function editUser(id, userToSend){
  return getToken().then(token => {
    return fetch(`${_apiUrl}/Update/${id}`, {
      method: "PUT",
      headers:{
        Authorization: `Bearer ${token}`,
        "Content-Type": "application/json"
      },
      body: JSON.stringify(userToSend)
    })
  })
}