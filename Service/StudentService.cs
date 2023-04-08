using Data;
using Entities.Enums;
using Entities.Models;
using Infrastructure.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

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
                string sqlQuery = @"BEGIN TRY
	                                BEGIN TRAN
                                        DECLARE @StudentId int = 0,@SessionId int = 0;
                                        INSERT INTO SESSIONMASTER(FirstName,MiddleName,LastName,FatherName,MotherName,ContactNumber,AlternateContact,
                                                                  [Address],PostalCode,IdentityType,IdentityNumber,DOB,DOJ,DOL,IsDiscontinued)
                                                           Values(@FirstName,@MiddleName,@LastName,@FatherName,@MotherName,@ContactNumber,
                                                                  @AlternateContact,@Address,@PostalCode,@IdentityType,@IdentityNumber,@DOB,@DOJ,@DOL,
                                                                  @IsDiscontinued);
                                        SELECT @StudentId = SCOPE_IDENTITY();
                                        SELECT TOP 1 @SessionId = Id FROM SessionMaster ORDER BY Id DESC
                                        INSERT INTO SessionWiseStudent(SessionId,StudentId,Section,IsNew,EntryOn,ModifyOn,ClassId) 
                                                                VALUES(@SessionId,@StudentId,@Section,1,GETDATE(),GETDATE(),@ClassId);
                                    	COMMIT TRAN
                                    END TRY
                                    BEGIN CATCH
                                    	ROLLBACK TRAN
                                    END CATCH";

                var result = await _repository.ExecuteAsync(sqlQuery, entity, commandType: CommandType.Text);

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
            var res = new PagedResult<StudentColumn>();
            try
            {
                string tableName = "Student(nolock)";
                string sqlQuery = @$"SELECT * FROM {tableName};
                                 SELECT COUNT(1) TotalItems,@PageNumber PageNumber ,@PageSize PageSize FROM {tableName}";

                var result = await _repository.GetMultipleAsync<PagedResult<StudentColumn>, StudentColumn>(sqlQuery,
                    new
                    {
                        request.PageNumber,
                        request.PageSize
                    }, CommandType.Text);

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
    }
}
