// https://medium.com/@MattiaManzati/tips-to-handle-authentication-in-redux-4d596e11bb21

var {authDiscardToken} = require('./actions');
var request = require('superagent');
var API_ROOT = 'http://localhost:3000/api';

/*
This below is the format the middleware accepts.
{
  types: [ACT_LOGIN_PENDING, ACT_LOGIN_SUCCESS, ACT_LOGIN_ERROR],
  url: '/auth/login',
  method: 'post',
  query: {'ajax': true},
  data: {username, password},
  dataType: 'json'
}
*/
module.exports = store => next => action => {
  // If its not an core-app async action, pass next.
  if(typeof action.types === 'undefined' || typeof action.url === 'undefined') return next(action);

  // get action types
  var [pendingType, successType, errorType] = action.types;

  // construct the request
  var {
    url,
    method = 'get',
    dataType = 'json',
    query = {},
    data = {},
  } = action;

  // Prepend prefix for requests
  if(url.indexOf('/') === 0) url = API_ROOT + url;

  var req = method === 'post' ? request.post(url).send(data) : request.get(url);
  req = req.set('X-Requested-With', 'XMLHttpRequest');
  // here it's the magic part which appends the token
  var {auth: {token} = {}} = store.getState();
  req = req.query({
    ...query,
    token
  });

  // Dispatch the pending action
  next({
    type: pendingType,
    isPending: true
  });

  req.end((err, data) => {
    if(err){
      // here is the auto-logout on rest call failed with 401 http status
      if(err.status === 401) return store.dispatch(authDiscardToken());
      
      // Return the error action
      next({
        type: errorType,
        isPending: false,
        errors: err
      });

      return;
    }

    // Successfully pass the result action
    next({
      type: successType,
      isPending: false,
      data: data.body
    });

    return;

  });
};