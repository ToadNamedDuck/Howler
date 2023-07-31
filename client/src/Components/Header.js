import React, { useState } from 'react';
import { NavLink as RRNavLink } from "react-router-dom";
import {
    Card,
  CardBody,
  CardImg,
  Nav,
  NavItem,
  NavLink
} from 'reactstrap';
import { logout } from '../Modules/authManager';

export default function Header({ isLoggedIn }) {

  return (
    <div className='fixed-top'>
        <Card>
            <CardImg top src="logo192.png" height="100px" width="auto"/>
            <CardBody>
                <Nav className="mr-auto justify-content-around" >
                    { /* When isLoggedIn === true, we will render the Home link */}
                    {isLoggedIn &&
                    <>
                        <NavLink tag={RRNavLink} to="/">Home</NavLink>
                    </>
                    }
                    {isLoggedIn &&
                    <>
                        <NavItem>
                        <a aria-current="page" className="nav-link" 
                            style={{ cursor: "pointer" }} onClick={logout}>Logout</a>
                        </NavItem>
                    </>
                    }
                    {!isLoggedIn &&
                    <>
                        <NavItem>
                        <NavLink tag={RRNavLink} to="/login">Login</NavLink>
                        </NavItem>
                        <NavItem>
                        <NavLink style={{color: '#d4c81d'}} tag={RRNavLink} to="/register">Register</NavLink>
                        </NavItem>
                    </>
                    }
                </Nav>
            </CardBody>
        </Card>
    </div>
  );
}
