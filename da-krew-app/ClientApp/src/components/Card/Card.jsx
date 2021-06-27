import React, {Component} from 'react';
import {Link} from 'react-router-dom';
import {withStyles} from 'material-ui/styles';
import Card, {CardActions, CardContent} from 'material-ui/Card';
import Button from 'material-ui/Button';
import Typography from 'material-ui/Typography';
import './Card.css';

export default class SimpleCard extends Component {

    constructor(props) {
        super(props);
        
        this.state = {
            buttonsHidden: false
        };
        
        this.hide = this.hide.bind(this);
    }

    hide() {
        this.setState({
            buttonsHidden: true
        })
    }
    
    render() {
        return (
            <Card className="card">
                <CardContent>
                    {this.props.title && <Typography className="cardTitle">{this.props.title}</Typography>}
                    <Typography variant="headline" component="h2">
                        {this.props.headline}
                    </Typography>
                    <Typography className="pos">{this.props.subtext}</Typography>
                    <Typography className="cardDescription" component="p">
                        {this.props.description}
                    </Typography>
                </CardContent>
                <CardActions>
                    {
                        this.props.hrefAction == undefined && this.props.href != undefined &&
                        <Button component={Link} to={this.props.href} size="small">{this.props.hrefText}</Button>
                    }
                    {
                        this.props.hrefAction != undefined && !this.state.buttonsHidden &&
                        <Button onClick={() => {this.props.hrefAction(); this.hide();}} size="small">{this.props.hrefText}</Button>
                    }

                    {
                        this.props.href2 !== undefined && this.props.hrefAction2 == undefined &&
                        <Button component={Link} to={this.props.href2} size="small">{this.props.hrefText2}</Button>
                    }
                    {
                        this.props.hrefAction2 != undefined && !this.state.buttonsHidden &&
                        <Button onClick={() => {this.props.hrefAction2(); this.hide();}} size="small">{this.props.hrefText2}</Button>
                    }
                </CardActions>
            </Card>
        );
    }
}