"use strict";
/*
 * ATTENTION: An "eval-source-map" devtool has been used.
 * This devtool is neither made for production nor for readable output files.
 * It uses "eval()" calls to create a separate source file with attached SourceMaps in the browser devtools.
 * If you are trying to read the output file, select a different devtool (https://webpack.js.org/configuration/devtool/)
 * or disable the default devtool with "devtool: false".
 * If you are looking for production-ready output files, see mode: "production" (https://webpack.js.org/configuration/mode/).
 */
exports.id = "vendor-chunks/rehype-raw@7.0.0";
exports.ids = ["vendor-chunks/rehype-raw@7.0.0"];
exports.modules = {

/***/ "(ssr)/../../node_modules/.pnpm/rehype-raw@7.0.0/node_modules/rehype-raw/index.js":
/*!**********************************************************************************!*\
  !*** ../../node_modules/.pnpm/rehype-raw@7.0.0/node_modules/rehype-raw/index.js ***!
  \**********************************************************************************/
/***/ ((__unused_webpack___webpack_module__, __webpack_exports__, __webpack_require__) => {

eval("__webpack_require__.r(__webpack_exports__);\n/* harmony export */ __webpack_require__.d(__webpack_exports__, {\n/* harmony export */   \"default\": () => (/* reexport safe */ _lib_index_js__WEBPACK_IMPORTED_MODULE_0__[\"default\"])\n/* harmony export */ });\n/* harmony import */ var _lib_index_js__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./lib/index.js */ \"(ssr)/../../node_modules/.pnpm/rehype-raw@7.0.0/node_modules/rehype-raw/lib/index.js\");\n/**\n * @typedef {import('./lib/index.js').Options} Options\n */\n\n\n//# sourceURL=[module]\n//# sourceMappingURL=data:application/json;charset=utf-8;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiKHNzcikvLi4vLi4vbm9kZV9tb2R1bGVzLy5wbnBtL3JlaHlwZS1yYXdANy4wLjAvbm9kZV9tb2R1bGVzL3JlaHlwZS1yYXcvaW5kZXguanMiLCJtYXBwaW5ncyI6Ijs7Ozs7QUFBQTtBQUNBLGFBQWEsa0NBQWtDO0FBQy9DOztBQUVzQyIsInNvdXJjZXMiOlsiL1VzZXJzL0Vib290aC9hZ2VudF9jX2ZyYW1ld29yay9zcmMvcmVhbHRpbWVfY2xpZW50L25vZGVfbW9kdWxlcy8ucG5wbS9yZWh5cGUtcmF3QDcuMC4wL25vZGVfbW9kdWxlcy9yZWh5cGUtcmF3L2luZGV4LmpzIl0sInNvdXJjZXNDb250ZW50IjpbIi8qKlxuICogQHR5cGVkZWYge2ltcG9ydCgnLi9saWIvaW5kZXguanMnKS5PcHRpb25zfSBPcHRpb25zXG4gKi9cblxuZXhwb3J0IHtkZWZhdWx0fSBmcm9tICcuL2xpYi9pbmRleC5qcydcbiJdLCJuYW1lcyI6W10sImlnbm9yZUxpc3QiOlswXSwic291cmNlUm9vdCI6IiJ9\n//# sourceURL=webpack-internal:///(ssr)/../../node_modules/.pnpm/rehype-raw@7.0.0/node_modules/rehype-raw/index.js\n");

/***/ }),

/***/ "(ssr)/../../node_modules/.pnpm/rehype-raw@7.0.0/node_modules/rehype-raw/lib/index.js":
/*!**************************************************************************************!*\
  !*** ../../node_modules/.pnpm/rehype-raw@7.0.0/node_modules/rehype-raw/lib/index.js ***!
  \**************************************************************************************/
/***/ ((__unused_webpack___webpack_module__, __webpack_exports__, __webpack_require__) => {

eval("__webpack_require__.r(__webpack_exports__);\n/* harmony export */ __webpack_require__.d(__webpack_exports__, {\n/* harmony export */   \"default\": () => (/* binding */ rehypeRaw)\n/* harmony export */ });\n/* harmony import */ var hast_util_raw__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! hast-util-raw */ \"(ssr)/../../node_modules/.pnpm/hast-util-raw@9.1.0/node_modules/hast-util-raw/lib/index.js\");\n/**\n * @typedef {import('hast').Root} Root\n * @typedef {import('hast-util-raw').Options} RawOptions\n * @typedef {import('vfile').VFile} VFile\n */\n\n/**\n * @typedef {Omit<RawOptions, 'file'>} Options\n *   Configuration.\n */\n\n\n\n/**\n * Parse the tree (and raw nodes) again, keeping positional info okay.\n *\n * @param {Options | null | undefined}  [options]\n *   Configuration (optional).\n * @returns\n *   Transform.\n */\nfunction rehypeRaw(options) {\n  /**\n   * @param {Root} tree\n   *   Tree.\n   * @param {VFile} file\n   *   File.\n   * @returns {Root}\n   *   New tree.\n   */\n  return function (tree, file) {\n    // Assume root in -> root out.\n    const result = /** @type {Root} */ ((0,hast_util_raw__WEBPACK_IMPORTED_MODULE_0__.raw)(tree, {...options, file}))\n    return result\n  }\n}\n//# sourceURL=[module]\n//# sourceMappingURL=data:application/json;charset=utf-8;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiKHNzcikvLi4vLi4vbm9kZV9tb2R1bGVzLy5wbnBtL3JlaHlwZS1yYXdANy4wLjAvbm9kZV9tb2R1bGVzL3JlaHlwZS1yYXcvbGliL2luZGV4LmpzIiwibWFwcGluZ3MiOiI7Ozs7O0FBQUE7QUFDQSxhQUFhLHFCQUFxQjtBQUNsQyxhQUFhLGlDQUFpQztBQUM5QyxhQUFhLHVCQUF1QjtBQUNwQzs7QUFFQTtBQUNBLGFBQWEsMEJBQTBCO0FBQ3ZDO0FBQ0E7O0FBRWlDOztBQUVqQztBQUNBO0FBQ0E7QUFDQSxXQUFXLDZCQUE2QjtBQUN4QztBQUNBO0FBQ0E7QUFDQTtBQUNlO0FBQ2Y7QUFDQSxhQUFhLE1BQU07QUFDbkI7QUFDQSxhQUFhLE9BQU87QUFDcEI7QUFDQSxlQUFlO0FBQ2Y7QUFDQTtBQUNBO0FBQ0E7QUFDQSw4QkFBOEIsTUFBTSxJQUFJLGtEQUFHLFFBQVEsaUJBQWlCO0FBQ3BFO0FBQ0E7QUFDQSIsInNvdXJjZXMiOlsiL1VzZXJzL0Vib290aC9hZ2VudF9jX2ZyYW1ld29yay9zcmMvcmVhbHRpbWVfY2xpZW50L25vZGVfbW9kdWxlcy8ucG5wbS9yZWh5cGUtcmF3QDcuMC4wL25vZGVfbW9kdWxlcy9yZWh5cGUtcmF3L2xpYi9pbmRleC5qcyJdLCJzb3VyY2VzQ29udGVudCI6WyIvKipcbiAqIEB0eXBlZGVmIHtpbXBvcnQoJ2hhc3QnKS5Sb290fSBSb290XG4gKiBAdHlwZWRlZiB7aW1wb3J0KCdoYXN0LXV0aWwtcmF3JykuT3B0aW9uc30gUmF3T3B0aW9uc1xuICogQHR5cGVkZWYge2ltcG9ydCgndmZpbGUnKS5WRmlsZX0gVkZpbGVcbiAqL1xuXG4vKipcbiAqIEB0eXBlZGVmIHtPbWl0PFJhd09wdGlvbnMsICdmaWxlJz59IE9wdGlvbnNcbiAqICAgQ29uZmlndXJhdGlvbi5cbiAqL1xuXG5pbXBvcnQge3Jhd30gZnJvbSAnaGFzdC11dGlsLXJhdydcblxuLyoqXG4gKiBQYXJzZSB0aGUgdHJlZSAoYW5kIHJhdyBub2RlcykgYWdhaW4sIGtlZXBpbmcgcG9zaXRpb25hbCBpbmZvIG9rYXkuXG4gKlxuICogQHBhcmFtIHtPcHRpb25zIHwgbnVsbCB8IHVuZGVmaW5lZH0gIFtvcHRpb25zXVxuICogICBDb25maWd1cmF0aW9uIChvcHRpb25hbCkuXG4gKiBAcmV0dXJuc1xuICogICBUcmFuc2Zvcm0uXG4gKi9cbmV4cG9ydCBkZWZhdWx0IGZ1bmN0aW9uIHJlaHlwZVJhdyhvcHRpb25zKSB7XG4gIC8qKlxuICAgKiBAcGFyYW0ge1Jvb3R9IHRyZWVcbiAgICogICBUcmVlLlxuICAgKiBAcGFyYW0ge1ZGaWxlfSBmaWxlXG4gICAqICAgRmlsZS5cbiAgICogQHJldHVybnMge1Jvb3R9XG4gICAqICAgTmV3IHRyZWUuXG4gICAqL1xuICByZXR1cm4gZnVuY3Rpb24gKHRyZWUsIGZpbGUpIHtcbiAgICAvLyBBc3N1bWUgcm9vdCBpbiAtPiByb290IG91dC5cbiAgICBjb25zdCByZXN1bHQgPSAvKiogQHR5cGUge1Jvb3R9ICovIChyYXcodHJlZSwgey4uLm9wdGlvbnMsIGZpbGV9KSlcbiAgICByZXR1cm4gcmVzdWx0XG4gIH1cbn1cbiJdLCJuYW1lcyI6W10sImlnbm9yZUxpc3QiOlswXSwic291cmNlUm9vdCI6IiJ9\n//# sourceURL=webpack-internal:///(ssr)/../../node_modules/.pnpm/rehype-raw@7.0.0/node_modules/rehype-raw/lib/index.js\n");

/***/ })

};
;