import { Button, Card, CardBody, CardFooter, CardHeader, Spinner } from "reactstrap";
import UserPartial from "../UserComponents/UserPartial";
import { deleteComment, editComment } from "../../Modules/commentManager";
import { useEffect, useState } from "react";
import { BsFillTrashFill } from "react-icons/bs";
import { BsPencil } from "react-icons/bs"


export default function Comment({ comment, board, loggedInUser, retrievePost }) {
    const commentDate = new Date(comment.createdOn).toLocaleDateString()

    const [editState, setEdit] = useState(null);
    const [editText, setEditText] = useState("");

    useEffect(() => {
        setEditText(comment.content)
    }, [comment])

    const editButtonClickHandler = (e) => {
        e.preventDefault();
        const editedComment = { ...comment };
        editedComment.content = editText;
        if (!editedComment.content.length > 0) {
            alert("Comment's content must not be empty!")
            return;
        }
        try {
            editComment(comment.id, editedComment);
            retrievePost();
            setEdit(null);
        }
        catch (ex) {
            alert(ex.message)
        }
    }

    if (loggedInUser === null || comment === undefined || board === undefined || retrievePost === undefined) {
        return <Spinner className="app-spinner dark" />
    }

    return <Card className="flex-row-reverse">
        <CardFooter className="d-flex flex-column-reverse" style={ {width: "7%"} }>

            {
                loggedInUser !== null && loggedInUser.id === board.boardOwner.id || loggedInUser.id === comment.user.id ?
                    <Button color="danger" onClick={e => {
                        e.preventDefault();
                        deleteComment(comment.id);
                        retrievePost();
                    }}><BsFillTrashFill fontSize={"24px"}/></Button>
                    : ""
            }

            {
                loggedInUser !== null && loggedInUser.id === comment.user.id ?
                    <Button color="warning" onClick={e => {
                        e.preventDefault();
                        setEdit(true);
                    }}><BsPencil fontSize={"24px"}/></Button>
                    :
                    ""
            }
        </CardFooter>
        <CardBody style={ {width: "68%"} }>
            {
                editState === null ?
                    <h5>{comment.content}</h5>
                    :
                    <>
                        <input type="text" value={editText} onChange={e => { setEditText(e.target.value) }} style={{ width: "100%", height: "50%" }} />
                        <Button color="info" onClick={e => { editButtonClickHandler(e) }}>Submit Edit</Button>
                        <Button color="danger" onClick={() => { setEdit(null) }}>Cancel</Button>
                    </>
            }
            <h6>Comment made on: {commentDate}</h6>
        </CardBody>
        <UserPartial userInfo={comment.user} />
    </Card>
}