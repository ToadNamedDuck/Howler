import { Spinner } from "reactstrap"

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