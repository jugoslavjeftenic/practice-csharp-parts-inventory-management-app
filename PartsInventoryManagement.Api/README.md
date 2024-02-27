# PartsInventoryManagement.Api
(.NET 8, Dapper, MS SQL)<br>
https://westeu-parts-inventory-management-api-v1.azurewebsites.net

Endpoints:

- /api/v1/Test<br>
"GET: /api/v1/Test/AppIsRunning" - test if API is running<br>
"GET: /api/v1/Test/DbConnection" - test if DB connection<br>

- /api/v1/PartCategories<br>
"POST:   /api/v1/PartCategories" - post new part category - { "partCategoryName": "string" }<br>
"GET:    /api/v1/PartCategories" - get all part categories<br>
"PUT:    /api/v1/PartCategories" - update part category - { "partCategoryId": 0, "partCategoryName": "string" }<br>
"DELETE: /api/v1/PartCategories/{partCategoryId}" - delete part category<br>
"GET:    /api/v1/PartCategories/{partCategoryId}" - get part category by Id<br>
"GET:    /api/v1/PartCategories/name/{partCategoryName}" - get part category by full/partial part category name<br>
"POST:   /api/v1/PartCategories/seed" - seed predefined data in part categories table<br>

- /api/v1/Parts<br>
"POST:   /api/v1/Parts" - post new part - { "partCategoryId": 0, "partName": "string" }<br>
"GET:    /api/v1/Parts" - get all parts<br>
"PUT:    /api/v1/Parts" - update part - { "partId": 0, "partCategoryId": 0, "partName": "string" }<br>
"DELETE: /api/v1/Parts/{partId}" - delete part<br>
"GET:    /api/v1/Parts/{partId}" - get part by Id<br>
"GET:    /api/v1/Parts/category/{partCategoryId}" - get parts by part category Id<br>
"GET:    /api/v1/Parts/name/{partName}" - get parts by full/partial part name<br>
"POST:   /api/v1/Parts/seed" - seed predefined data in parts table<br>

- /api/v1/Locations<br>
"POST:   /api/v1/Locations" - post new location - { "locationAlpha": "string", "locationName": "string", "locationHexColor": "string" }<br>
"GET:    /api/v1/Locations" - get all locations<br>
"PUT:    /api/v1/Locations" - update location - { "locationId": 0, "locationAlpha": "string", "locationName": "string", "locationHexColor": "string" }<br>
"DELETE: /api/v1/Locations/{partId}" - delete location<br>
"GET:    /api/v1/Locations/{partId}" - get location by Id<br>
"GET:    /api/v1/Locations/alpha/{locationAlpha}" - get location by full/partial location alpha<br>
"GET:    /api/v1/Locations/name/{locationName}" - get location by full/partial location name<br>
"GET:    /api/v1/Locations/color/{locationHexColor}" - get location by color<br>
"POST:   /api/v1/Locations/seed" - seed predefined data in locations table<br>

- /api/v1/Inventory<br>
"POST:   /api/v1/Inventory" - post new inventory item - { "partId": 0, "locationId": 0, "partQuantity": 0 }<br>
"GET:    /api/v1/Inventory" - get all inventory items<br>
"PUT:    /api/v1/Inventory" - update inventory item - { "inventoryId": 0, "partId": 0, "locationId": 0, "partQuantity": 0 }<br>
"DELETE: /api/v1/Inventory/{inventoryId}" - delete inventory item<br>
"GET:    /api/v1/Inventory/{inventoryId}" - get inventory item by  inventory item Id<br>
"GET:    /api/v1/Inventory/part/{partId}" - get inventory item by part Id<br>
"GET:    /api/v1/Inventory/location/{locationId}" - get inventory item by location id<br>
"POST:   /api/v1/Inventory/randomseed" - seed random data in inventory table<br>

- /api/v1/Users<br>
"POST:   /api/v1/Users" - post new user - { "userName": "string", "locationId": 0, "password": "string", "passwordConfirm": "string" }<br>
"GET:    /api/v1/Users" - get all users<br>
"PUT:    /api/v1/Users" - update user - { "userId": 0, "userName": "string", "locationId": 0, "password": "string", "passwordConfirm": "string" }<br>
"DELETE: /api/v1/Users/{userId}" - delete user<br>
"GET:    /api/v1/Users/{userId}" - get user by user Id<br>
"GET:    /api/v1/Users/name/{userName}" - get user by full/partial user name<br>
"GET:    /api/v1/Users/location/{locationId}" - get user by location Id<br>
"POST:   /api/v1/Users/login" - post user credentials and get JWT - { "userName": "string", "password": "string" }<br>
"GET:    /api/v1/Users/RefreshToken" - get new JWT for logged user<br>
"POST:   /api/v1/Users/seed" - seed predefined data in locations table<br>
