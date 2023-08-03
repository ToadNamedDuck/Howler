import { useEffect, useState } from "react";
import { addPack } from "../../Modules/packManager";
import { Button, Card, Spinner } from "reactstrap";

export default function PackForm({loggedInUser, getAllPacks, userUpdater, setAllState}){
    const [packState, setPackState] = useState(null);
    const [packName, setName] = useState("");
    const [packDescription, setDesc] = useState("")

    useEffect(() => {
        if(loggedInUser !== null || loggedInUser !== undefined){
            setPackState({packLeaderId: loggedInUser.id})
        }
    },[])

    if(loggedInUser === null || getAllPacks === null || userUpdater === null){
        return <Spinner className="app-spinner dark"/>;
    }

    function createPackButtonHandler(e){
        e.preventDefault();
        const packToSend = {...packState};
        packToSend.name = packName;
        packToSend.description = packDescription;
        if(packToSend.name === "" || packToSend.description === ""){
            alert("Cannot create a pack with an empty name or description!")
        }
        else{
            addPack(packToSend).then(() => userUpdater()).then(() => getAllPacks())
            setAllState(false)
        }
    }

    return <Card>
        <input type="text" value={packName} onChange={e => setName(e.target.value)} placeholder="New pack's name here..."/>
        <input type="text" value={packDescription} onChange={e => setDesc(e.target.value)} placeholder="New pack's description here..."/>
        <Button color="warning" onClick={e => {createPackButtonHandler(e)} }>Create New Pack</Button>
    </Card>
}