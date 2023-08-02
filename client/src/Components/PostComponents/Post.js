import { Button, Card, CardBody, CardFooter, Spinner } from "reactstrap";
import UserPartial from "../UserComponents/UserPartial";
import { Link, useNavigate } from "react-router-dom";
import { BsFillTrashFill, BsPencil } from "react-icons/bs";
import { deletePost, editPost } from "../../Modules/postManager";
import { useEffect, useState } from "react";

export default function Post({ post, onDetails, board, loggedInUser, retrievePost }) {
    const [editState, setEdit] = useState(false);
    const [editTitle, setTitle] = useState("")
    const [editContent, setContent] = useState("")
    const navigate = useNavigate();

    useEffect(() => {
        setTitle(post.title);
        setContent(post.content);
    }, [])

    function savePostEditClick(e) {
        e.preventDefault();
        const postCopy = { ...post }
        postCopy.title = editTitle;
        postCopy.content = editContent;
        if (!postCopy.title.length > 0 || !postCopy.content.length > 0) {
            alert("Cannot edit a comment and put a blank title or content!")
        }
        else {
            editPost(post.id, postCopy).then(() => retrievePost()).then(() => setEdit(false))
        }
    }

    if (board === null || post === null || loggedInUser === null || retrievePost === null) {
        return <Spinner className="app-spinner dark" />
    }

    return <Card className="flex-row">
        <UserPartial userInfo={post.user} />
        <CardBody>
            {
                !onDetails && !editState ?
                    <Link to={`posts/${post.id}`}><h2>{post.title}</h2></Link>
                    : onDetails && !editState ?
                        <h2>{post.title}</h2>
                        :
                        <div>
                            <input type="text" value={editTitle} onChange={(e) => { setTitle(e.target.value) }} />
                        </div>
            }
            {
                post.content.length >= 123 && !onDetails && !editState ?//post length greater than 123 and not on details and not in edit state
                    post.content.substring(0, 123) + "..."
                    : !onDetails && !editState && !post.content.length > 123 ? //not on details, not in edit state, post length less than 123
                        post.content
                        : onDetails && !editState ? //on details, not in edit state
                            post.content
                            : onDetails && editState ?
                                <>
                                    <input type="text" value={editContent} onChange={(e) => { setContent(e.target.value) }} style={{ width: "100%" }} />
                                    <Button color="danger" onClick={() => { setEdit(false) }}>Cancel Edit</Button>
                                    <Button color="warning" onClick={e => { savePostEditClick(e) }}>Save Edit</Button>
                                </>
                                : post.content
            }
        </CardBody>
        <CardFooter className="d-flex flex-column-reverse" style={{ width: "7%" }}>
            {
                post.user.id === loggedInUser.id || loggedInUser.id === board.boardOwnerId ?
                    <>
                        {
                            !onDetails
                                ?
                                <Button color="danger" onClick={e => {
                                    e.preventDefault();
                                    deletePost(post.id).then(() => { retrievePost() })
                                }}><BsFillTrashFill fontSize={"24px"} /></Button>

                                :
                                <Button color="danger" onClick={e => {
                                    e.preventDefault();
                                    deletePost(post.id).then(() => { navigate(`/boards/${board.id}`) })
                                }}><BsFillTrashFill fontSize={"24px"} /></Button>
                        }
                    </>
                    :
                    ""
            }
            {
                post.user.id === loggedInUser.id && onDetails ?
                    <Button color="warning" onClick={e => {
                        setEdit(!editState)
                    }}><BsPencil fontSize={"24px"} /></Button>
                    :
                    ""
            }
        </CardFooter>
    </Card>
}