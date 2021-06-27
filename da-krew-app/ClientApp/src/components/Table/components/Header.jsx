// State: { col[] columns, current_sort: { column_index: int, direction: int } }
// Actions: SortChange: { column_index: int, direction: int (1 = asc, -1 = desc, 0 = none) }

import React from 'react';

let header = ({ columns, current_sort, sort_changed }) => {
    return ( // TODO: Sorting
        <thead>
        <tr>
            {columns.map((col, i) => (<th scope="col" onClick={e => sort_changed({ column_index: i, direction: (current_sort.direction + 1) % 3 - 1 })}>{col.name}</th>))}
        </tr>
        </thead>
        );
}

export default header;