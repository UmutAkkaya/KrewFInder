import React, {Component} from 'react';
import Grid from 'material-ui/Grid';
import './Groups.css';
import CardCollection from "../CardCollection/CardCollection";
import {credentialStore} from "../../credentialStore/store";
import {CircularProgress} from 'material-ui/Progress';
import {GROUP_API_ROOT, GROUP_MERGER_API_ROOT, USER_API_ROOT} from "../../ApiConstants"

var {
    GROUP_INVITE_STATUS,
    MEMBER_INVITE_STATUS
} = require('./constants');
var request = require('superagent');

export default class InvitesPage extends Component {
    constructor(props) {
        super(props);

        this.state = {
            groupId: this.props.group.id,
            group: this.props.group,
            isLoading: true
        };

        this.convertMemberToCardObject = this.convertMemberToCardObject.bind(this);
        this.convertGroupToCardObject = this.convertGroupToCardObject.bind(this);
        this.renderStatus = this.renderStatus.bind(this);
        this.page = this.page.bind(this);
        this.getMemberStatus = this.getMemberStatus.bind(this);
        this.getStatus = this.getStatus.bind(this);
        this.fetchSuggestions = this.fetchSuggestions.bind(this);

        this.fetchSuggestions();
    }

    fetchSuggestions() {
        let credentials = credentialStore.getState();

        let url = GROUP_API_ROOT + this.state.groupId + '/suggest';
        request
            .post(url)
            .set('Content-Type', 'application/json')
            .set('Authorization', credentials.token)
            .then(
                response => {
                    this.setState({
                        ...this.state,
                        group: response.body,
                        isLoading: false
                    });
                },
                err => {
                    // TODO: display error
                    console.log(err);
                    this.setState({
                        ...this.state,
                        isLoading: false
                    });
                });
    }

    renderStatus() {
        let component = '';
        if (this.state.isLoading) {
            component = <CircularProgress/>;
        } else {
            component = this.page();
        }

        return component;
    }

    page() {
        return (
            <div>
                <Grid container spacing={16} style={{padding: 8 * 3}}>
                    <Grid item xs={1}/>
                    <Grid item xs={11}>
                        <h3> Member Invites </h3>
                    </Grid>
                    <Grid item xs={1}/>
                    <Grid item xs={10}>
                        <CardCollection callToAction="View Course"
                                        objects={this.convertMemberToCardObject()}/>
                    </Grid>
                    <Grid item xs={1}/>
                </Grid>
                <Grid container spacing={16} style={{padding: 8 * 3}}>
                    <Grid item xs={1}/>
                    <Grid item xs={11}>
                        <h3> Group Invites </h3>
                    </Grid>
                    <Grid item xs={1}/>
                    <Grid item xs={10}>
                        <CardCollection
                            objects={this.convertGroupToCardObject()}/>
                    </Grid>
                    <Grid item xs={1}/>
                </Grid>
            </div>
        );
    }

    sendInvite(url) {
        return function () {

            let credentials = credentialStore.getState();
            request
                .post(url)
                .set('Content-Type', 'application/json')
                .set('Authorization', credentials.token)
                .then(
                    response => {
                        alert("Response sent")
                    },
                    err => {
                        alert("Error")
                    });
        }
    }

    getStatus(value, cardObj) {
        switch (value) {
            case GROUP_INVITE_STATUS.INVITED.VALUE:
                cardObj.title = "Invited You";
                cardObj.hrefAction = this.sendInvite(GROUP_MERGER_API_ROOT + cardObj.id + "/" + this.state.group.id + "/accept");
                cardObj.hrefText = "Accept";
                cardObj.hrefAction2 = this.sendInvite(GROUP_MERGER_API_ROOT + cardObj.id + "/" + this.state.group.id + "/reject");
                cardObj.hrefText2 = "Reject";
                break;
            case GROUP_INVITE_STATUS.SENT.VALUE:
                cardObj.title = "Invitation Sent";
                cardObj.hrefAction = this.sendInvite(GROUP_MERGER_API_ROOT + this.state.group.id + "/" + cardObj.id + "/revoke");
                cardObj.hrefText = "Revoke";
                break;
            case GROUP_INVITE_STATUS.REVOKED.VALUE:
                cardObj.title = "Revoked";
                cardObj.hrefAction = this.sendInvite(GROUP_MERGER_API_ROOT + "create/" + this.state.group.id + "/" + cardObj.id);
                cardObj.hrefText = "Resend";
                break;
            case GROUP_INVITE_STATUS.REJECTED.VALUE:
                cardObj.title = "Invitation Rejected";
                cardObj.href = "/groups/" + cardObj.id;
                cardObj.hrefText = "View Group";
                break;
            case GROUP_INVITE_STATUS.ACCEPTED.VALUE:
                cardObj.title = "Merged";
                cardObj.href = "/groups/" + cardObj.id;
                cardObj.hrefText = "View Group";
                break;
            case GROUP_INVITE_STATUS.SUGGESTED.VALUE:
                cardObj.title = "Suggested";
                cardObj.hrefAction = this.sendInvite(GROUP_MERGER_API_ROOT + "create/" + this.state.group.id + "/" + cardObj.id);
                cardObj.hrefText = "Invite";
                break;
        }
    }

    convertGroupToCardObject() {
        let cards = [];
        for (var i in this.state.group.mergerInvitations.keys) {
            let group = this.state.group.mergerInvitations.keys[i];
            let cardObj = {
                id: group.id,
                headline: group.name,
                subtext: "",
                description: group.bio,
            };

            this.getStatus(this.state.group.mergerInvitations.values[i], cardObj);
            if (cardObj.title !== undefined) {
                cards.push(cardObj);
            }
        }

        return cards;

    }

    convertMemberToCardObject() {
        let cards = [];

        for (const i in this.state.group.invitations.keys) {

            let member = this.state.group.invitations.keys[i];
            let cardObj = {
                id: member.id,
                headline: member.firstName + " " + member.lastName,
                subtext: "",
            };

            this.getMemberStatus(this.state.group.invitations.values[i], cardObj);

            if (cardObj.title !== undefined) {
                cards.push(cardObj);
            }
        }

        return cards;
    }

    getMemberStatus(value, cardObj) {

        let url = USER_API_ROOT;
        switch (value) {
            case MEMBER_INVITE_STATUS.INVITED.VALUE:
                cardObj.title = "Invitation Sent";
                cardObj.href = "/organizations/" + this.state.group.courseId + "/user/" + cardObj.id;
                cardObj.hrefText = "View User";
                break;
            case MEMBER_INVITE_STATUS.REJECTED.VALUE:
                cardObj.title = "Invitation Rejected";
                cardObj.href = "/organizations/" + this.state.group.courseId + "/user/" + cardObj.id;
                cardObj.hrefText = "View User";
                break;
            case MEMBER_INVITE_STATUS.ACCEPTED.VALUE:
                cardObj.title = "Invitation Accepted";
                cardObj.href = "/organizations/" + this.state.group.courseId + "/user/" + cardObj.id;
                cardObj.hrefText = "View User";
                break;
            case MEMBER_INVITE_STATUS.SUGGESTED.VALUE:
                cardObj.title = "Suggested";
                cardObj.hrefAction = this.sendInvite(url + 'AddGroupInvitation/' + cardObj.id + '/' + this.state.groupId );
                cardObj.hrefText = "Invite";
                break;
        }
    }

    render() {
        return (
            <div>
                {this.renderStatus()}
            </div>
        )
    }
}