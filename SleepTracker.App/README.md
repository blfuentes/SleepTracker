# Basic web application without frameworks
## Description
The purpose of this basic application is to set up a web stack for development that doesn't rely on web frameworks like react, angular... or any other 'html enhancers' (htmx...)

## Technologies
- HTML
- SCSS
- TypeScript
- Webpack
  - ts-loader
  - style-loader
  - css-loader
  - sass-loader
  - file-loader
  - Plugins
    - HtmlWebpackPlugin
    - CleanWebpackPlugin
    - MiniCssExtractPlugin
    - ImageMinimizerPlugin
  - liveReload (enabled)

## How to run
- For installing: `npm install`
- For running local dev server: `npm start`
- For building production `npm run build` outputs to `/dist` folder.

## Credits
Based on https://github.com/AdamGuar/no-frameworks-typescript-app-starter with modifications to use `scss` and minimizer of images to `webp`.