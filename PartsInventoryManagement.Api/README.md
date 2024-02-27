# PartsInventoryManagement.Api
(.NET 8, Dapper, MS SQL)<br>
https://westeu-parts-inventory-management-api-v1.azurewebsites.net

Endpoints:

- /api/v1/Test<br>
	- "GET: /api/v1/Test/AppIsRunning"<br>
	test if API is running<br>
	- "GET: /api/v1/Test/DbConnection"<br>
	test if DB connection<br>

- /api/v1/PartCategories<br>
	- "POST:   /api/v1/PartCategories"<br>
	post new part category<br>
	JSON: { "partCategoryName": "string" }<br>
	- "GET:    /api/v1/PartCategories"<br>
	get all part categories<br>
	- "PUT:    /api/v1/PartCategories"<br>
	update part category<br>
	JSON: { "partCategoryId": 0, "partCategoryName": "string" }<br>
	- "DELETE: /api/v1/PartCategories/{partCategoryId}"<br>
	delete part category<br>
	- "GET:    /api/v1/PartCategories/{partCategoryId}"<br>
	get part category by part category Id<br>
	- "GET:    /api/v1/PartCategories/name/{partCategoryName}"<br>
	get part category by full/partial part category name<br>
	- "POST:   /api/v1/PartCategories/seed"<br>
	seed predefined data in part categories table<br>

- /api/v1/Parts<br>
	- "POST:   /api/v1/Parts"<br>
	post new part<br>
	JSON: { "partCategoryId": 0, "partName": "string" }<br>
	- "GET:    /api/v1/Parts"<br>
	get all parts<br>
	- "PUT:    /api/v1/Parts"<br>
	update part<br>
	JSON: { "partId": 0, "partCategoryId": 0, "partName": "string" }<br>
	- "DELETE: /api/v1/Parts/{partId}"<br>
	delete part<br>
	- "GET:    /api/v1/Parts/{partId}"<br>
	get part by part Id<br>
	- "GET:    /api/v1/Parts/category/{partCategoryId}"<br>
	get parts by part category Id<br>
	- "GET:    /api/v1/Parts/name/{partName}"<br>
	get parts by full/partial part name<br>
	- "POST:   /api/v1/Parts/seed"<br>
	seed predefined data in parts table<br>

- /api/v1/Locations<br>
	- "POST:   /api/v1/Locations"<br>
	post new location<br>
	JSON: { "locationAlpha": "string", "locationName": "string", "locationHexColor": "string" }<br>
	- "GET:    /api/v1/Locations"<br>
	get all locations<br>
	- "PUT:    /api/v1/Locations"<br>
	update location<br>
	JSON: { "locationId": 0, "locationAlpha": "string", "locationName": "string", "locationHexColor": "string" }<br>
	- "DELETE: /api/v1/Locations/{partId}"<br>
	delete location<br>
	- "GET:    /api/v1/Locations/{partId}"<br>
	get location by location Id<br>
	- "GET:    /api/v1/Locations/alpha/{locationAlpha}"<br>
	get location by full/partial location alpha<br>
	- "GET:    /api/v1/Locations/name/{locationName}"<br>
	get location by full/partial location name<br>
	- "GET:    /api/v1/Locations/color/{locationHexColor}"<br>
	get location by location color<br>
	- "POST:   /api/v1/Locations/seed"<br>
	seed predefined data in locations table<br>

- /api/v1/Inventory<br>
	- "POST:   /api/v1/Inventory"<br>
	post new inventory item<br>
	JSON: { "partId": 0, "locationId": 0, "partQuantity": 0 }<br>
	- "GET:    /api/v1/Inventory"<br>
	get all inventory items<br>
	- "PUT:    /api/v1/Inventory"<br>
	update inventory item<br>
	JSON: { "inventoryId": 0, "partId": 0, "locationId": 0, "partQuantity": 0 }<br>
	- "DELETE: /api/v1/Inventory/{inventoryId}"<br>
	delete inventory item<br>
	- "GET:    /api/v1/Inventory/{inventoryId}"<br>
	get inventory item by inventory item Id<br>
	- "GET:    /api/v1/Inventory/part/{partId}"<br>
	get inventory item by part Id<br>
	- "GET:    /api/v1/Inventory/location/{locationId}"<br>
	get inventory item by location id<br>
	- "POST:   /api/v1/Inventory/randomseed"<br>
	seed random data in inventory table<br>

- /api/v1/Users<br>
	- "POST:   /api/v1/Users"<br>
	post new user<br>
	JSON: { "userName": "string", "locationId": 0, "password": "string", "passwordConfirm": "string" }<br>
	- "GET:    /api/v1/Users"<br>
	get all users<br>
	- "PUT:    /api/v1/Users"<br>
	update user<br>
	JSON: { "userId": 0, "userName": "string", "locationId": 0, "password": "string", "passwordConfirm": "string" }<br>
	- "DELETE: /api/v1/Users/{userId}"<br>
	delete user<br>
	- "GET:    /api/v1/Users/{userId}"<br>
	get user by user Id<br>
	- "GET:    /api/v1/Users/name/{userName}"<br>
	get user by full/partial user name<br>
	- "GET:    /api/v1/Users/location/{locationId}"<br>
	get user by location Id<br>
	- "POST:   /api/v1/Users/login"<br>
	post user credentials and get JWT<br>
	JSON: { "userName": "string", "password": "string" }<br>
	- "GET:    /api/v1/Users/RefreshToken"<br>
	get new JWT for logged user<br>
	- "POST:   /api/v1/Users/seed"<br>
	seed predefined data in locations table<br>
