import React, { Component } from 'react';
import Grid from 'material-ui/Grid';
import OrganizationCreationStepper from './OrganizationsCreationStepper';
import { credentialStore } from "../../credentialStore/store";
import OrganizationNameEmailsForm from './OrganizationsNameEmailsForm'
import OrganizationSkillSetForm from './OrganizationsSkillSetForm'
import OrganizationPreferencesForm from './OrganizationsPreferencesForm'
import { COURSE_API_ROOT } from "../../ApiConstants";

var request = require('superagent');

const styles = theme => ({
    root: {
        width: "90%",
        display: "flex",
        "flex-direction": "column",
        "justify-content": "center",
        "align-items": "center",
    },
    stepper: {
        "align-self": "flex-end",
    },
    form: {
        "align-self": "flex-start",
    }
});

function getSteps() {
    return [
        "Enter Organization Description",
        "Select Organization Skills",
        "Selet Organization Preferences"
    ];
}

export default class OrganizationCreatePage extends Component {
    constructor(props) {
        super(props);

        credentialStore.subscribe(() =>
            console.log(credentialStore.getState())
        );

        this.state = {
            spacing: '16',
            activeStep: 0,
            data: {
                organizationName: '',
                emailInvites: [],
                organizationSkillSet: [],
                groupSizeMin: '',
                groupSizeMax: '',
                startDate: '',
                endDate: ''
            }
        };
        this.handleNext = this.handleNext.bind(this);
        this.handleBack = this.handleBack.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
        this.nameEmailsChangeHandler = this.nameEmailsChangeHandler.bind(this);
        this.skillsChangeHandler = this.skillsChangeHandler.bind(this);
        this.preferencesChangeHandler = this.preferencesChangeHandler.bind(this);
    }

    handleSubmit = event => {
        let credentials = credentialStore.getState();
        let createUrl = COURSE_API_ROOT + "CreateCourse";
        let model = {
            name: this.state.data.organizationName,
            description: "This the the organization for " + this.state.data.organizationName,
            minGroupSize: this.state.data.groupSizeMin,
            maxGroupSize: this.state.data.groupSizeMax,
            startDate: this.state.data.startDate,
            endDate: this.state.data.endDate,
            skills: this.state.data.organizationSkillSet,
            emails: this.state.data.emailInvites
        };
        request
            .post(createUrl)
            .set('Content-Type', 'application/json')
            .set('Authorization', credentials.token)
            .send(model)
            .then(
                response => {
                    console.log(response.body);                   
                    window.location = "/organizations/" + response.body.id;
                },
                err => {
                    console.log(err);
                });

    };

    handleNext() {
        const { activeStep } = this.state;
        this.setState({
            activeStep: activeStep + 1
        });
    }

    handleBack() {
        const { activeStep } = this.state;
        this.setState({
            activeStep: activeStep - 1
        });
    }

    skillsChangeHandler(updatedSkills) {
        this.state.data = {
            ...this.state.data,
            skills: updatedSkills
        }
    }

    preferencesChangeHandler(newPreferences) {
        let newData = this.state.data;
        newData.groupSizeMax = newPreferences.groupSizeMax;
        newData.groupSizeMin = newPreferences.groupSizeMin;
        newData.startDate = newPreferences.startDate;
        newData.endDate = newPreferences.endDate;
        this.state.data = newData;
    }

    nameEmailsChangeHandler(newNameEmails) {
        let newData = this.state.data;
        newData.organizationName = newNameEmails.organizationName;
        newData.emailInvites = newNameEmails.emailInvites;
        this.state.data = newData;
    }

    renderNextStep() {
        switch (this.state.activeStep) {
            case 0:
                return <OrganizationNameEmailsForm data={this.state.data} onValueChanged={this.nameEmailsChangeHandler} />;
            case 1:
                return <OrganizationSkillSetForm editable={true} organizationSkillSet={this.state.data.organizationSkillSet} onValueChanged={this.skillsChangeHandler
                } />;
            case 2:
                return <OrganizationPreferencesForm data={this.state.data} onValueChanged={this.preferencesChangeHandler} />;
            default:
                return <OrganizationNameEmailsForm />;
        }
    }

    render() {
        const { classes } = this.props;
        const { spacing, objects } = this.state;

        return (
            <div className={styles.root} pacing={Number(spacing)}>
                <form action="">
                    <div className={styles.stepper}>
                        <OrganizationCreationStepper
                            handleNext={this.handleNext}
                            handleBack={this.handleBack}
                            handleSubmit={this.handleSubmit}
                            steps={getSteps()}
                            activeStep={this.state.activeStep} />
                    </div>
                    <div className={styles.form}>
                        {this.renderNextStep()}
                    </div>
                </form>
            </div>
        )
    }
}