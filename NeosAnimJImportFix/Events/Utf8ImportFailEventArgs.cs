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
    internal class Utf8ImportFailEventArgs : Utf8ImportStartEventArgs
    {
        public string ErrorMessage { get; }
        public  Utf8ImportFailEventArgs(World world, FileId id, string fileTypeName, long byteSize, string errMessage) : base(world, id, fileTypeName, byteSize)
        {
            ErrorMessage = errMessage;
        }
    }
}