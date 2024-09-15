import { Component, Prop, h, State } from '@stencil/core';
import { format } from '../../utils/utils';

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

  @State() message: string;
  @State() date: any;

  componentWillLoad() {
    fetch('http://localhost:8099/api/simple')
    .then(response => response.json())
    .then(data => {
      this.message = data.message;
      this.date = data.date;
    })
    .catch(error => {
      console.error(error);
    });
  }

  private getText(): string {
    return format("this.first", this.middle, this.last);
  }

  render() {
    return <host>
      <div>Hello, World! I'm {this.getText()}</div>
      <p>{this.message}</p>
      <p>{this.date}</p>
      <p>Does this work d!!!?</p>
      <p>WHY not???</p>
    </host>;
  }
}
