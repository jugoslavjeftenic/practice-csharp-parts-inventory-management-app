# PartsInventoryManagement.Api
(.NET 8, Dapper, MS SQL)<br>
https://westeu-parts-inventory-management-api-v1.azurewebsites.net/api/v1

Endpoints:

- /PartCategories
"POST: /PartCategories" - post new part category - { "partCategoryName": "string" }
"GET: /PartCategories" - get all part categories
"PUT: /PartCategories" - update part category - { "partCategoryId": 0, "partCategoryName": "string" }
"DELETE: /PartCategories/{partCategoryId}" - delete part category
"GET: /PartCategories/{partCategoryId}" - get part category by Id
"GET: /PartCategories/name/{partCategoryName}" - get part category by full/partial name
"POST: /PartCategories/seed" - seed predefined data in part categories table

- /Parts
"POST: /Parts" - post new part - { "partCategoryId": 0, "partName": "string" }
"GET: /Parts" - get all parts
"PUT: /Parts" - update part - { "partId": 0, "partCategoryId": 0, "partName": "string" }
"DELETE: /Parts/{partId}" - delete part
"GET: /Parts/{partId}" - get part by Id
"GET: /Parts/category/{partCategoryId}" - get parts by part category Id
"GET: /Parts/name/{partName}" - get parts by full/partial name
POST: /Parts/seed" - seed predefined data in parts table
