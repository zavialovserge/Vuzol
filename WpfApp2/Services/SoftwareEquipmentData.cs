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
                     FROM ""SoftwareEquipment"" where ""MainPropertyFactoryNumber"" = @factoryNumber";

        private const string UPDATE_SOFTWAREEQUIPMENT_SQL =
                   @"UPDATE ""SoftwareEquipment"" SET ""Description"" = @Description, ""Quantity"" = @Quantity WHERE ""Id"" = @Id";
        private const string INSERT_SOFTWAREEQUIPMENT_SQL =
                  @"INSERT INTO ""SoftwareEquipment""
                            (""MainPropertyFactoryNumber"",
                             ""Description"",""Quantity"")
                             VALUES (@MainPropertyFactoryNumber, @Description, @Quantity)";
        private const string DELETE_SOFTWAREEQUIPMENT_SQL =
                  @"DELETE FROM ""SoftwareEquipment"" WHERE ""Id"" = @Id";
        public static IEnumerable<SoftwareEquipment> GetAllSoftwareEquipment(int factoryNumber)
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            IEnumerable<SoftwareEquipmentDTO> softwareEquipmentDTOs =
                database.Query<SoftwareEquipmentDTO>(GET_ALL_SOFTWAREEQUIPMENT_SQL, new { factoryNumber });
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
            var result = database.Execute(DELETE_SOFTWAREEQUIPMENT_SQL, new { Id = softwareEquipmentDTO.Id });
            return result == 1;
        }

        public static bool EditSoftwareEquipment(SoftwareEquipment softwareEquipment)
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            SoftwareEquipmentDTO softwareEquipmentDTO = ToSoftwareEquipmentDTO(softwareEquipment);
            var result = database.Execute(UPDATE_SOFTWAREEQUIPMENT_SQL, new { softwareEquipmentDTO.Description, 
                softwareEquipmentDTO.Quantity,  
                softwareEquipmentDTO.Id });
            return result == 1;
        }
        public static bool InsertSoftwareEquipment(SoftwareEquipment softwareEquipment)
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            SoftwareEquipmentDTO softwareEquipmentDTO = ToSoftwareEquipmentDTO(softwareEquipment);
            var result = database.Execute(INSERT_SOFTWAREEQUIPMENT_SQL, new { softwareEquipmentDTO.MainPropertyFactoryNumber, 
                softwareEquipmentDTO.Description, 
                softwareEquipmentDTO.Quantity });
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
