# C#
# Create migration (adjust --project and --startup-project to your solution)
dotnet ef migrations add AddIsDeletedToCategory --project `src/TheGourmet.Infrastructure` --startup-project `src/TheGourmet.Api`

# Apply migration to database
dotnet ef database update --project `src/TheGourmet.Infrastructure` --startup-project `src/TheGourmet.Api`
