using Dapper;
using System.Data;
using Vuzol.Model.Db;
using Vuzol.Model.DTO;
using Vuzol.ViewModel.Model;

namespace Vuzol.Services
{
    public class CategoryData
    {
        private const string GET_ALL_CategoryS_SQL =
                   @"SELECT ""Id"", ""Description"" FROM ""Category""";

        private const string UPDATE_Category_SQL =
                   @"UPDATE ""Category"" SET ""Description"" = @Description WHERE ""Id"" = @Id";

        private const string INSERT_Category_SQL =
                  @"INSERT INTO ""Category"" (""Description"") VALUES (@Description)";

        private const string DELETE_Category_SQL =
                  @"DELETE FROM ""Category"" WHERE ""Id"" = @Id";

        public static IEnumerable<Category> getAllCategorys()
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            IEnumerable<CategoryDTO> CategoryDTOs =
                database.Query<CategoryDTO>(GET_ALL_CategoryS_SQL);
            return CategoryDTOs.Select(ToCategory).ToList();
        }

        public static bool DeleteFromDb(Category Category)
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            CategoryDTO CategoryDTO = ToCategoryDTO(Category);
            var result = database.Execute(DELETE_Category_SQL, new { Id = CategoryDTO.Id });
            return result == 1;
        }

        public static bool EditIEank(Category Category)
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            CategoryDTO CategoryDTO = ToCategoryDTO(Category);
            var escapedDesc = EscapePostgresString(CategoryDTO.Description);
            var result = database.Execute(UPDATE_Category_SQL, new { Description = escapedDesc, ID = CategoryDTO.Id });
            return result == 1;
        }

        public static bool InsertCategory(Category Category)
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            CategoryDTO CategoryDTO = ToCategoryDTO(Category);
            var result = database.Execute(INSERT_Category_SQL, new { Description = EscapePostgresString(CategoryDTO.Description) });
            return result == 1;
        }

        private static string EscapePostgresString(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;
            return input.Replace("'", "''");
        }

        private static Category ToCategory(CategoryDTO dto) =>
                 new Category(dto.Id, dto.Description);

        private static CategoryDTO ToCategoryDTO(Category dto) =>
                 new CategoryDTO() { Id = dto.Id, Description = dto.Description };
    }
}
