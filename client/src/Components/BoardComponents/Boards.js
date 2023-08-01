import { useEffect, useState } from "react";
import { getAllBoards } from "../../Modules/boardManager";
import { Spinner } from "reactstrap";
import { Board } from "./Board";

export default function Boards(){
    const [allBoards, setAllBoards] = useState(null);

    useEffect(() => {
        getAllBoards().then(boards => setAllBoards(boards))
    },[])

    if(allBoards === null){
        return <Spinner className="app-spinner dark"/>
    }

    return<>
        <h2>All Public Boards</h2>
        {allBoards.map(board => {
            return <Board board={board} key={`board-on-all-${board.id}`}/> 
        })}
    </>
}