import { Card, CardBody, CardHeader, Spinner } from "reactstrap";
import UserPartial from "../UserComponents/UserPartial";
import { Link } from "react-router-dom";

export function Board({board}){
    if(board === null){
        return <Spinner className="app-spinner dark"/>;
    }

    return <Card>
        {board.boardOwner ? <UserPartial userInfo={board.boardOwner}/> : ""}
        <CardHeader>
            <Link to={`${board.id}`}>{board.name}</Link>
        </CardHeader>
        <CardBody>
            <p>{board.topic}</p>
            <p>{board.description}</p>
        </CardBody>
    </Card>
}