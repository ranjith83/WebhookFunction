# WebhookFunction


The application is developed in Azure Functions sets up a webhook which listens for order creation messages. Once a message is received on that webhook, it transforms it into an email that includes barcode images for items that have the code defined as metadata.

It has two functions are Post and Get , Below are the url to test this application

http://localhost:7071/api/PostMessage?to=jane.doe@mail.com&currency=eur&metadatakey=eancode&storeId=1234&storeId=12345&to=ranjith@mail.com


http://localhost:7071/api/GetMessage?to=jane.doe@mail.com&currency=eur&metadatakey=eancode&storeId=1234&storeId=12345&to=ranjith@mail.com
