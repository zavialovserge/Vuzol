using Dapper;
using Microsoft.Office.Interop.Excel;
using Microsoft.SqlServer.Server;
using System.Data;
using System.Diagnostics;
using System.DirectoryServices.ActiveDirectory;
using System.Net;
using System.Text;
using System.Windows.Controls.Primitives;
using Vuzol.Model.Db;
using Vuzol.ViewModel.Model;

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

        private const string INSERT_PROPERTYS_SQL = @"DO $$
            DECLARE
            statusNew integer:= @PropertyTypeIdNew;
            PropertyTypeIdNew integer := @InvoiceId;
            BEGIN 
            IF NOT EXISTS(SELECT 1 FROM ""Invoice"" WHERE ""InvoiceID"" = @InvoiceId) THEN 
            INSERT INTO ""Invoice""(""InvoiceID"", ""Date_From"") 
            VALUES(@InvoiceId,NOW()); 
            END IF; 
            IF NOT EXISTS(SELECT 1 FROM ""OrderBook"" WHERE ""OrderBookID"" = @OrderBookId) THEN 
                INSERT INTO ""Orderbook""(""OrderBookID"", ""Date_D"") 
            VALUES(@OrderBookId, NOW()); 
            END IF; 
            IF NOT EXISTS(SELECT 1 FROM ""Form"" WHERE ""FormId"" = @FormId) THEN 
            INSERT INTO ""Form""(""FormId"", ""Date_D"") 
            VALUES( @FormId, NOW()); 
            END IF; 
            IF NOT EXISTS(SELECT 1 FROM ""Order"" WHERE ""OrderId"" = @FormId) THEN
              INSERT INTO ""Order""(""Orderid"", ""Date_D"") 
            VALUES(@FormId, NOW()); 
            END IF; 
            IF NOT EXISTS(SELECT 1 FROM ""PropertyStatus"" ps WHERE ps.""Name"" = @statusOld) THEN 
            INSERT INTO ""PropertyStatus""(""Name"") VALUES(@statusOld); 
            END IF;
            statusNew:= (SELECT ""Id"" FROM ""PropertyStatus"" ps WHERE ps.""Name"" = @statusOld 
            LIMIT 1); 
            IF NOT EXISTS(SELECT 1 FROM ""PropertyType"" pt WHERE pt.""Name"" = @PropertyTypeName) THEN 
                INSERT INTO ""PropertyType""(""Name"") VALUES(@PropertyTypeName); 
            END IF; 
            PropertyTypeIdNew:= (SELECT ""Id"" FROM ""PropertyType"" pt WHERE pt.""Name"" = @PropertyTypeName
            LIMIT 1); 
            INSERT INTO ""Property""
                   (""FactoryNumber"", ""InventoryNumber"", ""Name"", ""InvoiceId"", ""BookId"", 
                    ""BookPage"", ""FormId"", ""OrderId"", ""OrderBookId"", ""OrderBookPage"", 
                    ""PropertyTypeId"", ""Status"", ""Additionalnfo"", ""Quantity"", ""Price"", ""DLM"")
                   VALUES (@FactoryNumber,@InventoryNumber,@Name,@InvoiceId,@BookId,
                           @BookPage,@FormId,@OrderId,@OrderBookId,@OrderBookPage,
                           @PropertyTypeId,@Status,@Additionalnfo,@Quantity,@Price,@Date_D)
                           END $$;";

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
            var result = database.Execute(UPDATE_PROPERTYS_SQL, new { propertyDTO.InventoryNumber, propertyDTO.Name,
                                                                      propertyDTO.InvoiceId, propertyDTO.BookId, 
                                                                      propertyDTO.OrderBookId, propertyDTO.OrderId, 
                                                                      propertyDTO.PropertyTypeId, propertyDTO.Status,
                                                                      propertyDTO.Additionalnfo, propertyDTO.BookPage, 
                                                                      propertyDTO.OrderBookPage, propertyDTO.Quantity, 
                                                                      propertyDTO.Price, propertyDTO.FactoryNumber });
            return result == 1;
        }
        public static bool InsertIntoDb(Property selectedProperty)
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            PropertyDTO propertyDTO = ToDtoProperty(selectedProperty); 
            var result = database.Execute(INSERT_PROPERTYS_SQL, new { propertyDTO.FactoryNumber, propertyDTO.InventoryNumber,
                                                           propertyDTO.Name, propertyDTO.InvoiceId, 
                                                           propertyDTO.BookId, propertyDTO.BookPage, 
                                                           propertyDTO.FormId, propertyDTO.OrderId, 
                                                           propertyDTO.OrderBookId, propertyDTO.OrderBookPage, 
                                                           PropertyTypeIdNew = propertyDTO.PropertyTypeId, statusNew = propertyDTO.StatusName,
                                                           statusOld = propertyDTO.StatusName,
                                                           propertyDTO.Additionalnfo, propertyDTO.Quantity, 
                                                           propertyDTO.Price,propertyDTO.PropertyTypeName
                                                          });
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
                          $"GETDATE())") ;

            }
            var strInsert = INSERT_PROPERTYS_SQL + sb.ToString().TrimEnd(',');
            var result = database.Execute(strInsert);
            return result == 1;
        }
        public static void DeleteFromDb(Property selectedProperty)
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();       
            database.Execute(DELETE_PROPERTYS_SQL, new { FactoryNumber = selectedProperty.FactoryNumber });            
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
                       Price=dto.Price
                   };
        private static PropertyDTO ToDtoProperty(Property prop) =>
                   new PropertyDTO()
                   {  FactoryNumber = prop.FactoryNumber,
                       Name=prop.Name,
                       InventoryNumber=prop.InventoryNumber,
                       InvoiceId=prop.InvoiceId,
                       InvoiceDate=prop.InvoiceDate,
                       BookId=prop.BookId,
                       OrderBookId=prop.OrderBookId,
                       FormId=prop.FormId,
                       FormName=prop.FormName,
                       FormDate=prop.FormDate,
                       OrderId=prop.OrderId,
                       PropertyTypeId=prop.PropertyTypeId,
                       PropertyTypeName = prop.PropertyTypeName.Contains("\'")
                                              ? prop.PropertyTypeName.Replace("\'", "''")
                                              : prop.PropertyTypeName,
                       Additionalnfo =prop.Additionalnfo.Contains("\'") 
                                              ? prop.Additionalnfo.Replace("\'", "''")
                                              : prop.Additionalnfo,
                       FIO_I=prop.FIO_I,
                       FIO_R=prop.FIO_R,
                       BookPage=prop.BookPage,
                       OrderBookPage=prop.OrderBookPage,
                       OrderDate=prop.OrderDate,
                       Status = prop.Status,
                       StatusName=prop.StatusName.Contains("\'") ? prop.StatusName.Replace("\'", "''") 
                                                                 : prop.StatusName,
                       Quantity=prop.Quantity,
                       Price=prop.Price
                   };        
    }
}
