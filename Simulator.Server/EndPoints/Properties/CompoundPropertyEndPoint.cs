using Simulator.Server.Databases.Entities.Equilibrio;
using Simulator.Shared.Models.CompoundProperties;
using Simulator.Shared.NewModels.Compounds;

namespace Simulator.Server.EndPoints.Properties
{
    public class CompoundPropertyEndpoint : IEndPoint
    {

        public void MapEndPoint(IEndpointRouteBuilder app)
        {
            app.MapPost("api/save/NewCompoundPropertyDTO", async (NewCompoundPropertyDTO dto, IServerCrudService service) =>
            {

                var result = await service.SaveAsync<CompoundProperty>(dto);

                if (result == 0)
                {
                    return Result.Fail("Something went wrong");
                }


                return Result.Success("Updated");

            });
            app.MapPost("api/delete/NewCompoundPropertyDTO", async (NewCompoundPropertyDTO dto, IServerCrudService service) =>
            {
                var result = await service.DeleteAsync<CompoundProperty>(dto);
                if (result == 0)
                {
                    return Result.Fail("Something went wrong");
                }


                return Result.Success("Deleted");
            });
            app.MapPost("api/getall/NewCompoundPropertyDTO", async (NewCompoundPropertyDTO dto, IServerCrudService service) =>
            {
                var query = await service.GetAllAsync<CompoundProperty>(dto);

                var result = query.Select(x => x.MapToDto<NewCompoundPropertyDTO>()).ToList();

                return Result.Success(result);
            });

            // GetById
            app.MapPost("api/getbyid/NewCompoundPropertyDTO", async (NewCompoundPropertyDTO dto, IServerCrudService service) =>
            {

                var query = await service.GetById<CompoundProperty>(dto);

                if (query == null)
                {
                    return Result.Fail("Not Found");
                }

                var result = query.MapToDto<NewCompoundPropertyDTO>();
                return Result.Success(result);
            });
        }

    }




    public static class CreateUpdateCompoundPropertyEndPoint
    {



        public static CompoundProperty Map(this CompoundPropertyDTO request, CompoundProperty row)
        {
            row.Name = request.Name;
            row.Formula = request.Formula;
            row.StructuralFormula = request.StructuralFormula;
            row.MainFamily = request.MainFamily;
            row.SecondaryFamily = request.SecondaryFamily;
            row.MolecularWeight = request.MolecularWeight;
            row.Critical_Z = request.Critical_Z;
            row.Acentric_Factor = request.Acentric_Factor;
            row.Acentric_Factor_SRK = request.Acentric_Factor_SRK;
            row.Critical_Temperature = request.Critical_Temperature.Value;
            row.Critical_Temperature_Unit = request.Critical_Temperature.UnitName;
            row.Critical_Pressure = request.Critical_Pressure.Value;
            row.Critical_Pressure_Unit = request.Critical_Pressure.UnitName;
            row.Critical_Volume = request.Critical_Volume.Value;
            row.Critical_Volume_Unit = request.Critical_Volume.UnitName;
            row.Boiling_Temperature = request.Boiling_Temperature.Value;
            row.Boiling_Temperature_Unit = request.Boiling_Temperature.UnitName;
            row.Melting_Temperature = request.Melting_Temperature.Value;
            row.Melting_Temperature_Unit = request.Melting_Temperature.UnitName;
            row.Asterisk_Volume = request.Asterisk_Volume.Value;
            row.Asterisk_Volume_Unit = request.Asterisk_Volume.UnitName;
            row.Gibbs_Energy_Formation = request.Gibbs_Energy_Formation.Value;
            row.Gibbs_Energy_Formation_Unit = request.Gibbs_Energy_Formation.UnitName;
            row.Enthalpy_Formation = request.Enthalpy_Formation.Value;
            row.Enthalpy_Formation_Unit = request.Enthalpy_Formation.UnitName;
            row.Entropy_Formation = request.Entropy_Formation.Value;
            row.Entropy_Formation_Unit = request.Entropy_Formation.UnitName;
            row.Enthalpy_Combustion = request.Enthalpy_Combustion.Value;
            row.Enthalpy_Combustion_Unit = request.Enthalpy_Combustion.UnitName;

            row.VapourPressure = request.VapourPressure.Map(row.VapourPressure);
            row.HeatOfVaporization = request.HeatOfVaporization.Map(row.HeatOfVaporization);
            row.LiquidCp = request.LiquidCp.Map(row.LiquidCp);
            row.GasCp = request.GasCp.Map(row.GasCp);
            row.LiquidViscosity = request.LiquidViscosity.Map(row.LiquidViscosity);
            row.GasViscosity = request.GasViscosity.Map(row.GasViscosity);
            row.LiquidThermalConductivity = request.LiquidThermalConductivity.Map(row.LiquidThermalConductivity);
            row.GasThermalConductivity = request.GasThermalConductivity.Map(row.GasThermalConductivity);
            row.LiquidDensity = request.LiquidDensity.Map(row.LiquidDensity);
            row.SuperficialTension = request.SuperficialTension.Map(row.SuperficialTension);

            return row;
        }
        public static CompoundConstant Map(this CompoundConstantDTO request, CompoundConstant row)
        {
            row.C1 = request.C1;
            row.C2 = request.C2;
            row.C3 = request.C3;
            row.C4 = request.C4;
            row.C5 = request.C5;
            row.C6 = request.C6;
            row.C7 = request.C7;
            row.Maximum_Temperature = request.Maximum_Temperature.Value;
            row.Maximum_Temperature_Unit = request.Maximum_Temperature.UnitName;
            row.Minimal_Temperature = request.Minimal_Temperature.Value;
            row.Minimal_Temperature_Unit = request.Minimal_Temperature.UnitName;
            return row;
        }
        public static CompoundConstantDTO Map(this CompoundConstant row)
        {
            var result = new CompoundConstantDTO()
            {

                C1 = row.C1,
                C2 = row.C2,
                C3 = row.C3,
                C4 = row.C4,
                C5 = row.C5,
                C6 = row.C6,
                C7 = row.C7,

            };
            result.Minimal_Temperature_Unit = row.Minimal_Temperature_Unit;
            result.Maximum_Temperature_Unit = row.Maximum_Temperature_Unit;
            result.Maximum_Temperature_Value = row.Maximum_Temperature;

            result.Minimal_Temperature_Value = row.Minimal_Temperature;


            return result;
        }
    }
    //}
    //public static class DeleteCompoundPropertyEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.CompoundPropertys.EndPoint.Delete, async (DeleteCompoundPropertyRequest Data, IRepository Repository) =>
    //            {
    //                var row = await Repository.GetByIdAsync<CompoundProperty>(Data.Id);
    //                if (row == null) { return Result.Fail(Data.NotFound); }
    //                await Repository.RemoveAsync(row);

