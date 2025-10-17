using Simulator.Shared.NuevaSimlationconQwen.States.BaseClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.Shared.NuevaSimlationconQwen.Equipments.Tanks.States
{
    public abstract class RawMaterialTankInletState : InletState<ProcessRawMaterialTank>
    {
        protected ProcessRawMaterialTank _tank { get; set; }

        public RawMaterialTankInletState(ProcessRawMaterialTank tank) : base(tank)
        {
            _tank = tank;
        }
    }
    public class RawMaterialTankInletInitialState : RawMaterialTankInletState, ITankOuletStarved
    {



        public RawMaterialTankInletInitialState(ProcessRawMaterialTank tank) : base(tank)
        {

            StateLabel = $"{tank.Name} Initial State";
            AddTransition<RawMaterialTankInletLolevelState>();
        }

    }
    public class RawMaterialTankInletLolevelState : RawMaterialTankInletState, ITankOuletStarved
    {



        public RawMaterialTankInletLolevelState(ProcessRawMaterialTank tank) : base(tank)
        {

            StateLabel = $"{tank.Name} available";
            AddTransition<RawMaterialTankInletFillingState>(tank=> tank.IsTankInLoLoLevel());
        }

    }
    public class RawMaterialTankInletFillingState : RawMaterialTankInletState, ITankOuletStarved
    {



        public RawMaterialTankInletFillingState(ProcessRawMaterialTank tank) : base(tank)
        {

            StateLabel = $"{tank.Name} filling State";
            AddTransition<RawMaterialTankInletLolevelState>(tank => tank.FillingRawMaterialTank());
        }

    }
}
