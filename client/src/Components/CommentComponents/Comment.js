import { Button, Card, CardBody, CardFooter, CardHeader } from "reactstrap";
import UserPartial from "../UserComponents/UserPartial";
import { deleteComment } from "../../Modules/commentManager";

export default function Comment({ comment, board, loggedInUser, retrievePost }) {
    const commentDate = new Date(comment.createdOn).toLocaleDateString()
    return <Card>
        <CardBody>
            <p>{comment.content}</p>
        </CardBody>
        <p>Comment made on: {commentDate}</p>
        <UserPartial userInfo={comment.user} />
        <CardFooter>
            {
                loggedInUser.id === board.boardOwner.id || loggedInUser.id === comment.user.id ?
                    <Button color="danger" onClick={e => {
                        e.preventDefault();
                        deleteComment(comment.id);
                        retrievePost();
                    }}>Delete</Button>
                    : ""
            }
        </CardFooter>
    </Card>
}