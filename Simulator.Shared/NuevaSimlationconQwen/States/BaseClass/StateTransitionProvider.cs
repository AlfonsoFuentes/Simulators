using Simulator.Shared.NuevaSimlationconQwen.Equipments;

namespace Simulator.Shared.NuevaSimlationconQwen.States.BaseClass
{
    //public class StateTransitionProvider<T, TState> : IStateTransitionProvider<T>
    // where T : IEquipment
    // where TState : IEquipmentState
    //{
    //    private readonly List<ITransitionFactory<T, TState>> _factories;
    //    private readonly Action<T, TState> _transitionAction;

    //    public StateTransitionProvider(
    //        List<ITransitionFactory<T, TState>> factories,
    //        Action<T, TState> transitionAction)
    //    {
    //        _factories = factories;
    //        _transitionAction = transitionAction;
    //    }

    //    public void ReviewChangeState(T equipment, DateTime currentdate)
    //    {
    //        var factory = _factories.FirstOrDefault(f => f.CanHandle(equipment, currentdate));
    //        if (factory != null)
    //        {
    //            var newState = factory.Create(equipment, currentdate);
    //            _transitionAction(equipment, newState); // ← Sin if — acción directa
    //        }
    //    }
    //}

}
