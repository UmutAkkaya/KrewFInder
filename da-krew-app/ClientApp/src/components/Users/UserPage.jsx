import React, {Component} from 'react';
import {credentialStore, getUser, getUserName} from "../../credentialStore/store";
import Grid from 'material-ui/Grid';
import {CircularProgress} from 'material-ui/Progress';
import SkillTable from '../SkillTable/SkillTable';
import IconButton from 'material-ui/IconButton';
import Icon from 'material-ui/Icon';
import Button from 'material-ui/Button';
import "./UserPage.css";
import CardCollection from "../CardCollection/CardCollection";
import {SkillTableEditModes} from "../SkillTable/SkillTable";
import {USER_API_ROOT, COURSE_API_ROOT, GROUP_MERGER_API_ROOT} from "../../ApiConstants";

var {
    MEMBER_INVITE_STATUS
} = require('../Groups/constants');

var request = require('superagent');

export default class UserPage extends Component {
    constructor(props) {
        super(props);

        this.state = {
            userId: this.props.userId !== undefined ? this.props.userId : this.props.match.params.userId,
            orgId: this.props.orgId !== undefined ? this.props.orgId : this.props.match.params.orgId,
            user: undefined,
            isLoadingSkill: true,
            isLoadingUser: true,
            isEditing: false,
            skills: [],
            prevSkills: []
        };

        this.fetchUserInfo = this.fetchUserInfo.bind(this);
        this.fetchSkills = this.fetchSkills.bind(this);
        this.handleClick = this.handleClick.bind(this);
        this.page = this.page.bind(this);
        this.renderStatus = this.renderStatus.bind(this);
        this.isOwner = this.isOwner.bind(this);
        this.cancel = this.cancel.bind(this);
        this.saveChanges = this.saveChanges.bind(this);
        this.skillsChangeHandler = this.skillsChangeHandler.bind(this);

        this.fetchUserInfo();
        this.fetchSkills();
    }


    fetchUserInfo() {
        let credentials = credentialStore.getState();

        let url = USER_API_ROOT + this.state.userId;
        request
            .get(url)
            .set('Content-Type', 'application/json')
            .set('Authorization', credentials.token)
            .then(
                response => {
                    this.setState({
                        ...this.state,
                        user: response.body,
                        userName: response.body.firstName + " " + response.body.lastName,
                        isLoadingUser: false
                    });
                },
                err => {
                    // TODO: display error
                    console.log(err);
                    this.setState({
                        ...this.state,
                        user: undefined,
                        userName: "Failed to fetch the user",
                        isLoadingUser: false
                    });
                });
    }

    fetchSkills() {
        let credentials = credentialStore.getState();

        let getUserSkillsURL = COURSE_API_ROOT + this.state.orgId + '/GetUserSkills/' + this.state.userId;
        let getSkillDomainURL = COURSE_API_ROOT + this.state.orgId + '/GetSkillDomain';
        request
            .get(getUserSkillsURL)
            .set('Content-Type', 'application/json')
            .set('Authorization', credentials.token)
            .then(
                response => {
                    this.setState({
                        ...this.state,
                        prevSkills: Object.assign([], response.body),
                        skills: Object.assign([], response.body),
                    });

                    request
                        .get(getSkillDomainURL)
                        .set('Content-Type', 'application/json')
                        .set('Authorization', credentials.token)
                        .then(
                            response2 => {
                                this.setState({
                                    ...this.state,
                                    skillDomain: response2.body,
                                    isLoadingSkill: false
                                });
                            },
                            err2 => {
                                console.log(err2);
                                this.setState({
                                    ...this.state,
                                    skillDomain: [],
                                    isLoadingSkill: false
                                });
                            });
                },
                err => {
                    // TODO: display error
                    console.log(err);
                    this.setState({
                        ...this.state,
                        prevSkills: [],
                        skills: [],
                        skillDomain: [],
                        isLoadingSkill: false
                    });
                })
    }

    isOwner() {
        return getUser() !== undefined
            && this.state.userId === getUser().id
            && this.state.user !== undefined
    }

