using FrooxEngine;
using FrooxEngine.LogiX.References;
using JworkzNeosMod.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace JworkzNeosMod.Events
{
    internal class Utf8ImportFailEventArgs : Utf8ImportStartEventArgs
    {
        public string ErrorMessage { get; }
        public  Utf8ImportFailEventArgs(User allocatingUser, FileId id, string fileTypeName, long byteSize, string errMessage) : base(allocatingUser, id, fileTypeName, byteSize)
        {
            ErrorMessage = errMessage;
        }
    }
}