using Dapper;
using System.Data;
using System.Text;
using Vuzol.Model.Db;
using Vuzol.ViewModel.Model;
using WpfApp2.Services;

namespace Vuzol.Services
{
    public class PropertyData
    {
        private const string GET_ALL_PROPERTYS_SQL =
                  @"SELECT pr.""FactoryNumber"", pr.""Name"", pr.""InventoryNumber"",
                          pr.""InvoiceId"", pr.""FIO_R"", pr.""Fio_I"", pr.""BookId"", pr.""FormId"", 
                          pr.""OrderId"", o.""Date_D"" as ""OrderDate"",
                          pr.""OrderBookId"", pr.""PropertyTypeId"", pr.""Status"", ps.""Name"" as ""StatusName"",
                          COALESCE(pr.""Additionalnfo"", '') as ""Additionalnfo"", pr.""DLM"", f.""Name"" as ""FormName"",
                          pr.""BookPage"", pr.""Quantity"" as ""quantity"", pr.""Price"" as ""price"",
                          pr.""OrderBookPage"", f.""Date_D"" as ""FormDate"",
                          COALESCE(e1.""LastName"" || ' ' || e1.""FirstName"", '') as ""FIO_R_STR"",
                          COALESCE(e2.""LastName"" || ' ' || e2.""FirstName"", '') as ""FIO_I_STR"",
                          COALESCE(u.""Name"", '') as ""UnitName""
                   FROM ""Property"" as pr
                   LEFT JOIN ""Form"" as f ON f.""FormId"" = pr.""FormId""
                   LEFT JOIN ""Employee"" as e1 ON e1.""Id"" = pr.""FIO_R""
                   LEFT JOIN ""Employee"" as e2 ON e2.""Id"" = pr.""Fio_I""
                   LEFT JOIN ""Unit"" as u ON u.""Id"" = e2.""UnitId""
                   LEFT JOIN ""Order"" as o ON o.""OrderId"" = pr.""OrderId""
                   LEFT JOIN ""PropertyStatus"" as ps ON ps.""Id"" = pr.""Status""";

        private const string INSERT_PROPERTYS_SQL = @"CALL sp_insert_property(
    @InvoiceId, @OrderBookId, @FormId, @OrderId,
    @FactoryNumber, @InventoryNumber, @Name, @BookId,
    @BookPage, @OrderBookPage, @PropertyTypeName, @StatusName,
    @Additionalnfo, @Quantity, @Price, @Date_D)";


        private const string UPDATE_PROPERTYS_SQL =
                  @"UPDATE ""Property"" 
                   SET ""InventoryNumber"" =@InventoryNumber,
                       ""Name"" =@Name,
                       ""InvoiceId"" =@InvoiceId,
                       ""BookId"" =@BookId,
                       ""OrderBookId"" =@OrderBookId,
                       ""OrderId"" =@OrderId,
                       ""PropertyTypeId"" =@PropertyTypeId,
                       ""Status"" =@Status,
                       ""Additionalnfo"" =@Additionalnfo,
                       ""BookPage"" =@BookPage,
                       ""OrderBookPage"" =@OrderBookPage,
                       ""Quantity"" =@Quantity,
                       ""Price"" =@Price,
                       ""DLM"" = Now() 
                       WHERE ""FactoryNumber"" =@FactoryNumber";

        private const string DELETE_PROPERTYS_SQL =
                                                 @"DELETE FROM ""HardwareEquipment"" 
                                                  WHERE ""SubPropertyFactoryNumber"" = @FactoryNumber;
                                                  DELETE FROM ""Property"" WHERE ""FactoryNumber"" = @FactoryNumber;";
        public static List<Property> GetAllProperty()
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            IEnumerable<PropertyDTO> propertyDTOs =
                database.Query<PropertyDTO>(GET_ALL_PROPERTYS_SQL);
            return propertyDTOs.Select(ToProperty).ToList();
        }
        public static bool UpdateDb(Property selectedProperty)
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            PropertyDTO propertyDTO = ToDtoProperty(selectedProperty);
            int result = 0;
            try
            {
                result = database.Execute(UPDATE_PROPERTYS_SQL, new
                {
                    propertyDTO.InventoryNumber,
                    propertyDTO.Name,
                    propertyDTO.InvoiceId,
                    propertyDTO.BookId,
                    propertyDTO.OrderBookId,
                    propertyDTO.OrderId,
                    propertyDTO.PropertyTypeId,
                    propertyDTO.Status,
                    propertyDTO.Additionalnfo,
                    propertyDTO.BookPage,
                    propertyDTO.OrderBookPage,
                    propertyDTO.Quantity,
                    propertyDTO.Price,
                    propertyDTO.FactoryNumber
                });
            }
            catch (Exception ex)
            {
                string errorMessage = $"Помилка при оновеленні Property в базу даних: {ex.Message}";
                ErrorLogger.LogError(ex, errorMessage);
            }
            return result == 1;
        }
        public static bool InsertIntoDb(Property selectedProperty)
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            PropertyDTO propertyDTO = ToDtoProperty(selectedProperty);
            int formID = 0;
            Int32.TryParse(propertyDTO.FormId, out formID);
            int result = 0;
            try
            {

                result = database.Execute(INSERT_PROPERTYS_SQL, new
                {
                    propertyDTO.InvoiceId,
                    propertyDTO.OrderBookId,
                    FormId = formID,
                    propertyDTO.OrderId,
                    propertyDTO.FactoryNumber,
                    propertyDTO.InventoryNumber,
                    propertyDTO.Name,
                    propertyDTO.BookId,
                    propertyDTO.BookPage,
                    propertyDTO.OrderBookPage,
                    propertyDTO.PropertyTypeName,
                    propertyDTO.StatusName,
                    propertyDTO.Additionalnfo,
                    propertyDTO.Quantity,
                    propertyDTO.Price,
                    Date_D = propertyDTO.OrderDate
                });
            }
            catch (Exception ex)
            {
                string errorMessage = $"Помилка при додаванні Property в базу даних: {ex.Message}";
                ErrorLogger.LogError(ex, errorMessage);
            }
            return result == 1;
            
        }      

        //Need to fix Don`t use this method for now, it is not working properly
        public static bool InsertMassIntoDb(List<Property> selectedProperty)
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            List<PropertyDTO> propertyListDTO = selectedProperty
                                           .Select(x => ToDtoProperty(x))
                                           .ToList();
            StringBuilder sb = new StringBuilder();
            foreach (var propertyDTO in propertyListDTO)
            {
                sb.Append($"({propertyDTO.FactoryNumber}," +
                          $"{propertyDTO.InventoryNumber}," +
                          $"N'{propertyDTO.Name}'," +
                          $"{propertyDTO.InvoiceId}," +
                          $"{propertyDTO.BookId}," +
                          $"{propertyDTO.BookPage}," +
                          $"{propertyDTO.FormId}," +
                          $"{propertyDTO.OrderId}," +
                          $"{propertyDTO.OrderBookId}," +
                          $"{propertyDTO.OrderBookPage}," +
                          $"{propertyDTO.PropertyTypeId}," +
                          $"N'{propertyDTO.Additionalnfo}'," +
                          $"{propertyDTO.Quantity}," +
                          $"{propertyDTO.Price}," +
                          $"GETDATE())");

            }
            var strInsert = INSERT_PROPERTYS_SQL + sb.ToString().TrimEnd(',');
            var result = database.Execute(strInsert);
            return result == 1;
        }
        public static void DeleteFromDb(Property selectedProperty)
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            try
            {
                database.Execute(DELETE_PROPERTYS_SQL, new { FactoryNumber = selectedProperty.FactoryNumber });
            }
            catch (Exception ex)
            {
                string errorMessage = $"Помилка при видаленні Property в базу даних: {ex.Message}";
                ErrorLogger.LogError(ex, errorMessage);
            }
            
        }
        private static Property ToProperty(PropertyDTO dto) =>
                   new Property(dto.FactoryNumber,
                                dto.Name,
                                dto.InventoryNumber,
                                dto.InvoiceId,
                                dto.BookId,
                                dto.OrderBookId,
                                dto.FormId,
                                dto.FormName,
                                dto.FormDate,
                                dto.OrderId,
                                dto.PropertyTypeId,
                                dto.Additionalnfo,
                                dto.DLM,
                                dto.FIO_R,
                                dto.FIO_I,
                                dto.FIO_R_STR,
                                dto.FIO_I_STR,
                                dto.UnitName,
                                dto.BookPage,
                                dto.OrderBookPage,
                                dto.OrderDate,
                                dto.Status)
                   {
                       StatusName = dto.StatusName,
                       Quantity = dto.Quantity,
                       Price = dto.Price
                   };
        private static PropertyDTO ToDtoProperty(Property prop) =>
                   new PropertyDTO()
                   {
                       FactoryNumber = prop.FactoryNumber,
                       Name = prop.Name,
                       InventoryNumber = prop.InventoryNumber,
                       InvoiceId = prop.InvoiceId,
                       InvoiceDate = prop.InvoiceDate,
                       BookId = prop.BookId,
                       OrderBookId = prop.OrderBookId,
                       FormId = prop.FormId,
                       FormName = prop.FormName,
                       FormDate = prop.FormDate,
                       OrderId = prop.OrderId,
                       PropertyTypeId = prop.PropertyTypeId,
                       PropertyTypeName = prop.PropertyTypeName.Contains("\'")
                                              ? prop.PropertyTypeName.Replace("\'", "''")
                                              : prop.PropertyTypeName,
                       Additionalnfo = prop.Additionalnfo.Contains("\'")
                                              ? prop.Additionalnfo.Replace("\'", "''")
                                              : prop.Additionalnfo,
                       FIO_I = prop.FIO_I,
                       FIO_R = prop.FIO_R,
                       BookPage = prop.BookPage,
                       OrderBookPage = prop.OrderBookPage,
                       OrderDate = prop.OrderDate,
                       Status = prop.Status,
                       StatusName = prop.StatusName.Contains("\'") ? prop.StatusName.Replace("\'", "''")
                                                                 : prop.StatusName,
                       Quantity = prop.Quantity,
                       Price = prop.Price
                   };
    }
}
