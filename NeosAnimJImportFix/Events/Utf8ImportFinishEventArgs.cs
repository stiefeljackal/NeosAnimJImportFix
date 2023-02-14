using FrooxEngine;
using JworkzNeosMod.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace JworkzNeosMod.Events
{
    internal class Utf8ImportFinishEventArgs : Utf8ImportStartEventArgs
    {
        public  Utf8ImportFinishEventArgs(User allocatingUser, FileId id, string fileTypeName, long byteSize) : base(allocatingUser, id, fileTypeName, byteSize) { }
    }
}