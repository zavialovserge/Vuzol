using Dapper;
using System.Data;
using Vuzol.Model.Db;
using Vuzol.Model.DTO;
using Vuzol.ViewModel.Model;

namespace Vuzol.Services
{
    public class EmployeeData
    {
        private const string GET_ALL_EMPLOYEES_SQL =
                  @"SELECT em.""Id"",
                           em.""FirstName"",
                           em.""LastName"",
                           em.""FatherName"",
                           em.""Rank"",
                           em.""Position"",
                           r.""Description"" as ""RankDescription"",
                           em.""UnitId"",
                           u.""Name"" as ""UnitName""
                    FROM ""Employee"" as em
                    LEFT JOIN ""Rank"" as r ON r.""ID"" = em.""Rank""
                    LEFT JOIN ""Unit"" as u ON u.""Id"" = em.""UnitId""";

        private const string Insert_EMPLOYEE_SQL =
                  @"INSERT INTO [dbo].[Employee]
                                (
                                  [FirstName]
                                ,[LastName]
                                ,[FatherName]
                                ,[Rank]
                                ,[UnitId]
                                ,[Position])
                                VALUES";
        private const string UPDATE_EMPLOYEE_SQL =
              @"UPDATE [dbo].[Employee] Set";
        private const string DELETE_EMPLOYEE_SQL =
              @"DELETE FROM [dbo].[Employee]";
        public static List<Employee> GetAllEmployees()
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            IEnumerable<EmployeeDTO> propertyDTOs =
                database.Query<EmployeeDTO>(GET_ALL_EMPLOYEES_SQL);
            return propertyDTOs.Select(ToEmployee).ToList();
        }
        public static bool InsertIntoDb(Employee employee)
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            EmployeeDTO employeeDTO = ToEmployeeDTO(employee);
            var strInsert = Insert_EMPLOYEE_SQL + $"(N'{employeeDTO.FirstName}'," +
                                                                 $"N'{employeeDTO.LastName}'," +
                                                                 $"N'{employeeDTO.FatherName}'," +
                                                                 $"{employeeDTO.Rank}," +
                                                                 $"{employeeDTO.UnitId}," +
                                                                 $"N'{employeeDTO.Position}')";
            var result = database.Execute(strInsert);
            return result == 1;
        }    

        public static bool EditIntoDb(Employee employee)
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            EmployeeDTO EmployeeDTO = ToEmployeeDTO(employee);
            var strUpdate = UPDATE_EMPLOYEE_SQL + 
            $"  [FirstName] =" + $"N'{EmployeeDTO.FirstName}'," +
                $"[LastName] =" + $"N'{EmployeeDTO.LastName}'," +
                $"[FatherName] = " + $"N'{EmployeeDTO.FatherName}'," +
                $"[Rank] = " + $"{EmployeeDTO.Rank}," +
                $"[Position] = " + $"N'{EmployeeDTO.Position}', " +
                $"[UnitId] = " + $"'{EmployeeDTO.UnitId}' " +
                " WHERE [Id] =" + $"{EmployeeDTO.Id}";
            var result = database.Execute(strUpdate);
            return result == 1;
        }
        public static bool DeleteFromDb(Employee employee)
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            EmployeeDTO EmployeeDTO = ToEmployeeDTO(employee);
            var strUpdate = DELETE_EMPLOYEE_SQL  +
                " WHERE [Id] =" + $"{EmployeeDTO.Id}";
            var result = database.Execute(strUpdate);
            return result == 1;
        }
        private static EmployeeDTO ToEmployeeDTO(Employee employee)
        {
            return new EmployeeDTO()
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                FatherName = employee.FatherName,
                Rank = employee.Rank,
                Position = employee.Position.Contains('\'') ? employee.Position.Replace("\'","''") 
                                                            : employee.Position,
                UnitId = employee.Unit
            };
        }
        private static Employee ToEmployee(EmployeeDTO dto) =>
                   new Employee(dto.Id, dto.FirstName, dto.LastName,
                                dto.FatherName, dto.Rank, dto.Position,dto.RankDescription,
                                dto.UnitId,dto.UnitName);
    }
}
