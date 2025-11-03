using MudBlazor.Charts;
using Simulator.Client.HCPages.ContinuousSystems;
using Simulator.Client.HCPages.Lines;
using Simulator.Client.HCPages.Mixers;
using Simulator.Client.HCPages.Operators;
using Simulator.Client.HCPages.Pumps;
using Simulator.Client.HCPages.SimulationPlanneds.ProcessFlowDiagram;
using Simulator.Client.HCPages.StreamJoiners;
using Simulator.Client.HCPages.Tanks;
using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Models.HCs.ContinuousSystems;
using Simulator.Shared.Models.HCs.Lines;
using Simulator.Shared.Models.HCs.Mixers;
using Simulator.Shared.Models.HCs.Operators;
using Simulator.Shared.Models.HCs.Pumps;
using Simulator.Shared.Models.HCs.StreamJoiners;
using Simulator.Shared.Models.HCs.Tanks;
using Simulator.Shared.NuevaSimlationconQwen;
using Simulator.Shared.NuevaSimlationconQwen.Equipments;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Lines;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Mixers;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Operators;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Pumps;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Skids;
using Simulator.Shared.NuevaSimlationconQwen.Equipments.Tanks;
using Simulator.Shared.NuevaSimlationconQwen.StreanJoiners;

namespace Simulator.Client.HCPages.MainProcesses.Process
{
    public partial class NewProcessFlowDiagram
    {

        [Parameter]
        public GeneralSimulation Simulation { get; set; } = null!;

        override protected void OnInitialized()
        {

        }

        private const int START_X = 100;
        private const int START_Y = 100;
        private const int VERTICAL_SPACING = 85;
        private const int HORIZONTAL_SPACING_BETWEEN_COLUMNS = 20;
        private const int PUMP_HORIZONTAL_SPACING = 150;

        private const int SECTION_VERTICAL_SPACING = 15;
        private const int LINE_VERTICAL_SPACING = 115; // Más espacio para líneas
        private const int MAX_SVG_WIDTH = 2000;
        private const int MAX_SVG_HEIGHT = 1000;
        private const int OPERATOR_HORIZONTAL_OFFSET = -70; // Más separación del mezclador

        private List<EquipmentNodeBase> allEquipment = new();
        private List<ConnectionLine> connections = new();
        private IEquipment? selectedEquipment;
        private IEquipment? highlightedEquipment;
        private int svgWidth = 2000;
        private int svgHeight = 1000;
        [Parameter]
        public EventCallback GetAll { get; set; }

        override protected void OnParametersSet()
        {
            GenerateVisualization();
            LoadingSimulation = false;
        }

        private void GenerateVisualization()
        {
            if (Simulation == null) return;

            allEquipment.Clear();
            connections.Clear();

            var equipmentList = Simulation.Equipments;

            PositionEquipment(equipmentList);
            GenerateConnections(equipmentList);

            StateHasChanged();
        }
        private void GenerateConnections(List<IEquipment> equipmentList)
        {
            connections.Clear();

            if (highlightedEquipment == null) return;

            var fromNode = allEquipment.FirstOrDefault(n => n.Equipment.Id == highlightedEquipment.Id);
            if (fromNode == null) return;
            foreach (var outlet in highlightedEquipment.OutletEquipments)
            {
                var toNode = allEquipment.FirstOrDefault(n => n.Equipment.Id == outlet.Id);
                if (toNode != null)
                {
                    connections.Add(new ConnectionLine
                    {
                        FromX = fromNode.GetConnectionPointX(false),
                        FromY = fromNode.GetConnectionPointY(false),
                        ToX = toNode.GetConnectionPointX(true),
                        ToY = toNode.GetConnectionPointY(true),
                        FromEquipment = highlightedEquipment,
                        ToEquipment = outlet,
                        StrokeColor = "#007bff",
                        StrokeWidth = 1,
                        DashArray = "0",
                        MarkerId = "arrowhead-blue"
                    });
                }
            }

            // Conexiones de entrada (entrantes)
            foreach (var inlet in highlightedEquipment.InletEquipments)
            {
                var toNode = allEquipment.FirstOrDefault(n => n.Equipment.Id == inlet.Id);
                if (toNode != null)
                {
                    connections.Add(new ConnectionLine
                    {
                        FromX = toNode.GetConnectionPointX(false),
                        FromY = toNode.GetConnectionPointY(false),
                        ToX = fromNode.GetConnectionPointX(true),
                        ToY = fromNode.GetConnectionPointY(true),
                        FromEquipment = inlet,
                        ToEquipment = highlightedEquipment,
                        StrokeColor = "#28a745",
                        StrokeWidth = 1,
                        DashArray = "5,5",
                        MarkerId = "arrowhead-green"
                    });
                }
            }
        }


