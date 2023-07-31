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

  useEffect(() => {
    onLoginStatusChange(setIsLoggedIn);
  },[])

  useEffect(() => {
    if(isLoggedIn){
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
    else{
      setLoggedInUser(null);
    }
  }, [isLoggedIn])


  if(isLoggedIn == null){
    return <Spinner className="app-spinner dark"/>
  }
  
  return (
  <Router>
      <Header isLoggedIn={isLoggedIn}/>
      <ApplicationViews isLoggedIn={isLoggedIn} loggedInUser={loggedInUser}/>
  </Router>
  );

}


export default App;
