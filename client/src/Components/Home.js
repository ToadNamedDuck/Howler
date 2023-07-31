import { Spinner } from "reactstrap"
import UserPartial from "./UserComponents/UserPartial"

export default function Home({isLoggedIn, loggedInUser}){
    if(!isLoggedIn){
        return <h2>You shouldn't see this, ever.</h2>
    }
    if(loggedInUser === null){
        return <Spinner className="app-spinner dark"/>
    }
    if(isLoggedIn){
        return <>
            <h2>Hello {loggedInUser.displayName}!</h2>
            <UserPartial userId={loggedInUser.id}/>
            <p>a</p>
            <p>a</p>
            <p>a</p>
            <p>a</p>
            <p>a</p>
            <p>a</p>
            <p>a</p>
            <p>a</p>
            <p>a</p>
            <p>a</p>
            <p>a</p>
            <p>a</p>
            <p>a</p>
            <p>a</p>
            <p>a</p>
            <p>a</p>
            <p>a</p>
            <p>a</p>
            <p>a</p>
            <p>a</p>
            <p>a</p>
            <p>a</p>
        </>
    }
}