using ByteSizeLib;
using Microsoft.AspNetCore.Mvc;
using SharpBB.Server.DbContexts;
using WebPWrapper.Encoder;

namespace SharpBB.Server.Endpoints;

public static partial class ForumEndpoints
{
    extension(WebApplication app)
    {
        public WebApplication MapBbsImageEndpoints()
        {
            var imageApis = app.MapGroup("api/bbs/binary");
            imageApis.MapPost("post", ([FromForm] IFormFile binaryFile) =>
            {
                var uuid = Guid.NewGuid().ToString();
                using var mStream = new MemoryStream();
                binaryFile.CopyTo(mStream);
                switch (binaryFile.ContentType)
                {
                    case var ct when ct.StartsWith("image/"):
                        try
                        {
                            var cwebp = new WebPEncoderBuilder();
                            var encoder = cwebp
                                .CompressionConfig(x => x.Lossy(y => y.Size((int)(400 * ByteSize.BytesInKiloByte))))
                                .Build();
                            using var outputStream = new MemoryStream();
                            mStream.Position = 0;
                            encoder.Encode(mStream, outputStream);
                            using var binariesDbContext = new BinariesDbContext();
                            binariesDbContext.Binaries.Add(new()
                            {
                                Uuid = uuid,
                                Content = outputStream.ToArray(),
                                MimeType = "image/webp",
                                FileName = binaryFile.FileName.Split(".")[0] + ".webp"
                            });
                            binariesDbContext.SaveChanges();
                            return Results.Ok(uuid);
                        }
                        catch (Exception e)
                        {
                            return GeneralHandler(e);
                        }

                }
                try
                {
                    using var binariesDbContext = new BinariesDbContext();
                    binariesDbContext.Binaries.Add(new()
                    {
                        Uuid = uuid,
                        Content = mStream.ToArray(),
                        MimeType = binaryFile.ContentType,
                        FileName = binaryFile.FileName
                    });
                    binariesDbContext.SaveChanges();
                }
                catch (Exception e)
                {
                    return GeneralHandler(e);
                }
                return Results.Ok(uuid);
            }).DisableAntiforgery().WithFormOptions(multipartBodyLengthLimit: 10 * ByteSize.BytesInMegaByte);
            imageApis.MapGet("get/{uuid}", (string uuid) =>
            {
                using var db = new BinariesDbContext();
                var res = db.Binaries.FirstOrDefault(i => i.Uuid == uuid);
                if (res is null)
                {
                    return Results.NotFound();
                }
                return Results.File(res.Content, contentType: res.MimeType, fileDownloadName: res.FileName);
            });
            return app;
        }
    }
}