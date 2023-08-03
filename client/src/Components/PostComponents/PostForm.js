import { useEffect, useState } from "react"
import { Button, Card, CardBody, CardFooter, Spinner } from "reactstrap";
import { addPost } from "../../Modules/postManager";

export default function PostForm({BoardId, loggedInUser, retrieveBoard, setNewPost}){
    const [postState, setPostState] = useState(null);
    const [postTitle, setTitle] = useState("")
    const [postContent, setContent] = useState("")

    useEffect(() => {
        if(loggedInUser && BoardId){
            setPostState({userId: loggedInUser.id, boardId: BoardId})
        }
    },[BoardId, loggedInUser])

    function buttonClickHandler(e){
        postState.title = postTitle
        postState.content = postContent;
        e.preventDefault();
        if(postState.content === "" || postState.title === ""){
            alert("Both the title and the content must be filled out!")
        }
        try{
            addPost(postState).then(() => retrieveBoard())
            setContent("")
            setTitle("")
            setNewPost(false);
        }
        catch(ex){
            alert(ex.message);
        }
    }


    return <Card>
        <CardBody>
                <input type="text" placeholder="Write your post's title here..." value={postTitle} onChange={e =>
                    setTitle(e.target.value)
                } style={{width:"100%", height:"50%"}}/>
                <input type="text" placeholder="Write your post's content here..." value={postContent} onChange={e =>
                    setContent(e.target.value)
                } style={{width:"100%", height:"50%"}}/>

        </CardBody>
        <CardFooter>
            <Button color="warning" type="button" onClick={(e) => {buttonClickHandler(e)} }>Create Post</Button>
            <Button color="danger" onClick={() => setNewPost(false)}>Cancel New Post</Button>
        </CardFooter>
    </Card>
}