        private string GetEquipmentType(IEquipment equipment)
        {
            return equipment.EquipmentType switch
            {
                ProccesEquipmentType.Line => "Line",
                ProccesEquipmentType.Tank => "Tank",
                ProccesEquipmentType.Pump => "Pump",
                ProccesEquipmentType.Mixer => "Mixer",
                ProccesEquipmentType.ContinuousSystem => "SKID",
                ProccesEquipmentType.Operator => "Operator",
                ProccesEquipmentType.StreamJoiner => "Stream Joiner", // 👈 NUEVO
                _ => equipment.GetType().Name
            };
        }

        private void OnEquipmentClick(EquipmentNodeBase node)
        {
            // Si se hace click en el mismo equipo, limpiar selección
            if (highlightedEquipment?.Id == node.Equipment.Id)
            {
                ClearSelection();
            }
            else
            {
                selectedEquipment = node.Equipment;
                highlightedEquipment = node.Equipment;
                GenerateConnections(Simulation?.Equipments!);
            }
            StateHasChanged();
        }

        bool LoadingSimulation = false;



        private void ClearSelection()
        {
            highlightedEquipment = null;
            selectedEquipment = null;
            connections.Clear();
            StateHasChanged();
        }

        public int TotalEquipmentCount => Simulation?.Equipments.Count ?? 0;



        public class ConnectionLine
        {
            public double FromX { get; set; }
            public double FromY { get; set; }
            public double ToX { get; set; }
            public double ToY { get; set; }
            public IEquipment FromEquipment { get; set; } = null!;
            public IEquipment ToEquipment { get; set; } = null!;
            public string StrokeColor { get; set; } = "#6c757d";
            public int StrokeWidth { get; set; } = 2;
            public string DashArray { get; set; } = "0";
            public string MarkerId { get; set; } = "arrowhead-gray";  // Valor por defecto
        }
        private string AddTitleToSvg(string svgContent, string title)
        {
            // Insertar <title> al inicio de cada elemento SVG principal
            return svgContent.Replace(
                "<circle", $"<title>{System.Web.HttpUtility.HtmlEncode(title)}</title><circle")
                .Replace("<rect", $"<title>{System.Web.HttpUtility.HtmlEncode(title)}</title><rect")
                .Replace("<path", $"<title>{System.Web.HttpUtility.HtmlEncode(title)}</title><path")
                .Replace("<polygon", $"<title>{System.Web.HttpUtility.HtmlEncode(title)}</title><polygon")
                .Replace("<line", $"<title>{System.Web.HttpUtility.HtmlEncode(title)}</title><line")
                .Replace("<ellipse", $"<title>{System.Web.HttpUtility.HtmlEncode(title)}</title><ellipse");
        }

        private void PositionEquipment(List<IEquipment> equipmentList)
        {
            if (!equipmentList.Any()) return;

            var positionedEquipment = new Dictionary<Guid, (double X, double Y)>();

            // Calcular dimensiones disponibles
            int totalGroups = 5; // Volvemos a 5 grupos (sin columna separada de operadores)
            int groupWidth = (MAX_SVG_WIDTH - START_X * 2) / totalGroups;

            // Posicionar cada columna
            PositionColumn1RawMaterialTanks(equipmentList, positionedEquipment, START_X, groupWidth);
            PositionColumn2MixersAndSkidsWithOperators(equipmentList, positionedEquipment, START_X, groupWidth); // Integrado con operadores
            PositionColumn4WipTanks(equipmentList, positionedEquipment, START_X, groupWidth);


            // Manejar equipos no posicionados
            PositionUnpositionedEquipment(equipmentList, positionedEquipment);

            // CREAR NODOS DE EQUIPO
            CreateEquipmentNodes(equipmentList, positionedEquipment);

            // Calcular dimensiones del SVG
            CalculateSvgDimensions();
        }

