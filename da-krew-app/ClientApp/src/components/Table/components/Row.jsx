//State: { row : { cells:[{ contents:[text, action: callback?] }] } }
//Actions: ActableClicked: { callback: action }

import React from 'react';

let row = ({ cells, cellClicked }) => {
    return (
        <tr>
            {cells.map(cell => (
                <td>{cell.contents.map(con =>
                    con.action ?
                        (<a href="#" onClick={e => cellClicked(cell, con.action)}>{con.text}</a>) :
                        con.text
                    )
                }</td>
            ))}
        </tr>
    );
}

export default row;