import { useEffect, useState } from "react";
import { Card, CardBody, CardHeader, Spinner } from "reactstrap";
import { getAllPacks } from "../../Modules/packManager";
import { Pack } from "./Pack";

export default function Packs({loggedInUser}){
    const [packs, setPacks] = useState(null)

    useEffect(() => {
        getAllPacks().then(packs => setPacks(packs))
    },[])

    if(packs === null){
        return <Spinner className="app-spinner dark"/>;
    }

    return <Card>
        <CardHeader>
            <h2>All Packs</h2>
        </CardHeader>
        <CardBody>
            {
                packs.map(pack => <Pack loggedInUser={loggedInUser} pack={pack} key={`pack-${pack.id}`}/>)
            }
        </CardBody>
    </Card>
}