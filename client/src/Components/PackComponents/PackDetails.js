import { Card, Spinner } from "reactstrap";
import { Pack } from "./Pack";
import PackUserList from "./PackUserList";
import { Link, useParams } from "react-router-dom";
import { useEffect, useState } from "react";
import { getPackById } from "../../Modules/packManager";

export default function PackDetails({loggedInUser, userUpdater}){
    const {id} = useParams();
    const [pack, setPack] = useState(null)

    useEffect(() => {
        retrievePack()
    },[])

    const retrievePack = () => {
        getPackById(id).then(pack => setPack(pack))
    }

    if(pack === null || pack === undefined ||loggedInUser === null){
        return <Spinner className="app-spinner dark"/>;
    }

    return <Card>
        <Pack pack={pack} loggedInUser={loggedInUser} onDetails={true} userUpdater={userUpdater} retrievePack={retrievePack}/>
        <PackUserList pack={pack}/>
        <h4>For Members Only Board, Click <Link to={`/boards/${pack.primaryBoardId}`}>Here</Link></h4>
    </Card>
}