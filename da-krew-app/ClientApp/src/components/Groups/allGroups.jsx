import React, {Component} from 'react';
import CardCollection from "../CardCollection/CardCollection";
import Grid from 'material-ui/Grid'
import {credentialStore, getUser} from "../../credentialStore/store";
import {CircularProgress} from 'material-ui/Progress';
import {COURSE_API_ROOT} from "../../ApiConstants";

var request = require('superagent');

export default class AllGroups extends Component {
    constructor(props) {
        super(props);

        this.state = {
            organizationId: this.props.orgId,
            groupList: [],
            userInGroup: this.props.userInGroup,
            isLoading: true
        };

        this.renderStatus = this.renderStatus.bind(this);
        this.page = this.page.bind(this);
        this.fetchGroups = this.fetchGroups.bind(this);

        this.fetchGroups();
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
                {
                    this.state.groupList !== undefined &&
                    <Grid container spacing={16} style={{padding: 8 * 3}}>
                        <Grid item xs={1}/>
                        <Grid item xs={10}>
                            <CardCollection callToAction="View Group"
                                            objects={this.state.groupList.map(this.convertGroupToCardObject)}/>
                        </Grid>
                        <Grid item xs={1}/>
                    </Grid>
                }
            </div>
        );
    }

    convertGroupToCardObject(group) {
        let cardObj = {
            id: group + group.id,
            title: getGroupStatus(group),
            headline: group.name,
            subtext: "",
            description: group.bio,
            href: "/groups/" + group.id,
            hrefText: "View Group"
        };

        return cardObj;
    }

    fetchGroups() {
        let credentials = credentialStore.getState();

        let url = COURSE_API_ROOT + this.state.organizationId + '/GetCourseGroups';
        request
            .get(url)
            .set('Content-Type', 'application/json')
            .set('Authorization', credentials.token)
            .then(
                response => {
                    this.setState({
                        ...this.state,
                        // TODO: get from response
                        groupList: response.body !== null ? response.body : [],
                        isLoading: false
                    });
                },
                err => {
                    // TODO: display error
                    console.log(err);
                    this.setState({
                        ...this.state,
                        groupList: [],
                        isLoading: false
                    });
                });
    }

    render() {
        return (
            <div>
                {this.renderStatus()}
            </div>
        );
    }
}

function getGroupStatus(group) {
    let user = getUser();
    if (user.groupIds.includes(group.id)) {
        return "Joined";
    } else {
        return "";
    }
}
