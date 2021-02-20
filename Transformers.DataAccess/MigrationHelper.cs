using System.IO;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Transformers.DataAccess
{
    internal static class MigrationHelper
    {
        public static void CreateStoredProcedures(MigrationBuilder migrationBuilder)
        {
            var type = typeof(TransformersDbContext).GetTypeInfo();
            var assembly = type.Assembly;
            using var resourceStream = assembly.GetManifestResourceStream($"{type.Namespace}.storedProcs.sql");
            using var sr = new StreamReader(resourceStream);
            migrationBuilder.Sql(sr.ReadToEnd());
        }
    }
}
