using FrooxEngine;
using Moq;
using System;
using System.Linq;
using System.Reflection;
using Xunit;
using NeosAutoAttachIsPlayingDriver.Extensions;
using BepuPhysics;
using HarmonyLib;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeosAutoAttachIsPlayingDriver.Test
{
    interface ITest
    {
        ISyncBoolMock EnabledField { get; set; }
    }
    interface ISyncBoolMock
    {
        bool IsDriven { get; set;}
    }
    public class UnitTest1
    {
        [Fact(DisplayName = "Should ignore already driven Enabled field.")]
        public void IgnoreDrivenEnabledField()
        {
            WorkerInitializer.Initialize(new List<Type> { typeof(Connector<Slot>), typeof(Slot), typeof(World), typeof(Worker) });
            var mockWorld = AccessTools.CreateInstance(typeof(World)) as World;
            mockWorld.Parent = mockWorld;
            AccessTools.Property(typeof(World), "ReferenceController").SetValue(mockWorld, new ReferenceController(mockWorld));
            AccessTools.Property(typeof(World), "ConnectorManager").SetValue(mockWorld, new ConnectorManager(mockWorld));
            var mockSlot = AccessTools.CreateInstance(typeof(Slot)) as Slot;

            var bbb = AccessTools.Field(typeof(Worker), "InitInfo").GetValue(mockSlot);
            var ccc = AccessTools.Method(typeof(WorkerInitializer), "GetInitInfo", new Type[] { typeof(IWorker) }).Invoke(null, new object[] { mockSlot });
            AccessTools.Field(typeof(Worker), "InitInfo").SetValue(mockSlot, ccc);
            var aaa = mockSlot.SyncMemberCount;
            AccessTools.Method(typeof(Slot), "Initialize", new Type[] { typeof(World), typeof(bool) }).Invoke(mockSlot, new object[] { mockWorld, true });
            AccessTools.Field(typeof(Slot), "ParentReference").SetValue(mockSlot, new SyncRef<Slot>());
            //AccessTools.Property(typeof(Worker), "World").SetValue(mockSlot, mockWorld);
            var mockAudioOutputComp = mockSlot.AttachComponent<AudioOutput>();
            //mockAudioOutputComp.As<ITest>()
            //    .Setup(ao => ao.EnabledField.IsDriven)
            //    .Returns(true);
            var i = mockAudioOutputComp;
            (mockSlot as Slot).CopyComponent(i);

            (mockSlot as Slot).AddAudioBufferPrevention();

            Assert.DoesNotContain(typeof(IsPlayingDriver), (mockSlot as Slot).Components.Select(c => c.GetType()));
        }
    }
}
