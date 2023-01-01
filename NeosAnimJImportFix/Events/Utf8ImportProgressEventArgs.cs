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
    internal class Utf8ImportProgressEventArgs : Utf8ImportStartEventArgs
    {
        public long ReadSize { get; }

        public float Percent => (float)Math.Round(ReadSize / (double)ByteSize * 100d);

        public  Utf8ImportProgressEventArgs(World world, FileId id, string fileTypeName, long byteSize, long readSize) : base(world, id, fileTypeName, byteSize)
        {
            ReadSize = readSize;
        }
    }
}