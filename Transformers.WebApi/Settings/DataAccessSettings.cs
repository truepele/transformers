using Microsoft.Data.SqlClient;

namespace Transformers.WebApi.Settings
{
    public sealed class DataAccessSettings
    {
        public DataAccessType DataAccessType { get; set; }
        public DockerSqlSettings DockerSql { get; set; }
        public SqlConnectionStringBuilder SqlConnectionStringBuilder { get; set; }
    }
}
