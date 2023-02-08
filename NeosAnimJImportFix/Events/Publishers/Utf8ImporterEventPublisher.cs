using FrooxEngine;
using JworkzNeosMod.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JworkzNeosMod.Events.Publishers
{
    internal static class Utf8ImporterEventPublisher
    {
        public static event EventHandler<Utf8ImportStartEventArgs> OnImportStart;

        public static event EventHandler<Utf8ImportProgressEventArgs> OnImportProgress;

        public static event EventHandler<Utf8ImportFinishEventArgs> OnImportFinish;

        public static event EventHandler<Utf8ImportFailEventArgs> OnImportFail;

        public static void RaiseOnImportStartEvent(object source, World world, User user, FileId id, string fileTypeName, long byteSize) =>
            OnImportStart?.Invoke(source, new Utf8ImportStartEventArgs(world, user, id, fileTypeName, byteSize));

        public static void RaiseOnImportProgressEvent(object source, World world, FileId id, string fileTypeName, long byteSize, long readSize) =>
            OnImportProgress?.Invoke(source, new Utf8ImportProgressEventArgs(world, id, fileTypeName, byteSize, readSize));

        public static void RaiseOnImportFinishEvent(object source, World world, FileId id, string fileTypeName, long byteSize) =>
            OnImportFinish?.Invoke(source, new Utf8ImportFinishEventArgs(world, id, fileTypeName, byteSize));

        public static void RaiseOnImportFailEvent(object source, World world, FileId id, string fileTypeName, long byteSize, string errMessage) =>
            OnImportFail?.Invoke(source, new Utf8ImportFailEventArgs(world, id, fileTypeName, byteSize, errMessage));
    }
}
