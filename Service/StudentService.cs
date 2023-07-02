using Data;
using Entities.Enums;
using Entities.Models;
using Infrastructure.Interface;
using System.Data;

namespace Service
{
    public class StudentService : IStudentService
    {
        private readonly IDapperRepository _repository;
        public StudentService(IDapperRepository repository)
        {
            _repository = repository;
        }

        public async Task<IResponse> AddAsync(Student entity)
        {
            Response res = new Response();
            try
            {
                var result = await _repository.ExecuteAsync("proc_AddStudent", new
                {
                    StudentId = entity.Id,
                    entity.FirstName,
                    entity.MiddleName,
                    entity.LastName,
                    entity.MotherName,
                    entity.FatherName,
                    entity.PostalCode,
                    entity.Address,
                    entity.AlternateContact,
                    entity.DOJ,
                    entity.DOB,
                    entity.ClassId,
                    entity.ContactNumber,
                    entity.Section,
                    entity.IdentityType,
                    entity.IdentityNumber,
                }, commandType: CommandType.StoredProcedure);
                if (result > -1 && result < 50)
                {
                    res = new Response
                    {
                        StatusCode = ResponseStatus.Success,
                        ResponseText = ResponseStatus.Success.ToString(),
                    };
                }
            }
            catch (Exception ex)
            {
                res = new Response
                {
                    StatusCode = ResponseStatus.Failed,
                    ResponseText = ResponseStatus.Failed.ToString(),
                };
            }
            return res;
        }

        public Task<IResponse> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<PagedResult<StudentColumn>> GetAsync(PagedRequest request)
        {
            var filter = new StudentFilter();
            if (request.Param!=null)
            {
                filter = (StudentFilter)request.Param;
            }
            var res = new PagedResult<StudentColumn>();
            try
            {
                var result = await _repository.GetMultipleAsync<StudentColumn, PagedResult<StudentColumn>>("proc_GetStudents",
                    new
                    {
                        request.PageNumber,
                        request.PageSize
                    }, CommandType.StoredProcedure);

                var data = (List<StudentColumn>)result.GetType().GetProperty("Table1").GetValue(result, null);
                var summery = (List<PagedResult<StudentColumn>>)result.GetType().GetProperty("Table2").GetValue(result, null);
                var pagedResult = summery.FirstOrDefault();
                pagedResult.Data = data;
                res = pagedResult;
            }
            catch (Exception ex)
            {

            }
            return res;
        }

        public Task<StudentColumn> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        } 

        public async Task<PagedResult<StudentColumn>> SessionWiseStudent(PagedRequest request)
        {
            var filter = new StudentFilter();
            if (request.Param!=null)
            {
                filter = (StudentFilter)request.Param;
            }
            var res = new PagedResult<StudentColumn>();
            try
            {
                var result = await _repository.GetMultipleAsync<StudentColumn, PagedResult<StudentColumn>>("proc_getSessionWiseStudent",
                    new
                    {
                        request.PageNumber,
                        request.PageSize,
                        filter.SessionId
                    }, CommandType.StoredProcedure);

                var data = (List<StudentColumn>)result.GetType().GetProperty("Table1").GetValue(result, null);
                var summery = (List<PagedResult<StudentColumn>>)result.GetType().GetProperty("Table2").GetValue(result, null);
                var pagedResult = summery.FirstOrDefault();
                pagedResult.Data = data;
                res = pagedResult;
            }
            catch (Exception ex)
            {

            }
            return res;
        }
    }
}