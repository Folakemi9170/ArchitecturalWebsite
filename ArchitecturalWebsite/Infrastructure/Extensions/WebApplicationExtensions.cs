//using Serilog;

//namespace ArchitecturalWebsite.Infrastructure.Extensions
//{
//    public static class WebApplicationExtensions
//    {
//        public static void EnsureDirectories(this WebApplication app)
//        {
//            var webRootPath = app.Environment.WebRootPath
//                ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

//            EnsureDirectory(Path.Combine(Directory.GetCurrentDirectory(), "logs"));
//            EnsureDirectory(Path.Combine(webRootPath, "uploads", "services"));
//        }

//        private static void EnsureDirectory(string path)
//        {
//            if (!Directory.Exists(path))
//            {
//                Directory.CreateDirectory(path);
//                Log.Information("Created directory: {Path}", Path.GetFullPath(path));
//            }
//        }
//    }
//}
