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

        private const string INSERT_PROPERTYS_SQL =
                  @"INSERT INTO ""Property""
                   (""FactoryNumber"", ""InventoryNumber"", ""Name"", ""InvoiceId"", ""BookId"", 
                    ""BookPage"", ""FormId"", ""OrderId"", ""OrderBookId"", ""OrderBookPage"", 
                    ""PropertyTypeId"", ""Status"", ""Additionalnfo"", ""Quantity"", ""Price"", ""DLM"")
                   VALUES";

        private const string UPDATE_PROPERTYS_SQL =
                  @"UPDATE ""Property""";
        private const string DELETE_PROPERTYS_SQL =
                  @" delete from  ""Property"" where ""FactoryNumber"" =  ";
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
            string updateSql = $" SET \"InventoryNumber\" = " + $"{propertyDTO.InventoryNumber}," +
                               "\"Name\" = " + $"N'{propertyDTO.Name}'," +
                               "\"InvoiceId\" = " + $"{propertyDTO.InvoiceId}," +
                               "\"BookId\" = " + $"{propertyDTO.BookId}," +
                               "\"OrderBookId\" = " + $"{propertyDTO.OrderBookId}," +
                               "\"OrderId\" =" + $"{propertyDTO.OrderId}," +
                               "\"PropertyTypeId\" =" + $"{propertyDTO.PropertyTypeId}," +
                               "\"Status\" =" + $"{propertyDTO.Status}," +
                               "\"Additionalnfo\" =" + $"N'{propertyDTO.Additionalnfo}'," +
                               "\"BookPage\" =" + $"{propertyDTO.BookPage}," +
                               "\"OrderBookPage\" =" + $"{propertyDTO.OrderBookPage}," +
                               "\"Quantity\" =" + $"{propertyDTO.Quantity.ToString().Replace(',', '.')}," +
                               "\"Price\" =" + $"{propertyDTO.Price.ToString().Replace(',', '.')}," +
                               "\"DLM\" = Now() " +
                               " WHERE \"FactoryNumber\" =" + $"{propertyDTO.FactoryNumber}";
            var strUpdate = UPDATE_PROPERTYS_SQL + updateSql;
            var result = database.Execute(strUpdate);
            return result == 1;
        }
        public static bool InsertIntoDb(Property selectedProperty)
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            PropertyDTO propertyDTO = ToDtoProperty(selectedProperty);
            var strInsert = @"DO $$"+
            "DECLARE\n" +
            $"statusNew integer:= { propertyDTO.PropertyTypeId};\n" +
            $"PropertyTypeIdNew integer := { propertyDTO.InvoiceId};\n"+
            $"BEGIN "+
            $"IF NOT EXISTS(SELECT 1 FROM \"Invoice\" WHERE \"InvoiceID\" = {propertyDTO.InvoiceId}) THEN "+
            $"INSERT INTO \"Invoice\"(\"InvoiceID\", \"Date_From\") "+
            $"VALUES({propertyDTO.InvoiceId},NOW()); " +
            $"END IF; "+
            $"IF NOT EXISTS(SELECT 1 FROM \"OrderBook\" WHERE \"OrderBookID\" = {propertyDTO.OrderBookId}) THEN "+
            $"    INSERT INTO \"Orderbook\"(\"OrderBookID\", \"Date_D\") "+
            $"VALUES({ propertyDTO.OrderBookId}, NOW()); " +
            $"END IF; "+
            $"IF NOT EXISTS(SELECT 1 FROM \"Form\" WHERE \"FormId\" = {propertyDTO.FormId}) THEN "+
            $"INSERT INTO \"Form\"(\"FormId\", \"Date_D\") "+
            $"VALUES( {propertyDTO.FormId}, NOW()); "+
            $"END IF; "+
            $"IF NOT EXISTS(SELECT 1 FROM \"Order\" WHERE \"OrderId\" = {propertyDTO.OrderId}) THEN "+
            $"  INSERT INTO \"Order\"(\"Orderid\", \"Date_D\") "+
            $"VALUES({ propertyDTO.OrderId}, NOW()); " +
            $"END IF; "+
            $"IF NOT EXISTS(SELECT 1 FROM \"PropertyStatus\" ps WHERE ps.\"Name\" = '{propertyDTO.StatusName}') THEN "+
            $"INSERT INTO \"PropertyStatus\"(\"Name\") VALUES('{propertyDTO.StatusName}'); "+
            $"END IF; "+
            $"statusNew:= (SELECT \"Id\" FROM \"PropertyStatus\" ps WHERE ps.\"Name\" = '{propertyDTO.StatusName}' "+
            $"LIMIT 1); "+
            $"IF NOT EXISTS(SELECT 1 FROM \"PropertyType\" pt WHERE pt.\"Name\" = '{propertyDTO.PropertyTypeName}') THEN "+
            $"    INSERT INTO \"PropertyType\"(\"Name\") VALUES('{propertyDTO.PropertyTypeName}'); "+
            $"END IF; "+
            $"PropertyTypeIdNew:= (SELECT \"Id\" FROM \"PropertyType\" pt WHERE pt.\"Name\" = '{propertyDTO.PropertyTypeName}'"+
            $"LIMIT 1); "+
            INSERT_PROPERTYS_SQL +   
                                $"({propertyDTO.FactoryNumber}," +
                                $"{propertyDTO.InventoryNumber}," +
                                $"'{propertyDTO.Name}'," +
                                $"{propertyDTO.InvoiceId}," +
                                $"{propertyDTO.BookId}," +
                                $"{propertyDTO.BookPage}," +
                                $"{propertyDTO.FormId}," +
                                $"{propertyDTO.OrderId}," +
                                $"{propertyDTO.OrderBookId}," +
                                $"{propertyDTO.OrderBookPage}," +
                                $"PropertyTypeIdNew," +
                                $"statusNew," +
                                $"'{propertyDTO.Additionalnfo}'," +
                                $"'{propertyDTO.Quantity.ToString().Replace(',', '.')}'," +
                                $"'{propertyDTO.Price.ToString().Replace(',', '.')}'," +
                                $"NOW());" +
                                "END $$;";  
            var result = database.Execute(strInsert);
            return result == 1;
        }
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
        public static void DeletefFromDb(Property selectedProperty)
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            string strDelete = "DO $$\r\nBEGIN\r\n    " +
                "IF EXISTS (SELECT 1 FROM \"HardwareEquipment\" " +
                $"WHERE \"SubPropertyFactoryNumber\" = {selectedProperty.FactoryNumber}) " +
                "THEN\r\n        DELETE FROM \"HardwareEquipment\" " +
                $"WHERE \"SubPropertyFactoryNumber\" = {selectedProperty.FactoryNumber};\r\n \r\nEND IF;  " +
                $"DELETE FROM \"Property\" WHERE \"FactoryNumber\" = {selectedProperty.FactoryNumber};" +
                "\r\nEND $$;";         
            database.Execute(strDelete);            
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
