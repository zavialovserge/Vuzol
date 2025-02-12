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
                   @"SELECT  [ID],[MainPropertyFactoryNumber]
                            ,[Description]
                            FROM [dbo].[HardwareEquipment]";

        private const string UPDATE_HARDWAREEQUIPMENT_SQL =
                   @"UPDATE [dbo].[HardwareEquipment] SET [Description] =";
        private const string INSERT_HARDWAREEQUIPMENT_SQL =
                  @"INSERT INTO [dbo].[HardwareEquipment]
                            ([MainPropertyFactoryNumber],
                             [Description])
                             VALUES ";
        private const string DELETE_HARDWAREEQUIPMENT_SQL =
                  @"DELETE FROM [dbo].[HardwareEquipment]";
        public static IEnumerable<HardwareEquipment> GetAllHardwareEquipment(int factoryNumber)
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            IEnumerable<HardwareEquipmentDTO> HardwareEquipmentDTOs =
                database.Query<HardwareEquipmentDTO>(GET_ALL_HARDWAREEQUIPMENT_SQL
                                                    + $"where MainPropertyFactoryNumber = {factoryNumber}");
            var HardwareEquipmentList = HardwareEquipmentDTOs.Select(ToHardwareEquipment).ToList();
            int i = 1;
            foreach (var HardwareEquipment in HardwareEquipmentList)
            {
                HardwareEquipment.CountId = i;
                i++;
            }
            return HardwareEquipmentList;
        }

        public static bool DeleteFromDb(HardwareEquipment HardwareEquipment)
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            HardwareEquipmentDTO HardwareEquipmentDTO = ToHardwareEquipmentDTO(HardwareEquipment);
            var strDelete = DELETE_HARDWAREEQUIPMENT_SQL +
                " WHERE [MainPropertyFactoryNumber] = " + $"{HardwareEquipmentDTO.MainPropertyFactoryNumber} " +
                $" and SubPropertyFactoryNumber = {HardwareEquipmentDTO.SubPropertyFactoryNumber}";
            var result = database.Execute(strDelete);
            return result == 1;
        }

        public static bool EditHardwareEquipment(HardwareEquipment HardwareEquipment)
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            HardwareEquipmentDTO HardwareEquipmentDTO = ToHardwareEquipmentDTO(HardwareEquipment);
            var strUpdate = UPDATE_HARDWAREEQUIPMENT_SQL + $"N'{HardwareEquipmentDTO.Description}' " +
                " WHERE [MainPropertyFactoryNumber] = " + $"{HardwareEquipmentDTO.MainPropertyFactoryNumber} " +
                $" and SubPropertyFactoryNumber = {HardwareEquipmentDTO.SubPropertyFactoryNumber}";
            var result = database.Execute(strUpdate);
            return result == 1;
        }
        public static bool InsertHardwareEquipment(HardwareEquipment HardwareEquipment)
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            HardwareEquipmentDTO HardwareEquipmentDTO = ToHardwareEquipmentDTO(HardwareEquipment);
            var strInsert = INSERT_HARDWAREEQUIPMENT_SQL + $"({HardwareEquipmentDTO.MainPropertyFactoryNumber}," +
                                                               $"{HardwareEquipmentDTO.MainPropertyFactoryNumber}," +
                                                               $"N'{HardwareEquipmentDTO.Description}')";
            var result = database.Execute(strInsert);
            return result == 1;
        }
        private static HardwareEquipment ToHardwareEquipment(HardwareEquipmentDTO dto) =>
         new HardwareEquipment(dto.MainPropertyFactoryNumber,dto.SubPropertyFactoryNumber, dto.Description);
        private static HardwareEquipmentDTO ToHardwareEquipmentDTO(HardwareEquipment dto) =>
         new HardwareEquipmentDTO()
         {
             SubPropertyFactoryNumber = dto.SubPropertyFactoryNumber,
             MainPropertyFactoryNumber = dto.MainPropertyFactoryNumber,
             Description = dto.Description
         };

    }
}
