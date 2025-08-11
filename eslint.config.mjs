import path from "node:path";
import { fileURLToPath } from "node:url";

import { fixupConfigRules } from "@eslint/compat";
import { FlatCompat } from "@eslint/eslintrc";
import js from "@eslint/js";
import { defineConfig, globalIgnores } from "eslint/config";
import simpleImportSort from "eslint-plugin-simple-import-sort";
import globals from "globals";

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);
const compat = new FlatCompat({
  baseDirectory: __dirname,
  recommendedConfig: js.configs.recommended,
  allConfig: js.configs.all,
});

export default defineConfig([
  globalIgnores(["**/node_modules"]),
  {
    files: ["app/client-app/src/**/*.{js,jsx,ts,tsx}"],
    extends: fixupConfigRules(
      compat.extends(
        "eslint:recommended",
        "plugin:react/recommended",
        "plugin:react-hooks/recommended",
        "plugin:import/warnings",
        "plugin:prettier/recommended"
      )
    ),
    plugins: {
      "simple-import-sort": simpleImportSort,
    },

    settings: {
      react: {
        version: "detect",
      },
      "simple-import-sort/imports": [
        "^react",
        "^@?\\w",
        "^[^.]",
        "^\\.",
        "^.*\\.css$",
      ],
      "simple-import-sort/exports": [
        "^react",
        "^@?\\w",
        "^[^.]",
        "^\\.",
        "^.*\\.css$",
      ],
    },

    languageOptions: {
      globals: {
        ...globals.browser,
      },
      ecmaVersion: 2022,
      sourceType: "module",
      parserOptions: {
        ecmaFeatures: {
          jsx: true,
        },
      },
    },

    rules: {
      "react/react-in-jsx-scope": "off",
      "react/jsx-uses-react": "off",
      "react/prop-types": "off",
      "import/no-duplicates": "error",
      "no-unused-vars": "warn",
      "simple-import-sort/imports": "warn",
      "simple-import-sort/exports": "warn",
      "prettier/prettier": [
        "warn",
        {
          endOfLine: "auto",
        },
      ],
    },
  },
]);
