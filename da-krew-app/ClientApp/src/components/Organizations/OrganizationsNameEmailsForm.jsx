import React, {Component} from "react";
import {InputLabel} from 'material-ui/Input';
import TextField from 'material-ui/TextField';
import Grid from 'material-ui/Grid';
import {withStyles} from 'material-ui/styles';

const styles = theme => ({
    container: {
        display: 'flex',
        "flex-direction":"column",
        "align-items": "center",
    },
    textField: {
        marginLeft: theme.spacing.unit,
        marginRight: theme.spacing.unit,
        width: 200,
    },
});

class OrganizationNameEmailsForm extends Component {
    constructor(props) {
        super(props);
        this.state = props;
        this.onOrganizationNameChanged = this.onOrganizationNameChanged.bind(this);
        this.onEmailInvitesChanged = this.onEmailInvitesChanged.bind(this);
    }


    componentDidUpdate() {
        this.props.onValueChanged(this.state.data);
    }

    onOrganizationNameChanged(event) {
        let newData = this.state.data;
        newData.organizationName = event.target.value;
        this.setState({
            data: newData
        });
    }

    onEmailInvitesChanged(event) {
        let newData = this.state.data;
        
        //get a list of emails by splitting text by commas, " ", "\n"
        newData.emailInvites = event.target.value.split(",")
        this.setState({
            data: newData
        });
    };

    render() {
        return (
            <Grid container spacing={16} className={this.state.classes.container}>
                <Grid item xs={12}>
                    <InputLabel htmlFor="organizationName">Organization name:</InputLabel>
                    <TextField
                        id="organizationName"
                        className={this.state.classes.textField}
                        placeholder="Enter Organization name"
                        onChange={this.onOrganizationNameChanged}
                        defaultValue={this.state.data.organizationName}
                    />
                </Grid>
                <Grid item xs={12}>
                    <InputLabel htmlFor="emailInvites">Email invites:</InputLabel>
                    <TextField
                        id="emailInvites"
                        className={this.state.classes.textField}
                        placeholder="List emails to send invitations to join:"
                        floatingLabelText="List emails to send invitations to join"
                        multiLine={true}
                        onChange={this.onEmailInvitesChanged}
                        defaultValue={this.state.data.emailInvites}
                    />
                </Grid>
            </Grid>
        );
    }
}

export default withStyles(styles)(OrganizationNameEmailsForm);

