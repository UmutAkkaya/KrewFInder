import React from 'react';
import classNames from 'classnames';
import PropTypes from 'prop-types';
import {withStyles} from 'material-ui/styles';
import Table, {
    TableBody,
    TableCell,
    TableFooter,
    TableHead,
    TablePagination,
    TableRow,
    TableSortLabel,
} from 'material-ui/Table';
import Toolbar from 'material-ui/Toolbar';
import Typography from 'material-ui/Typography';
import Paper from 'material-ui/Paper';
import Checkbox from 'material-ui/Checkbox';
import IconButton from 'material-ui/IconButton';
import Button from 'material-ui/Button';
import Tooltip from 'material-ui/Tooltip';
import DeleteIcon from 'material-ui-icons/Delete';
import FilterListIcon from 'material-ui-icons/FilterList';
import {lighten} from 'material-ui/styles/colorManipulator';
import Input, {InputLabel} from 'material-ui/Input';
import Select from 'material-ui/Select';
import {MenuItem} from 'material-ui/Menu';

const columnData = [
    {id: 'skill', numeric: false, disablePadding: false, label: 'Skill', sortKey: "Name"},
    {id: 'value', numeric: false, disablePadding: true, label: 'Value', sortKey: "Value"}
];

class SkillTableHead extends React.Component {
    createSortHandler = property => event => {
        this.props.onRequestSort(event, property);
    };

    render() {
        const {order, orderBy} = this.props;

        return (
            <TableHead>
                <TableRow>
                    {columnData.map(column => {
                            return (
                                <TableCell
                                    key={column.id}
                                    numeric={column.numeric}
                                    padding={column.disablePadding ? 'none' : 'default'}
                                    sortDirection={orderBy === column.id ? order : false}>
                                    <Tooltip
                                        title="Sort"
                                        placement={column.numeric ? 'bottom-end' : 'bottom-start'}
                                        enterDelay={300}>
                                        <TableSortLabel
                                            active={orderBy === column.sortKey}
                                            direction={order}
                                            onClick={this.createSortHandler(column.sortKey)}>
                                            {column.label}
                                        </TableSortLabel>
                                    </Tooltip>
                                </TableCell>
                            );
                        },
                        this)}

                </TableRow>
            </TableHead>
        );
    }
}

SkillTableHead.propTypes = {
    onRequestSort: PropTypes.func.isRequired,
    order: PropTypes.string.isRequired,
    orderBy: PropTypes.string.isRequired,
};

const toolbarStyles = theme => ({
    root: {
        paddingRight: theme.spacing.unit,
    },
    highlight:
        theme.palette.type === 'light'
            ? {
                color: theme.palette.secondary.main,
                backgroundColor: lighten(theme.palette.secondary.light, 0.85),
            }
            : {
                color: theme.palette.text.primary,
                backgroundColor: theme.palette.secondary.dark,
            },
    spacer: {
        flex: '1 1 100%',
    },
    actions: {
        color: theme.palette.text.secondary,
    },
    title: {
        flex: '0 0 auto',
    },
});

let EnhancedTableToolbar = props => {
    const {numSelected, classes} = props;

    return (
        <Toolbar
            className={classNames(classes.root,
                {
                    [classes.highlight]: numSelected > 0,
                })}>
            <div className={classes.title}>
                {numSelected > 0
                    ? (
                        <Typography color="inherit" variant="subheading">
                            {numSelected} selected
                        </Typography>
                    )
                    : (
                        <Typography variant="title">Group Skills</Typography>
                    )}
            </div>
            <div className={classes.spacer}/>
            <div className={classes.actions}>
                {numSelected > 0
                    ? (
                        <Tooltip title="Delete">
                            <IconButton aria-label="Delete">
                                <DeleteIcon/>
                            </IconButton>
                        </Tooltip>
                    )
                    : (
                        <Tooltip title="Filter list">
                            <IconButton aria-label="Filter list">
                                <FilterListIcon/>
                            </IconButton>
                        </Tooltip>
                    )}
            </div>
        </Toolbar>
    );
};

EnhancedTableToolbar.propTypes = {
    classes: PropTypes.object.isRequired,
    numSelected: PropTypes.number.isRequired,
};

