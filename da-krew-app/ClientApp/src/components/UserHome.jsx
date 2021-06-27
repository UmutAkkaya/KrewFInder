import React, { Component } from 'react';
import { Redirect } from 'react-router-dom';




export class UserHome extends Component {
    render() {
        return (
            <Redirect to="/dashboard"/>
        );
    }
}
