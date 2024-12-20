const HtmlWebpackPlugin = require("html-webpack-plugin");
const { CleanWebpackPlugin } = require("clean-webpack-plugin");
const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const ImageMinimizerPlugin = require("image-minimizer-webpack-plugin");
const { DefinePlugin } = require("webpack");
const Dotenv = require('dotenv-webpack');
const path = require("path");

module.exports = {
  entry: "./src/index.ts",
  module: {
    rules: [
      {
        test: /\.ts?$/,
        use: "ts-loader",
        exclude: /node_modules/,
      },
      {
        test: /\.s[ac]ss$/i,
        use: [
          // fallback to style-loader in development
          process.env.NODE_ENV !== "production"
            ? "style-loader"
            : MiniCssExtractPlugin.loader,
          "css-loader",
          "sass-loader",
        ],
      },
      {
        test: /\.(jpe?g|png|gif|svg|webp)$/i,
        use: [
          {
            loader: "file-loader",
            options: {
              name: "[path][name].[ext]",
            },
          },
          {
            loader: "image-webpack-loader",
            options: {
              disable: process.env.NODE_ENV !== "production", // Disable during development
              webp: {
                quality: 75,
              },
            },
          },
        ],
      },
      {
        test: /\.html$/i,
        loader: "html-loader",
      },
    ],
  },
  resolve: {
    extensions: [".tsx", ".ts", ".js", ".css", ".scss", ".sass"],
  },
  output: {
    filename: "bundle.js",
    path: path.resolve(__dirname, "dist"),
  },

  plugins: [
    new Dotenv(),
    new DefinePlugin({ 
      'process.env.API_URL_HTTP': JSON.stringify(process.env.services__sleeptracker__http__0),
      'process.env.API_URL_HTTPS': JSON.stringify(process.env.services__sleeptracker__https__0),
      'process.env.PORT': JSON.stringify(process.env.PORT) }),
    new HtmlWebpackPlugin({
      title: "our project",
      template: "src/index.html",
    }),
    new CleanWebpackPlugin(),
    new MiniCssExtractPlugin({
      // Options similar to the same options in webpackOptions.output
      // both options are optional
      filename: "[name].css",
      chunkFilename: "[id].css",
    }),
    new ImageMinimizerPlugin({
      minimizer: {
        implementation: ImageMinimizerPlugin.imageminMinify,
        options: {
          plugins: [["imagemin-webp", { quality: 75 }]],
        },
      },
      generator: [
        {
          preset: "webp",
          implementation: ImageMinimizerPlugin.imageminGenerate,
          options: {
            plugins: [["imagemin-webp", { quality: 75 }]],
          },
        },
      ],
    }),
  ],

  devServer: {
    static: path.join(__dirname, "dist"),
    compress: true,
    port: process.env.PORT,
    hot: false,
    liveReload: true
  },
};
