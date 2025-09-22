using Simulator.Shared.Simulations.Materials;
using Simulator.Shared.Simulations.Mixers;
using Simulator.Shared.Simulations.Operators;
using Simulator.Shared.Simulations.Pumps;
using Simulator.Shared.Simulations.Skids;

namespace Simulator.Shared.Simulations.ProductionAnalyzers
{
    // Nueva clase para el análisis de producción

    public class ProductionAnalyzer
    {
        private readonly NewSimulation _simulation;

        public ProductionAnalyzer(NewSimulation simulation)
        {
            _simulation = simulation;
        }

        // Propiedad para almacenar resultados del análisis de mezcladores
        public Dictionary<BaseMixer, Dictionary<BackBoneSimulation, bool>> MixerProductionAnalysisResults { get; private set; } = new();

        // NUEVA: Propiedad para almacenar resultados del análisis de SKIDs
        public Dictionary<BaseSKID, Dictionary<BackBoneSimulation, bool>> SkidProductionAnalysisResults { get; private set; } = new();

        // Método principal para ejecutar el análisis de mezcladores
        public void AnalyzeMixerProductionCapabilities()
        {
            MixerProductionAnalysisResults.Clear();

            // Usar ToDictionary para mejor rendimiento
            MixerProductionAnalysisResults = _simulation.Mixers.ToDictionary(
                mixer => mixer,
                mixer => GetProducibleMaterialsForEquipment(mixer)
                    .ToDictionary(material => material, material => CanEquipmentProduceMaterial(mixer, material))
            );
        }

        // NUEVO: Método principal para ejecutar el análisis de SKIDs
        public void AnalyzeSkidProductionCapabilities()
        {
            SkidProductionAnalysisResults.Clear();

            // CORRECCIÓN: Usar _simulation.Skids (consistente con mayúsculas/minúsculas)
            SkidProductionAnalysisResults = _simulation.SKIDs.ToDictionary(
                skid => skid,
                skid => GetProducibleMaterialsForEquipment(skid)
                    .ToDictionary(material => material, material => CanEquipmentProduceMaterial(skid, material))
            );
        }

        // NUEVO: Método para ejecutar ambos análisis
        public void AnalyzeAllProductionCapabilities()
        {
            AnalyzeMixerProductionCapabilities();
            AnalyzeSkidProductionCapabilities();
        }

        // Obtener materiales que un equipo puede producir (genérico para Mixer y SKID)
        private List<BackBoneSimulation> GetProducibleMaterialsForEquipment(NewBaseEquipment equipment)
        {
            // Ya que estos son los únicos tipos que pueden producir, podemos hacerlo más directo
            return equipment.MaterialSimulations
                .OfType<BackBoneSimulation>() // Esto ya incluye ambos tipos
                .ToList();
        }

        // Verificar si un equipo puede producir un material específico (genérico)
        private bool CanEquipmentProduceMaterial(NewBaseEquipment equipment, MaterialSimulation material)
        {
            // Para productos backbone, verificar las etapas
            if (material is BackBoneSimulation backBoneMaterial)
            {
                return CanEquipmentProduceBackBoneMaterial(equipment, backBoneMaterial);
            }

            return false;
        }

        // Verificar si un equipo puede producir un material BackBone (genérico)
        private bool CanEquipmentProduceBackBoneMaterial(NewBaseEquipment equipment, BackBoneSimulation backBoneMaterial)
        {
            // Verificar que todas las materias primas requeridas estén disponibles
            foreach (var step in backBoneMaterial.RawMaterialSteps)
            {
                if (!equipment.HasAnyInletEquipmentMaterial(step.StepRawMaterial))
                {
                    return false; // Si alguna materia prima no está disponible, no puede producir
                }
            }

            return true; // Todas las materias primas están disponibles
        }




