import './App.css';
import React, { useState, useEffect } from 'react';
import { BrowserRouter as Router } from 'react-router-dom';
import { getUserDetails, onLoginStatusChange } from './Modules/authManager';
import { Spinner } from 'reactstrap';
import ApplicationViews from './Components/ApplicationViews';
import firebase from 'firebase';
import Header from './Components/Header';

function App() {
  const [isLoggedIn, setIsLoggedIn] = useState(null);
  const [loggedInUser, setLoggedInUser] = useState(null);
  //Search bar state
  const [searchQuery, setQuery] = useState("");//We want to make sure that if this is "" or null or undefined or whatever we dont send the req.
  //Both pieces need to go to the header (So the value of the bar can be searchQuery), and so it has the setter
  //only the searchQuery needs to go to application views. When you hit enter on the search bar, it should pass that state as a prop to results page.

  useEffect(() => {
    onLoginStatusChange(setIsLoggedIn);
  },[])

  function userUpdater(){
      getUserDetails(firebase.auth().currentUser.uid)
      .then(fullUserObject => {
        const userStripped = {
          id: fullUserObject.id,
          displayName: fullUserObject.displayName,
          profilePictureUrl: fullUserObject.profilePictureUrl,
          isBanned: fullUserObject.isBanned,
          packId: fullUserObject.packId
        };
        setLoggedInUser(userStripped);
      })
  }

  useEffect(() => {
    if(isLoggedIn){
      userUpdater()
    }
    else{
      setLoggedInUser(null);
    }
  }, [isLoggedIn])


  if(isLoggedIn == null){
    return <Spinner className="app-spinner dark"/>
  }
  
  return (
  <Router>
      <Header isLoggedIn={isLoggedIn} loggedInUser={loggedInUser} searchQuery={searchQuery} setQuery={setQuery}/>
      <ApplicationViews isLoggedIn={isLoggedIn} userUpdater={userUpdater} loggedInUser={loggedInUser} searchQuery={searchQuery}/>
  </Router>
  );

}


export default App;
