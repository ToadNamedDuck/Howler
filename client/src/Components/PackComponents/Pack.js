import { Button, Card, CardBody, CardFooter, Spinner } from "reactstrap";
import UserPartial from "../UserComponents/UserPartial";
import { Link, useNavigate } from "react-router-dom";
import { BsFillPersonDashFill, BsFillPersonPlusFill, BsFillTrashFill, BsPencil } from "react-icons/bs";
import { deletePack, editPack, joinPack, leavePack } from "../../Modules/packManager";
import { useEffect, useState } from "react";

export function Pack({pack, loggedInUser, onDetails, retrievePack, userUpdater}){

    const navigate = useNavigate();
    const [edit, setEdit] = useState(false);
    const [editName, setName] = useState("");
    const [editDesc, setDesc] = useState("");

    useEffect(() => {
        setName(pack.name)
        setDesc(pack.description)
    },[])

    if(loggedInUser === null || loggedInUser === undefined || pack === null || retrievePack == null || userUpdater === null){
        return <Spinner className="app-spinner dark"/>;
    }

    function joinPackClickHandler(e){
        e.preventDefault();
        joinPack(loggedInUser, pack.id).then(() => {
            userUpdater()
        }).then(() => retrievePack())
    }

    function leavePackClickHandler(e){
        e.preventDefault();
        leavePack(loggedInUser).then(() => {
            userUpdater()
        }).then(() => retrievePack())
    }

    function deletePackClickHandler(e){
        e.preventDefault();
        if(onDetails){
            deletePack(pack.id).then(() => userUpdater()).then(() => navigate("/packs"))
        }
        else{
            deletePack(pack.id).then(() => userUpdater()).then(() => retrievePack())
        }
    }

    function saveButtonEditHandler(e){
        e.preventDefault();
        const packToSend = {...pack}
        packToSend.name = editName
        packToSend.description = editDesc

        if(!(packToSend.name.length > 0) || !(packToSend.description.length > 0)){
            alert("You can't edit a pack's name or description to be empty!")
        }
        else{
            editPack(pack.id, packToSend).then(() => retrievePack())
            setEdit(false)
        }
    }

    return <Card className="d-flex flex-row">
        <UserPartial userInfo={pack.packLeader}/>
        <CardBody>
            {
                onDetails && !edit ?
                <h2>{pack.name}</h2>
                : !onDetails && !edit ?
                <Link to={`/packs/${pack.id}`}><h2>{pack.name}</h2></Link>
                :
                <input type="text" value={editName} onChange={e => setName(e.target.value)} style={ {width:"50%"} }/>
            }
            {
                !edit ?
                <h3>{pack.description}</h3>
                :
                <div className="d-flex flex-column">
                    <input type="text" value={editDesc} onChange={e => setDesc(e.target.value)} style={ {width:"50%"} }/>
                    <Button color="danger" onClick={() => setEdit(false)} style={ {width:"25%"} }>Cancel Edit</Button>
                    <Button color="warning" onClick={e => saveButtonEditHandler(e) } style={ {width:"25%"} }>Save Edit</Button>
                </div>
            }
        </CardBody>
        <CardFooter>
            {
                loggedInUser.packId === null ?
                <Button color="warning" onClick={e => {joinPackClickHandler(e)} }><BsFillPersonPlusFill fontSize={"24px"}/> Join Pack</Button>
                :
                loggedInUser.packId === pack.id && pack.packLeaderId === loggedInUser.id ?
                <Button color="danger" onClick={e => {deletePackClickHandler(e)} }><BsFillTrashFill fontSize={"24px"}/></Button>
                :
                loggedInUser.packId === pack.id && pack.packLeaderId !== loggedInUser.id ?
                <Button color="danger" onClick={e => {leavePackClickHandler(e)} }><BsFillPersonDashFill fontSize={"24px"}/> Leave Pack</Button>
                :
                ""
            }
            {
                loggedInUser.id === pack.packLeaderId ?
                <Button color="warning" onClick={e => {setEdit(!edit)} }><BsPencil fontSize={"24px"} /></Button>
                :
                ""
            }
        </CardFooter>
    </Card>
}