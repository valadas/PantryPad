import { Component, Host, h, Element, Prop } from '@stencil/core';
import { href, Router } from 'stencil-router-v2';
import { Icon } from '../../icons/icons';

@Component({
  tag: 'pp-menu',
  styleUrl: 'pp-menu.scss',
  shadow: true,
})
export class PpMenu {

  @Element() el: HTMLElement;

  @Prop() Router!: Router;

  render() {
    return (
      <Host>
        <a
          {...href("/stores")}
          class={{active: this.Router.activePath === "/stores"}}
        >
          {Icon.store} Stores
        </a>
        <a
          {...href("/locations")}
          class={{active: this.Router.activePath === "/locations"}}
        >
          {Icon.door} Locationss
        </a>
        <a
          {...href("/units")}
          class={{active: this.Router.activePath === "/units"}}
        >
          {Icon.measuringCup} Units
        </a>
      </Host>
    );
  }
}
