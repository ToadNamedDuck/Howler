import { useEffect, useState } from "react";
import { addBoard, getAllBoards } from "../../Modules/boardManager";
import { Button, Card, CardFooter, Spinner } from "reactstrap";
import { Board } from "./Board";
import { BsFillPlusSquareFill } from "react-icons/bs";

export default function Boards({loggedInUser}){
    const [allBoards, setAllBoards] = useState(null);
    const [newBoardCreation, setNewBoard] = useState(false);
    const [newTitle, setNewTitle] = useState("");
    const [newTopic, setNewTopic] = useState("");
    const [newDescription, setNewDesc] = useState("");

    useEffect(() => {
        getBoards()
    },[])

    const getBoards = () => {
        getAllBoards().then(boards => setAllBoards(boards))
    }

    if(allBoards === null){
        return <Spinner className="app-spinner dark"/>
    }

    function newBoardSubmitClickHandler(e){
        e.preventDefault();
        if(!newTitle.length > 0, !newTopic.length > 0, !newDescription.length > 0){
            alert("All three fields must be filled in to create a board!")
        }
        else{
            const boardToSend = {
                name: newTitle,
                topic: newTopic,
                description: newDescription
            }
            boardToSend.boardOwnerId = loggedInUser.id
            try{
                addBoard(boardToSend).then(() => getBoards())
            }
            catch(ex){
                alert(ex.message)
            }
        }
    }

    return<>
        <h2>All Public Boards</h2>
        <Button color="warning" onClick={() => {setNewBoard(!newBoardCreation)} }><BsFillPlusSquareFill fontSize={"24px"}/></Button>
        {
            newBoardCreation === true ?
            <Card>
                <input type="text" placeholder="New board name..." onChange={(e) => setNewTitle(e.target.value)}/>
                <input type="text" placeholder="New board topic..." onChange={(e) => setNewTopic(e.target.value)}/>
                <input type="text" placeholder="New board description..." onChange={(e) => setNewDesc(e.target.value)}/>
                <CardFooter>
                    <Button color="danger" onClick={e => {setNewBoard(false)} }>Cancel</Button>
                    <Button color="warning" onClick={e => {newBoardSubmitClickHandler(e)} }>Create New Board</Button>
                </CardFooter>
            </Card>
            :
            ""
        }
        {allBoards.map(board => {
            return <Board board={board} getBoards={getBoards} loggedInUser={loggedInUser} key={`board-on-all-${board.id}`}/> 
        })}
    </>
}