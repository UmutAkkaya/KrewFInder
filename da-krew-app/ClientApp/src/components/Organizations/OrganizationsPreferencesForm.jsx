import React, { Component } from "react";
import { InputLabel } from 'material-ui/Input';
import TextField from 'material-ui/TextField';
import Grid from 'material-ui/Grid';
import { withStyles } from 'material-ui/styles';
// import {DatePicker} from 'material-ui/';


const styles = theme => ({

    textField: {
        marginLeft: theme.spacing.unit,
        marginRight: theme.spacing.unit,
        width: 200,
    },
});

class OrganizationPreferencesForm extends Component {
    constructor(props) {
        super(props);
        this.state = props
        this.onStartDateChanged = this.onStartDateChanged.bind(this);
        this.onEndDateChanged = this.onEndDateChanged.bind(this);
        this.onMinGroupSizeChanged = this.onMinGroupSizeChanged.bind(this);
        this.onMaxGroupSizeChanged = this.onMaxGroupSizeChanged.bind(this);
    }

    componentDidUpdate() {
        this.props.onValueChanged(this.state.data);
    }

    onStartDateChanged(event) {
        let newData = this.state.data;
        newData.startDate = event.target.value;
        this.setState({
            data: newData
        });
    }

    onEndDateChanged(event) {
        let newData = this.state.data;
        newData.endDate = event.target.value;
        this.setState({
            data: newData
        });
    }

    onMinGroupSizeChanged(event) {
        let newData = this.state.data;
        newData.groupSizeMin = event.target.value;
        if (Number.isInteger(newData.groupSizeMin)) {
            this.setState({
                data: newData
            });
        }
    };

    onMaxGroupSizeChanged(event) {
        let newData = this.state.data;
        newData.groupSizeMax = event.target.value;
        if (Number.isInteger(newData.groupSizeMax)) {
            this.setState({
                data: newData
            });
        }

    };

    render() {
        return (
            <Grid container id="groupPreferencesForm" spacing={8}>
                <Grid item xs={12}>
                    <Grid container justify='center' alignItems='center'>
                        <Grid item>
                            <InputLabel htmlFor="groupSizeMin">Group size minimum:</InputLabel>
                            <TextField
                                id="groupSizeMin"
                                className={this.state.classes.textField}
                                placeholder="Min"
                                onChange={this.onMinGroupSizeChanged}
                                defaultValue={this.state.data.groupSizeMin}
                                required={true}
                                pattern="[0-9]+"
                            />
                        </Grid>
                        <Grid item>
                            <InputLabel htmlFor="groupSizeMax">Group size maximum:</InputLabel>
                            <TextField
                                id="groupSizeMax"
                                className={this.state.classes.textField}
                                placeholder="Max"
                                onChange={this.onMaxGroupSizeChanged}
                                defaultValue={this.state.data.groupSizeMax}
                                required={true}
                                pattern="[0-9]+"
                            />
                        </Grid>
                    </Grid>
                </Grid>
                <Grid item xs={12}>
                    <Grid container justify='center' alignItems='center'>
                        <Grid item>
                            <InputLabel htmlFor="startDate">Start Date</InputLabel>
                            <TextField
                                id="startDate"
                                className={this.state.classes.textField}
                                hintText="Organization Start Date"
                                onChange={this.onStartDateChanged}
                                defaultValue={this.state.data.startDate}
                                type="date"
                                required={true}
                            />
                        </Grid>
                        <Grid item>
                            <InputLabel htmlFor="endDate">End Date</InputLabel>
                            <TextField
                                id="endDate"
                                className={this.state.classes.textField}
                                hintText="Organization End Date"
                                onChange={this.onEndDateChanged}
                                defaultValue={this.state.data.endDate}
                                type="date"
                                required={true}
                            />
                        </Grid>

                    </Grid>
                </Grid>
            </Grid>
        );
    }
}

export default withStyles(styles)(OrganizationPreferencesForm);

