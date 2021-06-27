// State: { row[] rows }

import React from 'react';
import Row from '../containers/Row'

let tbody = ({ rows }) => {
    return (
        <tbody>
            {rows.map(row => (<Row {...row}/>))}
        </tbody>
        );
}

export default tbody;