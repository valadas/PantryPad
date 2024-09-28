import { Component, Host, h } from '@stencil/core';
import { Route, createRouter, match } from "stencil-router-v2";

const Router = createRouter();

@Component({
  tag: 'app-root',
  styleUrl: 'app-root.scss',
  shadow: true,
})
export class AppRoot {
  render() {
    return (
      <Host>
        <header>
          <h1>PantryPad</h1>
          <dnn-searchbox />
          {/* This is a hack to make sure other svgs in shadow-root render. */}
          <svg width={0} height={0}></svg>
          <nav>[user] [settings] [mobile-menu]</nav>
        </header>
        <section class="mid">
          <aside>
            <pp-menu Router={Router}></pp-menu>
          </aside>
          <main>
            <Router.Switch>
              <Route path="/" to="/units" />
              <Route path="/stores" render={() => <div>
                <div class="some-surface">
                  <p>
                    Hello world!
                  </p>
                </div>
              </div>} />
              <Route path="/locations" render={() => <div>Locations</div>} />
              <Route path="/units" render={() => <div>Units</div>} />
              <Route
                path={match("/unit/:unitId")}
                render={({ unitId }) => (
                  <p>Unit ID: {unitId}</p>
                )}
              />
            </Router.Switch>
          </main>
        </section>
        <footer>
          <pp-footer />
        </footer>
      </Host>
    );
  }
}
