import React, { Component } from "react";
import Button from 'material-ui/Button';
import { FormControl, FormGroup } from 'material-ui/Form';
import Input, { InputLabel } from 'material-ui/Input';
import "./Login.css";
import { credentialStore } from "../credentialStore/store"
import {
    authSetToken,
    authDiscardToken,
    authSetUser,
    authSetCredentials
} from '../credentialStore/actions'
import { FormErrors } from "../components/FormErrors/FormErrors"
import { AUTH_API_ROOT } from "../ApiConstants";
var request = require('superagent');

export default class Login extends Component {
    constructor(props) {
        super(props);

        this.state = {
            email: "",
            password: "",
            formErrors: { email: '' },
            emailValid: false,
            formValid: false
        };
    }

    errorClass(error) {
        return (error.length === 0 ? '' : 'has-error');
    }

    validateField(fieldName, value) {
        let fieldValidationErrors = this.state.formErrors;
        let emailValid = this.state.emailValid;

        switch (fieldName) {
            case 'email':
                emailValid = value.match(/^([\w.%+-]+)@([\w-]+\.)+([\w]{2,})$/i);
                fieldValidationErrors.email = emailValid ? '' : ' Email is invalid';
                break;
            default:
                break;
        }
        this.setState({
            formErrors: fieldValidationErrors,
            emailValid: emailValid,
        }, this.validateLoginForm);
    }

    validateLoginForm() {
        this.setState({ formValid: this.state.emailValid });
    }


    validateForm() {
        return this.state.email.length > 0 && this.state.password.length > 0;
    }

    handleUserInput(e) {

        const name = e.target.name;
        const value = e.target.value;
        this.setState({ [name]: value },
            () => { this.validateField(name, value) });
    }

    handleChange = event => {
        this.setState({
            [event.target.id]: event.target.value
        });
    };

    handleSubmit = event => {
        event.preventDefault();
        let credentials = this.state;
        let url = AUTH_API_ROOT + "login";

        request
            .post(url)
            .set('Content-Type', 'application/json')
            .send({ login: credentials.email, password: credentials.password })
            .end((err, res) => {
                if (res && res.statusCode >= 400) {
                    let errors = this.state.formErrors;
                    errors.general = JSON.parse(res.text).Error;

                    this.setState({
                        formErrors: errors,
                        emailValid: true,
                    }, this.validateLoginForm);
                }
                else if (res) {
                    credentialStore.dispatch(authSetCredentials(res.headers['authorization'], res.body));
                    window.location = '/dashboard';
                } else {
                    credentialStore.dispatch(authDiscardToken());
                    console.log(err);
                }
            });
    };

    render() {
        return (
            <div className={"background"}>
                <div className={"Login"}>
                    <form onSubmit={this.handleSubmit} className={"loginForm"}>
                        <div className={"showError"}>
                            <FormErrors formErrors={this.state.formErrors} />
                        </div>
                        <h1>Please login to begin using the service.</h1>
                        <FormGroup id="email" className={`form-group ${this.errorClass(this.state.formErrors.email)}`}>
                            <FormControl>
                                <InputLabel htmlFor="email">Email</InputLabel>
                                <Input id="email" type="email" value={this.state.email} name="email" onChange={(event) => this.handleUserInput(event)} />
                            </FormControl>
                        </FormGroup>
                        <FormGroup id="password">
                            <FormControl>
                                <InputLabel htmlFor="password">Password</InputLabel>
                                <Input id="password" type="password" value={this.state.password} onChange={this.handleChange} />
                            </FormControl>
                        </FormGroup>
                        <Button type="submit" l
                            abel="login"
                            className="button-submit"
                            disabled={!this.validateForm()}>
                            Login
                        </Button>
                    </form>
                </div>
            </div>
        );
    }
}
