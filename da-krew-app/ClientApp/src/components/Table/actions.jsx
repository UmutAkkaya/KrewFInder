//export

export const sortChange = (sort) => {
    return {
        type: "SORT_CHANGE",
        sort
    }
}

export const actionClicked = (action) => {
    return {
        type: "ACTION_CLICKED",
        action
    }
}