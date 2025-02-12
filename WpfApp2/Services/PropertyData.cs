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
                  CompletnessId,Additionalnfo,pr.DLM,f.Name as FormName,BookPage
                  ,OrderBookPage,f.date_d as FormDate
                  ,e1.LastName + ' ' + e1.FirstName as [FIO_R_STR]
	              ,e2.LastName + ' ' + e2.FirstName as [FIO_I_STR]
                  ,u.Name as UnitName
                  from Property  as pr 
                  left join Completeness as c on c.ID = pr.CompletnessId
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
           ,[CompletnessId]
           ,[Additionalnfo]
           ,[DLM])
            VALUES ";
        private const string UPDATE_PROPERTYS_SQL =
                  @"UPDATE [dbo].[Property]";
        private const string DELETE_PROPERTYS_SQL =
                  @"delete from  [dbo].[Property] where [FactoryNumber] =  ";
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
                               "[CompletnessId] =" + $"{propertyDTO.CompletnessId}," +
                               "[Additionalnfo] =" + $"N'{propertyDTO.Additionalnfo}'," +
                               "[BookPage] =" + $"N'{propertyDTO.BookPage}'," +
                               "[OrderBookPage] =" + $"N'{propertyDTO.OrderBookPage}'," +
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
            var strInsert = INSERT_PROPERTYS_SQL + $"({propertyDTO.FactoryNumber}," +
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
                                                                 $"{propertyDTO.Status}," +
                                                                 $"{propertyDTO.CompletnessId}," +
                                                                 $"N'{propertyDTO.Additionalnfo}'," +
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
                          $"{propertyDTO.CompletnessId}," +
                          $"N'{propertyDTO.Additionalnfo}'," +
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
            var result = database.Execute(DELETE_PROPERTYS_SQL + $"{selectedProperty.FactoryNumber}");
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
                                dto.CompletnessId,
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
                       StatusName = dto.StatusName
                   };
        private static PropertyDTO ToDtoProperty(Property prop) =>
                   new PropertyDTO()
                   {  FactoryNumber = prop.FactoryNumber,
                       Name=prop.Name,
                       InventoryNumber=prop.InventoryNumber,
                       InvoiceId=prop.InvoiceId,
                       BookId=prop.BookId,
                       OrderBookId=prop.OrderBookId,
                       FormId=prop.FormId,
                       FormName=prop.FormName,
                       FormDate=prop.FormDate,
                       OrderId=prop.OrderId,
                       PropertyTypeId=prop.PropertyTypeId,
                       CompletnessId=prop.CompletnessId,
                       Additionalnfo=prop.Additionalnfo,
                       FIO_I=prop.FIO_I,
                       FIO_R=prop.FIO_R,
                       BookPage=prop.BookPage,
                       OrderBookPage=prop.OrderBookPage,
                       OrderDate=prop.OrderDate,
                       Status = prop.Status
                   };        
    }
}
