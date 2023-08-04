const path = require("path");

module.exports = {
  stories: ["../src/**/*.stories.mdx", "../src/**/*.stories.@(js|jsx|ts|tsx)"],
  addons: ["@storybook/addon-links", "@storybook/addon-essentials"],
  framework: "@storybook/react",
  features: {
    emotionAlias: false,
    previewCsfV3: true,
  },
  core: {
    builder: "webpack5",
  },
  webpackFinal: async (config, { configType }) => {
    // absolute import paths to match tsconfig
    config.resolve.modules.push(path.resolve(__dirname, "../src"));

    // we want to load yaml files as raw text
    // so we can parse them ourselves
    config.module.rules.push({
      test: /\.yaml/,
      type: "asset/source",
    });
    return config;
  },
};
