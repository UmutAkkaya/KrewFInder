import React, { Component } from "react";
import { InputLabel } from 'material-ui/Input';
import TextField from 'material-ui/TextField';
import Grid from 'material-ui/Grid';
import { withStyles } from 'material-ui/styles';
import SkillTable, { SkillTableEditModes } from '../SkillTable/SkillTable';

const styles = theme => ({
    container: {
        display: 'flex',
        flexWrap: 'wrap',
        'flex-direction': 'column',
        'align-content': 'center',
        'align-items': 'center',
    },
    textField: {
        marginLeft: theme.spacing.unit,
        marginRight: theme.spacing.unit,
        width: 200,
    },
});

class OrganizationSkillSetForm extends Component {
    constructor(props) {
        super(props);
        this.state = props;
    }

    render() {
        const { editable } = this.props;

        return (
            <Grid container spacing={16} className={this.state.classes.container}>

                <Grid item xs={12}>
                    <SkillTable editable={editable} skills={this.props.organizationSkillSet} onDataChanged={this.props.onValueChanged} />
                </Grid>
            </Grid>
        );
    }
}

export default withStyles(styles)(OrganizationSkillSetForm);

