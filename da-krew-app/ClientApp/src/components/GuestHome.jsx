import React, { Component } from 'react';
import { Provider } from 'react-redux'
import { createStore } from 'redux'
import reducers from "./Table/reducers"

import { Link } from 'react-router-dom';
import Particles from 'react-particles-js';

import './GuestHome.css';

let store = createStore(reducers);

export class GuestHome extends Component {

    render() {
        return (
            <Provider store={store}>
                <div className="background">
                    <div className={"left"}>
                        <Particles className={"particles"}/>
                    </div>
                    <div className={"right"}>
                        <div className={"text"}>
                            Find and pair up with reliable classmates tailored to your projects needs.
                        </div>
                        <div className={"registerText"}>
                            Register now at no cost for both course organizers and students in university.
                        </div>
                        <Link className={"link registerButton"} to={'/register'}>Register</Link>
                        <a className={"loginTextStatic"}> or </a>
                        <Link className={"link loginText"} to={'/login'}>Login</Link>
                    </div>
                </div>
            </Provider>
        );
    }
}
