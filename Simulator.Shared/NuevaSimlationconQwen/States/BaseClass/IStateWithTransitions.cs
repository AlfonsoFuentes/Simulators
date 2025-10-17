using Simulator.Shared.NuevaSimlationconQwen.Equipments;

namespace Simulator.Shared.NuevaSimlationconQwen.States.BaseClass
{
    public abstract class EquipmentState<TContext, TState> : IEquipmentState
     where TContext : IEquipment
     where TState : EquipmentState<TContext, TState>
    {
        protected TContext Context { get; private set; }
        public string StateLabel { get; set; } = string.Empty;


        private readonly List<(int Order, Func<TContext, bool> condition, Func<TContext, TState> factory)> _transitions = new();

        protected EquipmentState(TContext context)
        {
            Context = context;
        }

        // ✅ Ciclo de vida (compatible con tu Calculate actual)
        public virtual void Calculate(DateTime currentdate)
        {
            try
            {
                BeforeRun(currentdate);
                Run(currentdate);
                AfterRun(currentdate);

                BeforeCheckStatus(currentdate);
                CheckStatus(currentdate);
                AfterCheckStatus(currentdate);

                BeforeReport(currentdate);
                Report(currentdate);
                AfterReport(currentdate);
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }
        }

        // ✅ Métodos del ciclo (override en estados concretos)
        public virtual void Run(DateTime currentdate) { }
        public virtual void Report(DateTime currentdate) { }

        // ✅ CheckStatus con evaluación de transiciones
        public virtual void CheckStatus(DateTime currentdate)
        {
           var ordered= _transitions.OrderBy(x => x.Order).ToList();
            foreach (var (order, condition, factory) in ordered)
            {
                if (condition(Context))
                {
                    var nextState = factory(Context);
                    OnTransition(nextState);
                    return;
                }
            }
        }

        // ✅ Método abstracto para aplicar transición (implementado en InletState/OutletState)
        protected abstract void OnTransition(TState nextState);

        // ✅ API para definir transiciones dentro del estado
        protected void AddTransition<T>(Func<TContext, bool> condition) where T : TState
        {
            int lastorder = _transitions.Count + 1;
            _transitions.Add((lastorder,
                condition,
                ctx => (T)Activator.CreateInstance(typeof(T), ctx)!
            ));
        }

        protected void AddTransition<T>() where T : TState
        {
            AddTransition<T>(ctx => true);
        }

        // ✅ Hooks protegidos (igual que antes)
        public virtual void BeforeRun(DateTime currentdate) { }
        protected virtual void AfterRun(DateTime currentdate) { }
        protected virtual void BeforeCheckStatus(DateTime currentdate) { }
        protected virtual void AfterCheckStatus(DateTime currentdate) { }
        protected virtual void BeforeReport(DateTime currentdate) { }
        protected virtual void AfterReport(DateTime currentdate) { }
    }
    public abstract class InletState<TContext> : EquipmentState<TContext, InletState<TContext>>
    where TContext : IEquipment
    {
        protected InletState(TContext context) : base(context) { }

        // ✅ Implementación de OnTransition para inlet
        protected override void OnTransition(InletState<TContext> nextState)
        {
            Context.InletState = nextState;
        }
    }
    public abstract class OutletState<TContext> : EquipmentState<TContext, OutletState<TContext>>
    where TContext : IEquipment
    {
        protected OutletState(TContext context) : base(context) { }

        // ✅ Implementación de OnTransition para outlet
        protected override void OnTransition(OutletState<TContext> nextState)
        {
            Context.OutletState = nextState;
        }
    }
}
