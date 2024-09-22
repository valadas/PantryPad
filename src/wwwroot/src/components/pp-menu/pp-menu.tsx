import { Component, Host, h, Element } from '@stencil/core';
import { href } from 'stencil-router-v2';
import { Router } from '../..';
import { Icon } from '../../icons/icons';

@Component({
  tag: 'pp-menu',
  styleUrl: 'pp-menu.scss',
  shadow: true,
})
export class PpMenu {

  @Element() el: HTMLElement;

  render() {
    return (
      <Host>
        <a
          {...href("#/stores")}
          class={{active: Router.activePath === "/stores"}}
        >
          {Icon.store} Stores
        </a>
        <a
          {...href("#/locations")}
          class={{active: Router.activePath === "/locations"}}
        >
          {Icon.door} Locations
        </a>
        <a
          {...href("#/units")}
          class={{active: Router.activePath === "/units"}}
        >
          {Icon.measuringCup} Units
        </a>
      </Host>
    );
  }
}