    //                List<string> cache = [.. StaticClass.CompoundPropertys.Cache.Key(row.Id)];

    //                var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache.ToArray());
    //                return Result.EndPointResult(result,
    //                    Data.Succesfully,
    //                    Data.Fail);

    //            });
    //        }
    //    }




    //}
    //public static class DeleteGroupCompoundPropertyEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.CompoundPropertys.EndPoint.DeleteGroup, async (DeleteGroupCompoundPropertyRequest Data, IRepository Repository) =>
    //            {
    //                foreach (var rowItem in Data.SelecteItems)
    //                {
    //                    var row = await Repository.GetByIdAsync<CompoundProperty>(rowItem.Id);
    //                    if (row != null)
    //                    {
    //                        await Repository.RemoveAsync(row);
    //                    }
    //                }


    //                var cache = StaticClass.CompoundPropertys.Cache.GetAll;

    //                var result = await Repository.Context.SaveChangesAndRemoveCacheAsync(cache);
    //                return Result.EndPointResult(result,
    //                    Data.Succesfully,
    //                    Data.Fail);

    //            });
    //        }
    //    }




    //}
    //public static class GetAllCompoundPropertyEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.CompoundPropertys.EndPoint.GetAll, async (CompoundPropertyGetAll request, IQueryRepository Repository) =>
    //            {


    //                string CacheKey = StaticClass.CompoundPropertys.Cache.GetAll;

    //                var rows = await Repository.GetAllAsync<CompoundProperty>(Cache: CacheKey);

    //                if (rows == null)
    //                {
    //                    return Result<CompoundPropertyResponseList>.Fail(
    //                    StaticClass.ResponseMessages.ReponseNotFound(StaticClass.CompoundPropertys.ClassLegend));
    //                }

    //                var maps = rows.OrderBy(x => x.Order).Select(x => x.Map()).ToList();


    //                CompoundPropertyResponseList response = new CompoundPropertyResponseList()
    //                {
    //                    Items = maps
    //                };
    //                return Result<CompoundPropertyResponseList>.Success(response);

    //            });
    //        }
    //    }
    //}
    //public static class GetCompoundPropertyByIdEndPoint
    //{
    //    public class EndPoint : IEndPoint
    //    {
    //        public void MapEndPoint(IEndpointRouteBuilder app)
    //        {
    //            app.MapPost(StaticClass.CompoundPropertys.EndPoint.GetById, async (GetCompoundPropertyByIdRequest request, IQueryRepository Repository) =>
    //            {
    //                Func<IQueryable<CompoundProperty>, IIncludableQueryable<CompoundProperty, object>> includes = x => x
    //                .Include(y => y.VapourPressure)
    //                .Include(y => y.HeatOfVaporization)
    //                .Include(y => y.LiquidCp)
    //                .Include(y => y.GasCp)
    //                .Include(y => y.LiquidViscosity)
    //                .Include(y => y.GasViscosity)
    //                .Include(y => y.LiquidThermalConductivity)
    //                .Include(y => y.GasThermalConductivity)
    //                .Include(y => y.LiquidDensity)
    //                .Include(y => y.SuperficialTension);
    //                Expression<Func<CompoundProperty, bool>> Criteria = x => x.Id == request.Id;

