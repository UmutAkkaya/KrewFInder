
export default (state, action) =>
{
    switch (action.type) {
        case "QUERY_CHANGE":
            return handleQueryChange(state, action);
        default:
            return state;
    }
}

const handleQueryChange = (state, action) => {
    return {
        ...state,
        query: action.query
    }
};