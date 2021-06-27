import React, { Component } from 'react';
import { Route, Switch } from 'react-router';
import Login from "./authentication/Login";
import Register from "./authentication/Register";
import Dashboard from "./components/Dashboard";
import OrganizationCreatePage from "./components/Organizations/OrganizationsCreatePage";
import Groups from "./components/Groups/Groups";
import { Home } from './components/Home';
import { Layout } from './components/Layout';
import GroupPage from "./components/Groups/GroupPage";
import UserPage from "./components/Users/UserPage";
import { credentialStore } from "./credentialStore/store";
import { authDiscardToken } from "./credentialStore/actions";

export default class App extends Component {
    displayName = App.name;

    render() {
        // TODO: if no token redirect to login

        return (
            <Layout>
                <Route exact path='/' component={Home} />
                <Route exact path="/login" component={Login} />
                <Route exact path="/logout" component={Logout} />
                <Route exact path="/register" component={Register} />
                <Route exact path="/dashboard" component={Dashboard} />

                <Switch>
                    <Route exact path="/organizations/create" component={OrganizationCreatePage} />
                    <Route path="/organizations/:orgId/user/:userId" component={UserPage} />
                    <Route path="/organizations/:organizationId" component={Groups} />
                </Switch>

                <Route path="/groups/:groupId" component={GroupPage} />
            </Layout>);
    }
}

const Logout = () => {
    credentialStore.dispatch(authDiscardToken());
    window.location = '/';
}