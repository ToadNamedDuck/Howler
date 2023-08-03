import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { getUserWithPosts } from "../../Modules/userManager";
import { Button, Card, CardBody, CardFooter, CardHeader, Spinner } from "reactstrap";
import { getPackById } from "../../Modules/packManager";
import { Pack } from "../PackComponents/Pack";
import { BsPencilFill } from "react-icons/bs";
import UserEditForm from "./UserEditForm";
import ProfilePost from "../PostComponents/ProfilePost";

export default function UserProfile({ loggedInUser, userUpdater }) {
    const { id } = useParams();
    const [userWithPosts, setUser] = useState(null);
    const [userPack, setPack] = useState(null);
    const [editState, setEditState] = useState(false);

    function userRefresher(){
        getUserWithPosts(id).then(user => setUser(user)).then(() => {
            if(userWithPosts !== null && userWithPosts.profilePictureUrl === null){
                userWithPosts.profilePictureUrl = `https://robohash.org/${userWithPosts.userName}?set=set4`
            }
        })
    }

    useEffect(() => {
            userRefresher()
    }, [id])

    useEffect(() => {
        if(userWithPosts !== null){
            if(userWithPosts.packId !== null){
                getPackById(userWithPosts.packId).then(pack => setPack(pack))
            }
            else{
                setPack(null)
            }
        }
    },[userWithPosts])

    if(loggedInUser === undefined || loggedInUser === null || userWithPosts === null || userWithPosts === undefined){
        return <Spinner className="app-spinner dark"/>;
    }

    return <Card>
        {
            loggedInUser.id === userWithPosts.id ?
            <Button color="warning" onClick={() => setEditState(!editState)} style={ {width:"7%"} }><BsPencilFill fontSize={"24px"}/></Button>
            :
            ""
        }
        {
            editState !== false ?
            <UserEditForm loggedInUser={loggedInUser} userUpdater={userUpdater} editStateUpdater={setEditState} userId={id} userStateChange={userRefresher}/>
            :
            ""
        }
        <CardHeader className="d-flex flex-row" >
            {
                userWithPosts.profilePictureUrl !== null && userWithPosts.profilePictureUrl !== undefined && userWithPosts.profilePictureUrl.includes("wikia.nocookie")
                    ?
                    <img src={userWithPosts.profilePictureUrl} crossOrigin="anonymous" referrerPolicy="no-referrer" alt={userWithPosts.displayName} height="75px" width="75px" />
                    :
                    <img src={userWithPosts.profilePictureUrl} width="75px" height="75px" alt={userWithPosts.displayName} />
            }
            <h2>{userWithPosts.displayName}'s Profile</h2>
        </CardHeader>
        <CardBody>
            <h2>{userWithPosts.displayName}'s Pack</h2>
            {
                userPack !== null ?
                <div>
                    <Pack pack={userPack} loggedInUser={loggedInUser} userUpdater={userUpdater} retrievePack={userRefresher}/>
                </div>
                :
                <h4>{userWithPosts.displayName} ia not a member of a pack!</h4>
            }
        </CardBody>
        <CardFooter>
            <h3>{userWithPosts.displayName}'s Posts</h3>
            {
                userWithPosts.posts.length > 0 ?
                    userWithPosts.posts.map(post => <ProfilePost post={post} key={`profile-post-${post.id}`}/>)
                    :
                    <h5>User has no posts.</h5>
            }
        </CardFooter>
    </Card>
}