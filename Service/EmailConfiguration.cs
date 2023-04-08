using Data;
using Infrastructure.Interface;

namespace Service
{
    public class EmailConfiguration : IEmailConfiguration
    {
        private IDapperRepository _dapper;
        public EmailConfiguration(IDapperRepository dapper)
        {
            _dapper = dapper;
        }

        public Task<IResponse> AddAsync(Entities.Models.EmailConfig entity)
        {
            throw new NotImplementedException();
        }

        public Task<IResponse> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Entities.Models.EmailConfig>> GetAllAsync(int loginId = 0, Entities.Models.EmailConfig entity = null)
        {
            string sqlQuery = @"Select * from EmailConfig(nolock)";
            entity = entity == null ? new Entities.Models.EmailConfig() : entity;
            var res = await _dapper.GetAllAsync<Entities.Models.EmailConfig>(entity, sqlQuery);
            return res ?? new List<Entities.Models.EmailConfig>();
        }

        public Task<IResponse<Entities.Models.EmailConfig>> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<Entities.Models.EmailConfig>> GetDropdownAsync(Entities.Models.EmailConfig entity)
        {
            throw new NotImplementedException();
        }
    }
}
