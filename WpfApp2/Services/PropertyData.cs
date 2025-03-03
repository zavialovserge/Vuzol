using Dapper;
using System.Data;
using System.Text;
using Vuzol.Model.Db;
using Vuzol.ViewModel.Model;

namespace Vuzol.Services
{
    public class PropertyData
    {
        private const string GET_ALL_PROPERTYS_SQL =
                  @"Select pr.FactoryNumber,pr.Name,pr.FactoryNumber,pr.InventoryNumber,
                  InvoiceId,FIO_R,FIO_I,BookId,pr.FormId,pr.OrderId,o.Date_D as OrderDate,
                  OrderBookId,PropertyTypeId,[Status],ps.Name as StatusName,
                  isnull(pr.Additionalnfo,'') as Additionalnfo,pr.DLM,f.Name as FormName,
                   BookPage,pr.Quantity as quantity,pr.Price as price 
                  ,OrderBookPage,f.date_d as FormDate
                  ,isnull(e1.LastName + ' ' + e1.FirstName,'') as [FIO_R_STR]
	              ,e2.LastName + ' ' + e2.FirstName as [FIO_I_STR]
                  ,isnull(u.Name,'') as UnitName
                  from Property  as pr 
                  left join Form as f on f.FormId = pr.FormId
                  left join [dbo].[Employee] as e1 on e1.Id = pr.FIO_R
                  left join [dbo].[Employee] as e2 on e2.Id = pr.Fio_I
                  left join [dbo].Unit as u on u.Id = e2.UnitId
                  left join [dbo].[Order] as o on o.OrderId = pr.OrderId
                  left join [dbo].[PropertyStatus] as ps on ps.Id = pr.Status";

