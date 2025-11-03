using Simulator.Shared.Enums.HCEnums.Enums;
using System.Reflection;

namespace Simulator.Shared.StaticClasses
{
    public static partial class StaticClass
    {
        public static class Actions
        {
            public static string CreateUpdate = "CreateUpdate";

            public static string Update = "Update";

            public static string CopyAndPaste = "CopyAndPaste";
            public static string GetAll = "GetAll";
            public static string ToSearch = "ToSearch";
            public static string Delete = "Delete";
            public static string DeleteGroup = "DeleteGroup";
            public static string GetById = "GetById";
            public static string Export = "Export";
            public static string Validate = $"Validate";

        }
        public static class ResponseMessages
        {
            public static string ReponseSuccesfullyMessage(string rowName, string tablename, string ResponseType) =>
               $"{rowName} was {ResponseType} succesfully in table: {tablename}";
            public static string ReponseFailMessage(string rowName, string tablename, string ResponseType) =>
               $"{rowName} was not {ResponseType} succesfully in table: {tablename}";

            public static string ReponseSuccesfullyMessageCreated(string rowName, string tablename) =>
                $"{rowName} was {ResponseType.Created} succesfully in table: {tablename}";
            public static string ReponseSuccesfullyMessageUpdated(string rowName, string tablename) =>
               $"{rowName} was {ResponseType.Updated} succesfully in table: {tablename}";
            public static string ReponseSuccesfullyMessageDelete(string rowName, string tablename) =>
               $"{rowName} was {ResponseType.Delete} succesfully in table: {tablename}";
            public static string ReponseFailMessageCreated(string rowName, string tablename) =>
                $"{rowName} was not {ResponseType.Created} succesfully in table: {tablename}";
            public static string ReponseFailMessageUpdate(string rowName, string tablename) =>
                $"{rowName} was not {ResponseType.Updated} succesfully in table: {tablename}";
            public static string ReponseFailMessageDelete(string rowName, string tablename) =>
                $"{rowName} was not {ResponseType.Delete} succesfully in table: {tablename}";
            public static string ReponseNotFound(string tablename) =>
               $"row was not {ResponseType.NotFound} succesfully in table: {tablename}";

        }
        public static class ResponseType
        {
            public static string Created = "created";
            public static string Updated = "updated";
            public static string Delete = "delete";
            public static string NotFound = "found";
            public static string UnApprove = "un approved";
            public static string Approve = "approved";
            public static string Reopen = "re opened";
            public static string Received = "received";
            public static string ChangeStatus = "changed status";
        }



