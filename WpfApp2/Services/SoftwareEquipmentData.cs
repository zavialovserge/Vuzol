using Dapper;
using System.Data;
using Vuzol.Model.Db;
using Vuzol.Model.DTO;
using Vuzol.ViewModel.Model;

namespace Vuzol.Services
{
    public class SoftwareEquipmentData
    {
        private const string GET_ALL_SOFTWAREEQUIPMENT_SQL =
                   @"SELECT ""Id"",""MainPropertyFactoryNumber"",""Description"",""Quantity""
                     FROM ""SoftwareEquipment"" ";

        private const string UPDATE_SOFTWAREEQUIPMENT_SQL =
                   @"UPDATE [dbo].[SoftwareEquipment] SET ";
        private const string INSERT_SOFTWAREEQUIPMENT_SQL =
                  @"INSERT INTO [dbo].[SoftwareEquipment]
                            ([MainPropertyFactoryNumber],
                             [Description],[Quantity])
                             VALUES ";
        private const string DELETE_SOFTWAREEQUIPMENT_SQL =
                  @"DELETE FROM [dbo].[SoftwareEquipment]";
        public static IEnumerable<SoftwareEquipment> GetAllSoftwareEquipment(int factoryNumber)
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            IEnumerable<SoftwareEquipmentDTO> softwareEquipmentDTOs =
                database.Query<SoftwareEquipmentDTO>(GET_ALL_SOFTWAREEQUIPMENT_SQL 
                                                    + $"where \"MainPropertyFactoryNumber\" = {factoryNumber}");
            var softwareEquipmentList = softwareEquipmentDTOs.Select(ToSoftwareEquipment).ToList();
            int i = 1;
            foreach (var softwareEquipment in softwareEquipmentList)
            {
                softwareEquipment.CountId = i;
                i++;
            }
            return softwareEquipmentList;
        }

        public static bool DeleteFromDb(SoftwareEquipment softwareEquipment)
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            SoftwareEquipmentDTO softwareEquipmentDTO = ToSoftwareEquipmentDTO(softwareEquipment);
            var strDelete = DELETE_SOFTWAREEQUIPMENT_SQL +
                " WHERE [Id] =" + $"{softwareEquipmentDTO.Id}";
            var result = database.Execute(strDelete);
            return result == 1;
        }

        public static bool EditSoftwareEquipment(SoftwareEquipment softwareEquipment)
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            SoftwareEquipmentDTO softwareEquipmentDTO = ToSoftwareEquipmentDTO(softwareEquipment);
            var strUpdate = UPDATE_SOFTWAREEQUIPMENT_SQL + $"[Description] = N'{softwareEquipmentDTO.Description}', " +
                                                           $"[Quantity] ={softwareEquipmentDTO.Quantity.ToString().Replace(',','.')} " +
                " WHERE [Id] =" + $"{softwareEquipmentDTO.Id}";
            var result = database.Execute(strUpdate);
            return result == 1;
        }
        public static bool InsertSoftwareEquipment(SoftwareEquipment softwareEquipment)
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            SoftwareEquipmentDTO softwareEquipmentDTO = ToSoftwareEquipmentDTO(softwareEquipment);
            var strInsert = INSERT_SOFTWAREEQUIPMENT_SQL +  $"({softwareEquipmentDTO.MainPropertyFactoryNumber},"+
                                                               $"N'{softwareEquipmentDTO.Description}'," +
                                                               $"{softwareEquipmentDTO.Quantity.ToString().Replace(',', '.')})";
            var result = database.Execute(strInsert);
            return result == 1;
        }
        private static SoftwareEquipment ToSoftwareEquipment(SoftwareEquipmentDTO dto) =>
         new SoftwareEquipment(dto.Id, dto.MainPropertyFactoryNumber, dto.Description,dto.Quantity);
        private static SoftwareEquipmentDTO ToSoftwareEquipmentDTO(SoftwareEquipment dto) =>
         new SoftwareEquipmentDTO() { Id = dto.Id, 
                                      MainPropertyFactoryNumber = dto.MainPropertyFactoryNumber, 
                                      Description = dto.Description,
         Quantity = dto.Quantity};

    }
}
