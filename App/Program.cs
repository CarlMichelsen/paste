using App;
using App.Logging;
using App.Middleware;
using Application;
using Presentation.Dto.User;
using Presentation.Repository;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddApplicationDependencies();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi()
        .CacheOutput();

    app.MapScalarApiReference();
}
else
{
    // "Who throws a shoe? Honestly!"
    await app.Services.EnsureDatabaseUpdated();
}

app.UseMiddleware<TraceIdMiddleware>();

app.UseAuthentication();

app.UseAuthorization();

app.UseResponseCompression();

app.UseStaticFiles(StaticFileOptionsFactory.CreateFileOptions());

app.RegisterEndpoints();

app.LogStartup();

using (var scope = app.Services.CreateScope())
{
    var sp = scope.ServiceProvider;
    var userRepo = sp.GetRequiredService<IUserRepository>();
    var fileWriteRepo = sp.GetRequiredService<IFileWriteRepository>();

    const int userId = 666;
    await userRepo.TryUpsertUser(new AuthenticatedUser(
        Id: userId,
        AuthenticationMethod: "development",
        AuthenticationId: "1",
        Username: "test-user",
        Email: "test@test.com",
        AvatarUrl: "https://whitebox.survivethething.com/c/01969af4-1a76-710e-8982-925a224de667"));

    var file = await fileWriteRepo.CreateFile("hello.txt", new FileContent("Hello World!"u8.ToArray()), 666);
    
    await fileWriteRepo.SetFileName(file.Id, "weeee.txt", userId);
    
    await fileWriteRepo.SetFileContent(file.Id, new FileContent("Hi!"u8.ToArray()), userId);
    
    await fileWriteRepo.SetFileName(file.Id, "bye.txt", userId);
    
    await fileWriteRepo.DeleteFile(file.Id, userId);
}

app.Run();