using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.Shared.Simulations.Tanks
{
    public abstract class TankState
    {
       
        public TankState()
        {
           
        }
       


    }
    public class TankProducing : TankState
    {
        public TankProducing():base()
        {
        }
    }
    public class TankLoLevel : TankState
    {
        public TankLoLevel() : base()
        {
        }
    }

    public class TankChangingSKU : TankState
    {
        public TankChangingSKU() : base()
        {
        }
    }
    public class TankAvailable : TankState
    {
        public TankAvailable() : base()
        {
        }
    }

}
