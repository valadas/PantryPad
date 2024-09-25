import { Component, Host, h, State } from '@stencil/core';
import { Icon } from '../../icons/icons';

@Component({
  tag: 'pp-color-mode-toggler',
  styleUrl: 'pp-color-mode-toggler.scss',
  shadow: true,
})
export class PpColorModeToggler {
  
  @State() colorMode: 'light' | 'dark' | 'auto' = 'auto';

  componentWillLoad(): void {
    this.colorMode = localStorage.getItem('pp-color-mode') as 'light' | 'dark' | 'auto' || 'auto';
    window.onstorage = e => {
      if (e.key == 'pp-color-mode') {
        this.colorMode = localStorage.getItem('pp-color-mode') as 'light' | 'dark' | 'auto' || 'auto';
      }
    };
  }

  private changeColorMode(): void {
    switch (this.colorMode) {
      case 'light':
        this.colorMode = 'dark';
        break;
      case 'dark':
        this.colorMode = 'auto';
        break;
      case 'auto':
        this.colorMode = 'light';
        break;
    }
    localStorage.setItem('pp-color-mode', this.colorMode);
  }

  componentWillRender()
  {
    return new Promise<void>(resolve => {
      document.documentElement.classList.remove('dnn-color-scheme-light', 'dnn-color-scheme-dark', 'dnn-color-scheme-auto');
      document.documentElement.classList.add(`dnn-color-scheme-${this.colorMode}`);
      resolve();
    })
  }

  render() {
    return (
      <Host>
        <button onClick={() => this.changeColorMode()}>
          {this.colorMode === 'light' &&
            Icon.lightMode
          }
          {this.colorMode === 'dark' &&
            Icon.darkMode
          }
          {this.colorMode === 'auto' &&
            Icon.contrast
          }
        </button>
      </Host>
    );
  }
}