        private const string INSERT_PROPERTYS_SQL =
                  @"INSERT INTO [dbo].[Property]
           ([FactoryNumber]
           ,[InventoryNumber]
           ,[Name]
           ,[InvoiceId]
           ,[BookId]
           ,[BookPage]
           ,[FormId]
           ,[OrderId]
           ,[OrderBookId]
           ,[OrderBookPage]
           ,[PropertyTypeId]
           ,[Status]
           ,[Additionalnfo]
           ,[Quantity]
           ,[Price]
           ,[DLM])
            VALUES ";
        private const string UPDATE_PROPERTYS_SQL =
                  @"UPDATE [dbo].[Property]";
        private const string DELETE_PROPERTYS_SQL =
                  @" delete from  [dbo].[Property] where [FactoryNumber] =  ";
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
            string updateSql = $" SET [InventoryNumber] = "+ $"{propertyDTO.InventoryNumber}," +
                               "[Name] = "+ $"N'{propertyDTO.Name}'," +
                               "[InvoiceId] =" + $"{propertyDTO.InvoiceId}," +
                               "[BookId] =" + $"{propertyDTO.BookId}," +
                               "[OrderBookId] =" + $"{propertyDTO.OrderBookId}," +
                               "[OrderId] =" + $"{propertyDTO.OrderId}," +                               
                               "[PropertyTypeId] =" + $"{propertyDTO.PropertyTypeId}," +
                               "[Status] =" + $"{propertyDTO.Status}," +
                               "[Additionalnfo] =" + $"N'{propertyDTO.Additionalnfo}'," +
                               "[BookPage] =" + $"N'{propertyDTO.BookPage}'," +
                               "[OrderBookPage] =" + $"N'{propertyDTO.OrderBookPage}'," +
                               "[Quantity] =" + $"{propertyDTO.Quantity.ToString().Replace(',', '.')}," +
                               "[Price] =" + $"{propertyDTO.Price.ToString().Replace(',', '.')}," +
                               "[DLM] = Getdate() " +
                               " WHERE [FactoryNumber] =" + $"{propertyDTO.FactoryNumber}";
            var strUpdate = UPDATE_PROPERTYS_SQL + updateSql;
            var result = database.Execute(strUpdate);
            return result == 1;
        }
        public static bool InsertIntoDb(Property selectedProperty)
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            PropertyDTO propertyDTO = ToDtoProperty(selectedProperty);
            var strInsert = $"Declare @statusNew int = {propertyDTO.Status} " +
                            $"Declare @PropertyTypeIdNew int = {propertyDTO.PropertyTypeId} " +
                            $"if not exists( select 1 from Invoice where InvoiceID = {propertyDTO.InvoiceId})" +
                            $"begin " +
                            $"Insert into [dbo].[Invoice]   ([InvoiceID] ,[Date_From])  " +
                            $"values ( {propertyDTO.InvoiceId},N'{propertyDTO.InvoiceDate}') " +
                            $"end " +
                            $"if not exists( select 1 from OrderBook where OrderBookID = {propertyDTO.OrderBookId})" +
                            $"begin " +
                            $"Insert into [dbo].[OrderBook]    ([OrderBookID] ,[Date_D])  " +
                            $"values ( {propertyDTO.OrderBookId},N'{DateTime.Now}') " +
                            $"end " +
                            $"if not exists( select 1 from OrderBook where OrderBookID = {propertyDTO.BookId})" +
                            $"begin " +
                            $"Insert into [dbo].[OrderBook]   ([OrderBookID] ,[Date_D])  " +
                            $"values ( {propertyDTO.BookId},N'{DateTime.Now}') " +
                            $"end " +
                            $"if not exists( select 1 from [dbo].[Form]  where [FormId] = {propertyDTO.FormId})" +
                            $"begin " +
                            $"Insert into [dbo].[Form]   ([FormId] ,[Date_D])  " +
                            $"values ( {propertyDTO.FormId},N'{DateTime.Now}') " +
                            $"end " +
                            $"if not exists( select 1 from [dbo].[Order]  where [OrderId] = {propertyDTO.OrderId})" +
                            $"begin " +
                            $"Insert into [dbo].[Order]   ([OrderId] ,[Date_D])  " +
                            $"values ( {propertyDTO.OrderId},N'{propertyDTO.OrderDate}') " +
                            $"end " +
                            $"if not exists (SELECT TOP 1 id FROM PropertyStatus AS ps WHERE ps.[Name] = N'{propertyDTO.StatusName}') " +
                            "begin " +
                            "INSERT INTO [dbo].[PropertyStatus] (Name) " +
                            $"VALUES(N'{propertyDTO.StatusName}') " +
                            $"set @statusNew = (SELECT top 1 id  from PropertyStatus as ps where ps.[Name] =N'{propertyDTO.StatusName}' ); " +
                            "end " +
                            "else " +
                            "SET @statusNew =(SELECT TOP 1 id " +
                            " FROM PropertyStatus AS ps " +
                            $"WHERE ps.[Name] = N'{propertyDTO.StatusName}') " +
                             $"if not exists (SELECT TOP 1 id FROM [dbo].[PropertyType] AS pt WHERE pt.[Name] = N'{propertyDTO.PropertyTypeName}') " +
                            "begin " +
                            "INSERT INTO [dbo].[PropertyType] (Name) " +
                            $"VALUES(N'{propertyDTO.PropertyTypeName}') " +
                            $"set @PropertyTypeIdNew = (SELECT top 1 id  from PropertyType as pt where pt.[Name] =N'{propertyDTO.PropertyTypeName}' ); " +
                            "end " +
                            "else " +
                            "SET @PropertyTypeIdNew =(SELECT TOP 1 id " +
                            " FROM PropertyType AS pt " +
                            $"WHERE pt.[Name] = N'{propertyDTO.PropertyTypeName}') " +
                            INSERT_PROPERTYS_SQL + $"({propertyDTO.FactoryNumber}," +
                                                    $"{propertyDTO.InventoryNumber}," +
                                                    $"N'{propertyDTO.Name}'," +
                                                    $"{propertyDTO.InvoiceId}," +
                                                    $"{propertyDTO.BookId}," +
                                                    $"{propertyDTO.BookPage}," +
                                                    $"{propertyDTO.FormId}," +
                                                    $"{propertyDTO.OrderId}," +
                                                    $"{propertyDTO.OrderBookId}," +
                                                    $"{propertyDTO.OrderBookPage}," +
                                                    $"@PropertyTypeIdNew," +
                                                    $"@statusNew," +                                                                 
                                                    $"N'{propertyDTO.Additionalnfo}'," +
                                                    $"{propertyDTO.Quantity.ToString().Replace(',', '.')}," +
                                                    $"{propertyDTO.Price.ToString().Replace(',', '.')}," +
                                                    $"GETDATE())";
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
        public static bool DeletefFromDb(Property selectedProperty)
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            string strDelete = $"if exists(Select * from dbo.HardwareEquipment where SubPropertyFactoryNumber = {selectedProperty.FactoryNumber}) "+
                               $" delete from  dbo.HardwareEquipment where SubPropertyFactoryNumber = {selectedProperty.FactoryNumber}" +
                               DELETE_PROPERTYS_SQL + $"{selectedProperty.FactoryNumber}";
            var result = database.Execute(strDelete);
            return result == 1;
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
