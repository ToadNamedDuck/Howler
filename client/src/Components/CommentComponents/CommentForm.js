import { useEffect, useState } from "react"
import { postComment } from "../../Modules/commentManager";
import { Button, Card, CardBody, CardFooter, Spinner } from "reactstrap";

export default function CommentForm({PostId, loggedInUser, retrievePost}){
    const [commentState, setCommentState] = useState(null);
    const [commentContent, setContent] = useState("")

    useEffect(() => {
        if(loggedInUser && PostId){
            setCommentState({userId: loggedInUser.id, postId: PostId})
        }
    },[PostId, loggedInUser])

    function buttonClickHandler(e){
        commentState.content = commentContent;
        e.preventDefault();
        if(commentState.content === ""){
            alert("You can't post an empty comment!")
        }
        try{
            postComment(commentState).then((resp) => retrievePost())
            setContent("")
        }
        catch(ex){
            alert(ex.message);
        }
    }


    return <Card>
        <CardBody>
                <input type="text" placeholder="Write your comment here..." value={commentContent} onChange={e =>
                    setContent(e.target.value)
                } style={{width:"100%", height:"100%"}}/>

        </CardBody>
        <CardFooter>
            <Button color="warning" type="button" onClick={(e) => {buttonClickHandler(e)} }>Send Comment</Button>
        </CardFooter>
    </Card>
}