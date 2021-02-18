using Microsoft.Data.SqlClient;

namespace Transformers.WebApi.Settings
{
    public class DataAccessSettings
    {
        public DataAccessType DataAccessType { get; set; }
        public SqlConnectionStringBuilder SqlConnectionStringBuilder { get; set; }
    }
}
