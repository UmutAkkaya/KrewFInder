import React, { Component } from 'react';
import Paper from 'material-ui/Paper';
import './Groups.css';
import SkillTable from '../SkillTable/SkillTable';
import { credentialStore } from "../../credentialStore/store";
import TextField from 'material-ui/TextField';
import { CircularProgress } from 'material-ui/Progress';
import { SkillTableEditModes } from "../SkillTable/SkillTable";
import { COURSE_API_ROOT, GROUP_API_ROOT } from "../../ApiConstants";
var request = require('superagent');

export default class CreateGroup extends Component {
    constructor(props) {
        super(props);

        this.state = {
            name: "",
            bio: "",
            isLoading: true,
            organizationId: this.props.orgId,
            skills: []
        };

        this.handleSubmit = this.handleSubmit.bind(this);
        this.fetchSkillDomain = this.fetchSkillDomain.bind(this);
        this.renderStatus = this.renderStatus.bind(this);
        this.page = this.page.bind(this);
        this.skillsChangeHandler = this.skillsChangeHandler.bind(this);
        this.handleChange = this.handleChange.bind(this);

        this.fetchSkillDomain();
    }

    handleChange(event) {
        this.setState({
            ...this.state,
            [event.target.name]: event.target.value
        });
    }

    fetchSkillDomain() {
        let credentials = credentialStore.getState();
        let getSkillDomainURL = COURSE_API_ROOT + this.state.organizationId + '/GetSkillDomain';

        request
            .get(getSkillDomainURL)
            .set('Content-Type', 'application/json')
            .set('Authorization', credentials.token)
            .then(
            response2 => {
                console.log(response2);
                this.setState({
                    ...this.state,
                    skillDomain: response2.body,
                    isLoading: false
                });
            },
            err2 => {
                console.log(err2);
                this.setState({
                    ...this.state,
                    skillDomain: [],
                    isLoading: false
                });
            });
    }

    skillsChangeHandler(updatedSkills) {
        this.state = {
            ...this.state,
            skills: updatedSkills
        }
    }

    handleSubmit(event) {
        event.preventDefault();

        let credentials = credentialStore.getState();
        let url = GROUP_API_ROOT + 'CreateGroup';

        request
            .post(url)
            .set('Content-Type', 'application/json')
            .set('Authorization', credentials.token)
            .send({
                name: this.state.name,
                bio: this.state.bio,
                courseId: this.state.organizationId,
                skills: this.state.skills
            })
            .then(
            response => {
                console.log(response.body);
                window.location = "/groups/" + response.body.id;
            },
            err => {
                // TODO: display error
                console.log(err);
            });
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
                <Paper className="root">
                    <div className={"outerBox"}>
                        <div className={"rightBox"}>
                            <form onSubmit={this.handleSubmit}>
                                <TextField
                                    name="name"
                                    onChange={this.handleChange}
                                    minLength={1}
                                    label="Group Name"
                                    value={this.state.name} /> <br />
                                <TextField
                                    name="bio"
                                    onChange={this.handleChange}
                                    multiline
                                    style={{ minWidth: '18em' }}
                                    rows="4"
                                    label="Group Description"
                                    value={this.state.bio} />

                                <SkillTable
                                    onDataChanged={this.skillsChangeHandler}
                                    editMode={SkillTableEditModes.skillSelector}
                                    editable={true}
                                    skillDomain={this.state.skillDomain}
                                    skills={this.state.skills} />

                                <input type="submit" value="Create" className={"skillButton"} />
                                <h6>When you finish creating the group, you can begin to add team members.</h6>
                            </form>
                        </div>
                    </div>
                </Paper>
            </div>
        );
    }

    render() {
        return (
            <div>
                {this.renderStatus()}
            </div>
        );
    }
}

