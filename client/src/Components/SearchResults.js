import { useEffect, useState } from "react"
import { Search } from "../Modules/searchManager";
import { Card, CardBody, CardHeader, Spinner } from "reactstrap";
import Post from "./PostComponents/Post";
import { getAllBoards } from "../Modules/boardManager";
import Comment from "./CommentComponents/Comment";

export default function SearchResults({loggedInUser, searchQuery}){
    const [searchResults, setResults] = useState(null);
    const [boards, setBoards] = useState(null);
    const [errorState, setError] = useState(null);
    const defaultNoDeleteCommentBoard = {id: -1, boardOwnierId: -1, boardOwner: {id: -1} }
    //Get board by comment id would be awesome lol

    function retrieveResults(){
        setError(null);
        Search(searchQuery).then(resp => {
            if(resp.ok){
                resp.json().then(results => setResults(results)) 
            }
            else{
                setError(true)
            }
        }).then(() => getAllBoards().then(boards => setBoards(boards)))
    }

    useEffect(() => {
        retrieveResults();
    }, [searchQuery])

    if(errorState !== null){
        return <h2>An error has occurred with your search, please try again momentarily.</h2>
    }
    if(loggedInUser === null || loggedInUser === undefined || searchQuery === null || searchQuery === undefined || searchResults === null || boards === null){
        return <Spinner className="app-spinner dark"/>
    }
    if(searchQuery === ""){
        return <h2>Please make sure you have entered a valid search query.</h2>
    }

    return <Card>
        <CardHeader>
            <h2>Posts</h2>
        </CardHeader>
        <CardBody>
            {
                searchResults.posts.length > 0 ? 
                searchResults.posts.map(post => <Post post={post}
                    loggedInUser={loggedInUser}
                    board={boards.find(board => board.id === post.boardId)}
                    retrievePost={retrieveResults}
                    key={`search-result-post-${post.id}`}/>)
                :
                "There are no public posts that match your search criteria."
            }
        </CardBody>
        <CardHeader>
            <h2>Comments</h2>
        </CardHeader>
        <CardBody>
            {
                searchResults.comments.length > 0 ?
                    searchResults.comments.map(comment => <Comment board={defaultNoDeleteCommentBoard}
                        comment={comment}
                        retrievePost={retrieveResults}
                        loggedInUser={loggedInUser}
                        key={`search-result-comment-${comment.id}`}
                        />)
                :
                "There are no public comments that match your search criteria."
            }
        </CardBody>
    </Card>
}