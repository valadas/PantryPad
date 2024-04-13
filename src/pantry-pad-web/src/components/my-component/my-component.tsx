import { Component, Prop, h, State } from '@stencil/core';
import { format } from '../../utils/utils';
import { WeatherForecast } from './types';

@Component({
  tag: 'my-component',
  styleUrl: 'my-component.css',
  shadow: true,
})
export class MyComponent {
  /**
   * The first name
   */
  @Prop() first: string;

  /**
   * The middle name
   */
  @Prop() middle: string;

  /**
   * The last name
   */
  @Prop() last: string;
  
  @State()data: WeatherForecast[];

  componentWillLoad() {
    fetch("http://localhost:5055/weatherforecast", {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
      },
    })
    .then((response) => response.json())
    .then(data => this.data = data)
    .catch((error) => console.error("Error:", error));
  }

  private getText(): string {
    return format(this.first, this.middle, this.last);
  }

  render() {
    return <div>
      <p>Hello, World! I'm {this.getText()}</p>
      {this.data != undefined && 
        <fieldset>
          <legend>Weather Forcasts</legend>
          <table>
            <thead>
              <tr>
                <th>Date</th>
                <th>Temperature (C)</th>
                <th>Temperature (F)</th>
                <th>Summary</th>
              </tr>
            </thead>
            <tbody>
              {this.data.map((forecast) => 
                <tr>
                  <td>{forecast.date}</td>
                  <td>{forecast.temperatureC}</td>
                  <td>{forecast.temperatureF}</td>
                  <td>{forecast.summary}</td>
                </tr>
              )}
            </tbody>
          </table>
        </fieldset>
      }
    </div>;
  }
}