    cancel() {
        let originalUserCopy = Object.assign({}, this.state.prevSkills);

        this.setState({
            ...this.state,
            skills: originalUserCopy,
            isEditing: false
        });
    }

    handleClick() {
        this.setState({
            ...this.state,
            isEditing: true
        });
    }

    saveChanges() {
        let credentials = credentialStore.getState();
        let url = COURSE_API_ROOT + this.state.orgId + "/UpdateUserSkills/" + this.state.userId;

        request
            .post(url)
            .set('Content-Type', 'application/json')
            .set('Authorization', credentials.token)
            .send({updatedSkills: this.state.skills})
            .then(
                response => {
                    this.setState({
                        ...this.state,
                        isEditing: false
                    })
                },
                err => {
                    // TODO: display error
                    console.log(err);
                });
    }

    skillsChangeHandler(updatedSkills) {
        this.state = {
            ...this.state,
            skills: updatedSkills
        }
    }

    convertInvitationstoCardObj() {
        let cards = [];
        for (let i in this.state.user.invitations.keys) {
            let group = this.state.user.invitations.keys[i];

            if (group.courseId !== this.state.orgId) {
                continue;
            }

            let cardObj = {
                id: group.id,
                headline: group.name,
                subtext: "",
                description: group.bio,
            };

            this.getStatus(this.state.user.invitations.values[i], cardObj);
            if (cardObj.title !== undefined) {
                cards.push(cardObj);
            }
        }

        return cards;
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

        let url = USER_API_ROOT + 'invite/';
        switch (value) {
            case MEMBER_INVITE_STATUS.INVITED.VALUE:
                cardObj.title = "Invited You";
                cardObj.hrefAction = this.sendInvite(USER_API_ROOT + "AcceptGroupInvitation/" + cardObj.id);
                cardObj.hrefText = "Accept";
                cardObj.hrefAction2 = this.sendInvite(USER_API_ROOT + "RejectGroupInvitation/" + cardObj.id);
                cardObj.hrefText2 = "Reject";
                break;
        }
    }

    page() {
        return (
            <div>
                <Grid container spacing={16} style={{padding: 8 * 3}}>
                    <Grid item xs={10}>
                        <h2> {this.state.userName}
                            <span id="Test" className="editButton">
                            {this.isOwner() && !this.state.isEditing &&
                            <IconButton mini onClick={this.handleClick} color="primary"
                                        aria-label="edit">
                                <Icon>edit_icon</Icon>
                            </IconButton>
                            }
                            </span>
                        </h2>
                    </Grid>
                    <Grid item xs={2}>
                        {this.state.isEditing && this.isOwner() &&
                        <div>
                            <Button onClick={this.saveChanges}>
                                Save
                            </Button>
                            <Button onClick={this.cancel}>
                                Cancel
                            </Button>
                        </div>
                        }
                    </Grid>

                    <Grid item xs={2}/>
                    <Grid item xs={8}>
                        <SkillTable
                            onDataChanged={this.skillsChangeHandler}
                            editMode={SkillTableEditModes.skillSelector}
                            editable={this.state.isEditing}
                            skillDomain={this.state.skillDomain}
                            skills={this.state.skills}/>
                    </Grid>
                    <Grid item xs={2}/>
                    <Grid item xs={1}/>
                    <Grid item xs={10}>
                        {
                            this.state.user !== undefined && this.state.user.invitations !== undefined &&
                            this.convertInvitationstoCardObj().length !== 0 &&
                            <CardCollection objects={this.convertInvitationstoCardObj()}/>
                        }
                    </Grid>
                    <Grid item xs={1}/>
                </Grid>
            </div>
        )
    }

    renderStatus() {
        let component = '';
        if (this.state.isLoadingSkill || this.state.isLoadingUser) {
            component = <CircularProgress/>;
        } else {
            component = this.page();
        }

        return component;
    };

    render() {
        return (
            <div>
                {this.renderStatus()}
            </div>
        )
    }
}