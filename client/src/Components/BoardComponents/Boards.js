import { useEffect, useState } from "react";
import { getAllBoards } from "../../Modules/boardManager";
import { Spinner } from "reactstrap";
import { Board } from "./Board";

export default function Boards({loggedInUser}){
    const [allBoards, setAllBoards] = useState(null);

    useEffect(() => {
        getBoards()
    },[])

    const getBoards = () => {
        getAllBoards().then(boards => setAllBoards(boards))
    }

    if(allBoards === null){
        return <Spinner className="app-spinner dark"/>
    }

    return<>
        <h2>All Public Boards</h2>
        {allBoards.map(board => {
            return <Board board={board} getBoards={getBoards} loggedInUser={loggedInUser} key={`board-on-all-${board.id}`}/> 
        })}
    </>
}