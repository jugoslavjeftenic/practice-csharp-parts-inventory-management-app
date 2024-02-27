# PartsInventoryManagement.Api
(.NET 8, Dapper, MS SQL)<br>
https://westeu-parts-inventory-management-api-v1.azurewebsites.net/api/v1

Endpoints:

- /PartCategories<br>
"POST: /PartCategories" - post new part category - { "partCategoryName": "string" }<br>
"GET: /PartCategories" - get all part categories<br>
"PUT: /PartCategories" - update part category - { "partCategoryId": 0, "partCategoryName": "string" }<br>
"DELETE: /PartCategories/{partCategoryId}" - delete part category<br>
"GET: /PartCategories/{partCategoryId}" - get part category by Id<br>
"GET: /PartCategories/name/{partCategoryName}" - get part category by full/partial name<br>
"POST: /PartCategories/seed" - seed predefined data in part categories table<br>

- /Parts<br>
"POST: /Parts" - post new part - { "partCategoryId": 0, "partName": "string" }<br>
"GET: /Parts" - get all parts<br>
"PUT: /Parts" - update part - { "partId": 0, "partCategoryId": 0, "partName": "string" }<br>
"DELETE: /Parts/{partId}" - delete part<br>
"GET: /Parts/{partId}" - get part by Id<br>
"GET: /Parts/category/{partCategoryId}" - get parts by part category Id<br>
"GET: /Parts/name/{partName}" - get parts by full/partial name<br>
POST: /Parts/seed" - seed predefined data in parts table<br>
