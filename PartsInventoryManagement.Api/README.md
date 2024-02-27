# C# Parts Inventory Management App - C# practice project
by Jugoslav Jeftenic

PartsInventoryManagement.Api<br>
(.NET 8, Dapper, MS SQL)
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

