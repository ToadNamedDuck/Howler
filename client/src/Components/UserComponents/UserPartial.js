//This is like one you stick on a comment, or a board, etc.
//Display should probably be set to flex on the parent. This component needs to be display: inline-block. 

import { useEffect, useState } from "react";
import { Spinner } from "reactstrap";
import { getById } from "../../Modules/userManager";
import { Link } from "react-router-dom";

export default function UserPartial({userId}){
    const [user, setUser] = useState({id: 0, profilePictureUrl:""});

    useEffect(() => {
        if(userId !== null){
            getById(userId).then(user => {
                if(user.profilePictureUrl === null){
                    user.profilePictureUrl = `https://robohash.org/${user.userName}?set=set4`
                }
                setUser(user)})

        }
    }
    ,[])

    const [userDate, setDate] = useState(null);


    useEffect(() => {
        if(user.dateCreated){
            setDate(new Date(user.dateCreated).toLocaleDateString())
        }
    },[user])

    if(user === null){
        return <Spinner className="app-spinner dark"/>
    }
    return <div className="userPartial display-flex column justify-center">
        {user.isBanned === true ? <h6>BANNED</h6> : ""}
        {user.profilePictureUrl.includes("wikia.nocookie") ? <img src={user.profilePictureUrl} crossOrigin="anonymous" referrerPolicy="no-referrer" alt={user.displayName} height="75px" width="75px"/> 
        : <img src={user.profilePictureUrl} width="75px" height="75px" alt={user.displayName}/>}
        <h4><Link to={`/users/${user.id}`}>{user.displayName}</Link></h4>
        {userDate !== null ? <p>Joined: {userDate}</p> : ""}
    </div>
}