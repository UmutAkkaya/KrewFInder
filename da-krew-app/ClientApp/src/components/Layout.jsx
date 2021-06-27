import React, { Component } from 'react';
import Grid from 'material-ui/Grid';
import { NavBar } from './NavBar';

export class Layout extends Component {
    displayName = Layout.name;

    render() {
        return (
            <Grid style={{background: "#eeeeee"}} item xs={12}>
                <Grid>
                    <NavBar />
                </Grid>
                <Grid item xs={12}>
                    {this.props.children}
                </Grid>
            </Grid>
        );
    }
}
