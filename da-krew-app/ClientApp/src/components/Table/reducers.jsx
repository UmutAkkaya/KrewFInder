

export default (state, action) =>
{
    switch (action.type) {
        case "SORT_CHANGE":
            return handleSortChange(state, action);
        case "ACTION_CLICKED":
            return handleActionClicked(state, action);
        default:
            return state;
    }
}

const handleSortChange = (state, action) => {
    return {
        columns: state.columns,
        current_sort: action.sort
    }
}

const handleActionClicked = (state, action) => {
    action.action.callback(action.action.cell);
    return state;
}