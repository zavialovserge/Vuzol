using Dapper;
using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Word;
using Microsoft.SqlServer.Server;
using System.Data;
using System.Diagnostics;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Linq;
using Vuzol.Model.Db;
using Vuzol.ViewModel.Model;
using WpfApp2.Services;

namespace Vuzol.Services
{
    public class PropertyData
    {
        private const string GET_ALL_PROPERTYS_SQL =
                  @"SELECT pr.""FactoryNumber"", pr.""Name"", pr.""InventoryNumber"",
                          pr.""InvoiceId"", pr.""Fio_R"", pr.""Fio_I"", pr.""Fio_V"", pr.""BookId"", pr.""FormId"", 
                          pr.""OrderId"", o.""Date_D"" as ""OrderDate"",
                          pr.""OrderBookId"", pr.""PropertyTypeId"", pr.""Status"", ps.""Name"" as ""StatusName"",
                          COALESCE(pr.""AdditionalInfo"", '') as ""AdditionalInfo"", pr.""DLM"", f.""Name"" as ""FormName"",
                          pr.""BookPage"", pr.""Quantity"" as ""quantity"", pr.""Price"" as ""price"",
                          pr.""OrderBookPage"", f.""Date_D"" as ""FormDate"",
                          pr.""CategoryId"", pr.""MaterialResourcesId"", pr.""QuantityTypeId"",
                          COALESCE(e1.""LastName"" || ' ' || e1.""FirstName"", '') as ""FIO_R_STR"",
                          COALESCE(e3.""LastName"" || ' ' || e3.""FirstName"", '') as ""FIO_V_STR"",
                          COALESCE(u.""Name"", '') as ""UnitName"",
                          COALESCE(c.""Description"", '') as ""CategoryDescription"",
                          COALESCE(mr.""Description"", '') as ""MaterialResourcesDescription"",
                          COALESCE(qt.""Description"", '') as ""QuantityTypeDescription"" 
                   FROM ""Property"" as pr
                   LEFT JOIN ""Form"" as f ON f.""FormId"" = pr.""FormId""
                   LEFT JOIN ""Employee"" as e1 ON e1.""Id"" = pr.""Fio_R""
                   LEFT JOIN ""Employee"" as e3 ON e3.""Id"" = pr.""Fio_V""
                   LEFT JOIN ""Unit"" as u ON u.""Id"" = e3.""UnitId""
                   LEFT JOIN ""Category"" as c ON c.""Id"" = pr.""CategoryId""
                   LEFT JOIN ""MaterialResources"" as mr ON mr.""Id"" = pr.""MaterialResourcesId""
                   LEFT JOIN ""QuantityType"" as qt ON qt.""Id"" = pr.""QuantityTypeId""
                   LEFT JOIN ""Order"" as o ON o.""OrderId"" = pr.""OrderId""
                   LEFT JOIN ""PropertyStatus"" as ps ON ps.""Id"" = pr.""Status""";
        
        private const string INSERT_EXCEL_PROPERTYS_SQL = @"CALL insertpropertyfromexcel(
        @InventoryNumber, @Name,@FactoryNumber,@PropertyTypeName,@MaterialResourcesDescription,@InvoiceId,@InvoiceDate,
        @BookId,@BookPage,@OrderBookId,@OrderBookPage,@CategoryDescription,@FormId,@FormDate, @OrderId,@OrderDate, @StatusName,@QuantityTypeDescription,
        @Quantity, @Price,@Fio_R_STR,@Fio_V_STR,@UnitName, @AdditionalInfo)";

        private const string INSERT_PROPERTYS_SQL =
            @"insert
    into
    public.""Property"" (""FactoryNumber"",
    ""InventoryNumber"",
    ""Name"",
    ""InvoiceId"",
    ""Fio_R"",
    ""BookId"",
    ""FormId"",
    ""OrderId"",
    ""PropertyTypeId"",
    ""AdditionalInfo"",
    ""DLM"",
    ""OrderBookId"",
    ""BookPage"",
    ""OrderBookPage"",
    ""Status"",
    ""Price"",
    ""Quantity"",
    ""CategoryId"",
    ""MaterialResourcesId"",
    ""QuantityTypeId"",
    ""Fio_V"")
values(@FactoryNumber, @InventoryNumber, @Name, @InvoiceId, @Fio_R, @BookId, @FormId, @OrderId, 
       @PropertyTypeId, @AdditionalInfo, CURRENT_TIMESTAMP, 
       @OrderBookId, @BookPage, @OrderBookPage, @Status, @Price, @Quantity, 
       @CategoryId, @MaterialResourcesId, @QuantityTypeId, @Fio_V);
";
        private const string UPDATE_PROPERTYS_SQL =
                  @"UPDATE ""Property"" 
                   SET ""FactoryNumber"" =@FactoryNumber,
                       ""Name"" =@Name,
                       ""InvoiceId"" =@InvoiceId,
                       ""BookId"" =@BookId,
                       ""OrderBookId"" =@OrderBookId,
                       ""OrderId"" =@OrderId,
                       ""PropertyTypeId"" =@PropertyTypeId,
                       ""Status"" =@Status,
                       ""AdditionalInfo"" =@AdditionalInfo,
                       ""BookPage"" =@BookPage,
                       ""OrderBookPage"" =@OrderBookPage,
                       ""Quantity"" =@Quantity,
                       ""CategoryId"" =@CategoryId,
                       ""MaterialResourcesId"" =@MaterialResourcesId,
                       ""QuantityTypeId"" =@QuantityTypeId,
                       ""Price"" =@Price,
                       ""Fio_R"" =@Fio_R,
                       ""Fio_V"" =@Fio_V,
                       ""FormId"" =@FormId,
                       ""DLM"" = Now()
                       WHERE ""InventoryNumber"" =@InventoryNumber";

        private const string DELETE_PROPERTYS_SQL =
                                                 @"DELETE FROM ""HardwareEquipment"" 
                                                  WHERE ""MainInventoryNumber"" = @InventoryNumber;
                                                  DELETE FROM ""SoftwareEquipment"" 
                                                  WHERE ""MainInventoryNumber"" = @InventoryNumber;
                                                  DELETE FROM ""Property"" WHERE ""InventoryNumber"" = @InventoryNumber;";

        private const string EXIST_PROPERTY_SQL =
                  @"SELECT ""InventoryNumber"" FROM ""Property"" WHERE ""InventoryNumber"" = @InventoryNumber";
        public static bool ExistProperty(string inventoryNumber)
        {
            DbData dbData = new DbData();   
            using IDbConnection database = dbData.Connect();
            string InventoryNumberFromDb =
                database.ExecuteScalar<string>(EXIST_PROPERTY_SQL, new { InventoryNumber = inventoryNumber });
            return inventoryNumber == InventoryNumberFromDb;
        }
        public static List<Property> GetAllProperty()
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            IEnumerable<PropertyDTO> propertyDTOs =
                database.Query<PropertyDTO>(GET_ALL_PROPERTYS_SQL);
            return propertyDTOs.Select(ToProperty).OrderBy(p => p.InventoryNumber).ToList();
        }
        public static bool UpdateDb(Property selectedProperty)
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            PropertyDTO propertyDTO = ToDtoProperty(selectedProperty);
            
            int result = 0;
            try
            {
                
                // Явно створюємо об'єкт параметрів
                var parameters = new
                {
                    InventoryNumber = propertyDTO.InventoryNumber,
                    Name = propertyDTO.Name,
                    InvoiceId = propertyDTO.InvoiceId,
                    BookId = propertyDTO.BookId,
                    FormId = propertyDTO.FormId,
                    OrderBookId = propertyDTO.OrderBookId,
                    OrderId = propertyDTO.OrderId,
                    PropertyTypeId = propertyDTO.PropertyTypeId,
                    Status = propertyDTO.Status,
                    AdditionalInfo = propertyDTO.AdditionalInfo,  
                    BookPage = propertyDTO.BookPage,
                    OrderBookPage = propertyDTO.OrderBookPage,
                    Quantity = propertyDTO.Quantity,
                    Price = propertyDTO.Price,
                    FactoryNumber = propertyDTO.FactoryNumber,
                    Fio_R = propertyDTO.Fio_R,
                    Fio_V = propertyDTO.FIO_V,
                    CategoryId = propertyDTO.CategoryId,
                    MaterialResourcesId = propertyDTO.MaterialResourcesId,
                    QuantityTypeId = propertyDTO.QuantityTypeId
                };

                result = database.Execute(UPDATE_PROPERTYS_SQL, parameters);
                
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
            int result = 0;
            try
            {
                var parameters = new
                {
                    InventoryNumber = propertyDTO.InventoryNumber,
                    Name = propertyDTO.Name,
                    InvoiceId = propertyDTO.InvoiceId,
                    BookId = propertyDTO.BookId,
                    FormId = propertyDTO.FormId,
                    OrderBookId = propertyDTO.OrderBookId,
                    OrderId = propertyDTO.OrderId,
                    PropertyTypeId = propertyDTO.PropertyTypeId,
                    Status = propertyDTO.Status,
                    AdditionalInfo = propertyDTO.AdditionalInfo,
                    BookPage = propertyDTO.BookPage,
                    OrderBookPage = propertyDTO.OrderBookPage,
                    Quantity = propertyDTO.Quantity,
                    Price = propertyDTO.Price,
                    FactoryNumber = propertyDTO.FactoryNumber,
                    Fio_R = propertyDTO.Fio_R,
                    Fio_V = propertyDTO.FIO_V,
                    CategoryId = propertyDTO.CategoryId,
                    MaterialResourcesId = propertyDTO.MaterialResourcesId,
                    QuantityTypeId = propertyDTO.QuantityTypeId
                };
                result = database.Execute(INSERT_PROPERTYS_SQL, parameters);
            }
            catch (Exception ex)
            {
                string errorMessage = $"Помилка при додаванні Property в базу даних: {ex.Message}";
                ErrorLogger.LogError(ex, errorMessage);
            }
            return result == 1;
            
        }
        public static void InsertFromExcelIntoDb(Property selectedProperty)
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            PropertyDTO propertyDTO = ToDtoProperty(selectedProperty);
            int result = 0;
            try
            {
                var parameters = new
                {
                    InventoryNumber = propertyDTO.InventoryNumber,
                    Name = propertyDTO.Name,
                    FactoryNumber = propertyDTO.FactoryNumber,
                    PropertyTypeName = propertyDTO.PropertyTypeName,
                    MaterialResourcesDescription = propertyDTO.MaterialResourcesDescription,
                    InvoiceId = propertyDTO.InvoiceId,
                    InvoiceDate = propertyDTO.InvoiceDate,
                    BookId = propertyDTO.BookId,
                    BookPage = propertyDTO.BookPage,
                    OrderBookId = propertyDTO.OrderBookId,
                    OrderBookPage = propertyDTO.OrderBookPage,
                    FormId = propertyDTO.FormId,
                    FormDate = propertyDTO.FormDate,
                    OrderId = propertyDTO.OrderId,
                    OrderDate = propertyDTO.OrderDate,
                    StatusName = propertyDTO.StatusName,
                    QuantityTypeDescription = propertyDTO.QuantityTypeDescription,
                    CategoryDescription = propertyDTO.CategoryDescription,
                    Quantity = propertyDTO.Quantity,
                    Price = propertyDTO.Price,
                    Fio_R_STR = propertyDTO.FIO_R_STR,
                    Fio_V_STR = propertyDTO.FIO_V_STR,
                    UnitName = propertyDTO.UnitName,
                    AdditionalInfo = propertyDTO.AdditionalInfo                   
                };
                var paramToStr = parameters.GetType().GetProperties()
                    .Select(prop => $"{prop.Name}: {prop.GetValue(parameters)}")
                    .ToArray();
                string pStr = string.Join(", ", paramToStr);
                database.Execute(INSERT_EXCEL_PROPERTYS_SQL, parameters);              
                
            }
            catch (Exception ex)
            {
                string errorMessage = $"Помилка при додаванні Property в базу даних: {ex.Message}";
                ErrorLogger.LogError(ex, errorMessage);
            }

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
                          $"N'{propertyDTO.AdditionalInfo}'," +
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
                database.Execute(DELETE_PROPERTYS_SQL, 
                    new { FactoryNumber = selectedProperty.FactoryNumber,
                          InventoryNumber = selectedProperty.InventoryNumber });
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
                                dto.AdditionalInfo,
                                dto.DLM,
                                dto.Fio_R,                                
                                dto.FIO_V,
                                dto.FIO_R_STR,                                
                                dto.FIO_V_STR,
                                dto.UnitName,
                                dto.BookPage,
                                dto.OrderBookPage,
                                dto.OrderDate,
                                dto.Status,
                                dto.CategoryDescription,
                                dto.MaterialResourcesDescription,
                                dto.QuantityTypeDescription)
                   {
                       StatusName = dto.StatusName,
                       Quantity = dto.Quantity,
                       Price = dto.Price,
                       Cost= dto.Price * dto.Quantity,
                       CategoryId  = dto.CategoryId,
                       MaterialResourcesId = dto.MaterialResourcesId,
                       QuantityTypeId = dto.QuantityTypeId
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
                       AdditionalInfo = prop.AdditionalInfo,                       
                       Fio_R = prop.FIO_R,
                       FIO_V = prop.FIO_V,
                       BookPage = prop.BookPage,
                       OrderBookPage = prop.OrderBookPage,
                       OrderDate = prop.OrderDate,
                       Status = prop.Status,
                       Quantity = prop.Quantity,
                       Price = prop.Price,
                       MaterialResourcesId = prop.MaterialResourcesId,
                       QuantityTypeId = prop.QuantityTypeId,    
                       CategoryId = prop.CategoryId,
                       MaterialResourcesDescription = prop.MaterialResourcesDescription,
                       QuantityTypeDescription = prop.QuantityTypeDescription,
                       StatusName = prop.StatusName,
                       FIO_R_STR = prop.FIO_R_STR,
                       FIO_V_STR = prop.FIO_V_STR,
                       UnitName = prop.UnitName,
                       CategoryDescription = prop.CategoryDescription,
                       PropertyTypeName = prop.PropertyTypeName
                   };
    }
}
