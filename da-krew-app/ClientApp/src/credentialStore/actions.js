var {
    AUTH_SET_TOKEN,
    AUTH_DISCARD_TOKEN,
    AUTH_SET_USER,
    AUTH_SET_CRED
  } = require('./constants');

function authSetToken(token) {
    return {
        type: AUTH_SET_TOKEN,
        token
    };
}

function authDiscardToken() {
    return {
        type: AUTH_DISCARD_TOKEN
    };
}

function authSetCredentials(token, user) {
    return {
        type: AUTH_SET_CRED,
        user: user,
        token: token
    };
}

function authSetUser(user) {
    return {
        type: AUTH_SET_USER,
        user
    };
}

module.exports = {
    authSetToken,
    authDiscardToken,
    authSetUser,
    authSetCredentials
};