        public static class ProductFormats
        {
            public static string ClassLegend = "ProductFormats";
            public static string ClassName => MethodBase.GetCurrentMethod()!.DeclaringType!.Name;
            public static class EndPoint
            {
                public static string DeleteGroup = $"{ClassName}/{Actions.DeleteGroup}";
                public static string CreateUpdate = $"{ClassName}/{Actions.CreateUpdate}";
                public static string UpdateUp = $"{ClassName}/{Actions.Update}Up";
                public static string UpdateDown = $"{ClassName}/{Actions.Update}Down";
                public static string GetAll = $"{ClassName}/{Actions.GetAll}";
                public static string GetAllByLine = $"{ClassName}/{Actions.GetAll}ByLine";
                public static string GetById = $"{ClassName}/{Actions.GetById}";
                public static string Delete = $"{ClassName}/{Actions.Delete}";
                public static string Export = $"{ClassName}/{Actions.Export}";
                public static string Validate = $"{ClassName}/{Actions.Validate}";
            }
            public static class Cache
            {
                public static string[] Key(Guid Id) => new[] { GetAll, GetById(Id) };
                public static string GetAll => $"GetAll-{ClassName}";
                public static string GetById(Guid Id) => $"GetById-{ClassName}-{Id}";
            }
            public static class PageName
            {
                public static string Create = $"Create{ClassName}";
                public static string Update = $"Update{ClassName}";
                public static string GetAll = $"GetAll{ClassName}";

            }


        }
       
     
        public static class BaseEquipments
        {
            public static string ClassLegend = "BaseEquipments";
            public static string ClassName => MethodBase.GetCurrentMethod()!.DeclaringType!.Name;
            public static class EndPoint
            {
                public static string DeleteGroup = $"{ClassName}/{Actions.DeleteGroup}";
                public static string CreateUpdate = $"{ClassName}/{Actions.CreateUpdate}";
                public static string UpdateUp = $"{ClassName}/{Actions.Update}Up";
                public static string UpdateDown = $"{ClassName}/{Actions.Update}Down";
                public static string GetAll = $"{ClassName}/{Actions.GetAll}";
                public static string GetAllOutlets = $"{ClassName}/{Actions.GetAll}Outlets";
                public static string GetAllInlets = $"{ClassName}/{Actions.GetAll}Inlets";
                public static string GetById = $"{ClassName}/{Actions.GetById}";
                public static string Delete = $"{ClassName}/{Actions.Delete}";
                public static string Export = $"{ClassName}/{Actions.Export}";
                public static string Validate = $"{ClassName}/{Actions.Validate}";
            }
            public static class Cache
            {
                public static string[] Key(Guid Id, Guid MainProcessId) => new[] { GetAll(MainProcessId), GetById(Id) };
                public static string GetAll(Guid MainProcessId) => $"GetAll-{ClassName}-{MainProcessId}";
                public static string GetById(Guid Id) => $"GetById-{ClassName}-{Id}";
            }
            public static class PageName
            {
                public static string Create = $"Create{ClassName}";
                public static string Update = $"Update{ClassName}";
                public static string GetAll = $"GetAll{ClassName}";

            }


        }
        //public static class Washouts
        //{
        //    public static string ClassLegend = "Washout";
        //    public static string ClassName => MethodBase.GetCurrentMethod()!.DeclaringType!.Name;
        //    public static class EndPoint
        //    {
        //        public static string DeleteGroup = $"{ClassName}/{Actions.DeleteGroup}";
        //        public static string CreateUpdate = $"{ClassName}/{Actions.CreateUpdate}";
        //        public static string UpdateUp = $"{ClassName}/{Actions.Update}Up";
        //        public static string UpdateDown = $"{ClassName}/{Actions.Update}Down";
        //        public static string GetAll = $"{ClassName}/{Actions.GetAll}";
        //        public static string GetById = $"{ClassName}/{Actions.GetById}";
        //        public static string Delete = $"{ClassName}/{Actions.Delete}";
        //        public static string Export = $"{ClassName}/{Actions.Export}";
        //        public static string Validate = $"{ClassName}/{Actions.Validate}";
        //    }
        //    public static class Cache
        //    {
        //        public static string[] Key(Guid Id) => new[] { GetAll, GetById(Id) };
        //        public static string GetAll => $"GetAll-{ClassName}";
        //        public static string GetById(Guid Id) => $"GetById-{ClassName}-{Id}";
        //    }
        //    public static class PageName
        //    {
        //        public static string Create = $"Create{ClassName}";
        //        public static string Update = $"Update{ClassName}";
        //        public static string GetAll = $"GetAll{ClassName}";

        //    }


        //}
        public static class Conectors
        {
            public static string ClassLegend = "Conector";
            public static string ClassName => MethodBase.GetCurrentMethod()!.DeclaringType!.Name;
            public static class EndPoint
            {
                public static string DeleteGroup = $"{ClassName}/{Actions.DeleteGroup}";
                public static string CreateUpdateInletConnectors = $"{ClassName}/{Actions.CreateUpdate}Inlet";
                public static string CreateUpdateOuletConnectors = $"{ClassName}/{Actions.CreateUpdate}Outlet";
                public static string UpdateUp = $"{ClassName}/{Actions.Update}Up";
                public static string UpdateDown = $"{ClassName}/{Actions.Update}Down";
                public static string GetAllOutlets = $"{ClassName}/{Actions.GetAll}Outlets";
                public static string GetAllInlets = $"{ClassName}/{Actions.GetAll}Inlets";

