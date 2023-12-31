import { Button, Card, CardBody, CardFooter, CardHeader, Spinner } from "reactstrap";
import UserPartial from "../UserComponents/UserPartial";
import { Link } from "react-router-dom";
import { deleteBoard, editBoard, getById } from "../../Modules/boardManager";
import { BsFillTrashFill, BsPencil } from "react-icons/bs";
import { useEffect, useState } from "react";

export function Board({board, onDetails, loggedInUser, getBoards}){
    const [editState, setEdit] = useState(null);
    const [editTitle, setTitle] = useState("");
    const [editTopic, setTopic] = useState("");
    const [editDesc, setDesc] = useState("");
    const [boardToDisplay, setBoard] = useState(board);

    useEffect(() => {
        setTitle(board.name);
        setTopic(board.topic);
        setDesc(board.description)
    },[board])

    useEffect(() => {
        setBoard(board);
    },[])

    function editSubmitOnClick(e){
        e.preventDefault();
        if(editTitle === "" || editTopic === "" || editDesc === ""){
            alert("All three fields must be populated to edit a board!")
        }
        else{
            const editedBoard = {...board}
            editedBoard.name = editTitle;
            editedBoard.topic = editTopic;
            editedBoard.description = editDesc;
            try{
                editBoard(board.id, editedBoard)
                .then(() => {
                    if(getBoards !== undefined){
                        setBoard(editedBoard)
                        setEdit(null)
                        getBoards()
                    }
                    else{
                        getById(board.id).then(boardUpdated => setBoard(boardUpdated))
                        setEdit(null);
                    }
                })
            }
            catch(ex){
                alert(ex.message)
            }
        }
    }


    if(board === null || loggedInUser === null){
        return <Spinner className="app-spinner dark"/>;
    }



    return <Card className="d-flex flex-row">
        {boardToDisplay.boardOwner ? <UserPartial userInfo={boardToDisplay.boardOwner}/> : ""}
        <CardHeader className="text-center" style={ {width: "20%"} }>
            {
                !onDetails && editState === null ? <Link to={`${board.id}`}><h2>{boardToDisplay.name}</h2></Link> 
                : onDetails && editState === null ? <h2>{boardToDisplay.name}</h2>
                : <input type="text" value={editTitle} onChange={e => {setTitle(e.target.value)} }></input>
            }
        </CardHeader>
        <CardBody className="d-flex flex-column" style={ {width: "48%"} }>
            {
                editState === null ? <h3>{boardToDisplay.topic}</h3>
                :
                <input type="text" value={editTopic} onChange={e => {setTopic(e.target.value)} }></input>
            }
            {
                editState === null ? <h4>{boardToDisplay.description}</h4>
                :
                    <input type="text" value={editDesc} onChange={e => {setDesc(e.target.value)} }></input>
            }
            {
                editState !== null ?
                <div className="d-flex flex-row">
                    <Button color="danger" onClick={() => {setEdit(null)} }>Cancel Edit</Button>
                    <Button color="warning" onClick={(e) => {editSubmitOnClick(e)} }>Save Edit</Button>
                </div>
                    :
                    ""
            }
        </CardBody>
        <CardFooter className="d-flex flex-column-reverse" style={ {width: "7%"} } >
            {
                loggedInUser !== null && loggedInUser.id === boardToDisplay.boardOwnerId ?
                    <Button color="danger" onClick={e => {
                        e.preventDefault();
                        deleteBoard(board.id).then(() => {getBoards()});
                    }}><BsFillTrashFill fontSize={"24px"}/></Button>
                    : ""
            }
             {
                loggedInUser !== null && loggedInUser.id === boardToDisplay.boardOwnerId ?
                    <Button color="warning" onClick={e => {
                        e.preventDefault();
                        setEdit(true);
                    }}><BsPencil fontSize={"24px"}/></Button>
                    : ""
            }
        </CardFooter>
    </Card>
}