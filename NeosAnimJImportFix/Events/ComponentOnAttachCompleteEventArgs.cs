using FrooxEngine;

namespace JworkzNeosMod.Events
{
    internal class ComponentOnAttachCompleteEventArgs
    {
        public ComponentBase<Component> Component { get; private set; }

        public Slot Slot => Component.Parent as Slot;


        public ComponentOnAttachCompleteEventArgs(ComponentBase<Component> component)
        {
            Component = component;
        }
    }
}