    //                string CacheKey = StaticClass.CompoundPropertys.Cache.GetById(request.Id);
    //                var row = await Repository.GetAsync(Cache: CacheKey, Criteria: Criteria, Includes: includes);

    //                if (row == null)
    //                {
    //                    return Result.Fail(request.NotFound);
    //                }

    //                var response = row.Map();
    //                return Result.Success(response);

    //            });
    //        }
    //    }

    //    public static CompoundPropertyDTO Map(this CompoundProperty row)
    //    {
    //        var result = new CompoundPropertyDTO()
    //        {
    //            Id = row.Id,
    //            Acentric_Factor = row.Acentric_Factor,
    //            Acentric_Factor_SRK = row.Acentric_Factor_SRK,
    //            Formula = row.Formula,
    //            StructuralFormula = row.StructuralFormula,
    //            MainFamily = row.MainFamily,
    //            SecondaryFamily = row.SecondaryFamily,
    //            MolecularWeight = row.MolecularWeight,
    //            Name = row.Name,
    //            Critical_Z = row.Critical_Z,




    //        };
    //        result.Critical_Temperature_Unit = row.Critical_Temperature_Unit;
    //        result.Critical_Temperature_Value = row.Critical_Temperature;

    //        result.Critical_Pressure_Unit = row.Critical_Pressure_Unit;
    //        result.Critical_Pressure_Value = row.Critical_Pressure;

    //        result.Critical_Volume_Unit = row.Critical_Volume_Unit;
    //        result.Critical_Volume_Value = row.Critical_Volume;

    //        result.Boiling_Temperature_Unit = row.Boiling_Temperature_Unit;
    //        result.Boiling_Temperature_Value = row.Boiling_Temperature;

    //        result.Melting_Temperature_Unit = row.Melting_Temperature_Unit;
    //        result.Melting_Temperature_Value = row.Melting_Temperature;

    //        result.Asterisk_Volume_Unit = row.Asterisk_Volume_Unit;
    //        result.Asterisk_Volume_Value = row.Asterisk_Volume;

    //        result.Gibbs_Energy_Formation_Unit = row.Gibbs_Energy_Formation_Unit;
    //        result.Gibbs_Energy_Formation_Value = row.Gibbs_Energy_Formation;

    //        result.Enthalpy_Combustion_Unit = row.Enthalpy_Combustion_Unit;
    //        result.Enthalpy_Combustion_Value = row.Enthalpy_Combustion;
    //        result.Enthalpy_Formation_Unit = row.Enthalpy_Formation_Unit;
    //        result.Enthalpy_Formation_Value = row.Enthalpy_Formation;
    //        result.Entropy_Formation_Unit = row.Entropy_Formation_Unit;
    //        result.Entropy_Formation_Value = row.Entropy_Formation;


    //        result.VapourPressure = row.VapourPressure == null ? new() : row.VapourPressure.Map();
    //        result.HeatOfVaporization = row.HeatOfVaporization == null ? new() : row.HeatOfVaporization.Map();
    //        result.LiquidCp = row.LiquidCp == null ? new() : row.LiquidCp.Map();
    //        result.GasCp = row.GasCp == null ? new() : row.GasCp.Map();
    //        result.LiquidViscosity = row.LiquidViscosity == null ? new() : row.LiquidViscosity.Map();
    //        result.GasViscosity = row.GasViscosity == null ? new() : row.GasViscosity.Map();
    //        result.LiquidThermalConductivity = row.LiquidThermalConductivity == null ? new() : row.LiquidThermalConductivity.Map();
    //        result.GasThermalConductivity = row.GasThermalConductivity == null ? new() : row.GasThermalConductivity.Map();
    //        result.LiquidDensity = row.LiquidDensity == null ? new() : row.LiquidDensity.Map();
    //        result.SuperficialTension = row.SuperficialTension == null ? new() : row.SuperficialTension.Map();
    //        return result;
    //    }
    //    public static CompoundConstantDTO Map(this CompoundConstant row)
    //    {
    //        var result = new CompoundConstantDTO()
    //        {

    //            C1 = row.C1,
    //            C2 = row.C2,
    //            C3 = row.C3,
    //            C4 = row.C4,
    //            C5 = row.C5,
    //            C6 = row.C6,
    //            C7 = row.C7,

    //        };
    //        result.Minimal_Temperature_Unit = row.Minimal_Temperature_Unit;
    //        result.Maximum_Temperature_Unit = row.Maximum_Temperature_Unit;
    //        result.Maximum_Temperature_Value = row.Maximum_Temperature;

    //        result.Minimal_Temperature_Value = row.Minimal_Temperature;


    //        return result;
    //    }

    //}
}

