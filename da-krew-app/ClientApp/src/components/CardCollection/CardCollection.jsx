import React from 'react';
import PropTypes from 'prop-types';
import {withStyles} from 'material-ui/styles';
import Grid from 'material-ui/Grid';
import SimpleCard from "../Card/Card";
import TextField from 'material-ui/TextField';
import Paper from 'material-ui/Paper'
import Grow from 'material-ui/transitions/Grow';

const styles = theme => ({
    root: {
        flexGrow: 1,
        margin: 10
    },
    paper: {
        height: 140,
        width: 100,
    },
    textField: {
        marginLeft: theme.spacing.unit,
        marginRight: theme.spacing.unit,
        width: 200,
    },
});

class CardCollection extends React.Component {

    constructor(props, context) {
        super(props, context);

        this.state = {
            spacing: '16',
            objects: this.props.objects,
            query: this.props.query,
        };
    }


    handleChange = (event) => {
        this.setState({
            "query": event.target.value,
        });
    };

    filter = item => {
        const query = this.state.query || '';

        function tryCompareAsString(obj) {
            let lowercase = ((obj && obj.toLowerCase && obj.toLowerCase.bind(obj)) || (() => obj || {}))();
            let includes = ((lowercase.includes && lowercase.includes.bind(lowercase)) || (x => false));

            return includes(query.toLowerCase()) || lowercase === query.toLowerCase();
        }

        let matches = Object.keys(item)
            .filter(key => tryCompareAsString(item[key])); //We want type coercion here
        let wholeItemMatch = tryCompareAsString(item);

        return !query || matches.length > 0 || wholeItemMatch;
    };

    componentWillReceiveProps = (props) => {
        this.setState({
            spacing: '16',
            objects: props.objects,
            query: props.query,
        });
    }

    render = () => {
        const { classes} = this.props;
        const { spacing, objects } = this.state;

        return (
            <Paper style={{ maxWidth: 1000 }}>
                <Grid container justify="center" spacing={Number(spacing)}>
                    <Grid item xs={12}>
                        <Grid container justify="center" spacing={Number(spacing)}>
                            <TextField
                                label="Search"
                                className={classes.textField}
                                margin="normal"
                                onChange={this.handleChange}
                                value={this.state.query}/>
                        </Grid>
                    </Grid>
                    {objects
                        .filter(this.filter)
                        .map(value => (
                            <Grid key={value.id} item>
                                <SimpleCard
                                    title={value.title}
                                    headline={value.headline}
                                    subtext={value.subtext}
                                    description={value.description}
                                    hrefAction={value.hrefAction} hrefAction2={value.hrefAction2}
                                    href={value.href} hrefText={value.hrefText}
                                    href2={value.href2} hrefText2={value.hrefText2}/>
                            </Grid>
                        ))}
                </Grid>
            </Paper>
        );
    }
}

CardCollection.propTypes = {
    classes: PropTypes.object.isRequired,
};

export default withStyles(styles)(CardCollection);