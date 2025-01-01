using System.Data;

namespace Pienty.Diariest.Core.Services.Handlers
{
    public interface IDbService
    {
        IDbConnection GetDbConnection();
    }
}