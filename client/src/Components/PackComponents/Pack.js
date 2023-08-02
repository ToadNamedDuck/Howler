import { Card, CardBody } from "reactstrap";
import UserPartial from "../UserComponents/UserPartial";
import { Link } from "react-router-dom";

export function Pack({pack, loggedInUser, onDetails}){
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
    </Card>
}