                public static string GetInletById = $"{ClassName}/{Actions.GetById}Inlet";
                public static string GetOutletById = $"{ClassName}/{Actions.GetById}Oulet";
                public static string Delete = $"{ClassName}/{Actions.Delete}";
                public static string Export = $"{ClassName}/{Actions.Export}";
                public static string ValidateInlet = $"{ClassName}/{Actions.Validate}Inlet";
                public static string ValidateOutlet = $"{ClassName}/{Actions.Validate}Outlet";
            }
            public static class Cache
            {
                public static string[] KeyInlets(Guid Id, Guid ToId, Guid MainProcessId) => new[] { GetAllInlets(ToId), GetById(Id), GetAll(MainProcessId) };
                public static string[] KeyOutlets(Guid Id, Guid FromId, Guid MainProcessId) => new[] { GetAllOutlets(FromId), GetById(Id), GetAll(MainProcessId) };
                public static string GetAll(Guid MainProcessId) => $"GetAll-{ClassName}-{MainProcessId}";
                public static string GetAllInlets(Guid ToId) => $"GetAllInlet-{ClassName}-{ToId}";
                public static string GetAllOutlets(Guid FromId) => $"GetAllOutlet-{ClassName}-{FromId}";
                public static string GetById(Guid Id) => $"GetById-{ClassName}-{Id}";
            }
            public static class PageName
            {
                public static string Create = $"Create{ClassName}";
                public static string Update = $"Update{ClassName}";
                public static string GetAll = $"GetAll{ClassName}";

            }


        }
        //public static class MainProcesss
        //{
        //    public static string ClassLegend = "MainProcess";
        //    public static string ClassName => MethodBase.GetCurrentMethod()!.DeclaringType!.Name;
        //    public static class EndPoint
        //    {
        //        public static string DeleteGroup = $"{ClassName}/{Actions.DeleteGroup}";
        //        public static string CreateUpdate = $"{ClassName}/{Actions.CreateUpdate}";
        //        public static string UpdateUp = $"{ClassName}/{Actions.Update}Up";
        //        public static string UpdateDown = $"{ClassName}/{Actions.Update}Down";
        //        public static string GetAll = $"{ClassName}/{Actions.GetAll}";
        //        public static string GetById = $"{ClassName}/{Actions.GetById}";
        //        public static string Delete = $"{ClassName}/{Actions.Delete}";
        //        public static string Export = $"{ClassName}/{Actions.Export}";
        //        public static string Validate = $"{ClassName}/{Actions.Validate}";
        //        public static string CopyAndPaste = $"{ClassName}/{Actions.CopyAndPaste}";
        //    }
        //    public static class Cache
        //    {
        //        public static string[] Key(Guid Id) => new[] { GetAll, GetById(Id) };
        //        public static string GetAll => $"GetAll-{ClassName}";
        //        public static string GetById(Guid Id) => $"GetById-{ClassName}-{Id}";
        //    }
        //    public static class PageName
        //    {
        //        public static string Create = $"Create{ClassName}";
        //        public static string Update = $"Update{ClassName}";
        //        public static string GetAll = $"GetAll{ClassName}";

        //    }


        //}

        public static class ContinuousSystems
        {
            public static string ClassLegend = "ContinuousSystem";
            public static string ClassName => MethodBase.GetCurrentMethod()!.DeclaringType!.Name;
            public static class EndPoint
            {
                public static string DeleteGroup = $"{ClassName}/{Actions.DeleteGroup}";
                public static string CreateUpdate = $"{ClassName}/{Actions.CreateUpdate}";
                public static string UpdateUp = $"{ClassName}/{Actions.Update}Up";
                public static string UpdateDown = $"{ClassName}/{Actions.Update}Down";
                public static string GetAll = $"{ClassName}/{Actions.GetAll}";
                public static string GetById = $"{ClassName}/{Actions.GetById}";
                public static string Delete = $"{ClassName}/{Actions.Delete}";
                public static string Export = $"{ClassName}/{Actions.Export}";
                public static string Validate = $"{ClassName}/{Actions.Validate}";
            }
            public static class Cache
            {
                public static string[] Key(Guid Id, Guid MainProcessId) => new[] { GetAll(MainProcessId), BaseEquipments.Cache.GetAll(MainProcessId), GetById(Id) };
                public static string GetAll(Guid MainProcessId) => $"GetAll-{ClassName}-{MainProcessId}";
                public static string GetById(Guid Id) => $"GetById-{ClassName}-{Id}";
            }
            public static class PageName
            {
                public static string Create = $"Create{ClassName}";
                public static string Update = $"Update{ClassName}";
                public static string GetAll = $"GetAll{ClassName}";

            }


        }
        //public static class Lines
        //{
        //    public static string ClassLegend = "Line";
        //    public static string ClassName => MethodBase.GetCurrentMethod()!.DeclaringType!.Name;
        //    public static class EndPoint
        //    {
        //        public static string DeleteGroup = $"{ClassName}/{Actions.DeleteGroup}";
        //        public static string CreateUpdate = $"{ClassName}/{Actions.CreateUpdate}";
        //        public static string UpdateUp = $"{ClassName}/{Actions.Update}Up";
        //        public static string UpdateDown = $"{ClassName}/{Actions.Update}Down";
        //        public static string GetAll = $"{ClassName}/{Actions.GetAll}";
        //        public static string GetById = $"{ClassName}/{Actions.GetById}";
        //        public static string Delete = $"{ClassName}/{Actions.Delete}";
        //        public static string Export = $"{ClassName}/{Actions.Export}";
        //        public static string Validate = $"{ClassName}/{Actions.Validate}";
        //    }
        //    public static class Cache
        //    {
        //        public static string[] Key(Guid Id, Guid MainProcessId) => new[] { GetAll(MainProcessId), BaseEquipments.Cache.GetAll(MainProcessId), GetById(Id) };
        //        public static string GetAll(Guid MainProcessId) => $"GetAll-{ClassName}-{MainProcessId}";
        //        public static string GetById(Guid Id) => $"GetById-{ClassName}-{Id}";
        //    }
        //    public static class PageName
        //    {
        //        public static string Create = $"Create{ClassName}";
        //        public static string Update = $"Update{ClassName}";
        //        public static string GetAll = $"GetAll{ClassName}";

