import { Card, CardBody, CardHeader, Spinner } from "reactstrap"

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
            <Card>
                <CardHeader>
                    <h1 style={ {color:"var(--bs-warning)"} } className="text-center">🌕 Welcome to Howler 🐺</h1>
                </CardHeader>
                <CardBody>
                    <h3 style={ {color:"var(--bs-warning)"} }>About</h3>
                    <p style={ {fontSize:"24px"} }>
                        Howler is a site for werewolves (and werewolf enthusiasts) like us to be able to get together and discuss anything with our fellow lycanthropes.
                    </p>
                    <h3 style={ {color:"var(--bs-warning)"} }>Packs</h3>
                    <p style={ {fontSize:"24px"} }>Packs are private groups which you can join (or make) which have a private board. You can only be a member of 1 pack at any point, so be picky!</p>
                    <h3 style={ {color:"var(--bs-warning)"} }>Boards, Posts, Comments</h3>
                    <p style={ {fontSize:"24px"} }>Boards are a collection of posts which are related loosely by topic. A board owner can delete any post on their board, as well as any comments on any post on their board. Packs have private boards which cannot be deleted on their own, and are owner by the leader of the pack.</p>
                </CardBody>
            </Card>
        </>
    }
}