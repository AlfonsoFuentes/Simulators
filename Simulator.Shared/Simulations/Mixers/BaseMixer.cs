using Simulator.Shared.Enums.HCEnums.Enums;
using Simulator.Shared.Models.HCs.Mixers;
using Simulator.Shared.Simulations.Materials;
using Simulator.Shared.Simulations.Mixers.States.MixerCalculations.MixerStates;
using Simulator.Shared.Simulations.Mixers.States.MixerCalculations.MixerStepStates;
using Simulator.Shared.Simulations.Pumps;
using Simulator.Shared.Simulations.SimulationResults.Mixers;
using Simulator.Shared.Simulations.Tanks;

namespace Simulator.Shared.Simulations.Mixers
{
    public class BaseMixer : NewBaseEquipment
    {
        public override Guid Id => MixerDTO == null ? Guid.Empty : MixerDTO.Id;
        public override string Name => MixerDTO == null ? string.Empty : MixerDTO.Name;
        MixerDTO MixerDTO { get; set; }
        public Amount CurrentMixerBackBoneCapacity { get; set; } = new(MassUnits.KiloGram);
        public Amount CurrentLevel { get; set; } = new(MassUnits.KiloGram);
        public BasePump OutletPump => ConnectedOutletEquipments.Count == 1 ? ConnectedOutletEquipments.Select(x => x as BasePump).First()! : null!;
        public DateTime InitBatchDate { get; set; }
        public DateTime EndBatchDate { get; set; }
        public DateTime InitBatchTransferDate { get; set; }
        public DateTime EndBatchTransferDate { get; set; }

        public Amount CurrentTime { get; set; } = new(TimeUnits.Minute);

        public string ProducingToName => ProducingTo == null! ? string.Empty : ProducingTo.Name;
        public WIPTank ProducingTo { get; set; } = null!;

        public bool IsInletAbleToReceiveMixer => ProducingTo == null ? false : ProducingTo.InletOcupiedBy == null!;
        public string LabelMixerState => MixerState == null ? "" : $"{MixerState.LabelState}";
        List<WashoutSimulation> WashouTimes = null!;




        public MixerState MixerState { get; set; } = null!;
        public MixerStepState MixerStepState { get; set; } = null!;
        BackBoneSimulation _CurrentBackBoneSimulation = null!;
        public BackBoneSimulation CurrentBackBoneSimulation
        {
            get => _CurrentBackBoneSimulation;
            set
            {
                _CurrentBackBoneSimulation = value;
                if (value != null)
                {
                    if (_CurrentBackBoneSimulation.BatchDataMixer.ContainsKey(this))
                    {
                        CurrentMixerBackBoneCapacity = _CurrentBackBoneSimulation.BatchDataMixer[this].Capacity;
                    }
                }
                
            }
        }

        public List<MixerResults> MixerResults { get; private set; } = new();
        public BaseMixer(MixerDTO mixerDTO, List<WashoutSimulation> washouTimes)
        {
            MixerDTO = mixerDTO;
            EquipmentType = Enums.HCEnums.Enums.ProccesEquipmentType.Mixer;

            WashouTimes = washouTimes;

        }
        public Amount TransferTime { get; set; } = new(TimeUnits.Minute);
        public override void Init()
        {


        }
        public bool HasProcessConnected => ConnectedOutletEquipments.Any(x => x.ConnectedOutletEquipments.Any());
        public void Init(NewSimulation simulation)
        {
            var plannedMixer = simulation.Planned.
                PlannedMixers.FirstOrDefault(x => x.MixerId == MixerDTO.Id);
            var PlannedLines = simulation.ScheduledLines.ToList();
            MixerState = new MixerStateAvailable(this);
            Init();
            if (plannedMixer == null) return;
            CurrentDate = simulation.Planned.InitDate!.Value;
            MixerState = new MixerStateAvailable(this);
            if (plannedMixer.CurrentMixerState != CurrentMixerState.None)
            {
                if (!PlannedLines.Any(x => x.ProcessInletEquipments.
                Any(x => x.ProcessInletEquipments.Any(x => x.Id == plannedMixer.ProducingTo!.Id)))) return;

                CurrentLevel = plannedMixer.MixerLevel;
                ProducingTo = simulation.WIPMixerTank.First(x => x.Id == plannedMixer.ProducingTo!.Id);


                CurrentBackBoneSimulation = (BackBoneSimulation)MaterialSimulations.FirstOrDefault(x => x.Id == plannedMixer.BackBone.Id)!;
                SetCurrentMaterialSimulation(CurrentBackBoneSimulation);

                if (plannedMixer.CurrentMixerState == CurrentMixerState.Batching)
                {
                    var Step = CurrentBackBoneSimulation.StepSimulations.FirstOrDefault(x => x.Id == plannedMixer.BackBoneStep.Id)!;
                    InitBatchDate = simulation.InitDate;
                    CurrentTime = new(TimeUnits.Minute);
                    MixerState = new MixerStateBatching(this, Step);

                }
                else
                {
                    InitBatchDate = simulation.InitDate;
                    EndBatchDate = simulation.InitDate;
                    InitBatchTransferDate = simulation.InitDate;
                    MixerState = new MixerStateTransfering(this);

                }
                ProducingTo.SetInitFromMixer(this);
            }

        }


        public DateTime CurrentDate { get; private set; }
        public override void Calculate(DateTime currentdate)
        {
            CurrentDate = currentdate;
            MixerState.Calculate();
            if (MixerState is not MixerStateAvailable)
                CurrentTime += OneSecond;

        }


        public void InitBatch(BackBoneSimulation backBoneSimulation, DateTime init)
        {
            InitBatchDate = init;
            CurrentDate = init;
            CurrentTime = new(TimeUnits.Minute);
            CurrentBackBoneSimulation = backBoneSimulation;
            SetCurrentMaterialSimulation(backBoneSimulation);
            if (backBoneSimulation.BatchDataMixer.Any(x => x.Key.Id == Id))
            {
                var searchCapacity = backBoneSimulation.BatchDataMixer[this].Capacity;
                MixerState = new MixerStateBatching(this);
            }
            else
            {
                MixerState = new MixerStateCapacityNotFound(this);
            }


        }



        public Amount GetNextWashoutTime(MaterialSimulation nextMaterial)
        {
            if (CurrentBackBoneSimulation == null) return new Amount(TimeUnits.Minute);
            var found = WashouTimes.FirstOrDefault(x =>
                x.ProductCategoryCurrent == CurrentBackBoneSimulation.ProductCategory
                && x.ProductCategoryNext == nextMaterial.ProductCategory);

            return found == null ? new Amount(TimeUnits.Minute) : found.MixerWashoutTime;
        }
        public MixerResults CurrentMixerResult { get; private set; } = null!;
        public void AddMixerResult(string state)
        {
            CurrentMixerResult = new(this, state);
            MixerResults.Add(CurrentMixerResult);
        }

        public void AddMassToMixer(Amount mass)
        {
            CurrentLevel += mass;
        }




    }


}
