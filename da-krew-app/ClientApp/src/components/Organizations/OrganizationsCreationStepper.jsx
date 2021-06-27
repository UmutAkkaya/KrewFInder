import React from "react";
import PropTypes from "prop-types";
import {withStyles} from "material-ui/styles";
import Stepper, {Step, StepLabel} from "material-ui/Stepper";
import Button from "material-ui/Button";
import Typography from "material-ui/Typography";
import Grid from 'material-ui/Grid';
import Input from 'material-ui/Input';

const styles = theme => ({
    backButton: {
        marginRight: theme.spacing.unit
    },
});


class OrganizationCreationStepper extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            spacing: this.props.spacing,
            steps: this.props.steps,
        };
        this.handleBack = this.handleBack.bind(this);
        this.handleNext = this.handleNext.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);

    }

    handleBack() {
        this.props.handleBack();
    }

    handleNext() {
        this.props.handleNext();
    }

    handleSubmit() {
        this.props.handleSubmit();
    }

    render() {
        const {classes} = this.props;
        const steps = this.state.steps;

        return (
            <Grid container xs={12} direction='column'>
                <Grid item>
                    <Stepper activeStep={this.props.activeStep} alternativeLabel>
                        {steps.map(label => {
                            return (
                                <Step key={label}>
                                    <StepLabel>{label}</StepLabel>
                                </Step>
                            );
                        })}
                    </Stepper>
                </Grid>
                <Grid item>
                    {this.props.activeStep === steps.length ? (
                        <div className={styles.description}>
                            <Typography className={classes.instructions}>
                                Creating your organization!
                            </Typography>
                        </div>
                    ) : (
                        <div className={styles.stepper}>
                            <Grid container xs={12} justify='center'>
                                <Grid item>
                                    <div>
                                        <Button
                                            disabled={this.props.activeStep === 0}
                                            onClick={this.handleBack}
                                            className={classes.backButton}
                                        >
                                            Back
                                        </Button>
                                    </div>
                                </Grid>
                                <Grid item>
                                    {this.props.activeStep < steps.length - 1
                                        ?
                                        <Button
                                            variant="raised"
                                            color="primary"
                                            onClick={this.handleNext}
                                        >
                                            Next
                                        </Button>
                                        :
                                        <Button
                                            variant="raised"
                                            color="primary"
                                            onClick={this.handleSubmit}
                                        >
                                            Finish
                                        </Button>


                                    }
                                </Grid>
                            </Grid>
                        </div>
                    )}
                </Grid>
            </Grid>
        );
    }
}

OrganizationCreationStepper.propTypes = {
    classes: PropTypes.object
};

export default withStyles(styles)(OrganizationCreationStepper);
