import { Button, Card, CardBody, CardFooter, Spinner } from "reactstrap";
import UserPartial from "../UserComponents/UserPartial";
import { Link, useNavigate } from "react-router-dom";
import { BsFillPersonDashFill, BsFillPersonPlusFill, BsFillTrashFill } from "react-icons/bs";
import { deletePack, joinPack, leavePack } from "../../Modules/packManager";

export function Pack({pack, loggedInUser, onDetails, retrievePack, userUpdater}){

    if(loggedInUser === null || loggedInUser === undefined || pack === null || retrievePack == null || userUpdater === null){
        return <Spinner className="app-spinner dark"/>;
    }

    const navigate = useNavigate();

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
        deletePack(pack.id).then(() => userUpdater()).then(() => navigate("/packs"))
    }

    return <Card className="d-flex flex-row">
        <UserPartial userInfo={pack.packLeader}/>
        <CardBody>
            {
                onDetails ?
                <h2>{pack.name}</h2>
                :
                <Link to={`/packs/${pack.id}`}><h2>{pack.name}</h2></Link>
            }
            <h3>{pack.description}</h3>
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
        </CardFooter>
    </Card>
}