EnhancedTableToolbar = withStyles(toolbarStyles)(EnhancedTableToolbar);

const styles = theme => ({
    root: {
        width: '100%',
        marginTop: theme.spacing.unit * 3,
    },
    tableWrapper: {
        overflowX: 'auto',
    },
    button: {
        margin: theme.spacing.unit,
    },
});

const SkillTableEditModes = {
    skillDomainCreator: "SDC", // Skills are freetext, value is dropdown of types
    skillSelector: "SS" // Skills are dropdowns, value is slider/dropdown
}

const presets = {
    "cs": [".NET", "Django", "PHP", "NodeJs", "C#", "Python", "Java", "Javascript", "Docker", "SQL"]
};

const SkillValuesToProps = [
    {
        type: "NumRangeSkill",
        minValue: 1,
        maxValue: 5
    }
];

function union_arrays(x, y) {
    var obj = {};
    for (var i = x.length - 1; i >= 0; --i)
        obj[x[i]] = x[i];
    for (var i = y.length - 1; i >= 0; --i)
        obj[y[i]] = y[i];
    var res = []
    for (var k in obj) {
        if (obj.hasOwnProperty(k)) // <-- optional
            res.push(obj[k]);
    }
    return res;
}

// pass skills through props
class SkillTable extends React.Component {
    constructor(props, context) {
        super(props, context);

        this.state = {
            order: 'asc',
            orderBy: 'Name',
            skillDomain: this.props.skillDomain || [],
            data: (this.props.skills && this.props.skills.sort((a, b) => (a.value > b.value ? -1 : 1))) || [],
            editMode: this.props.editMode || SkillTableEditModes.skillDomainCreator,
            editable: this.props.editable,
            nextId: -1
        };

        this.componentDidUpdate = this.componentDidUpdate.bind(this);

        // Determine the index of the Type description in SkillValuesToProps for each skill. This will need to be changed when more types are added
        this.state.data.forEach(s => s.valueIndex =
            SkillValuesToProps.map((skillVal, i) => ({skillVal, i})).filter(x => x.skillVal.type === s.type)[0].i);
    }

    componentDidUpdate() {
        if (this.props.editable) {
            this.props.onDataChanged(this.state.data);
        }
    }

    componentWillReceiveProps(props) {
        this.setState({
            ...this.state,
            editable: props.editable
        })
    }

    handleRequestSort = (event, property) => {
        const orderBy = property;
        let order = 'desc';

        if (this.state.orderBy === property && this.state.order === 'desc') {
            order = 'asc';
        }

        const data =
            order === 'desc'
                ? this.state.data.sort((a, b) => (b[orderBy] < a[orderBy] ? -1 : 1))
                : this.state.data.sort((a, b) => (a[orderBy] < b[orderBy] ? -1 : 1));

        this.setState({data, order, orderBy});
    };

    handleNew = (event) => {
        const data = this.state.data;
        let newId = this.state.nextId;
        data.unshift({id: newId, name: null, valueIndex: 0, ...SkillValuesToProps[0], value: 1});
        this.setState({...this.state, nextId: newId - 1, data});
    }

    handleDelete = (id) => (event) => {
        const data = this.state.data;
        let index = data.indexOf(data.filter(d => d.id === id)[0]);
        data.splice(index, 1);
        this.setState({...this.state, data});
    }

    getSkillOptions = (skill) => {
        let opts = [];
        switch (skill.type) {
            case "NumRangeSkill":
                for (var i = skill.minValue; i <= skill.maxValue; i++) {
                    opts.push({label: i, value: i});
                }
                return opts;
        }
    };