        private void PositionColumn1RawMaterialTanks(List<IEquipment> equipmentList, Dictionary<Guid, (double X, double Y)> positionedEquipment, int minX, int groupWidth)
        {
            double group1StartX = minX;
            double currentY = START_Y;
            var rawMaterialTanks = equipmentList.OfType<ProcessBaseTankForRawMaterial>()

                .OrderBy(t => t.Name).ToList();
            // Identificar tanques de materia prima
            //var rawMaterialTanks = equipmentList.OfType<RawMaterialTank>().OrderBy(t => t.Name).ToList();

            // Identificar bombas que tienen múltiples tanques de entrada
            var pumpToTanksMap = new Dictionary<ProcessPump, List<ProcessBaseTankForRawMaterial>>();

            foreach (var tank in rawMaterialTanks)
            {
                var associatedPumps = tank.OutletEquipments.OfType<ProcessPump>().ToList();
                foreach (var pump in associatedPumps)
                {
                    if (!pumpToTanksMap.ContainsKey(pump))
                        pumpToTanksMap[pump] = new List<ProcessBaseTankForRawMaterial>();
                    pumpToTanksMap[pump].Add(tank);
                }
            }

            // Separar casos: bombas con un tanque vs bombas con múltiples tanques
            var singleTankPumps = pumpToTanksMap.Where(p => p.Value.Count == 1).ToDictionary(p => p.Key, p => p.Value.First());
            var multiTankPumps = pumpToTanksMap.Where(p => p.Value.Count > 1).ToDictionary(p => p.Key, p => p.Value);

            // Tanques que van a bombas de un solo tanque
            var processedTanks = new HashSet<Guid>();

            // Primero procesar tanques con bombas exclusivas (1:1)
            foreach (var tank in rawMaterialTanks)
            {
                var associatedPumps = tank.OutletEquipments.OfType<ProcessPump>().ToList();
                bool hasOnlySinglePumps = associatedPumps.All(p => singleTankPumps.ContainsKey(p));

                if (hasOnlySinglePumps && associatedPumps.Any())
                {
                    // Posicionar el tanque
                    positionedEquipment[tank.Id] = (group1StartX, currentY);
                    processedTanks.Add(tank.Id);

                    // Posicionar bombas
                    if (associatedPumps.Count == 1)
                    {
                        // Caso: una sola bomba - emparejada con el tanque
                        var pump = associatedPumps.First();
                        positionedEquipment[pump.Id] = (group1StartX + PUMP_HORIZONTAL_SPACING, currentY);
                    }
                    else
                    {
                        // Caso: múltiples bombas
                        var firstPump = associatedPumps.First();
                        positionedEquipment[firstPump.Id] = (group1StartX + PUMP_HORIZONTAL_SPACING, currentY);

                        for (int i = 1; i < associatedPumps.Count; i++)
                        {
                            var pump = associatedPumps[i];
                            positionedEquipment[pump.Id] = (group1StartX + PUMP_HORIZONTAL_SPACING, currentY + (i * VERTICAL_SPACING));
                        }

                        currentY += (associatedPumps.Count - 1) * VERTICAL_SPACING;
                    }

                    currentY += VERTICAL_SPACING;
                }
            }

            // Luego procesar grupos de tanques que van a la misma bomba
            foreach (var kvp in multiTankPumps)
            {
                var pump = kvp.Key;
                var tanks = kvp.Value.OrderBy(t => t.Name).ToList();

                // Posicionar el primer tanque en la posición actual
                if (tanks.Any() && !processedTanks.Contains(tanks[0].Id))
                {
                    positionedEquipment[tanks[0].Id] = (group1StartX, currentY);
                    processedTanks.Add(tanks[0].Id);

                    // Posicionar la bomba al frente del primer tanque
                    positionedEquipment[pump.Id] = (group1StartX + PUMP_HORIZONTAL_SPACING, currentY);

                    // Posicionar los tanques restantes debajo
                    for (int i = 1; i < tanks.Count; i++)
                    {
                        if (!processedTanks.Contains(tanks[i].Id))
                        {
                            positionedEquipment[tanks[i].Id] = (group1StartX, currentY + (i * VERTICAL_SPACING));
                            processedTanks.Add(tanks[i].Id);
                        }
                    }

                    currentY += Math.Max(tanks.Count - 1, 1) * VERTICAL_SPACING;
                    currentY += VERTICAL_SPACING;
                }
            }

            // Finalmente procesar cualquier tanque que no haya sido procesado
            foreach (var tank in rawMaterialTanks)
            {
                if (!processedTanks.Contains(tank.Id))
                {
                    positionedEquipment[tank.Id] = (group1StartX, currentY);

                    // Procesar bombas de tanques no procesados
                    var associatedPumps = tank.OutletEquipments.OfType<ProcessPump>().ToList();
                    if (associatedPumps.Any())
                    {
                        if (associatedPumps.Count == 1)
                        {
                            var pump = associatedPumps.First();
                            positionedEquipment[pump.Id] = (group1StartX + PUMP_HORIZONTAL_SPACING, currentY);
                        }
                        else
                        {
                            var firstPump = associatedPumps.First();
                            positionedEquipment[firstPump.Id] = (group1StartX + PUMP_HORIZONTAL_SPACING, currentY);

                            for (int i = 1; i < associatedPumps.Count; i++)
                            {
                                var pump = associatedPumps[i];
                                positionedEquipment[pump.Id] = (group1StartX + PUMP_HORIZONTAL_SPACING, currentY + (i * VERTICAL_SPACING));
                            }

                            currentY += (associatedPumps.Count - 1) * VERTICAL_SPACING;
                        }
                    }

                    currentY += VERTICAL_SPACING;
                }
            }
        }
        private void PositionColumn2MixersAndSkidsWithOperators(List<IEquipment> equipmentList, Dictionary<Guid, (double X, double Y)> positionedEquipment, int minX, int groupWidth)
        {
            // Calcular posición de la columna 2 (mezcladores)
            double column2StartX = minX + groupWidth; // Columna 2 empieza después de la 1
            double currentY = START_Y;

            // Combinar mezcladores y SKIDs
            var mixers = equipmentList.OfType<ManufaturingEquipment>().OrderBy(m => m.Name).ToList();

            var mixerAndSkidEquipment = new List<IEquipment>();
            mixerAndSkidEquipment.AddRange(mixers);

            mixerAndSkidEquipment = mixerAndSkidEquipment.OrderBy(e => e.Name).ToList();

            // Posicionar mezcladores/SKIDs y sus operadores asociados
            var mixerSkidPositions = new Dictionary<Guid, double>();
            var processedOperators = new HashSet<Guid>();

            foreach (var equipment in mixerAndSkidEquipment)
            {
                positionedEquipment[equipment.Id] = (column2StartX, currentY);
                mixerSkidPositions[equipment.Id] = currentY;

                // Posicionar operadores asociados (más separados)
                if (equipment is ProcessMixer mixer)
                {
                    var connectedOperators = equipmentList.OfType<ProcessOperator>()
                        .Where(op => op.OutletEquipments.Contains(mixer))
                        .ToList();

                    if (connectedOperators.Any())
                    {
                        var op = connectedOperators.First();
                        if (!processedOperators.Contains(op.Id))
                        {
                            positionedEquipment[op.Id] = (column2StartX + OPERATOR_HORIZONTAL_OFFSET - 20, currentY); // Más separación
                            processedOperators.Add(op.Id);
                        }
                    }
                }

                currentY += VERTICAL_SPACING + SECTION_VERTICAL_SPACING;
            }

            // Bombas asociadas
            double mixerSkidPumpColumnX = column2StartX + PUMP_HORIZONTAL_SPACING;
            foreach (var equipment in mixerAndSkidEquipment)
            {
                var equipmentY = mixerSkidPositions[equipment.Id];
                var associatedPumps = equipment.OutletEquipments.OfType<ProcessPump>().ToList();

                foreach (var pump in associatedPumps)
                {
                    if (!positionedEquipment.ContainsKey(pump.Id))
                    {
                        positionedEquipment[pump.Id] = (mixerSkidPumpColumnX, equipmentY);
                    }
                }
            }

            // Posicionar operadores sin mezcladores asociados
            var unpositionedOperators = equipmentList.OfType<ProcessOperator>()
                .Where(op => !processedOperators.Contains(op.Id))
                .ToList();

            foreach (var op in unpositionedOperators)
            {
                positionedEquipment[op.Id] = (column2StartX + OPERATOR_HORIZONTAL_OFFSET - 20, currentY);
                currentY += VERTICAL_SPACING;
            }
        }
        private void PositionColumn4WipTanks(List<IEquipment> equipmentList, Dictionary<Guid, (double X, double Y)> positionedEquipment, int minX, int groupWidth)
        {
            double column3EndX = minX + (groupWidth * 2); // Fin de columna 2 (mezcladores)
            double wipTanksStartX = column3EndX + HORIZONTAL_SPACING_BETWEEN_COLUMNS + 30;
            double pumpsX = wipTanksStartX + PUMP_HORIZONTAL_SPACING;
            double streamjoinerX = pumpsX + PUMP_HORIZONTAL_SPACING + 30; // Justo después de bombas
            double linesX = streamjoinerX + PUMP_HORIZONTAL_SPACING + 30; // Justo después de bombas
            double currentY = START_Y;

            var ProcessLines = equipmentList.OfType<ProcessLine>();

            foreach (var processLine in ProcessLines)
            {

                if (processLine.StreamJoinersAttached.Any())
                {
                    positionedEquipment[processLine.Id] = (linesX, currentY);
                    foreach (var streamjoiner in processLine.StreamJoinersAttached)
                    {
                        positionedEquipment[streamjoiner.Id] = (streamjoinerX, currentY);
                        var wiptanks = streamjoiner.WIPTanksAttached.OrderBy(x => x.Name);
                        foreach (var wip in wiptanks)
                        {
                            positionedEquipment[wip.Id] = (wipTanksStartX, currentY);
                            foreach (var pump in wip.OutletPumps)
                            {
                                positionedEquipment[pump.Id] = (pumpsX, currentY);
                                currentY += VERTICAL_SPACING;
                            }
                        }
                    }
                }
                if (processLine.WIPTanksAttached.Any())
                {
                    positionedEquipment[processLine.Id] = (streamjoinerX, currentY);
                    var wiptanks = processLine.WIPTanksAttached.OrderBy(x => x.Name);
                    foreach (var wip in wiptanks)
                    {
                        positionedEquipment[wip.Id] = (wipTanksStartX, currentY);
                        foreach (var pump in wip.OutletPumps)
                        {
                            positionedEquipment[pump.Id] = (pumpsX, currentY);
                            currentY += VERTICAL_SPACING;
                        }
                    }
                }
            }

        }

