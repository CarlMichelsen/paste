using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Presentation.Dto;
using Presentation.Handler;

namespace App.Endpoints;

public static class FileEndpoints
{
    public static void RegisterFileEndpoints(
        this IEndpointRouteBuilder app)
    {
        var fileGroup = app
            .MapGroup("file");

        fileGroup.MapPost(
            "upload",
            ([FromServices] IFileUploadHandler handler, IFormFile formFile) =>
            handler.UploadFile(formFile));

        fileGroup.MapGet(
            "list/{skip:int}/{amount:int}",
            ([FromServices] IFileSearchHandler handler, [FromRoute] int skip, [FromRoute] int amount) =>
                handler.GetFiles(skip, amount))
            .WithName("FileList");
        
        fileGroup.MapGet(
            "search/{partialFileName}/{amount:int}",
            ([FromServices] IFileSearchHandler handler, [FromRoute] string partialFileName, [FromRoute] int amount) =>
                handler.SearchFiles(partialFileName, amount))
            .WithName("FileSearch");
        
        fileGroup.MapGet(
                "{fileId:guid}",
                ([FromServices] IFileDownloadHandler handler, [FromRoute] Guid fileId) =>
                    handler.GetFileMetadata(fileId))
            .WithName("FileMetadata");
        
        fileGroup.MapDelete(
                "{fileId:guid}",
                ([FromServices] IFileDeletionHandler handler, [FromRoute] Guid fileId) =>
                    handler.DeleteFile(fileId))
            .WithName("FileDelete");
        
        fileGroup.MapGet(
                "download/{fileId:guid}",
                ([FromServices] IFileDownloadHandler handler, [FromRoute] Guid fileId) =>
                    handler.DownloadFile(fileId))
            .WithName("FileDownload");
        
        fileGroup.MapGet(
            "antiforgery/token",
            (HttpContext context, IAntiforgery antiforgery) =>
            {
                var tokens = antiforgery.GetAndStoreTokens(context);
                return new ServiceResponse<string>(tokens.RequestToken!);
            })
            .AllowAnonymous() // Allow access without authentication if using authorization
            .WithName("GetAntiforgeryToken");
    }
}