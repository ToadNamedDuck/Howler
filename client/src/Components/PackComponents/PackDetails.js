import { Card, Spinner } from "reactstrap";
import { Pack } from "./Pack";
import PackUserList from "./PackUserList";
import { useParams } from "react-router-dom";
import { useEffect, useState } from "react";
import { getPackById } from "../../Modules/packManager";

export default function PackDetails({loggedInUser}){
    const {id} = useParams();
    const [pack, setPack] = useState(null)

    useEffect(() => {
        getPackById(id).then(pack => setPack(pack))
    },[])

    if(pack === null || pack === undefined ||loggedInUser === null){
        return <Spinner className="app-spinner dark"/>;
    }

    return <Card>
        <Pack pack={pack} loggedInUser={loggedInUser} onDetails={true}/>
        <PackUserList packId={id}/>
    </Card>
}