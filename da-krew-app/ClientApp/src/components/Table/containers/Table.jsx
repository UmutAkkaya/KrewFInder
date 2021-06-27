import Table from '../components/Table'

import { connect } from 'react-redux'
import { sortChange } from '../actions'

const mapStateToProps = (state, ownProps) => {
    return {
        current_sort: {
            column_index: 0,
            direction: 0
        },
        columns: [{ name: "col 0" }, { name: "col 1" }],
        rows: [
            {
                cells: [
                    {
                        contents: [
                            {
                                text: "col0 text1",
                                action: null
                            }
                        ]
                    },
                    {
                        contents: [
                            {
                                text: "action1",
                                action: () => alert("action1")
                            },
                            {
                                text: "action2",
                                action: () => alert("action2")
                            }
                        ]
                    }
                ]
            }
        ]
    }
}
const mapDispatchToProps = (dispatch) => {
    return {
        sort_changed: (new_sort) => {
            dispatch(sortChange(new_sort));
        }
    }
}

let SortChange = connect(mapStateToProps, mapDispatchToProps)(Table);

export default SortChange;