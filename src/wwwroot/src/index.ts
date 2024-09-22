/**
 * @fileoverview entry point for your component library
 *
 * This is the entry point for your component library. Use this file to export utilities,
 * constants or data structure that accompany your components.
 *
 * DO NOT use this file to export your components. Instead, use the recommended approaches
 * to consume components of this package as outlined in the `README.md`.
 */

import { createRouter } from "stencil-router-v2";

export type * from './components.d.ts';

export const Router = createRouter({
    parseURL: url => {
        let result = `${url.hash.slice(1)}`
        if (result === "") {
            result = "/";
        }
        return result;
    },
    serializeURL: path => {
        const result = new URL(`#/${path}`)
        return result;
    },
})
