import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { GetWithComments } from "../../Modules/postManager";
import { Spinner } from "reactstrap";
import Post from "./Post";

export default function PostDetails(){
    const {id} = useParams();
    const [post, setPost] = useState(null);
    const [error, setError] = useState(null);

    useEffect(() => {
        GetWithComments(id).then(resp => {
            if(resp.ok){
                resp.json()
                .then(post => setPost(post))
            }
            else{
                setError(true);
                setPost({})
            }
        });
    },[id])

    if(post === null){
        return <Spinner className="app-spinner dark"/>
    }

    if(error){
        return <h1>Either you aren't allowed to view this page, or it doesn't exist! Maybe try again later, or join the correct Pack!</h1>
    }

    //add the add comment form to page
    return <>
        <Post post={post} onDetails={true}/>
        {
            post.comments?
            post.comments.map(comment => <p key={comment.id}>{comment.content}</p>)
            :
            <h3>No comments yet... feel free to add one!</h3>
        }
    </>
}