        private void PositionUnpositionedEquipment(List<IEquipment> equipmentList, Dictionary<Guid, (double X, double Y)> positionedEquipment)
        {
            const int verticalSpacing = 85;

            // Calcular posición para equipos no posicionados
            double lastGroupX = 1850; // Aproximadamente el final
            double currentY = 100;

            var unpositioned = equipmentList.Where(e => !positionedEquipment.ContainsKey(e.Id)).ToList();

            foreach (var equipment in unpositioned)
            {
                positionedEquipment[equipment.Id] = (lastGroupX, currentY);
                currentY += verticalSpacing;
            }
        }

        private void CreateEquipmentNodes(List<IEquipment> equipmentList, Dictionary<Guid, (double X, double Y)> positionedEquipment)
        {
            // Tu código existente para crear los nodos
            allEquipment.Clear();
            foreach (var equipment in equipmentList)
            {
                if (positionedEquipment.TryGetValue(equipment.Id, out var position))
                {
                    EquipmentNodeBase node = equipment switch
                    {
                        ProcessBaseTankForRawMaterial => new RawTankNode(),
                        ProcessWipTankForLine => new WipTankNode(),
                        ProcessMixer => new MixerNode(),
                        ProcessPump => new PumpNode(),
                        ProcessLine => new LineNode(),
                        ProcessContinuousSystem => new MixerNode(),
                        ProcessOperator => new OperatorNode(), // Nuevo tipo
                        ProcessStreamJoiner => new StreamJoinerNode(),
                        _ => new DefaultEquipmentNode()
                    };

                    node.Equipment = equipment;
                    node.X = position.X;
                    node.Y = position.Y;
                    node.Name = equipment.Name.Length > 15 ? equipment.Name.Substring(0, 12) + "..." : equipment.Name;

                    allEquipment.Add(node);
                }
            }
        }

