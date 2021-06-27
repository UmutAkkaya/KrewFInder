import React, { Component } from 'react';
import {credentialStore} from "../credentialStore/store"

import { UserHome } from './UserHome';
import { GuestHome } from './GuestHome';

import './Home.css';

function RenderHome(prop) {
    const isLoggedIn = prop.isLoggedIn;
    if (isLoggedIn)
        return <UserHome/>
    return <GuestHome/>
}

export class Home extends Component {
    constructor(props) {
        super(props);

        this.state = {
            auth: this.isLoggedIn()
        };
        
        credentialStore.subscribe(() =>{
            this.setState({ auth: this.isLoggedIn() });
        });
    }

    isLoggedIn() {
        return credentialStore.getState().token !== undefined && credentialStore.getState().token !== '';
    }

    render() {
        return (
            <RenderHome prop={this.state.auth} />
        );
    }
}
