import { createStore } from 'redux'
import { auth } from './reducers'
import { loadState, saveState } from './localStorage'

const persistedState = loadState();
const store = createStore(auth, persistedState);

store.subscribe(() => {
    saveState(store.getState());
});

export const credentialStore = store;

export const getUser = () => {
    return credentialStore.getState().user;
};

export const getUserName = () => {
    const name = credentialStore.getState().user !== "Not logged in" ? 
        credentialStore.getState().user.firstName + " " + credentialStore.getState().user.lastName :
        undefined;
    
    return name;
};

export const isCourseCreator = () => {
    //TODO: This is dangerous to hardcode global role (1) here, since enum can change server side
    return credentialStore.getState().user !== "Not logged in" && credentialStore.getState().user.globalRoles && credentialStore.getState().user.globalRoles.includes(1)
};