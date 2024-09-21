import { Component, h, State } from '@stencil/core';

@Component({
  tag: 'my-component',
  styleUrl: 'my-component.css',
  shadow: true,
})
export class MyComponent {
  @State() message: string;
  @State() date: any;

  componentWillLoad() {
    const basePath = window.location.pathname.replace(/\/$/, ""); // Remove trailing slash, if any.
    fetch(`${basePath}/api/simple`)
    .then(response => response.json())
    .then(data => {
      this.message = data.message;
      this.date = data.date;
    })
    .catch(error => {
      console.error(error);
    });
  }


  render() {
    return <host>
      <div>Hello, World!</div>
      <p>{this.message}</p>
      <p>{this.date}</p>
      <p>Does this work ?</p>
    </host>;
  }
}