    createCellControl = (isSkill, elemId, getter, setter) => {
        const {editable, data} = this.state;

        const handleChange = (e) => {
            setter(e.target.value);
            this.setState({...this.state});
        }

        if (!editable) {
            return (<div>{getter()}</div>);
        } else if (isSkill) {
            let skillOptions = [];
            switch (this.state.editMode) {
                case SkillTableEditModes.skillDomainCreator:
                    return (<Input
                        defaultValue={getter()}
                        onChange={handleChange}/>);
                case SkillTableEditModes.skillSelector:
                    skillOptions = union_arrays(this.state.data.map(s => s.name), this.state.skillDomain).map(s => ({
                        value: s,
                        label: s
                    })).sort((a, b) => (a.label < b.label ? -1 : 1));
                    return (
                        <Select value={getter()}
                                onChange={handleChange}>
                            {skillOptions.map(opt => (
                                <MenuItem value={opt.value}>
                                    {opt.label}
                                </MenuItem>
                            ))}
                        </Select>);
            }

        } else { // this is a value
            let skillOptions = [];
            switch (this.state.editMode) {
                case SkillTableEditModes.skillDomainCreator:
                    return (<Select value={getter()}
                                    onChange={handleChange}>
                        <MenuItem value={0}>
                            Range 1 - 5
                        </MenuItem>
                    </Select>);
                case SkillTableEditModes.skillSelector:
                    skillOptions = this.getSkillOptions(data.filter(s => s.id === elemId)[0]);
                    return (
                        <Select value={getter()}
                                onChange={handleChange}>
                            {skillOptions.map(opt => (
                                <MenuItem value={opt.value}>
                                    {opt.label}
                                </MenuItem>
                            ))}
                        </Select>);
            }
        }
    };

    handlePreset = (event) => {
        let presetKey = event.target.value;
        let presetSkills = presets[presetKey];
        let newId = this.state.nextId;
        let newSkills =
            presetSkills.map(skill => ({id: newId--, name: skill, valueIndex: 0, ...SkillValuesToProps[0]}));

        const data = this.state.data;
        data.unshift(...newSkills);
        this.setState({...this.state, nextId: newId - 1, data});

    };

    render() {
        const {classes} = this.props;
        const {data, order, orderBy, editable, editMode} = this.state;

        return (
            <Paper className={classes.root}>
                <div className={classes.tableWrapper}>
                    <Table className={classes.table}>
                        <SkillTableHead
                            order={order}
                            orderBy={orderBy}
                            onRequestSort={this.handleRequestSort}
                            rowCount={data.length}/>
                        <TableBody>
                            {editable
                                ? (<TableRow
                                    hover
                                    tabIndex={-1}>
                                    <TableCell>
                                        <Button variant="raised" onClick={this.handleNew
                                        } color="primary" className={classes.button}>
                                            New
                                        </Button>
                                    </TableCell>
                                    {editMode === SkillTableEditModes.skillDomainCreator
                                        ? <TableCell>
                                            <InputLabel htmlFor="preset-helper">Presets</InputLabel>
                                            <Select value={""} onChange={this.handlePreset} input={
                                                <Input id="preset-helper"/>}>
                                                <MenuItem value="cs">
                                                    Computer Science
                                                </MenuItem>
                                            </Select>
                                        </TableCell>
                                        : null}
                                </TableRow>)
                                : null}
                            {data.map((n, i) => {
                                return (
                                    <TableRow
                                        hover
                                        tabIndex={i}
                                        key={n.id}>
                                        <TableCell>{this.createCellControl(true,
                                            n.id,
                                            () => n.name,
                                            (val) => n.name = val)
                                        }</TableCell>
                                        <TableCell numeric>{this.createCellControl(
                                            false,
                                            n.id,
                                            () => (!this.state.editable || this.state.editMode === SkillTableEditModes.skillSelector ? n.value : n.valueIndex),
                                            (val) => {
                                                if (this.state.editable &&
                                                    this.state.editMode === SkillTableEditModes.skillSelector) {
                                                    n.value = val;
                                                } else {
                                                    Object.assign(n,
                                                        {valueIndex: val, ...SkillValuesToProps[val]});
                                                }
                                            })}</TableCell>
                                        {editable
                                            ? (<TableCell>
                                                <Button variant="raised" color="secondary" onClick={this
                                                    .handleDelete(n.id)} className={classes.button}>
                                                    Delete
                                                </Button>
                                            </TableCell>)
                                            : null}
                                    </TableRow>
                                );
                            })}
                        </TableBody>
                    </Table>
                </div>
            </Paper>
        );
    }
}

SkillTable.propTypes = {
    classes: PropTypes.object.isRequired,
};

export default withStyles(styles)(SkillTable);
export {SkillTableEditModes};