        // Clases para los resultados
        public class EquipmentAnalysisResult
        {
            public string EquipmentName { get; set; } = string.Empty;
            public string EquipmentType { get; set; } = string.Empty; // "Mixer" o "SKID"
            public List<ProductAnalysisResult> ProductResults { get; set; } = new();
            public List<MaterialStatus> UniqueConnectedMaterials { get; set; } = new();
            public List<MaterialStatus> UniqueMissingMaterials { get; set; } = new();
            public Dictionary<string, List<EquipmentCapabilityInfo>> BackboneCapabilities { get; set; } = new();
        }
        // Nueva clase para información de capacidades
        public class EquipmentCapabilityInfo
        {
            public string ProductName { get; set; } = string.Empty;
            public string EquipmentName { get; set; } = string.Empty;
            public Amount Capacity { get; set; } = new Amount(0, MassUnits.KiloGram);
            public Amount BatchTime { get; set; } = new Amount(0, TimeUnits.Minute);
            public Amount TransferTime { get; set; } = new Amount(0, TimeUnits.Minute);
            public Amount TotalTime => BatchTime + TransferTime;
            public bool CanProduce { get; set; }
            public List<string> MissingMaterials { get; set; } = new();
            public List<StepTimeDetail> StepTimes { get; set; } = new();
        }
        public class ProductAnalysisResult
        {
            public string ProductName { get; set; } = string.Empty;
            public bool CanProduce { get; set; }
            public string Reason { get; set; } = string.Empty;
            public List<MaterialStatus> ConnectedMaterials { get; set; } = new();
            public List<MaterialStatus> MissingMaterials { get; set; } = new();
        }

        public class MaterialStatus
        {
            public string MaterialName { get; set; } = string.Empty;
            public string EquipmentType { get; set; } = string.Empty;
            public string EquipmentName { get; set; } = string.Empty;
            public bool IsAvailable { get; set; }
        }

        // Método mejorado para obtener resultados para UI (ambos tipos de equipos)
        public List<EquipmentAnalysisResult> GetAnalysisResultsForUI()
        {
            var results = new List<EquipmentAnalysisResult>();

            // Agregar resultados de mezcladores
            if (MixerProductionAnalysisResults != null && MixerProductionAnalysisResults.Any())
            {
                foreach (var mixerResult in MixerProductionAnalysisResults)
                {
                    var equipmentResult = CreateEquipmentAnalysisResult(
                        mixerResult.Key,
                        mixerResult.Value,
                        "Mixer"
                    );
                    results.Add(equipmentResult);
                }
            }

            // Agregar resultados de SKIDs
            if (SkidProductionAnalysisResults != null && SkidProductionAnalysisResults.Any())
            {
                foreach (var skidResult in SkidProductionAnalysisResults)
                {
                    var equipmentResult = CreateEquipmentAnalysisResult(
                        skidResult.Key,
                        skidResult.Value,
                        "SKID"
                    );
                    results.Add(equipmentResult);
                }
            }

            return results.OrderBy(r => r.EquipmentType).ThenBy(r => r.EquipmentName).ToList();
        }
        // Método auxiliar para crear resultados de análisis de equipos (actualizado)
        private EquipmentAnalysisResult CreateEquipmentAnalysisResult<T>(
            T equipment,
            Dictionary<BackBoneSimulation, bool> products,
            string equipmentType) where T : NewBaseEquipment
        {
            var productResults = new List<ProductAnalysisResult>();
            var allConnectedMaterials = new Dictionary<string, MaterialStatus>();
            var allMissingMaterials = new Dictionary<string, MaterialStatus>();
            var backboneCapabilities = new Dictionary<string, List<EquipmentCapabilityInfo>>();

            foreach (var product in products)
            {
                var detailedMaterials = GetDetailedMaterialStatusForProduct(equipment, product.Key);
                var connectedMaterials = detailedMaterials.Where(m => m.IsAvailable).ToList();
                var missingMaterials = detailedMaterials.Where(m => !m.IsAvailable).ToList();

                // Agregar materiales conectados (evitar repeticiones)
                foreach (var material in connectedMaterials)
                {
                    if (!allConnectedMaterials.ContainsKey(material.MaterialName))
                    {
                        allConnectedMaterials[material.MaterialName] = material;
                    }
                }

                // Agregar materiales faltantes (evitar repeticiones)
                foreach (var material in missingMaterials)
                {
                    if (!allMissingMaterials.ContainsKey(material.MaterialName))
                    {
                        allMissingMaterials[material.MaterialName] = material;
                    }
                }

                productResults.Add(new ProductAnalysisResult
                {
                    ProductName = product.Key.M_NumberCommonName ?? "Unnamed Product",
                    CanProduce = product.Value,
                    Reason = product.Value ? "" : "Missing required materials",
                    ConnectedMaterials = connectedMaterials,
                    MissingMaterials = missingMaterials
                });

                // NUEVO: Agregar información de capacidades para productos backbone
                if (product.Key is BackBoneSimulation backboneProduct)
                {
                    var productName = backboneProduct.M_NumberCommonName ?? "Unnamed Product";
                    var capabilityInfo = new EquipmentCapabilityInfo
                    {
                        ProductName = productName,
                        EquipmentName = equipment.Name ?? "Unnamed Equipment",
                        CanProduce = product.Value,
                        MissingMaterials = missingMaterials.Where(m => !m.IsAvailable).Select(m => m.MaterialName).ToList()
                    };

                    // Obtener capacidades y tiempos si el equipo puede producir el producto
                    if (product.Value && equipment is BaseMixer mixer)
                    {
                        if (backboneProduct.BatchDataMixer.ContainsKey(mixer))
                        {
                            var batchData = backboneProduct.BatchDataMixer[mixer];
                            capabilityInfo.Capacity = batchData.Capacity;
                            capabilityInfo.BatchTime = batchData.BatchCycleTime;
                            capabilityInfo.TransferTime = batchData.TransferTime;
                            // NUEVO: Incluir tiempos de pasos individuales
                            capabilityInfo.StepTimes = batchData.StepTimes.ToList();
                        }
                    }

                    if (!backboneCapabilities.ContainsKey(productName))
                    {
                        backboneCapabilities[productName] = new List<EquipmentCapabilityInfo>();
                    }
                    backboneCapabilities[productName].Add(capabilityInfo);
                }
            }

            return new EquipmentAnalysisResult
            {
                EquipmentName = equipment.Name ?? "Unnamed Equipment",
                EquipmentType = equipmentType,
                ProductResults = productResults,
                UniqueConnectedMaterials = allConnectedMaterials.Values.ToList(),
                UniqueMissingMaterials = allMissingMaterials.Values.ToList(),
                BackboneCapabilities = backboneCapabilities
            };
        }
    
       

