using Dapper;
using System.Data;
using Vuzol.Model.Db;
using Vuzol.Model.DTO;
using Vuzol.ViewModel.Model;

namespace Vuzol.Services
{
    public class HardwareEquipmentData
    {
        private const string GET_ALL_HARDWAREEQUIPMENT_SQL =
                   @"SELECT  ""MainInventoryNumber""
                             ,""SubInventoryNumber""
                             ,""Quantity""
                             ,""Description""
                             FROM ""HardwareEquipment""
                             where ""MainInventoryNumber""=  @inventoryNumber ";

        private const string UPDATE_HARDWAREEQUIPMENT_SQL =
                   @"UPDATE ""HardwareEquipment"" SET ""SubInventoryNumber"" = @SubInventoryNumber ,
                            ""Quantity""=@Quantity, 
                            ""Description""=@Description
                    WHERE ""MainInventoryNumber"" = @MainInventoryNumber AND 
                            ""SubInventoryNumber"" = @PreviousSubInventoryNumber";
        private const string INSERT_HARDWAREEQUIPMENT_SQL =
                  @"INSERT INTO ""HardwareEquipment""
                            (""MainInventoryNumber""
           ,""SubInventoryNumber""
           ,""Quantity""
           ,""Description"")
                             VALUES (@MainInventoryNumber, @SubInventoryNumber, @Quantity, @Description)";
        private const string DELETE_HARDWAREEQUIPMENT_SQL =
                  @"DELETE FROM ""HardwareEquipment""
                    WHERE ""MainInventoryNumber"" = @MainInventoryNumber 
                    AND ""SubInventoryNumber"" = @SubInventoryNumber ";
        public static IEnumerable<HardwareEquipment> GetAllHardwareEquipment(string inventoryNumber)
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            IEnumerable<HardwareEquipmentDTO> HardwareEquipmentDTOs =
                database.Query<HardwareEquipmentDTO>(GET_ALL_HARDWAREEQUIPMENT_SQL, new { inventoryNumber });
            var HardwareEquipmentList = HardwareEquipmentDTOs.Select(ToHardwareEquipment).ToList();
            int i = 1;
            foreach (var hardwareEquipment in HardwareEquipmentList)
            {
                hardwareEquipment.CountId = i;
                i++;
            }
            return HardwareEquipmentList;
        }

        public static bool DeleteFromDb(HardwareEquipment HardwareEquipment)
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            HardwareEquipmentDTO HardwareEquipmentDTO = ToHardwareEquipmentDTO(HardwareEquipment);
            var result = database.Execute(DELETE_HARDWAREEQUIPMENT_SQL,
                                        new
                                        {
                                            HardwareEquipmentDTO.MainInventoryNumber,
                                            HardwareEquipmentDTO.SubInventoryNumber
                                        });
            return result == 1;
        }

        public static bool EditHardwareEquipment(HardwareEquipment HardwareEquipment, string previousSubInventoryNumber)
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            HardwareEquipmentDTO HardwareEquipmentDTO = ToHardwareEquipmentDTO(HardwareEquipment);
            var result = database.Execute(UPDATE_HARDWAREEQUIPMENT_SQL, new
            {
                HardwareEquipmentDTO.SubInventoryNumber,
                HardwareEquipmentDTO.Quantity,
                HardwareEquipmentDTO.Description,
                HardwareEquipmentDTO.MainInventoryNumber,
                PreviousSubInventoryNumber = previousSubInventoryNumber
            });
            return result == 1;
        }
        public static bool InsertHardwareEquipment(HardwareEquipment HardwareEquipment)
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            HardwareEquipmentDTO HardwareEquipmentDTO = ToHardwareEquipmentDTO(HardwareEquipment);
            var result = database.Execute(INSERT_HARDWAREEQUIPMENT_SQL, new
            {
                HardwareEquipmentDTO.SubInventoryNumber,
                HardwareEquipmentDTO.Quantity,
                HardwareEquipmentDTO.Description,
                HardwareEquipmentDTO.MainInventoryNumber,
            });
            return result == 1;
        }
        private static HardwareEquipment ToHardwareEquipment(HardwareEquipmentDTO dto) =>
         new HardwareEquipment(dto.MainInventoryNumber, dto.SubInventoryNumber, dto.Quantity, dto.Description);
        private static HardwareEquipmentDTO ToHardwareEquipmentDTO(HardwareEquipment equipment) =>
         new HardwareEquipmentDTO()
         {
             SubInventoryNumber = equipment.SubInventoryNumber,
             MainInventoryNumber = equipment.MainInventoryNumber,
             Quantity = equipment.Quantity,
             Description = equipment.Description
         };

    }
}
