import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { Spinner } from "reactstrap";
import { getWithPosts } from "../../Modules/boardManager";
import { Board } from "./Board";
import Post from "../PostComponents/Post";

export default function BoardDetails(){
    const [board, setBoard] = useState(null)
    const {id} = useParams();

    useEffect(() => {
        getWithPosts(id).then(boardWithPosts => {setBoard(boardWithPosts)})
    },[id])

    if(board == null){
        return <Spinner className="app-spinner dark"/>
    }

    //make a post element, please
    return <>
        <Board board={board} onDetails={true}/>
        {
            board.posts ? <>
                {board.posts.map(post => {
                    return <Post key={`${post.id}`} post={post}/>
                })}
            </>
            :
            ""
        }
    </>
}