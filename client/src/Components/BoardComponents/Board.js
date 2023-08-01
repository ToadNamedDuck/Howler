import { Card, CardBody, CardHeader, Spinner } from "reactstrap";
import UserPartial from "../UserComponents/UserPartial";
import { Link } from "react-router-dom";

export function Board({board, onDetails}){
    if(board === null){
        return <Spinner className="app-spinner dark"/>;
    }

    return <Card>
        {board.boardOwner ? <UserPartial userInfo={board.boardOwner}/> : ""}
        <CardHeader>
            {!onDetails ? <Link to={`${board.id}`}><h2>{board.name}</h2></Link> : <h2>{board.name}</h2>}
        </CardHeader>
        <CardBody>
            <p>{board.topic}</p>
            <p>{board.description}</p>
        </CardBody>
    </Card>
}