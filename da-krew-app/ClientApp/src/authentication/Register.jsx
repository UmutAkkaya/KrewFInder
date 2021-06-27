import React, { Component } from "react";
import Button from 'material-ui/Button';
import { FormControl, FormGroup } from 'material-ui/Form';
import Input, { InputLabel } from 'material-ui/Input';
import "./Register.css";
import { authDiscardToken, authSetToken, authSetUser } from "../credentialStore/actions";
import { credentialStore } from "../credentialStore/store";
import { FormErrors } from "../components/FormErrors/FormErrors"
import { USER_API_ROOT } from "../ApiConstants";
var request = require('superagent');

export default class Register extends Component {
    constructor(props) {
        super(props);

        this.state = {
            firstName: "",
            lastName: "",
            email: "",
            password: "",
            confirmPassword: "",
            formErrors: { email: '', password: '', passwordMatch: '' },
            emailValid: false,
            passwordValid: false,
            passwordConfirm: false,
            formValid: false
        };
    }

    errorClass(error) {
        return (error.length === 0 ? '' : 'has-error');
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


    validateField(fieldName, value) {
        let fieldValidationErrors = this.state.formErrors;
        let emailValid = this.state.emailValid;
        let passwordValid = this.state.passwordValid;
        let passwordConfirm = this.state.passwordConfirm;

        switch (fieldName) {
            case 'email':
                emailValid = value.match(/^([\w.%+-]+)@([\w-]+\.)+([\w]{2,})$/i);
                fieldValidationErrors.email = emailValid ? '' : ' Email is invalid';
                break;
            case 'password':
                passwordValid = value.length >= 6;
                fieldValidationErrors.password = passwordValid ? '' : 'Password is too short';
                break;
            case 'confirmPassword':
                passwordConfirm = (value == this.state.password);
                fieldValidationErrors.passwordMatch = passwordConfirm ? '' : 'Passwords do not match';
                break;
            default:
                break;
        }
        this.setState({
            formErrors: fieldValidationErrors,
            emailValid: emailValid,
            passwordValid: passwordValid,
            passwordConfirm: passwordConfirm
        }, this.validateForm);
    }

    validateForm() {
        this.setState({ formValid: this.state.emailValid && this.state.passwordValid && this.state.passwordConfirm });
    }

    handleSubmit = event => {
        event.preventDefault();
        let form = {
            "firstName": this.state.firstName,
            "lastName": this.state.lastName,
            "email": this.state.email,
            "password": this.state.password
        }

        this.setState({
            formErrors: { email: '', password: '', passwordMatch: '' }
        });

        let url = USER_API_ROOT + 'CreateUser';
        request
            .post(url)
            .set('Content-Type', 'application/json')
            // TODO: check with the backend if we need to serialize
            .send(form)
            .then(
            response => {
                window.location = "/login";
            })
            .catch(
            error => {
                let err = JSON.parse(error.response.text).Error;
                this.setState({
                    formErrors: { email: '', password: '', passwordMatch: '', resp: err }
                });
            }
            );
    };

    isEmpty(value) {
        if (value === '') return 'error';
        return 'success';
    }

    render() {
        return (
            <div className={"background"}>
                <div className={"Register"}>

                    <form onSubmit={this.handleSubmit} className={"registerForm"}>
                        <div className={"showError"}>
                            <FormErrors formErrors={this.state.formErrors} />
                        </div>
                        <h1>Please register to begin using our services.</h1>
                        <FormGroup id="firstName">
                            <FormControl>
                                <InputLabel htmlFor="firstName">First Name</InputLabel>
                                <Input id="firstName" value={this.state.firstName} name="firstName" onChange={this.handleChange} />
                            </FormControl>
                        </FormGroup>

                        <FormGroup id="lastName">
                            <FormControl>
                                <InputLabel htmlFor="lastName">Last Name</InputLabel>
                                <Input id="lastName" value={this.state.lastName} name="lastName" onChange={this.handleChange} />
                            </FormControl>
                        </FormGroup>

                        <FormGroup id="email" className={`form-group ${this.errorClass(this.state.formErrors.email)}`}>
                            <FormControl>
                                <InputLabel htmlFor="email">Email</InputLabel>
                                <Input id="email" type="email" value={this.state.email} name="email" onChange={(event) => this.handleUserInput(event)} />
                            </FormControl>
                        </FormGroup>

                        <FormGroup id="password" className={`form-group ${this.errorClass(this.state.formErrors.password)}`}>
                            <FormControl>
                                <InputLabel htmlFor="password">Password</InputLabel>
                                <Input id="password" type="password" value={this.state.password} name="password" onChange={(event) => this.handleUserInput(event)} />
                            </FormControl>
                        </FormGroup>

                        <FormGroup id="confirmPassword" className={`form-group ${this.errorClass(this.state.formErrors.passwordMatch)}`}>
                            <FormControl>
                                <InputLabel htmlFor="confirmPassword">Confirm Password</InputLabel>
                                <Input id="confirmPassword" type="password" value={this.state.confirmPassword} name="confirmPassword" onChange={(event) => this.handleUserInput(event)} />
                            </FormControl>
                        </FormGroup>



                        <Button type="submit"
                            label="login"
                            className="button-submit"
                            disabled={!this.state.formValid}>
                            Register
                        </Button>

                    </form>
                </div>
            </div>
        );
    }
}
