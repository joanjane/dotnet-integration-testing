# Integration testing of REST API on NET Core
This sample contains a ToDo REST API with CRUD operations. There are some integration tests on WebAPI to cover some test cases.
The solution uses Entity Framework Core and for testing, there's a configuration flag on appsettings.Testing.json that allows to swap the use of SQLServer implementation or InMemory.