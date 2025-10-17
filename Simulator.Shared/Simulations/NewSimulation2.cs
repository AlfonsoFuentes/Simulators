namespace Simulator.Shared.Simulations
{
    //public partial class NewSimulation
    //{
    //    public void AddConnector(NewBaseEquipment from, NewBaseEquipment to)
    //    {
    //        Conectors.Add(new()
    //        {
    //            From = from,
    //            To = to,


    //        });
    //    }
    //    void ConectLines(List<ConectorSimulation> Conectors)
    //    {

    //        var lineconectors = Conectors.Where(x => x.To is BaseLine).ToList();
    //        foreach (var lineconector in lineconectors)
    //        {
    //            var line = GetBaseLine(lineconector.To);
    //            if (lineconector.From is BasePump)
    //            {
    //                var pump = GetBasePump(lineconector.From);


    //                line.AddConnectedInletEquipment(pump);
    //                if (!pump.IsForWashing)
    //                {
    //                    var tank = pump.GetInletAttachedEquipment();
    //                    line.AddProcessInletEquipment(pump);

    //                    pump.AddProcessInletEquipment(tank);
    //                }


    //            }


    //        }

    //    }
    //    void ConectPumps(List<ConectorSimulation> Conectors)
    //    {
    //        var pumpconectors = Conectors.Where(x => x.To is BasePump).ToList();
    //        foreach (var pumpconector in pumpconectors)
    //        {
    //            var pump = GetBasePump(pumpconector.To);
    //            if (pumpconector.From is BaseTank)
    //            {
    //                var tank = GetBaseTank(pumpconector.From);


    //                pump.AddConnectedInletEquipment(tank);
    //                pump.AddProcessInletEquipment(tank);

    //            }
    //            else if (pumpconector.From is BaseMixer)
    //            {
    //                var mixer = GetBaseMixer(pumpconector.From);


    //                pump.AddConnectedInletEquipment(mixer);
    //                pump.AddProcessInletEquipment(mixer);

    //            }


    //        }
    //    }
    //    void ConectTanks(List<ConectorSimulation> Conectors)
    //    {
    //        var tankconectors = Conectors.Where(x => x.To is BaseTank).ToList();
    //        foreach (var tankconector in tankconectors)
    //        {
    //            var tank = GetBaseTank(tankconector.To);

    //            if (tankconector.From is BasePump)
    //            {
    //                var pump = GetBasePump(tankconector.From);

    //                tank.AddConnectedInletEquipment(pump);
    //            }
    //            else if (tankconector.From is BaseSKID)
    //            {
    //                var skid = GetBaseSKID(tankconector.From);
    //                tank.AddConnectedInletEquipment(skid);

    //            }
    //        }
    //    }

    //    void ConectMixers(List<ConectorSimulation> Conectors)
    //    {
    //        var mixerconectors = Conectors.Where(x => x.To is BaseMixer).ToList();
    //        foreach (var mixerconector in mixerconectors)
    //        {
    //            var mixer = GetBaseMixer(mixerconector.To);

    //            if (mixerconector.From is BasePump)
    //            {

    //                var pump = GetBasePump(mixerconector.From);
    //                mixer.AddConnectedInletEquipment(pump);



    //            }
    //            else if (mixerconector.From is BaseOperator)
    //            {
    //                var oper = GetBaseOperator(mixerconector.From);

    //                mixer.AddConnectedInletEquipment(oper);

    //            }
    //        }
    //    }
    //    void ConectSkids(List<ConectorSimulation> Conectors)
    //    {
    //        var skidconectors = Conectors.Where(x => x.To is BaseSKID).ToList();
    //        foreach (var skidconector in skidconectors)
    //        {
    //            var skid = GetBaseSKID(skidconector.To);
    //            if (skidconector.From is BasePump)
    //            {
    //                var pump = GetBasePump(skidconector.From);

    //                skid.AddConnectedInletEquipment(pump);

    //                skid.AddProcessInletEquipment(pump);
    //            }
    //        }
    //    }
    //    public void CreateProcess(List<MaterialEquipmentRecord> processEquipmentMaterials)
    //    {
    //        ConectPumps(Conectors);
    //        ConectTanks(Conectors);
    //        ConectMixers(Conectors);
    //        ConectSkids(Conectors);
    //        ConectLines(Conectors);
    //        SetMaterialToOtherEquipments();
    //        BackBoneSimulations.ForEach(x => x.Init(processEquipmentMaterials));
    //    }
    //    List<BaseLine> InitLines(DateTime currendate)
    //    {

    //        List<BaseLine> retorno = new();
    //        foreach (BaseLine? line in LinesOrdered)
    //        {
    //            var plannline = Planned.PlannedLines.FirstOrDefault(x => x.LineId == line.Id);
    //            if (plannline != null)
    //            {
    //                line!.SetTime(currendate);
    //                line.InitLine(plannline, SkuSimulations, SKULines);
    //            }


    //            if (line.LineScheduled)
    //            {
    //                retorno.Add(line!);

    //            }

    //        }

    //        return retorno!;
    //    }
    //    void InitTanks()
    //    {
    //        foreach (var tank in Tanks)
    //        {
    //            tank.Init();
    //        }





    //    }
    //    void InitMixers()
    //    {
    //        try
    //        {
    //            foreach (var mixer in Mixers)
    //            {
    //                mixer.Init(this);
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            string message = ex.Message;
    //        }

    //        //Mixers.ForEach(x => x.Init(this));
    //    }
    //    public void AddLines(List<LineDTO> equipments)
    //    {
    //        equipments.ForEach(x => AddBaseLine(x));
    //    }
    //    public void AddTanks(List<TankDTO> equipments)
    //    {
    //        equipments.ForEach(x => AddBaseTank(x));
    //    }
    //    public void AddMixers(List<MixerDTO> equipments)
    //    {
    //        equipments.ForEach(x => AddBaseMixer(x));
    //    }
    //    public void AddPumps(List<PumpDTO> equipments)
    //    {
    //        equipments.ForEach(x => AddBasePump(x));
    //    }
    //    public void AddSkids(List<ContinuousSystemDTO> equipments)
    //    {
    //        equipments.ForEach(x => AddBaseSKID(x));
    //    }
    //    public void AddOperators(List<OperatorDTO> equipments)
    //    {
    //        equipments.ForEach(x => AddBaseOperator(x));
    //    }




    //    void AddBasePump(PumpDTO dto)
    //    {
    //        if (!Pumps.Any(x => x.Id == dto.Id))
    //        {

    //            var retorno = new BasePump(dto);
    //            Pumps.Add(retorno);
    //            // SUSCRIBIR el equipo al manejador de eventos
    //            SubscribeEquipmentToEvents(retorno);
    //        }



    //    }

    //    BasePump GetBasePump(NewBaseEquipment dto)
    //    {
    //        if (Pumps.Any(x => x.Id == dto.Id))
    //        {
    //            var pump = Pumps.Single(x => x.Id == dto.Id);
    //            return pump!;
    //        }

    //        return null!;
    //    }

    //    void AddBaseLine(LineDTO dto)
    //    {
    //        if (!Lines.Any(x => x.Id == dto.Id))
    //        {
    //            var retorno = new BaseLine();
    //            retorno.SetLine(dto, WashouTimes);
    //            // SUSCRIBIR el equipo al manejador de eventos
    //            SubscribeEquipmentToEvents(retorno);
    //            Lines.Add(retorno);
    //        }


    //    }
    //    BaseLine GetBaseLine(NewBaseEquipment dto)
    //    {
    //        if (Lines.Any(x => x.Id == dto.Id))
    //        {
    //            var line = Lines.Single(x => x.Id == dto.Id);
    //            return line!;
    //        }

    //        return null!;
    //    }
    //    void AddBaseTank(TankDTO dto)
    //    {
    //        if (!Tanks.Any(x => x.Id == dto.Id))
    //        {
    //            var retorno = AddTank(dto);
    //            if (retorno == null)
    //            {
    //                return;
    //            }
    //            Tanks.Add(retorno);
    //            // SUSCRIBIR el equipo al manejador de eventos
    //            retorno.Simulation = this;
    //            SubscribeEquipmentToEvents(retorno);
    //        }

    //    }
    //    BaseTank GetBaseTank(NewBaseEquipment dto)
    //    {
    //        if (Tanks.Any(x => x.Id == dto.Id))
    //        {
    //            var tank = Tanks.Single(x => x.Id == dto.Id);

    //            return tank!;
    //        }

    //        return null!;
    //    }
    //    BaseTank AddTank(TankDTO dto)
    //    {
    //        switch (dto.FluidStorage)
    //        {
    //            case FluidToStorage.RawMaterial:
    //                {
    //                    var result = new RawMaterialTank(dto);
    //                    RawMaterialTank.Add(result);
    //                    // SUSCRIBIR el equipo al manejador de eventos
    //                    SubscribeEquipmentToEvents(result);
    //                    return result;
    //                }

    //            case FluidToStorage.ProductBackBone:
    //                {
    //                    switch (dto.TankCalculationType)
    //                    {
    //                        case TankCalculationType.BatchCycleTime:
    //                            {
    //                                var result = new WIPInletMixer(dto);
    //                                WIPMixerTank.Add(result);
    //                                SubscribeEquipmentToEvents(result);
    //                                return result;
    //                            }
    //                        case TankCalculationType.ContinuousSystemHiLoLevel:
    //                        case TankCalculationType.AutomaticHiLoLevel:
    //                            {
    //                                var result = new WIPInletSKID(dto);
    //                                WIPSKIDTank.Add(result);
    //                                SubscribeEquipmentToEvents(result);
    //                                return result;
    //                            }

    //                    }

    //                }
    //                break;
    //            //case FluidToStorage.ProductBackBoneToWIPs:
    //            //    {
    //            //        var result = new WIPForProductBackBone(dto);
    //            //        WIPProductTanks.Add(result);
    //            //        SubscribeEquipmentToEvents(result);

    //            //        return result;
    //            //    }
    //            case FluidToStorage.RawMaterialBackBone:
    //                {
    //                    switch (dto.TankCalculationType)

    //                    {
    //                        case TankCalculationType.BatchCycleTime:
    //                            {
    //                                var result = new BackBoneRawMaterialTank(dto);
    //                                BackBoneRawMaterialTanks.Add(result);
    //                                SubscribeEquipmentToEvents(result);
    //                                return result;

    //                            }
    //                        case TankCalculationType.AutomaticHiLoLevel:
    //                            {
    //                                var result = new RawMaterialTank(dto);
    //                                RawMaterialTank.Add(result);
    //                                SubscribeEquipmentToEvents(result);
    //                                return result;

    //                            }

    //                    }
    //                    break;
    //                }


    //        }
    //        return null!;
    //    }
    //    void AddBaseSKID(ContinuousSystemDTO dto)
    //    {
    //        if (!SKIDs.Any(x => x.Id == dto.Id))
    //        {
    //            var retorno = new BaseSKID(dto);
    //            // SUSCRIBIR el equipo al manejador de eventos
    //            SubscribeEquipmentToEvents(retorno);
    //            SKIDs.Add(retorno);
    //        }


    //    }
    //    BaseSKID GetBaseSKID(NewBaseEquipment dto)
    //    {
    //        if (SKIDs.Any(x => x.Id == dto.Id))
    //        {
    //            var skid = SKIDs.Single(x => x.Id == dto.Id);
    //            return skid!;
    //        }

    //        return null!;
    //    }

    //    void AddBaseOperator(OperatorDTO dto)
    //    {
    //        if (!Operators.Any(x => x.Id == dto.Id))
    //        {
    //            var retorno = new BaseOperator(dto);
    //            // SUSCRIBIR el equipo al manejador de eventos
    //            SubscribeEquipmentToEvents(retorno);

    //            Operators.Add(retorno);
    //        }


    //    }
    //    BaseOperator GetBaseOperator(NewBaseEquipment dto)
    //    {
    //        if (Operators.Any(x => x.Id == dto.Id))
    //        {
    //            var oper = Operators.Single(x => x.Id == dto.Id);
    //            return oper!;
    //        }

    //        return null!;
    //    }

    //    void AddBaseMixer(MixerDTO dto)
    //    {
    //        if (!Mixers.Any(x => x.Id == dto.Id))
    //        {
    //            var retorno = new BaseMixer(dto, WashouTimes);

    //            // SUSCRIBIR el equipo al manejador de eventos
    //            SubscribeEquipmentToEvents(retorno);
    //            Mixers.Add(retorno);
    //        }




    //    }
    //    BaseMixer GetBaseMixer(NewBaseEquipment dto)
    //    {
    //        if (Mixers.Any(x => x.Id == dto.Id))
    //        {
    //            var mixer = Mixers.Single(x => x.Id == dto.Id);
    //            return mixer!;
    //        }

    //        return null!;
    //    }

    //    public void MapMaterialEquipments(List<MaterialEquipmentRecord> processEquipmentMaterials)
    //    {
    //        foreach (var item in processEquipmentMaterials)
    //        {
    //            if (item.MaterialId == Guid.Empty || item.EquipmentId == Guid.Empty)
    //            {
    //                continue;
    //            }
    //            var equipment = SimulationEquipments.FirstOrDefault(x => x.Id == item.EquipmentId);
    //            var material = MaterialSimulations.FirstOrDefault(x => x.Id == item.MaterialId);

    //            if (material != null && equipment != null)
    //            {
    //                equipment.AddMaterialSimulation(material);


    //            }



    //        }
    //    }
    //    public void MapConnectors(List<ConnectorRecord> connectors)
    //    {
    //        foreach (var item in connectors)
    //        {
    //            if (item.FromId == Guid.Empty || item.ToId == Guid.Empty)
    //            {
    //                continue;
    //            }
    //            var from = SimulationEquipments.FirstOrDefault(x => x.Id == item.FromId);
    //            var to = SimulationEquipments.FirstOrDefault(x => x.Id == item.ToId);

    //            if (from != null && to != null)
    //            {
    //                AddConnector(from, to);


    //            }



    //        }
    //    }
    //}
}
