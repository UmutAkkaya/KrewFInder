import React from 'react';
import PropTypes from 'prop-types';
import {withStyles} from 'material-ui/styles';
import Grid from 'material-ui/Grid';
import {credentialStore, getUser, isCourseCreator } from "../credentialStore/store";
import Button from 'material-ui/Button';
import {Link} from 'react-router-dom';
import { USER_API_ROOT } from "../ApiConstants"
import CardCollection from "./CardCollection/CardCollection";
import Typography from "material-ui/es/Typography/Typography";
var request = require('superagent');

const styles = theme => ({
    root: {
        flexGrow: 1,
        margin: 10
    },
    paper: {
        height: 140,
        width: 100,
    },
});

class Dashboard extends React.Component {
     

    constructor(props, context) {
        super(props, context);

        this.state = { courses: [] };

        this.fetchCourses = this.fetchCourses.bind(this);
        this.fetchCourses();
    }

    fetchCourses() {
        let url = USER_API_ROOT + 'GetCourses';
        request
            .get(url)
            .set('Content-Type', 'application/json')
            .set('Authorization', credentialStore.getState().token)
            .then(
                response => {
                    this.setState({ courses: response.body });
                },
                err => {
                    console.log(err);
                });
    }

    state = {
        spacing: '16',
    };

    handleChange = key => (event, value) => {
        this.setState({
            [key]: value,
        });
    };

    convertCourseToViewModel(course) {
        return {
            id: course.name,
            headline: course.name,
            subtext: "Intro to " + course.name,
            description: course.description,
            href: "/organizations/" + course.id,
            hrefText: "View Course"
        }
    }

    render() {
        const { classes } = this.props;
        const { spacing, courses } = this.state;

        return (<div>

                    <Grid container justify="center" className={classes.root}>
                        <Grid item>
                            {getUser() !== undefined && isCourseCreator() &&
                                <Grid item style={{ float: 'right' }}>
                                    <Link to={"/organizations/create"}>
                                        <Button>
                                            Create New
                                        </Button>
                                    </Link>
                                </Grid>
                            }
                            <Grid container justify="center" className={classes.root}>
                                <Grid item>
                                    <Typography variant="display3">
                                        Your Courses
                                    </Typography>
                                </Grid>
                            </Grid>
                            <CardCollection
                        objects={courses.map(this.convertCourseToViewModel)}/>
                        </Grid>
                    </Grid>
                </div>
        );
    }
}

Dashboard.propTypes = {
    classes: PropTypes.object.isRequired,
};

export default withStyles(styles)(Dashboard);