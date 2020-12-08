import React, { Component } from 'react';
import DatePicker from "react-datepicker";
import debounce from 'lodash.debounce';
import find from 'lodash/find';
import "react-datepicker/dist/react-datepicker.css";
import './WhetherLayout.css';
import Popover from '@material-ui/core/Popover';



export class WhetherStats extends Component {
    static displayName = WhetherStats.name;

    constructor(props) {
        super(props);
        this.state = {
            data: {}, loading: true, date: new Date(), daysInPast: 5, daysInFuture: 15, city: 'minsk', isOpen: false, selectedDay: { details: [] }
        };
        this.onTextChangePast = this.onTextChangePast.bind(this);
        this.onTextChangeFuture = this.onTextChangeFuture.bind(this);
        this.onChangeDebounced = debounce(this.onChangeDebounced, 1000)
        this.fillData();
    }
    fillData = () => {
        fetch(`api/WhetherStats?date=${this.state.date.toISOString()}&daysInPast=${this.state.daysInPast}&daysInFuture=${this.state.daysInFuture}&city=${this.state.city}`)
            .then(response => response.json())
            .then(data => {
                this.setState({ data: data, loading: false });
            });
    }

    renderYearStats(yearStat) {
        return (
            <tr key={yearStat.year}>
                <td className={"year"}>{yearStat.year}</td>
                {
                    yearStat.daysStat.map(stat =>
                        <td key={stat.id} className={"day " + stat.dayClass}
                            onClick={(event) => this.handleClick(event, stat.id)}

                        >
                            <span className={"week-day-title " + stat.weekEndClass}>{stat.weekDay}</span>
                            <span style={{ fontSize: 20, color: "#ff7200" }}>{stat.dayTemperature}</span>/<span style={{ fontSize: 14, color: "gray" }}>{stat.nightTemperature}</span>
                            <p dangerouslySetInnerHTML={{ __html: stat.whetherIcons }}></p>
                        </td>
                    )}
            </tr>
        )
    }


    renderForecastsTable(data) {
        return (
            <table className='table table-col-striped' >
                <thead>
                    <tr>
                        <td>Year</td>
                        {data.daysRange.map(day =>
                            <th id={day.id} className={"day-title " + (day.isCurrentDay ? "today" : "")} key={day.dayTitle}>
                                {day.dayTitle}{day.isCurrentDay}

                            </th>

                        )}
                    </tr>
                </thead>
                <tbody >
                    {data.yearsStats.map(yearStat =>
                        this.renderYearStats(yearStat)
                    )}
                </tbody>
            </table>
        );
    }

    render() {
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : this.renderForecastsTable(this.state.data);

        return (
            <div>
                <h1>Weather Stats</h1>
                <Popover
                    id="simple-popover"
                    open={this.state.isOpen}
                    anchorEl={this.state.target}
                    onClose={this.handleClose}
                    anchorOrigin={{
                        vertical: 'top',
                        horizontal: 'right',
                    }}
                    transformOrigin={{
                        vertical: 'top',
                        horizontal: 'left',
                    }}
                >
                    <div className="whether-popover">
                        <div className="row">
                            <div className="col-sm-1">
                            </div>
                            <div className="col-sm-3">
                                Temperature
                            </div>
                                <div className="col-sm-2">
                                    Clouds
                            </div>
                                <div className="col-sm-2">
                                    Preasue
                            </div>
                                <div className="col-sm-2">
                                    Humidity
                            </div>
                                <div className="col-sm-2">
                                    Wind
                            </div>
                        </div>
                        {this.state.selectedDay.details.map(dayPart =>
                            <div className="row">
                                <div className="col-sm-1">
                                    {dayPart.dayPartName}:
                                </div>
                                <div className="col-sm-3">
                                    {dayPart.temperature}
                                </div>
                                <div className="col-sm-2" dangerouslySetInnerHTML={{ __html: dayPart.clouds }}>
                                </div>
                                <div className="col-sm-2">
                                    {dayPart.preasure}
                                </div>
                                <div className="col-sm-2">
                                    {dayPart.humidity}
                                </div>
                                <div className="col-sm-2" style={{ whiteSpace: "nowrap" }}>
                                    {dayPart.wind}&nbsp;(<div style={{ display: "inline-block" }} dangerouslySetInnerHTML={{ __html: dayPart.windDirection }}></div>)
                                </div>
                            </div>
                        )}
                    </div>
                </Popover>
                <div className="row">
                    <div className="col-sm-1">
                        <div className="form-group">
                            <label>Date</label>
                            <DatePicker className="form-control"
                                selected={this.state.date}
                                onChange={this.handleChange} //only when value has changed
                            />
                        </div>
                    </div>
                    <div className="col-sm-1">
                        <div className="form-group">
                            <label>City</label>
                            <select className="form-control input-sm" id="city" name="city" value={this.state.city} onChange={this.onCityChange}>
                                <option value="minsk">Minsk</option>
                                <option value="grodno">Grodno</option>
                                <option value="mogilev">Mogilev</option>
                                <option value="brest">Brest</option>
                                <option value="gomel">Gomel</option>
                                <option value="vitebsk">Vitebsk</option>
                            </select>
                        </div>
                    </div>
                    <div className="col-sm-1">
                        <div className="form-group">
                            <label>Days Back</label>
                            <input type="number" name="daysBack" id="daysBack" className="form-control" value={this.state.daysInPast} onChange={this.onTextChangePast} />
                        </div>
                    </div>
                    <div className="col-sm-1">
                        <div className="form-group">
                            <label>Days Forward</label>
                            <input type="number" name="daysForward" id="daysForward" className="form-control" value={this.state.daysInFuture} onChange={this.onTextChangeFuture} />
                        </div>
                    </div>
                </div>
                {contents}
            </div>
        );
    }

    handleChange = date => {
        this.setState({ date: date });
        this.onChangeDebounced()
    };

    onTextChangePast = (event) => {
        this.setState({ daysInPast: event.target.value });
        this.onChangeDebounced()
    };
    onTextChangeFuture = (event) => {
        this.setState({ daysInFuture: event.target.value });
        this.onChangeDebounced()
    };

    onCityChange = (event) => {
        this.setState({ city: event.target.value });
        this.onChangeDebounced()
    };

    onChangeDebounced = () => {
        this.setState({ loading: true });
        this.fillData();
    }

    handleClick = (event, id) => {
        let selectedDay = null;
        for (var i = 0; i < this.state.data.yearsStats.length; i++) {
            selectedDay = find(this.state.data.yearsStats[i].daysStat, x => x.id === id)
            if (selectedDay) {
                break;
            }
        }
        this.setState({ target: event.currentTarget, isOpen: true, selectedDay: selectedDay })
    };

    handleClose = () => {
        this.setState({ isOpen: false });
    };
}
