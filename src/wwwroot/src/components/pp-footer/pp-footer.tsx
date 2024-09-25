import { Component, Host, h } from '@stencil/core';

@Component({
  tag: 'pp-footer',
  styleUrl: 'pp-footer.scss',
  shadow: true,
})
export class PpFooter {
  render() {
    return (
      <Host>
          <pp-color-mode-toggler />
          <p class="copyright">© 2021 Daniel Valadas</p>
      </Host>
    );
  }
}
