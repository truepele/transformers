using System;
using System.Data;
using System.Threading.Tasks;
using Transformers.Model.Entities;
using Transformers.Model.Services;
using Dapper;

namespace Transformers.DataAccess.Services
{
    public class OverallScoreCalcServiceStoredProc : IOverallScoreCalcService
    {
        private readonly Func<IDbConnection> _connectionAccessor;

        private const string StoredProcName = "calc_overall_score";


        public OverallScoreCalcServiceStoredProc(Func<IDbConnection> connectionAccessor)
        {
            _connectionAccessor = connectionAccessor ?? throw new ArgumentNullException(nameof(connectionAccessor));
        }

        public Task<int> CalculateAsync(Transformer transformer)
        {
            var connection = _connectionAccessor();
            if(connection.State == ConnectionState.Closed) { connection.Open(); }

            var values = new
            {
                transformer.Courage,
                transformer.Endurance,
                transformer.Firepower,
                transformer.Intelligence,
                transformer.Rank,
                transformer.Skill,
                transformer.Speed,
                transformer.Strength
            };
            return connection.ExecuteScalarAsync<int>(StoredProcName, values, commandType: CommandType.StoredProcedure);
        }
    }
}
