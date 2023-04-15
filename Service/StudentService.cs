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
            var filter = new StudentFilter();
            if (request.Param!=null)
            {
                filter = (StudentFilter)request.Param;
            }
            var res = new PagedResult<StudentColumn>();
            try
            {
                string sqlQuery = @$"IF @SessionId = 0
                                        SELECT Top 1 @SessionId = Id FROM SessionMaster order by Id desc
                                     SELECT s.*,ss.classId,cm.ClassName 
                                     FROM Student s  INNER JOIN  SessionWiseStudent ss on ss.StudentId = s.Id 
                                                     INNER JOIN  ClassMaster cm on cm.ClassId = ss.ClassId 
                                     WHERE SessionId = 1;
                                     SELECT COUNT(1) TotalItems,@PageNumber PageNumber ,@PageSize PageSize 
                                     FROM Student s  INNER JOIN  SessionWiseStudent ss on ss.StudentId = s.Id 
                                                     INNER JOIN  ClassMaster cm on cm.ClassId = ss.ClassId 
                                     WHERE SessionId = @SessionId;";

                var result = await _repository.GetMultipleAsync<StudentColumn, PagedResult<StudentColumn>>(sqlQuery,
                    new
                    {
                        request.PageNumber,
                        request.PageSize,
                        filter.SessionId
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

        public async Task<PagedResult<FeeHistory>> FeeHistory(PagedRequest request)
        {
            var filter = new FeeHistoryFilter();
            if (request.Param!=null)
            {
                filter = (FeeHistoryFilter)request.Param;
            }
            var res = new PagedResult<FeeHistory>();
            try
            {
                string sqlQuery = @$"IF @SessionId = 0
                                        SELECT Top 1 @SessionId = Id FROM SessionMaster order by Id desc
                                     SELECT * into #FeeCollection FROM FeeCollection Where SessionId = 1 and [Month] = 1

                                     SELECT s.FirstName + ' '+ s.LastName StudentName,s.FatherName,s.DOB,f.* 
                                     FROM Student s Left join #FeeCollection f on s.Id = f.StudentId;

                                     SELECT COUNT(1) TotalItems,@PageNumber PageNumber ,@PageSize PageSize 
                                     FROM Student s Left join #FeeCollection f on s.Id = f.StudentId;";

                var result = await _repository.GetMultipleAsync<FeeHistory, PagedResult<FeeHistory>>(sqlQuery,
                    new
                    {
                        request.PageNumber,
                        request.PageSize,
                        filter.SessionId
                    }, CommandType.Text);

                var data = (List<FeeHistory>)result.GetType().GetProperty("Table1").GetValue(result, null);
                var summery = (List<PagedResult<FeeHistory>>)result.GetType().GetProperty("Table2").GetValue(result, null);
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
