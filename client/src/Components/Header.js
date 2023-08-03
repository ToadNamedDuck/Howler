import React, { useState } from 'react';
import { NavLink as RRNavLink } from "react-router-dom";
import {
    Card,
  CardBody,
  CardHeader,
  CardImg,
  Nav,
  NavItem,
  NavLink,
  Spinner
} from 'reactstrap';
import { logout } from '../Modules/authManager';

export default function Header({ isLoggedIn, loggedInUser }) {

  return (
    <div className='fixed-top'>
        <Card>
                <h1 style={{color:"var(--bs-warning)"}} className='text-center'>Howler</h1>

            <CardBody>
                <Nav className="mr-auto justify-content-around" >
                    { /* When isLoggedIn === true, we will render the Home link */}
                    {isLoggedIn &&
                    <>
                        <NavLink tag={RRNavLink} to="/"><h3>Home</h3></NavLink>
                        <NavLink tag={RRNavLink} to="/packs"><h3>View Packs</h3></NavLink>
                        <NavLink tag={RRNavLink} to="/boards"><h3>View Boards</h3></NavLink>
                        {loggedInUser === null ? "" : <NavLink tag={RRNavLink} to={`/users/${loggedInUser.id}`}><h3>My Profile</h3></NavLink>}
                    </>
                    }
                    {isLoggedIn &&
                    <>
                        <NavItem>
                        <a aria-current="page" className="nav-link" 
                            style={{ cursor: "pointer" }} onClick={logout}><h3>Logout</h3></a>
                        </NavItem>
                    </>
                    }
                    {!isLoggedIn &&
                    <>
                        <NavItem>
                        <NavLink tag={RRNavLink} to="/login"><h3>Login</h3></NavLink>
                        </NavItem>
                        <NavItem>
                        <NavLink tag={RRNavLink} to="/register"><h3>Register</h3></NavLink>
                        </NavItem>
                    </>
                    }
                </Nav>
            </CardBody>
        </Card>
    </div>
  );
}
