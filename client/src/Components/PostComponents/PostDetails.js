import { useEffect, useState } from "react";
import { Navigate, useNavigate, useParams } from "react-router-dom";
import { GetWithComments } from "../../Modules/postManager";
import { Button, Card, CardBody, CardHeader, Spinner } from "reactstrap";
import Post from "./Post";
import Comment from "../CommentComponents/Comment";
import CommentForm from "../CommentComponents/CommentForm";
import { getById } from "../../Modules/boardManager";
import { BsArrowLeft } from "react-icons/bs";

export default function PostDetails({loggedInUser}){
    const {id, postId} = useParams();
    const [post, setPost] = useState(null);
    const [error, setError] = useState(null);
    const [board, setBoard] = useState(null)

    const retrievePost = () => {
        GetWithComments(postId).then(resp => {
            if(resp.ok){
                setError(null)
                resp.json()
                .then(post => setPost(post))
            }
            else{
                setError(true);
                setPost({})
            }
        });
    }

    useEffect(() => {
        getById(id).then(board => setBoard(board))
    },[])

    const navigate = useNavigate();

    useEffect(() => {
        retrievePost();
    },[postId])

    const sortCommentsByDate = () => {
        post.comments = post.comments.sort(function(a,b){
            // Turn your strings into dates, and then subtract them
            // to get a value that is either negative, positive, or zero.
            //Thanks Phrogz on stack overflow!!!!
            return new Date(b.createdOn) - new Date(a.createdOn);
          })
        return post.comments.map(comment => <Comment retrievePost={retrievePost} comment={comment} board={board} loggedInUser={loggedInUser} key={"comment"+comment.id+`${post.id}`}/>)
    }

    if(post === null){
        return <Spinner className="app-spinner dark"/>
    }

    if(error){
        return <h1>Either you aren't allowed to view this page, or it doesn't exist! Maybe try again later, or join the correct Pack!</h1>
    }

    //add the add comment form to page
    return <>
        <Button color="warning" onClick={e => {
            e.preventDefault();
            navigate(`/boards/${id}`)
        } }><BsArrowLeft fontSize={"24px"}/>Go Back</Button>
        <Post post={post} onDetails={true} board={board} loggedInUser={loggedInUser} retrievePost={retrievePost}/>
        <Card>
            <CardHeader><h2>Comments</h2></CardHeader>
            <CardBody>
                {
                    post.comments.length > 0?
                    <>
                        <CommentForm PostId={postId} loggedInUser={loggedInUser} retrievePost={retrievePost}/>
                        {
                            sortCommentsByDate()
                        }
                    </>
                    :
                    <>
                        <h3>No comments yet... feel free to add one!</h3>
                        <CommentForm PostId={postId} loggedInUser={loggedInUser} retrievePost={retrievePost}/>
                    </>
                }
            </CardBody>
        </Card>
    </>
}