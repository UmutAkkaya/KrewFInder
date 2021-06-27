import React, {Component} from 'react';
import Grid from 'material-ui/Grid';
import './Groups.css';
import List, {
    ListItem,
    ListSubheader,
    ListItemText,
} from 'material-ui/List';
import Divider from 'material-ui/Divider';
import SkillTable from '../SkillTable/SkillTable';
import TextField from 'material-ui/TextField';
import IconButton from 'material-ui/IconButton';
import Button from 'material-ui/Button';
import Icon from 'material-ui/Icon';
import {credentialStore} from "../../credentialStore/store";
import {CircularProgress} from 'material-ui/Progress';
import {GROUP_API_ROOT} from "../../ApiConstants";
import "./Groups.css"

var request = require('superagent');

function getMembersURL(groupId) {
    return GROUP_API_ROOT + groupId + "/members";
}

export default class GroupPage extends Component {
    constructor(props) {
        super(props);

        if (props.group !== undefined) {
            this.state = {
                isOwner: this.props.isOwner,
                isEditing: false,
                group: Object.assign({}, props.group),
                originalGroup: Object.assign({}, props.group),
                isLoading: false
            };
        } else {
            this.state = {
                isOwner: this.props.isOwner,
                isEditing: false,
                group: undefined,
                originalGroup: undefined,
                isLoading: true
            }
        }

        this.handleClick = this.handleClick.bind(this);
        this.descriptionChange = this.descriptionChange.bind(this);
        this.cancel = this.cancel.bind(this);
        this.saveChanges = this.saveChanges.bind(this);
        this.getGroupInfo = this.getGroupInfo.bind(this);
        this.page = this.page.bind(this);
        this.renderStatus = this.renderStatus.bind(this);
        this.setErrorState = this.setErrorState.bind(this);

        if (this.state.group === undefined) {
            this.getGroupInfo();
        }
    }

    setErrorState(error) {
        this.setState({
            ...this.state,
            isOwner: false,
            group: {
                groupId: 0,
                name: "Failed to fetch the group",
                members: [],
                bio: "",
                skills: []
            },
            originalGroup: {
                groupId: 0,
                name: "Failed to fetch the group",
                members: [],
                bio: "",
                skills: []
            },
            isLoading: false,
            error: error
        });
    }

    getGroupInfo() {
        let credentials = credentialStore.getState();
        let getGroupUrl = GROUP_API_ROOT + this.props.match.params.groupId;

        request
            .get(getGroupUrl)
            .set('Content-Type', 'application/json')
            .set('Authorization', credentials.token)
            .then(
                groupResponse => {
                    request
                        .get(getMembersURL(groupResponse.body.id))
                        .set('Content-Type', 'application/json')
                        .set('Authorization', credentials.token)
                        .then(
                            membersResponse => {
                                let newState = {
                                    ...this.state,
                                    group: groupResponse.body,
                                    isOwner: (membersResponse.body.findIndex((x) => {
                                        return x.id === credentials.user.id;
                                    }) !== -1),
                                    isLoading: false,
                                };
                                newState.group.members = membersResponse.body;
                                //original group would be returned when user click "Cancel" in editing the group
                                newState.originalGroup = Object.assign({}, newState.group);
                                this.setState(newState);
                            }, err => {
                                console.log(err);
                                this.setErrorState(err);
                            });
                },
                err => {
                    // TODO: display error
                    console.log(err);
                    this.setErrorState(err);
                });
    };

    handleClick() {
        this.setState({
            ...this.state,
            isEditing: true
        });
    }

    descriptionChange = event => {
        const newState = this.state;
        newState.group.bio = event.target.value;

        this.setState(newState)
    };

    saveChanges() {
        let credentials = credentialStore.getState();
        let infoUpdateUrl = GROUP_API_ROOT;

        const model = {
            id: this.state.group.id,
            name: this.state.group.name,
            bio: this.state.group.bio,
            skills: this.state.group.desiredSkills
        };

        request
            .post(infoUpdateUrl)
            .set('Content-Type', 'application/json')
            .set('Authorization', credentials.token)
            .send(model)
            .then(
                response => {
                    window.location.reload();
                },
                err => {
                    // TODO: display error
                    console.log(err);
                    window.location.reload();
                });
    }

    cancel() {
        const restoredState = this.state;
        restoredState.group = Object.assign({}, this.state.originalGroup);
        restoredState.isEditing = false;

        this.setState(restoredState);
    }

    page() {
        return (
            <div>
                <Grid container spacing={16} style={{padding: 8 * 3}}>
                    <Grid item xs={5}>
                        <h2> {this.state.group.name}
                            <span id="Test" className="editButton">
                                {
                                    this.state.isOwner && !this.state.isEditing &&
                                    <IconButton mini onClick={this.handleClick} color="primary"
                                                aria-label="edit">
                                        <Icon>edit_icon</Icon>
                                    </IconButton>
                                }
                            </span>

                        </h2>
                    </Grid>
                    <Grid item xs={5}>

                    </Grid>
                    <Grid item xs={2}>
                        {this.state.isEditing && this.state.isOwner &&
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
                    <Grid item xs={4}>
                        <List dense={false} subheader={<ListSubheader>Members</ListSubheader>}
                              style={{paddingTop: "20px"}}>
                            {this.state.group.members != undefined && this.state.group.members.map(member =>
                                <div key={member.id}>
                                    <ListItem>
                                        <ListItemText
                                            primary={member.firstName + " " + member.lastName}
                                        />
                                    </ListItem>
                                    <Divider/>
                                </div>
                            )}
                        </List>
                    </Grid>
                    <Grid item xs={8}>
                        <div className="skillTableContainer">
                        <SkillTable editable={false} skills={this.state.group.desiredSkills}/>
                        </div>
                    </Grid>
                </Grid>
                <Grid container spacing={16} style={{padding: 8 * 3}}>
                    <TextField
                        id="textarea"
                        label="Group Bio"
                        placeholder=""
                        multiline
                        onChange={this.descriptionChange}
                        className={"textField"}
                        disabled={!this.state.isEditing}
                        value={this.state.group.bio}
                        margin="normal"
                    />
                </Grid>
            </div>
        );
    }

    renderStatus() {
        let component = '';
        if (this.state.isLoading) {
            component = <CircularProgress/>;
        } else {
            component = this.page();
        }

        return component;
    };


    render() {
        return (
            <div id="groupPage">
                {this.renderStatus()}
            </div>
        )
    }
}