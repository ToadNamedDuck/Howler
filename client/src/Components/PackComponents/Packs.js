import { useEffect, useState } from "react";
import { Button, Card, CardBody, CardHeader, Spinner } from "reactstrap";
import { getAllPacks } from "../../Modules/packManager";
import { Pack } from "./Pack";
import { BsFillPlusSquareFill } from "react-icons/bs";
import PackForm from "./PackForm";

export default function Packs({loggedInUser, userUpdater}){
    const [packs, setPacks] = useState(null)
    const [newPack, setNew] = useState(false);

    useEffect(() => {
        retrievePacks()
    },[])

    const retrievePacks = () => {
        getAllPacks().then(packs => setPacks(packs))
    }

    if(packs === null || loggedInUser === null){
        return <Spinner className="app-spinner dark"/>;
    }

    return <Card>
        {
            loggedInUser.packId === null ?
            <Button color="warning" onClick={() => {setNew(!newPack)} } style={ {width:"3%"} }><BsFillPlusSquareFill fontSize={"24px"}/></Button>
            :
            ""
        }
        {
            newPack === true && loggedInUser.packId === null ?
            <PackForm loggedInUser={loggedInUser} userUpdater={userUpdater} getAllPacks={retrievePacks} setAllState={setNew}/>
            :
            ""
        }
        <CardHeader>
            <h2>All Packs</h2>
        </CardHeader>
        <CardBody>
            {
                packs.map(pack => <Pack setAllState={setNew} userUpdater={userUpdater} loggedInUser={loggedInUser} retrievePack={retrievePacks} pack={pack} key={`pack-${pack.id}`}/>)
            }
        </CardBody>
    </Card>
}