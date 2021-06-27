//State: { obj[] objects }

import React from "react";
import Header from "./Header";
import Body from "./Body";

/*
 * {
 *      current_sort: {column_index: int, direction: int}
 *      columns: [{
 *          name: "name"
 *      }]
 *      rows: [{
 *          cells: [{
 *              contents: [{
 *                  text: "text",
 *                  action: func(cell)
 *              }]
 *          }]
 *          cellClicked: func(cell, callback)
 *      }]
 * }
 */

let table = ({ rows, columns, current_sort, sort_changed }) => {
    return (
        <table className="table table-striped">
            <Header columns={columns} current_sort={current_sort} sort_changed={sort_changed} />
            <Body rows={rows} />
        </table>
    );
}

export default table;