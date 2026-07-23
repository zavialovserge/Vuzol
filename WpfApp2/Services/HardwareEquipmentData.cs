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
                   @"SELECT  ""MainPropertyFactoryNumber""
                             ,""SubPropertyFactoryNumber""
                             ,""Quantity""
                             ,""Description""
                             FROM ""HardwareEquipment""
                             where ""MainPropertyFactoryNumber""=  @factoryNumber ";

        private const string UPDATE_HARDWAREEQUIPMENT_SQL =
                   @"UPDATE ""HardwareEquipment"" SET SubPropertyFactoryNumber = @SubPropertyFactoryNumber ,
                            ""Quantity""=@Quantity, 
                            ""Description""=@Description
                    WHERE ""MainPropertyFactoryNumber"" = @MainPropertyFactoryNumber AND 
                            ""SubPropertyFactoryNumber"" = @PreviousSubPropertyFactoryNumber";
        private const string INSERT_HARDWAREEQUIPMENT_SQL =
                  @"INSERT INTO ""dbo"".""HardwareEquipment""
                            (""MainPropertyFactoryNumber""
           ,""SubPropertyFactoryNumber""
           ,""Quantity""
           ,""Description"")
                             VALUES (@MainPropertyFactoryNumber, @SubPropertyFactoryNumber, @Quantity, @Description)";
        private const string DELETE_HARDWAREEQUIPMENT_SQL =
                  @"DELETE FROM ""HardwareEquipment""
                    WHERE ""MainPropertyFactoryNumber"" = @MainPropertyFactoryNumber 
                    AND ""SubPropertyFactoryNumber"" = @SubPropertyFactoryNumber ";
        public static IEnumerable<HardwareEquipment> GetAllHardwareEquipment(int factoryNumber)
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            IEnumerable<HardwareEquipmentDTO> HardwareEquipmentDTOs =
                database.Query<HardwareEquipmentDTO>(GET_ALL_HARDWAREEQUIPMENT_SQL, new { factoryNumber });
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
                                        new { HardwareEquipmentDTO.MainPropertyFactoryNumber, 
                                            HardwareEquipmentDTO.SubPropertyFactoryNumber });
            return result == 1;
        }

        public static bool EditHardwareEquipment(HardwareEquipment HardwareEquipment,int previousSubPropertyFactoryNumber)
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            HardwareEquipmentDTO HardwareEquipmentDTO = ToHardwareEquipmentDTO(HardwareEquipment);
            var result = database.Execute(UPDATE_HARDWAREEQUIPMENT_SQL, new { 
                                                          HardwareEquipmentDTO.SubPropertyFactoryNumber, 
                                                          HardwareEquipmentDTO.Quantity, 
                                                          HardwareEquipmentDTO.Description, 
                                                          HardwareEquipmentDTO.MainPropertyFactoryNumber, 
                                                          PreviousSubPropertyFactoryNumber = previousSubPropertyFactoryNumber });
            return result == 1;
        }
        public static bool InsertHardwareEquipment(HardwareEquipment HardwareEquipment)
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            HardwareEquipmentDTO HardwareEquipmentDTO = ToHardwareEquipmentDTO(HardwareEquipment);
            var result = database.Execute(INSERT_HARDWAREEQUIPMENT_SQL, new {
                HardwareEquipmentDTO.SubPropertyFactoryNumber,
                HardwareEquipmentDTO.Quantity,
                HardwareEquipmentDTO.Description,
                HardwareEquipmentDTO.MainPropertyFactoryNumber,
            });
            return result == 1;
        }
        private static HardwareEquipment ToHardwareEquipment(HardwareEquipmentDTO dto) =>
         new HardwareEquipment(dto.MainPropertyFactoryNumber,dto.SubPropertyFactoryNumber,dto.Quantity, dto.Description);
        private static HardwareEquipmentDTO ToHardwareEquipmentDTO(HardwareEquipment equipment) =>
         new HardwareEquipmentDTO()
         {
             SubPropertyFactoryNumber = equipment.SubPropertyFactoryNumber,
             MainPropertyFactoryNumber = equipment.MainPropertyFactoryNumber,
             Quantity = equipment.Quantity,
             Description = equipment.Description
         };

    }
}
