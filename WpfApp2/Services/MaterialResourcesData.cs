using Dapper;
using System.Data;
using Vuzol.Model.Db;
using Vuzol.Model.DTO;
using Vuzol.ViewModel.Model;

namespace Vuzol.Services
{
    internal class MaterialResourcesData
    {
        private const string GET_ALL_MaterialResourcesS_SQL =
                  @"SELECT ""Id"", ""Description"" FROM ""MaterialResources""";

        private const string UPDATE_MaterialResources_SQL =
                   @"UPDATE ""MaterialResources"" SET ""Description"" = @Description WHERE ""Id"" = @Id";

        private const string INSERT_MaterialResources_SQL =
                  @"INSERT INTO ""MaterialResources"" (""Description"") VALUES (@Description)";

        private const string DELETE_MaterialResources_SQL =
                  @"DELETE FROM ""MaterialResources"" WHERE ""Id"" = @Id";

        public static IEnumerable<MaterialResources> getAllMaterialResourcess()
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            IEnumerable<MaterialResourcesDTO> MaterialResourcesDTOs =
                database.Query<MaterialResourcesDTO>(GET_ALL_MaterialResourcesS_SQL);
            return MaterialResourcesDTOs.Select(ToMaterialResources).ToList();
        }

        public static bool DeleteFromDb(MaterialResources MaterialResources)
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            MaterialResourcesDTO MaterialResourcesDTO = ToMaterialResourcesDTO(MaterialResources);
            var result = database.Execute(DELETE_MaterialResources_SQL, new { Id = MaterialResourcesDTO.Id });
            return result == 1;
        }

        public static bool EditIEank(MaterialResources MaterialResources)
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            MaterialResourcesDTO MaterialResourcesDTO = ToMaterialResourcesDTO(MaterialResources);
            var escapedDesc = EscapePostgresString(MaterialResourcesDTO.Description);
            var result = database.Execute(UPDATE_MaterialResources_SQL, new { Description = escapedDesc, ID = MaterialResourcesDTO.Id });
            return result == 1;
        }

        public static bool InsertMaterialResources(MaterialResources MaterialResources)
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            MaterialResourcesDTO MaterialResourcesDTO = ToMaterialResourcesDTO(MaterialResources);
            var result = database.Execute(INSERT_MaterialResources_SQL, new { Description = EscapePostgresString(MaterialResourcesDTO.Description) });
            return result == 1;
        }

        private static string EscapePostgresString(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;
            return input.Replace("'", "''");
        }

        private static MaterialResources ToMaterialResources(MaterialResourcesDTO dto) =>
                 new MaterialResources(dto.Id, dto.Description);

        private static MaterialResourcesDTO ToMaterialResourcesDTO(MaterialResources dto) =>
                 new MaterialResourcesDTO() { Id = dto.Id, Description = dto.Description };
    }
}
