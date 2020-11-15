# InStockNotifier

NetCore Console App to alert when a monitored product is in stock.
Webpage list in config.json can be modified for use with any product (currently tested and functional for Amazon and Newegg products).

By default, a browser window will open and a sound will be played when an item is found to be in stock.

This project is based on python's script [InStockNotifier](https://github.com/hmtessier/InStockNotifier).

## Configuration
config.json is a json array with the following parameters:
- url: (string) Defines the URL where the product is found. This will be 
- keyword: (string) A search will be performed on the webpage looking for this value. If found, item will be available. Dafaul value is "Add To Cart".
- alert: (bool) If true, web browser will be opened and a sound will be played.
- name: (string) Name to show on the console window.
- enabled: (bool) Defines if webpage will be processed.