        //    }


        //}
        public static class Mixers
        {
            public static string ClassLegend = "Mixer";
            public static string ClassName => MethodBase.GetCurrentMethod()!.DeclaringType!.Name;
            public static class EndPoint
            {
                public static string DeleteGroup = $"{ClassName}/{Actions.DeleteGroup}";
                public static string CreateUpdate = $"{ClassName}/{Actions.CreateUpdate}";
                public static string UpdateUp = $"{ClassName}/{Actions.Update}Up";
                public static string UpdateDown = $"{ClassName}/{Actions.Update}Down";
                public static string GetAll = $"{ClassName}/{Actions.GetAll}";
                public static string GetById = $"{ClassName}/{Actions.GetById}";
                public static string Delete = $"{ClassName}/{Actions.Delete}";
                public static string Export = $"{ClassName}/{Actions.Export}";
                public static string Validate = $"{ClassName}/{Actions.Validate}";
            }
            public static class Cache
            {
                public static string[] Key(Guid Id, Guid MainProcessId) => new[] { GetAll(MainProcessId), BaseEquipments.Cache.GetAll(MainProcessId), GetById(Id) };
                public static string GetAll(Guid MainProcessId) => $"GetAll-{ClassName}-{MainProcessId}";
                public static string GetById(Guid Id) => $"GetById-{ClassName}-{Id}";
            }
            public static class PageName
            {
                public static string Create = $"Create{ClassName}";
                public static string Update = $"Update{ClassName}";
                public static string GetAll = $"GetAll{ClassName}";

            }


        }
        public static class Pumps
        {
            public static string ClassLegend = "Pump";
            public static string ClassName => MethodBase.GetCurrentMethod()!.DeclaringType!.Name;
            public static class EndPoint
            {
                public static string DeleteGroup = $"{ClassName}/{Actions.DeleteGroup}";
                public static string CreateUpdate = $"{ClassName}/{Actions.CreateUpdate}";
                public static string UpdateUp = $"{ClassName}/{Actions.Update}Up";
                public static string UpdateDown = $"{ClassName}/{Actions.Update}Down";
                public static string GetAll = $"{ClassName}/{Actions.GetAll}";
                public static string GetById = $"{ClassName}/{Actions.GetById}";
                public static string Delete = $"{ClassName}/{Actions.Delete}";
                public static string Export = $"{ClassName}/{Actions.Export}";
                public static string Validate = $"{ClassName}/{Actions.Validate}";
            }
            public static class Cache
            {
                public static string[] Key(Guid Id, Guid MainProcessId) => new[] { GetAll(MainProcessId), BaseEquipments.Cache.GetAll(MainProcessId), GetById(Id) };
                public static string GetAll(Guid MainProcessId) => $"GetAll-{ClassName}-{MainProcessId}";
                public static string GetById(Guid Id) => $"GetById-{ClassName}-{Id}";
            }
            public static class PageName
            {
                public static string Create = $"Create{ClassName}";
                public static string Update = $"Update{ClassName}";
                public static string GetAll = $"GetAll{ClassName}";

            }


        }
        public static class StreamJoiners
        {
            public static string ClassLegend = "StreamJoiners";
            public static string ClassName => MethodBase.GetCurrentMethod()!.DeclaringType!.Name;
            public static class EndPoint
            {
                public static string DeleteGroup = $"{ClassName}/{Actions.DeleteGroup}";
                public static string CreateUpdate = $"{ClassName}/{Actions.CreateUpdate}";
                public static string UpdateUp = $"{ClassName}/{Actions.Update}Up";
                public static string UpdateDown = $"{ClassName}/{Actions.Update}Down";
                public static string GetAll = $"{ClassName}/{Actions.GetAll}";
                public static string GetById = $"{ClassName}/{Actions.GetById}";
                public static string Delete = $"{ClassName}/{Actions.Delete}";
                public static string Export = $"{ClassName}/{Actions.Export}";
                public static string Validate = $"{ClassName}/{Actions.Validate}";
            }
            public static class Cache
            {
                public static string[] Key(Guid Id, Guid MainProcessId) => new[] { GetAll(MainProcessId), BaseEquipments.Cache.GetAll(MainProcessId), GetById(Id) };
                public static string GetAll(Guid MainProcessId) => $"GetAll-{ClassName}-{MainProcessId}";
                public static string GetById(Guid Id) => $"GetById-{ClassName}-{Id}";
            }
            public static class PageName
            {
                public static string Create = $"Create{ClassName}";
                public static string Update = $"Update{ClassName}";
                public static string GetAll = $"GetAll{ClassName}";

            }


        }
        public static class Operators
        {
            public static string ClassLegend = "Operator";
            public static string ClassName => MethodBase.GetCurrentMethod()!.DeclaringType!.Name;
            public static class EndPoint
            {
                public static string DeleteGroup = $"{ClassName}/{Actions.DeleteGroup}";
                public static string CreateUpdate = $"{ClassName}/{Actions.CreateUpdate}";
                public static string UpdateUp = $"{ClassName}/{Actions.Update}Up";
                public static string UpdateDown = $"{ClassName}/{Actions.Update}Down";
                public static string GetAll = $"{ClassName}/{Actions.GetAll}";
                public static string GetById = $"{ClassName}/{Actions.GetById}";
                public static string Delete = $"{ClassName}/{Actions.Delete}";
                public static string Export = $"{ClassName}/{Actions.Export}";
                public static string Validate = $"{ClassName}/{Actions.Validate}";
            }
            public static class Cache
            {
                public static string[] Key(Guid Id, Guid MainProcessId) => new[] { GetAll(MainProcessId), BaseEquipments.Cache.GetAll(MainProcessId), GetById(Id) };
                public static string GetAll(Guid MainProcessId) => $"GetAll-{ClassName}-{MainProcessId}";
                public static string GetById(Guid Id) => $"GetById-{ClassName}-{Id}";
            }
            public static class PageName
            {
                public static string Create = $"Create{ClassName}";
                public static string Update = $"Update{ClassName}";
                public static string GetAll = $"GetAll{ClassName}";

            }


        }
        public static class Tanks
        {
            public static string ClassLegend = "Tank";
            public static string ClassName => MethodBase.GetCurrentMethod()!.DeclaringType!.Name;
            public static class EndPoint
            {
                public static string DeleteGroup = $"{ClassName}/{Actions.DeleteGroup}";
                public static string CreateUpdate = $"{ClassName}/{Actions.CreateUpdate}";
                public static string UpdateUp = $"{ClassName}/{Actions.Update}Up";
                public static string UpdateDown = $"{ClassName}/{Actions.Update}Down";
                public static string GetAll = $"{ClassName}/{Actions.GetAll}";
                public static string GetById = $"{ClassName}/{Actions.GetById}";
                public static string Delete = $"{ClassName}/{Actions.Delete}";
                public static string Export = $"{ClassName}/{Actions.Export}";
                public static string Validate = $"{ClassName}/{Actions.Validate}";
            }
            public static class Cache
            {
                public static string[] Key(Guid Id, Guid MainProcessId) => new[] { GetAll(MainProcessId), BaseEquipments.Cache.GetAll(MainProcessId), GetById(Id) };
                public static string GetAll(Guid MainProcessId) => $"GetAll-{ClassName}-{MainProcessId}";
                public static string GetById(Guid Id) => $"GetById-{ClassName}-{Id}";
            }
            public static class PageName
            {
                public static string Create = $"Create{ClassName}";
                public static string Update = $"Update{ClassName}";
                public static string GetAll = $"GetAll{ClassName}";

            }


        }
        public static class MaterialEquipments
        {
            public static string ClassLegend = "MaterialEquipment";
            public static string ClassName => MethodBase.GetCurrentMethod()!.DeclaringType!.Name;
            public static class EndPoint
            {
                public static string DeleteGroup = $"{ClassName}/{Actions.DeleteGroup}";
                public static string CreateUpdate = $"{ClassName}/{Actions.CreateUpdate}";
                public static string UpdateUp = $"{ClassName}/{Actions.Update}Up";
                public static string UpdateDown = $"{ClassName}/{Actions.Update}Down";
                public static string GetAll = $"{ClassName}/{Actions.GetAll}";
                public static string GetById = $"{ClassName}/{Actions.GetById}";
                public static string Delete = $"{ClassName}/{Actions.Delete}";
                public static string Export = $"{ClassName}/{Actions.Export}";
                public static string Validate = $"{ClassName}/{Actions.Validate}";
            }
            public static class Cache
            {
                public static string[] Key(Guid Id, Guid EquipmentId, Guid MainProcessId) => new[] { GetAllByMaterial(EquipmentId), GetAll(MainProcessId), GetById(Id) };
                public static string GetAllByMaterial(Guid EquipmentId) => $"GetAll-{ClassName}-{EquipmentId}";
                public static string GetAll(Guid MainProcessId) => $"GetAll-{ClassName}-{MainProcessId}";
                public static string GetById(Guid Id) => $"GetById-{ClassName}-{Id}";
            }
            public static class PageName
            {
                public static string Create = $"Create{ClassName}";
                public static string Update = $"Update{ClassName}";
                public static string GetAll = $"GetAll{ClassName}";

            }


        }
        public static class SKULines
        {
            public static string ClassLegend = "SKULine";
            public static string ClassName => MethodBase.GetCurrentMethod()!.DeclaringType!.Name;
            public static class EndPoint
            {
                public static string DeleteGroup = $"{ClassName}/{Actions.DeleteGroup}";
                public static string CreateUpdate = $"{ClassName}/{Actions.CreateUpdate}";
                public static string UpdateUp = $"{ClassName}/{Actions.Update}Up";
                public static string UpdateDown = $"{ClassName}/{Actions.Update}Down";
                public static string GetAll = $"{ClassName}/{Actions.GetAll}";
                public static string GetById = $"{ClassName}/{Actions.GetById}";
                public static string Delete = $"{ClassName}/{Actions.Delete}";
                public static string Export = $"{ClassName}/{Actions.Export}";
                public static string Validate = $"{ClassName}/{Actions.Validate}";
            }
            public static class Cache
            {
                public static string[] Key(Guid Id, Guid LineId) => new[] { GetAll(LineId), GetById(Id) };
                public static string GetAll(Guid LineId) => $"GetAll-{ClassName}-{LineId}";
                public static string GetById(Guid Id) => $"GetById-{ClassName}-{Id}";
            }
            public static class PageName
            {
                public static string Create = $"Create{ClassName}";
                public static string Update = $"Update{ClassName}";
                public static string GetAll = $"GetAll{ClassName}";

            }


        }
        public static class EquipmentPlannedDownTimes
        {
            public static string ClassLegend = "EquipmentPlannedDownTime";
            public static string ClassName => MethodBase.GetCurrentMethod()!.DeclaringType!.Name;
            public static class EndPoint
            {
                public static string DeleteGroup = $"{ClassName}/{Actions.DeleteGroup}";
                public static string CreateUpdate = $"{ClassName}/{Actions.CreateUpdate}";
                public static string UpdateUp = $"{ClassName}/{Actions.Update}Up";
                public static string UpdateDown = $"{ClassName}/{Actions.Update}Down";
                public static string GetAll = $"{ClassName}/{Actions.GetAll}";
                public static string GetById = $"{ClassName}/{Actions.GetById}";
                public static string Delete = $"{ClassName}/{Actions.Delete}";
                public static string Export = $"{ClassName}/{Actions.Export}";
                public static string Validate = $"{ClassName}/{Actions.Validate}";
            }
            public static class Cache
            {
                public static string[] Key(Guid Id, Guid EquipmentId) => new[] { GetAll(EquipmentId), GetById(Id) };
                public static string GetAll(Guid EquipmentId) => $"GetAll-{ClassName}-{EquipmentId}";
                public static string GetById(Guid Id) => $"GetById-{ClassName}-{Id}";
            }
            public static class PageName
            {
                public static string Create = $"Create{ClassName}";
                public static string Update = $"Update{ClassName}";
                public static string GetAll = $"GetAll{ClassName}";

            }


        }
        public static class SimulationPlanneds
        {
            public static string ClassLegend = "SimulationPlanned";
            public static string ClassName => MethodBase.GetCurrentMethod()!.DeclaringType!.Name;
            public static class EndPoint
            {
                public static string DeleteGroup = $"{ClassName}/{Actions.DeleteGroup}";
                public static string CreateUpdate = $"{ClassName}/{Actions.CreateUpdate}";
                public static string UpdateUp = $"{ClassName}/{Actions.Update}Up";
                public static string UpdateDown = $"{ClassName}/{Actions.Update}Down";
                public static string GetAll = $"{ClassName}/{Actions.GetAll}";
                public static string GetById = $"{ClassName}/{Actions.GetById}";
                public static string Delete = $"{ClassName}/{Actions.Delete}";
                public static string Export = $"{ClassName}/{Actions.Export}";
                public static string Validate = $"{ClassName}/{Actions.Validate}";
                public static string GetProcess = $"{ClassName}/{Actions.GetById}GetProcess";
                public static string GetPlanned = $"{ClassName}/{Actions.GetById}GetPlanned";
            }
            public static class Cache
            {
                public static string[] Key(Guid Id, Guid MainProcessId) => new[] { GetAll(MainProcessId), GetById(Id) };
                public static string GetAll(Guid MainProcessId) => $"GetAll-{ClassName}-{MainProcessId}";
                public static string GetById(Guid Id) => $"GetById-{ClassName}-{Id}";
            }
            public static class PageName
            {
                public static string Create = $"Create{ClassName}";
                public static string Update = $"Update{ClassName}";
                public static string GetAll = $"GetAll{ClassName}";

            }


        }
        public static class LinePlanneds
        {
            public static string ClassLegend = "LinePlanned";
            public static string ClassName => MethodBase.GetCurrentMethod()!.DeclaringType!.Name;
            public static class EndPoint
            {
                public static string DeleteGroup = $"{ClassName}/{Actions.DeleteGroup}";
                public static string CreateUpdate = $"{ClassName}/{Actions.CreateUpdate}";
                public static string UpdateUp = $"{ClassName}/{Actions.Update}Up";
                public static string UpdateDown = $"{ClassName}/{Actions.Update}Down";
                public static string GetAll = $"{ClassName}/{Actions.GetAll}";
                public static string GetById = $"{ClassName}/{Actions.GetById}";
                public static string Delete = $"{ClassName}/{Actions.Delete}";
                public static string Export = $"{ClassName}/{Actions.Export}";
                public static string Validate = $"{ClassName}/{Actions.Validate}";
            }
            public static class Cache
            {
                public static string[] Key(Guid Id, Guid SimulationPlannedId) => new[] { GetAll(SimulationPlannedId), GetById(Id) };
                public static string GetAll(Guid SimulationPlannedId) => $"GetAll-{ClassName}-{SimulationPlannedId}";
                public static string GetById(Guid Id) => $"GetById-{ClassName}-{Id}";
            }
            public static class PageName
            {
                public static string Create = $"Create{ClassName}";
                public static string Update = $"Update{ClassName}";
                public static string GetAll = $"GetAll{ClassName}";

            }


        }
        public static class MixerPlanneds
        {
            public static string ClassLegend = "MixerPlanned";
            public static string ClassName => MethodBase.GetCurrentMethod()!.DeclaringType!.Name;
            public static class EndPoint
            {
                public static string DeleteGroup = $"{ClassName}/{Actions.DeleteGroup}";
                public static string CreateUpdate = $"{ClassName}/{Actions.CreateUpdate}";
                public static string UpdateUp = $"{ClassName}/{Actions.Update}Up";
                public static string UpdateDown = $"{ClassName}/{Actions.Update}Down";
                public static string GetAll = $"{ClassName}/{Actions.GetAll}";
                public static string GetById = $"{ClassName}/{Actions.GetById}";
                public static string Delete = $"{ClassName}/{Actions.Delete}";
                public static string Export = $"{ClassName}/{Actions.Export}";
                public static string Validate = $"{ClassName}/{Actions.Validate}";
            }
            public static class Cache
            {
                public static string[] Key(Guid Id, Guid SimulationPlannedId) => new[] { GetAll(SimulationPlannedId), GetById(Id) };
                public static string GetAll(Guid SimulationPlannedId) => $"GetAll-{ClassName}-{SimulationPlannedId}";
                public static string GetById(Guid Id) => $"GetById-{ClassName}-{Id}";
            }
            public static class PageName
            {
                public static string Create = $"Create{ClassName}";
                public static string Update = $"Update{ClassName}";
                public static string GetAll = $"GetAll{ClassName}";

            }


        }
        public static class PlannedSKUs
        {
            public static string ClassLegend = "Material";
            public static string ClassName => MethodBase.GetCurrentMethod()!.DeclaringType!.Name;
            public static class EndPoint
            {
                public static string DeleteGroup = $"{ClassName}/{Actions.DeleteGroup}";
                public static string CreateUpdate = $"{ClassName}/{Actions.CreateUpdate}";
                public static string UpdateUp = $"{ClassName}/{Actions.Update}Up";
                public static string UpdateDown = $"{ClassName}/{Actions.Update}Down";
                public static string GetAll = $"{ClassName}/{Actions.GetAll}";
                public static string GetById = $"{ClassName}/{Actions.GetById}";
                public static string Delete = $"{ClassName}/{Actions.Delete}";
                public static string Export = $"{ClassName}/{Actions.Export}";
                public static string Validate = $"{ClassName}/{Actions.Validate}";
            }
            public static class Cache
            {
                public static string[] Key(Guid Id, Guid PlanedLineId) => new[] { GetAll(PlanedLineId), GetById(Id) };
                public static string GetAll(Guid PlanedLineId) => $"GetAll-{ClassName}-{PlanedLineId}";
                public static string GetById(Guid Id) => $"GetById-{ClassName}-{Id}";
            }
            public static class PageName
            {
                public static string Create = $"Create{ClassName}";
                public static string Update = $"Update{ClassName}";
                public static string GetAll = $"GetAll{ClassName}";

            }


        }
        public static class PreferedMixers
        {
            public static string ClassLegend = "PreferedMixers";
            public static string ClassName => MethodBase.GetCurrentMethod()!.DeclaringType!.Name;
            public static class EndPoint
            {
                public static string DeleteGroup = $"{ClassName}/{Actions.DeleteGroup}";
                public static string CreateUpdate = $"{ClassName}/{Actions.CreateUpdate}";
                public static string UpdateUp = $"{ClassName}/{Actions.Update}Up";
                public static string UpdateDown = $"{ClassName}/{Actions.Update}Down";
                public static string GetAll = $"{ClassName}/{Actions.GetAll}";
                public static string GetById = $"{ClassName}/{Actions.GetById}";
                public static string Delete = $"{ClassName}/{Actions.Delete}";
                public static string Export = $"{ClassName}/{Actions.Export}";
                public static string Validate = $"{ClassName}/{Actions.Validate}";
            }
            public static class Cache
            {
                public static string[] Key(Guid Id, Guid PlanedLineId) => new[] { GetAll(PlanedLineId), GetById(Id) };
                public static string GetAll(Guid PlanedLineId) => $"GetAll-{ClassName}-{PlanedLineId}";
                public static string GetById(Guid Id) => $"GetById-{ClassName}-{Id}";
            }
            public static class PageName
            {
                public static string Create = $"Create{ClassName}";
                public static string Update = $"Update{ClassName}";
                public static string GetAll = $"GetAll{ClassName}";

            }


        }

    }
}
