# PartsInventoryManagement.Api
(.NET 8, Dapper, MS SQL)<br>
https://westeu-parts-inventory-management-api-v1.azurewebsites.net/api/v1

Endpoints:

"POST: /PartCategories" - post new part category
{
  "partCategoryName": "string"
}

"GET: /PartCategories" - get all part categories

"PUT: /PartCategories" - update part category
{
  "partCategoryId": 0,
  "partCategoryName": "string"
}

"DELETE: /PartCategories/{partCategoryId}" - delete part category

"GET: /PartCategories/{partCategoryId}" - get part category by Id

"GET: /PartCategories/{partCategoryName}" - get part category by full/partial name

POST: /PartCategories/seed" - seed predefined data in part categories table
