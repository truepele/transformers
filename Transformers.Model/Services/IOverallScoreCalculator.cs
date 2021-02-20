using System.Threading.Tasks;
using Transformers.Model.Entities;

namespace Transformers.Model.Services
{
    public interface IOverallScoreCalcService
    {
        Task<int> CalculateAsync(Transformer transformer);
    }
}