        // Método mejorado para obtener resultados con estado detallado de materiales (genérico)
        private List<MaterialStatus> GetDetailedMaterialStatusForProduct(NewBaseEquipment equipment, MaterialSimulation material)
        {
            var materialStatus = new List<MaterialStatus>();

            if (material is BackBoneSimulation backBoneMaterial)
            {
                foreach (var step in backBoneMaterial.RawMaterialSteps)
                {
                    if (step.StepRawMaterial != null)
                    {
                        var rawMaterial = step.StepRawMaterial;
                        var materialName = rawMaterial.M_NumberCommonName ?? rawMaterial.SAPName ?? "Unknown Material";
                        var isAvailable = equipment.HasAnyInletEquipmentMaterial(rawMaterial);

                        // Buscar qué equipo suministra este material
                        string equipmentTypeResult = "None";
                        string equipmentNameResult = "Not connected";

                        // Verificar en bombas de entrada del equipo
                        var supplyingPump = equipment.ConnectedInletEquipments
                            .OfType<BasePump>()
                            .FirstOrDefault(pump => pump.MaterialSimulations.Any(x => x.Id == rawMaterial.Id));

                        // Verificar en operadores de entrada del equipo
                        var supplyingOperator = equipment.ConnectedInletEquipments
                            .OfType<BaseOperator>()
                            .FirstOrDefault(op => op.MaterialSimulations.Any(x => x.Id == rawMaterial.Id));

                        if (supplyingPump != null)
                        {
                            equipmentTypeResult = "Pump";
                            equipmentNameResult = supplyingPump.Name ?? "Unnamed Pump";
                        }
                        else if (supplyingOperator != null)
                        {
                            equipmentTypeResult = "Operator";
                            equipmentNameResult = supplyingOperator.Name ?? "Unnamed Operator";
                        }

                        materialStatus.Add(new MaterialStatus
                        {
                            MaterialName = materialName,
                            EquipmentType = equipmentTypeResult,
                            EquipmentName = equipmentNameResult,
                            IsAvailable = isAvailable
                        });
                    }
                }
            }

            return materialStatus;
        }
    }
}
