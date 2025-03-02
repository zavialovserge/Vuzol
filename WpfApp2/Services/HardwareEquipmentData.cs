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
                   @"SELECT  [MainPropertyFactoryNumber]
                             ,SubPropertyFactoryNumber
                             ,Quantity
                             ,[Description]
                             FROM [dbo].[HardwareEquipment]";

        private const string UPDATE_HARDWAREEQUIPMENT_SQL =
                   @"UPDATE [dbo].[HardwareEquipment] SET ";
        private const string INSERT_HARDWAREEQUIPMENT_SQL =
                  @"INSERT INTO [dbo].[HardwareEquipment]
                            ([MainPropertyFactoryNumber]
           ,[SubPropertyFactoryNumber]
           ,[Quantity]
           ,[Description])
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
            var strDelete = DELETE_HARDWAREEQUIPMENT_SQL +
                " WHERE [MainPropertyFactoryNumber] = " + $"{HardwareEquipmentDTO.MainPropertyFactoryNumber} " +
                $" and SubPropertyFactoryNumber = {HardwareEquipmentDTO.SubPropertyFactoryNumber}";
            var result = database.Execute(strDelete);
            return result == 1;
        }

        public static bool EditHardwareEquipment(HardwareEquipment HardwareEquipment,int previousSubPropertyFactoryNumber)
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            HardwareEquipmentDTO HardwareEquipmentDTO = ToHardwareEquipmentDTO(HardwareEquipment);
            var strUpdate = UPDATE_HARDWAREEQUIPMENT_SQL + "[SubPropertyFactoryNumber] = " + $"{HardwareEquipmentDTO.SubPropertyFactoryNumber}," 
                                                         + "[Quantity]= " + $"N'{HardwareEquipmentDTO.Quantity}'," 
                                                         + "[Description]= " + $"N'{HardwareEquipmentDTO.Description}' " +
                " WHERE [MainPropertyFactoryNumber] = " + $"{HardwareEquipmentDTO.MainPropertyFactoryNumber} " +
                $" and SubPropertyFactoryNumber = {previousSubPropertyFactoryNumber}";
            var result = database.Execute(strUpdate);
            return result == 1;
        }
        public static bool InsertHardwareEquipment(HardwareEquipment HardwareEquipment)
        {
            DbData dbData = new DbData();
            using IDbConnection database = dbData.Connect();
            HardwareEquipmentDTO HardwareEquipmentDTO = ToHardwareEquipmentDTO(HardwareEquipment);
            var strInsert = INSERT_HARDWAREEQUIPMENT_SQL + $"({HardwareEquipmentDTO.MainPropertyFactoryNumber}," +
                                                               $"{HardwareEquipmentDTO.SubPropertyFactoryNumber}," +
                                                               $"{HardwareEquipmentDTO.Quantity}," +
                                                               $"N'{HardwareEquipmentDTO.Description}')";
            var result = database.Execute(strInsert);
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
