using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Models.HCs.MaterialEquipments;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Mixers;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Operators;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Pumps;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Skids;
using Simulator.Shared.NuevaSimlationconQwen.Materials;

namespace Simulator.Shared.NuevaSimlationconQwen
{

    public class ManufacturingSystemAnalizer
    {
        GeneralSimulation GeneralSimulation { get; set; } = null!;
        public ManufacturingSystemAnalizer(GeneralSimulation GeneralSimulation)
        {
            this.GeneralSimulation = GeneralSimulation;
        }
        List<ManufaturingEquipment> ManufacturingEquipments => GeneralSimulation.Equipments.OfType<ManufaturingEquipment>().ToList();
        public void Analyze(List<MaterialEquipmentRecord> processEquipmentMaterials)
        {
            foreach (var equipment in ManufacturingEquipments)
            {
                ManufacturingAnalysisResult analisisResult = new();
                equipment.AnalysisResult = analisisResult;

                foreach (var recipedmaterial in equipment.RecipedMaterials)
                {
                    var materialequipmentrecord = processEquipmentMaterials
                        .FirstOrDefault(x => x.EquipmentId == equipment.Id && x.MaterialId == recipedmaterial.Material.Id);
                    if (materialequipmentrecord == null)
                    {
                        GeneralSimulation.AddWarningMessageServices(string.Format("Material {0} in Equipment {1} is not configured in Material Equipments",
                            recipedmaterial.Material.CommonName, equipment.Name));
                        continue;
                    }
                    var recipesteps = ((IRecipedMaterial)recipedmaterial.Material).RecipeSteps;
                    var recipedMaterials = recipesteps.Where(x => x.BackBoneStepType == BackBoneStepType.Add).ToList();
                    foreach (var step in recipedMaterials)
                    {
                        if (!step.RawMaterialId.HasValue)
                        {

                            continue;
                        }
                        var rawmaterial = GeneralSimulation.Materials.FirstOrDefault(x => x.Id == step.RawMaterialId.Value);
                        var inletfeeder = equipment.InletEquipments.OfType<IManufactureFeeder>()
                            .FirstOrDefault(x => x.EquipmentMaterials.Any(x => x.Material.Id == step.RawMaterialId));
                        step.RawMaterialName = rawmaterial?.CommonName ?? "Unknown";
                        if (inletfeeder is ProcessPump pump)
                        {

                            step.Flow = pump.Flow;
                            analisisResult.AddReport(ManufacturingReportType.MaterialConnected, rawmaterial!, equipment);

                            continue;
                        }

                        if (inletfeeder is ProcessOperator)
                        {
                            step.Flow = new Amount(1, MassFlowUnits.Kg_sg);
                            analisisResult.AddReport(ManufacturingReportType.MaterialConnected, rawmaterial!, equipment);

                            continue;
                        }
                        if (inletfeeder == null)
                        {
                            analisisResult.AddReport(ManufacturingReportType.MaterialNotConnected, rawmaterial!, equipment);
                        }



                    }
                    if (equipment.AnalysisResult.GetNotConnectedMaterials().Count > 0)
                    {
                        analisisResult.AddReport(ManufacturingReportType.ProductCannotProduce, recipedmaterial.Material!, equipment);


                    }
                    else
                    {
                        if (equipment is ProcessMixer mixer)
                        {
                            if (materialequipmentrecord.Capacity.Value <= 0)
                            {
                                analisisResult.AddReport(ManufacturingReportType.ProductCannotProduce, recipedmaterial.Material!, equipment);

                            }
                            else
                            {
                                ProcessPump? outletpump = mixer.OutletPump;
                                Amount transfertime = new Amount(0, TimeUnits.Minute);
                                if (outletpump?.Flow != null)
                                {
                                    var time = materialequipmentrecord.Capacity.GetValue(MassUnits.KiloGram) /
                                        outletpump.Flow.GetValue(MassFlowUnits.Kg_min);
                                    transfertime = new Amount(time, TimeUnits.Minute);
                                    recipedmaterial.TransferTime = transfertime;
                                }
                                recipedmaterial.CalculateTime(materialequipmentrecord.Capacity);
                                analisisResult.AddReport(ManufacturingReportType.ProductCanProduce,
                                    recipedmaterial.Material!, equipment,
                                    materialequipmentrecord.Capacity,
                                    recipedmaterial.BatchCycleTime,
                                    transfertime);
                            }
                        }
                        else if (equipment is ProcessContinuousSystem skid)
                        {
                            recipedmaterial.CalculateFlows(skid.Capacity);
                            analisisResult.AddReport(ManufacturingReportType.ProductCanProduce,
                                   recipedmaterial.Material!, equipment);
                        }




                    }

                }
            }
        }


    }
}
