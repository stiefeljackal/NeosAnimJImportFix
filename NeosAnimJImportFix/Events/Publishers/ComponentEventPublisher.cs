using FrooxEngine;
using NeosModLoader;
using System;
using System.Collections.Generic;

namespace JworkzNeosMod.Events.Publishers
{
    internal class ComponentEventPublisher
    {
        private static Dictionary<Type, ComponentEventPublisher> _registry = new Dictionary<Type, ComponentEventPublisher>();

        private EventHandler<ComponentOnAttachCompleteEventArgs> _onAttachComplete;
        public event EventHandler<ComponentOnAttachCompleteEventArgs> OnAttachComplete
        {
            add
            {
                if (!IsTypeRegistered(ComponentType))
                {
                    RegisterComponentEventPublisher(ComponentType, this);
                }
                if (this != GetComponentEventPublisherFromRegistry(ComponentType))
                {
                    return;
                }
                _onAttachComplete += value;
            }
            remove
            {
                if (_onAttachComplete == null) { return; }

                _onAttachComplete -= value;
                if (_onAttachComplete?.GetInvocationList().Length <= 0)
                {
                    UnregisterComponentEventPublisher(ComponentType);
                }
            }
        }

        public Type ComponentType { get; private set; }

        public ComponentEventPublisher(Type componentType)
        {
            ComponentType = componentType;
        }

        public static bool IsTypeRegistered(Type componentType) =>
            _registry.ContainsKey(componentType);

        public static void RaiseOnAttachCompleteEvent<T>(T component) where T : ComponentBase<Component>
        {
            var publisher = GetComponentEventPublisherFromRegistry(component.WorkerType);
            publisher?._onAttachComplete?.Invoke(component, new ComponentOnAttachCompleteEventArgs(component));
        }

        public static ComponentEventPublisher GetComponentEventPublisherFromRegistry<T>() =>
            GetComponentEventPublisherFromRegistry(typeof(T));
        public static ComponentEventPublisher GetComponentEventPublisherFromRegistry(Type componentType) =>
            IsTypeRegistered(componentType) ? _registry[componentType] : null;
        public static ComponentEventPublisher RegisterComponentEventPublisher<T>(ComponentEventPublisher publisher = null) =>
            RegisterComponentEventPublisher(typeof(T), publisher);

        public static ComponentEventPublisher RegisterComponentEventPublisher(Type componentType, ComponentEventPublisher publisher = null) =>
            IsTypeRegistered(componentType)
                ? _registry[componentType]
                : _registry[componentType] = publisher ?? new ComponentEventPublisher(componentType);

        public static bool UnregisterComponentEventPublisher<T>() =>
            UnregisterComponentEventPublisher(typeof(T));

        public static bool UnregisterComponentEventPublisher(Type componentType) =>
            _registry.Remove(componentType);
    }
}
