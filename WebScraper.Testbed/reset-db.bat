rmdir /s /q Migrations
dotnet ef database drop
dotnet ef migrations add Initial
dotnet ef database update
rmdir /s /q Migrations