/* eslint-disable */
/* tslint:disable */
/**
 * This is an autogenerated file created by the Stencil compiler.
 * It contains typing information for all components that exist in this project.
 */
import { HTMLStencilElement, JSXBase } from "@stencil/core/internal";
import { Router } from "stencil-router-v2";
export { Router } from "stencil-router-v2";
export namespace Components {
    interface AppRoot {
    }
    interface PpColorModeToggler {
    }
    interface PpFooter {
    }
    interface PpMenu {
        "Router": Router;
    }
}
declare global {
    interface HTMLAppRootElement extends Components.AppRoot, HTMLStencilElement {
    }
    var HTMLAppRootElement: {
        prototype: HTMLAppRootElement;
        new (): HTMLAppRootElement;
    };
    interface HTMLPpColorModeTogglerElement extends Components.PpColorModeToggler, HTMLStencilElement {
    }
    var HTMLPpColorModeTogglerElement: {
        prototype: HTMLPpColorModeTogglerElement;
        new (): HTMLPpColorModeTogglerElement;
    };
    interface HTMLPpFooterElement extends Components.PpFooter, HTMLStencilElement {
    }
    var HTMLPpFooterElement: {
        prototype: HTMLPpFooterElement;
        new (): HTMLPpFooterElement;
    };
    interface HTMLPpMenuElement extends Components.PpMenu, HTMLStencilElement {
    }
    var HTMLPpMenuElement: {
        prototype: HTMLPpMenuElement;
        new (): HTMLPpMenuElement;
    };
    interface HTMLElementTagNameMap {
        "app-root": HTMLAppRootElement;
        "pp-color-mode-toggler": HTMLPpColorModeTogglerElement;
        "pp-footer": HTMLPpFooterElement;
        "pp-menu": HTMLPpMenuElement;
    }
}
declare namespace LocalJSX {
    interface AppRoot {
    }
    interface PpColorModeToggler {
    }
    interface PpFooter {
    }
    interface PpMenu {
        "Router": Router;
    }
    interface IntrinsicElements {
        "app-root": AppRoot;
        "pp-color-mode-toggler": PpColorModeToggler;
        "pp-footer": PpFooter;
        "pp-menu": PpMenu;
    }
}
export { LocalJSX as JSX };
declare module "@stencil/core" {
    export namespace JSX {
        interface IntrinsicElements {
            "app-root": LocalJSX.AppRoot & JSXBase.HTMLAttributes<HTMLAppRootElement>;
            "pp-color-mode-toggler": LocalJSX.PpColorModeToggler & JSXBase.HTMLAttributes<HTMLPpColorModeTogglerElement>;
            "pp-footer": LocalJSX.PpFooter & JSXBase.HTMLAttributes<HTMLPpFooterElement>;
            "pp-menu": LocalJSX.PpMenu & JSXBase.HTMLAttributes<HTMLPpMenuElement>;
        }
    }
}
