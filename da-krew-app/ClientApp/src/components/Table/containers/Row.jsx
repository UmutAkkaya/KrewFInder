import Row from '../components/Row'

import { connect } from 'react-redux'
import { actionClicked } from '../actions'


const mapDispatchToProps = (dispatch) => {
    return {
        cellClicked: (cell, callback) => {
            dispatch(actionClicked({ cell, callback }));
        }
    }
}

let CellClicked = connect(null, mapDispatchToProps)(Row);

export default CellClicked;