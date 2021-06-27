import React, {Component} from 'react';
import AppBar from 'material-ui/AppBar';
import Toolbar from 'material-ui/Toolbar';
import Typography from 'material-ui/Typography';
import {credentialStore} from "../credentialStore/store"
import {Link} from 'react-router-dom';
import "./NavBar.css";


export class NavBar extends Component {
    constructor(props) {
        super(props);

        this.state = {
            auth: this.isLoggedIn()
        };

        credentialStore.subscribe(() => {
            this.setState({
                auth: this.isLoggedIn(),
                userId: this.getUserId()
            });
        });
    }

    isLoggedIn() {
        return credentialStore.getState().token !== undefined && credentialStore.getState().token !== '';
    }
    
    getUserId() {
        if (this.isLoggedIn()){
            return credentialStore.getState().user.id;
        }
        
        return -1;
    }

    render() {
        return (
            <div>
                <AppBar position="static" className={"root"} color={"default"}>
                    <Toolbar>
                        <Typography variant="title" color="inherit" className={"flex"}>
                            <Link className={"title"} to={this.state.auth ? '/dashboard' : '/'}>Krew Findr</Link>
                        </Typography>
                        <div>
                            {(this.state.auth && (
                                <div>
                                    <Link className={"link default"} to={'/logout'}>Logout</Link>
                                </div>
                            )) || (!this.state.auth && (
                                <div>
                                    <Link className={"link default"} to={'/login'}>Login</Link>
                                    <Link className={"link default"} to={'/register'}>Register</Link>
                                </div>
                            ))}
                        </div>
                    </Toolbar>
                </AppBar>
            </div>
        );
    }
}

export default NavBar;
