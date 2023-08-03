import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { Button, Spinner } from "reactstrap";
import { getWithPosts } from "../../Modules/boardManager";
import { Board } from "./Board";
import Post from "../PostComponents/Post";
import PostForm from "../PostComponents/PostForm";
import { BsFillPlusSquareFill } from "react-icons/bs";

export default function BoardDetails({loggedInUser}){
    const [board, setBoard] = useState(null);
    const {id} = useParams();
    const [error, setError] = useState(null);
    const [newPostCreation, setNewPost] = useState(false);

    useEffect(() => {
        getThisBoard()
    },[])

    function getThisBoard(){
        try{
            getWithPosts(id).then(resp => {
                if(resp.ok){
                    setError(null)
                    resp.json()
                    .then(boardWithPosts => {setBoard(boardWithPosts)})
                }
                else{
                    setError(true);
                    setBoard({});
                }
            })
        }
        catch{
            setError(true);
            setBoard({})
        }
    }

    if(board == null){
        return <Spinner className="app-spinner dark"/>
    }
    if(error){
        return <h1>Either you aren't allowed to view this page, or it doesn't exist! Maybe try again later, or join the correct Pack!</h1>
    }

    //make a post element, please
    //Add button needs added pls
    return <>
        <Board board={board} onDetails={true} loggedInUser={loggedInUser}/>
        <Button color="warning" onClick={() => {setNewPost(!newPostCreation)} }><BsFillPlusSquareFill fontSize={"24px"}/></Button>
        {
            newPostCreation === true ?
            <>
                <PostForm BoardId={board.id} loggedInUser={loggedInUser} retrieveBoard={getThisBoard} setNewPost={setNewPost}/>
            </>
            :
            ""
        }
        {
            board.posts.length > 0 ? 
            <>
                {board.posts.map(post => {
                    return <Post key={`${post.id}`} post={post} board={board} loggedInUser={loggedInUser} retrievePost={getThisBoard}/>
                })}
            </>
            :
            <h3>Board has no posts currently, feel free to create one!</h3>
        }
    </>
}