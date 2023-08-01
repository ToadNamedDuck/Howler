import { Navigate, Route, Routes } from "react-router-dom";
import Home from "./Home";
import Login from "./Login";
import Register from "./Register";
import Header from "./Header";
import Boards from "./BoardComponents/Boards";
import BoardDetails from "./BoardComponents/BoardDetails";
import PostDetails from "./PostComponents/PostDetails";

export default function ApplicationViews({isLoggedIn, loggedInUser}){
    return <main style={{marginTop: 175}}>
            <Routes>
                <Route index path="/" element={ isLoggedIn ? <Home isLoggedIn={isLoggedIn} loggedInUser={loggedInUser}/> : <Navigate to="/login"/>}/>
                <Route path="/login" element={ isLoggedIn ? <Navigate to="/"/> : <Login/>}/>
                <Route path="/register" element={ isLoggedIn ? <Navigate to="/"/> : <Register/>}/>
                <Route path="/boards" element={ isLoggedIn ? <Boards loggedInUser={loggedInUser}/> : <Navigate to="/login"/>}/>
                <Route path="/boards/:id" element={ isLoggedIn ? <BoardDetails loggedInUser={loggedInUser}/> : <Navigate to="/login"/>}/>
                <Route path="/boards/:id/posts/:postId" element={ isLoggedIn ? <PostDetails loggedInUser={loggedInUser}/> : <Navigate to="/login"/>}/>
                <Route path="*" element={ isLoggedIn ? <p>Nothing here yet...</p> : <Login/>}/>
            </Routes>
    </main>
}