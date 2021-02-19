namespace Transformers.WebApi.Settings
{
    public sealed class DockerSqlSettings
    {
        public string ContainerName { get; set; }
        public int HostPort { get; set; }
        public string Image { get; set; }
        public string SaPassword { get; set; } = "ComplexPW2021!";
        public string SqlCmdExecutable { get; set; } = "/opt/mssql-tools/bin/sqlcmd";
    }
}
