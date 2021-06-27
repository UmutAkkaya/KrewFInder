import React, { Component } from 'react';
import Paper from 'material-ui/Paper';
import Tabs, { Tab } from 'material-ui/Tabs';
import Input, { InputLabel } from 'material-ui/Input';
import './Groups.css';
import GroupPage from './GroupPage';
import InvitesPage from './InvitesPage';
import AllGroups from './allGroups';
import CreateGroup from './CreateGroup';
import UserPage from '../Users/UserPage';
import { credentialStore, getUser } from "../../credentialStore/store";
import { CircularProgress } from 'material-ui/Progress';
import { COURSE_API_ROOT } from "../../ApiConstants"

var request = require('superagent');

export default class Groups extends Component {
    constructor(props) {
        super(props);

        this.state = {
            currentTab: 0,
            organizationId: this.props.match.params.organizationId,
            userGroup: undefined,
            isLoading: true

        };

        this.handleChange = this.handleChange.bind(this);
        this.getUserInformation = this.getUserInformation.bind(this);
        this.renderStatus = this.renderStatus.bind(this);
        this.page = this.page.bind(this);

        this.getUserInformation();
    }

    renderStatus() {
        let component = '';
        if (this.state.isLoading) {
            component = <CircularProgress />;
        } else {
            component = this.page();
        }

        return component;
    }

    page() {
        return (
            <div>
                {this.state.currentTab === 0 &&
                    <AllGroups orgId={this.state.organizationId} userInGroup={this.state.userGroup !== undefined} />
                }
                {this.state.currentTab === 1 &&
                    <UserPage orgId={this.state.organizationId} userId={getUser().id} />
                }
                {this.state.currentTab === 2 &&
                    this.state.userGroup !== undefined &&
                    <GroupPage group={this.state.userGroup} isOwner={true} />
                }
                {this.state.currentTab === 3 &&
                    this.state.userGroup !== undefined &&
                    <InvitesPage group={this.state.userGroup} />
                }
                {this.state.currentTab === 4 &&
                    <CreateGroup orgId={this.state.organizationId} />
                }
            </div>
        );
    }

    handleChange = (event, value) => {
        this.setState({ currentTab: value });
    };


    getUserInformation() {
        var creds = credentialStore.getState();

        let url = COURSE_API_ROOT + this.state.organizationId + '/GetUsersGroup/' + creds.user.id;
        request
            .get(url)
            .set('Content-Type', 'application/json')
            .set('Authorization', creds.token)
            .then(
            response => {
                this.setState({
                    ...this.state,
                    userGroup: response.body,
                    isLoading: false
                });
            },
            err => {
                console.log(err);
                this.setState({
                    ...this.state,
                    userGroup: undefined,
                    isLoading: false
                });
            })
    }

    render() {


        return (
            <div>
                <Paper className="root">
                    <Tabs
                        value={this.state.currentTab}
                        onChange={this.handleChange}
                        indicatorColor="primary"
                        textColor="primary"
                        centered>
                        <Tab label="All Groups" />
                        <Tab label="My Profile" disabled={getUser() === undefined} />
                        <Tab label="My Group" disabled={this.state.userGroup === undefined} />
                        <Tab label="My Group Invites" disabled={this.state.userGroup === undefined} />
                        <Tab label="Create Group" disabled={this.state.userGroup !== undefined} />
                    </Tabs>
                    {this.renderStatus()}
                </Paper>

            </div>
        );
    }
}