        private void CalculateSvgDimensions()
        {
            if (allEquipment.Any())
            {
                svgWidth = 2000;
                svgHeight = (int)allEquipment.Max(e => e.Y) + 200;
                svgHeight = Math.Max(svgHeight, 1000);
            }
        }
        private async Task OnEquipmentDoubleClick(EquipmentNodeBase node)
        {
            bool result = node.Type switch
            {
                ProccesEquipmentType.Line => await UpdateLine(node.Id),
                ProccesEquipmentType.Tank => await UpdateTank(node.Id),
                ProccesEquipmentType.Pump => await UpdatePump(node.Id),
                ProccesEquipmentType.Mixer => await UpdateMixer(node.Id),
                ProccesEquipmentType.ContinuousSystem => await UpdateContinuousSystem(node.Id),
                ProccesEquipmentType.Operator => await UpdateOperator(node.Id),
                ProccesEquipmentType.StreamJoiner => await UpdateStreamJoiner(node.Id),
                ProccesEquipmentType.None => false,
                _ => false,
            };
            if (result)
            {
                await GetAll.InvokeAsync();
            }

            StateHasChanged();
        }
        async Task<bool> UpdateTank(Guid Id)
        {
            TankDTO response = new() { Id = Id, MainProcessId = Simulation.MainProcessId };
            var parameters = new DialogParameters<TankDialog>
        {

             { x => x.Model, response },
        };
            var options = new DialogOptions() { MaxWidth = MaxWidth.Medium };


            var dialog = await DialogService.ShowAsync<TankDialog>("Tank", parameters, options);
            var result = await dialog.Result;
            if (result != null && !result.Canceled)
            {
                return true;
            }
            return false;
        }
        async Task<bool> UpdateLine(Guid Id)
        {
            LineDTO response = new() { Id = Id, MainProcessId = Simulation.MainProcessId };
            var parameters = new DialogParameters<LineDialog>
        {

             { x => x.Model, response },
        };
            var options = new DialogOptions() { MaxWidth = MaxWidth.Large };


            var dialog = await DialogService.ShowAsync<LineDialog>("Line", parameters, options);
            var result = await dialog.Result;
            if (result != null && !result.Canceled)
            {
                return true;
            }
            return false;
        }
        async Task<bool> UpdatePump(Guid Id)
        {
            PumpDTO response = new() { Id = Id, MainProcessId = Simulation.MainProcessId };
            var parameters = new DialogParameters<PumpDialog>
        {

             { x => x.Model, response },
        };
            var options = new DialogOptions() { MaxWidth = MaxWidth.Medium };


            var dialog = await DialogService.ShowAsync<PumpDialog>("Pump", parameters, options);
            var result = await dialog.Result;
            if (result != null && !result.Canceled)
            {
                return true;
            }
            return false;
        }
        async Task<bool> UpdateMixer(Guid Id)
        {
            MixerDTO response = new() { Id = Id, MainProcessId = Simulation.MainProcessId };
            var parameters = new DialogParameters<MixerDialog>
        {
           { x => x.Model, response },
        };

            var options = new DialogOptions() { MaxWidth = MaxWidth.Medium };

            var dialog = await DialogService.ShowAsync<MixerDialog>("Mixer", parameters, options);
            var result = await dialog.Result;
            if (result != null && !result.Canceled)
            {
                return true;
            }
            return false;
        }
        async Task<bool> UpdateStreamJoiner(Guid Id)
        {
            var response = new StreamJoinerDTO { Id = Id, MainProcessId = Simulation.MainProcessId };
            var parameters = new DialogParameters<StreamJoinerDialog>
    {
        { x => x.Model, response }
    };
            var options = new DialogOptions { MaxWidth = MaxWidth.Medium };
            var dialog = await DialogService.ShowAsync<StreamJoinerDialog>("Stream Joiner", parameters, options);
            var result = await dialog.Result;
            return result is { Canceled: false };
        }
        async Task<bool> UpdateContinuousSystem(Guid Id)
        {
            ContinuousSystemDTO response = new() { Id = Id, MainProcessId = Simulation.MainProcessId };
            var parameters = new DialogParameters<ContinuousSystemDialog>
        {

             { x => x.Model, response },
        };
            var options = new DialogOptions() { MaxWidth = MaxWidth.Medium };


            var dialog = await DialogService.ShowAsync<ContinuousSystemDialog>("ContinuousSystem", parameters, options);
            var result = await dialog.Result;
            if (result != null && !result.Canceled)
            {
                return true;
            }
            return false;
        }
        async Task<bool> UpdateOperator(Guid Id)
        {
            OperatorDTO response = new() { Id = Id, MainProcessId = Simulation.MainProcessId };
            var parameters = new DialogParameters<OperatorDialog>
        {

             { x => x.Model, response },
        };
            var options = new DialogOptions() { MaxWidth = MaxWidth.Medium };


            var dialog = await DialogService.ShowAsync<OperatorDialog>("Operator", parameters, options);
            var result = await dialog.Result;
            if (result != null && !result.Canceled)
            {
                return true;
            }
            return false;
        }
    }
}
