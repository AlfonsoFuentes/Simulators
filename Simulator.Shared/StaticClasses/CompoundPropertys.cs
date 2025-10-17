using System.Reflection;

namespace Simulator.Shared.StaticClasses
{
    public static partial class StaticClass
    {
        public static class CompoundPropertys
        {
            public static string ClassLegend = "CompoundProperty";
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

    }
}
