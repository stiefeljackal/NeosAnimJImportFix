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
    internal class Utf8ImportStartEventArgs
    {
        public World World => AllocatingUser?.World;

        public User AllocatingUser { get; }

        public string FileTypeName { get; }

        public FileId Id { get; }

        public long ByteSize { get; }

        public  Utf8ImportStartEventArgs(User allocatingUser, FileId id, string fileTypeName, long byteSize)
        {
            AllocatingUser = allocatingUser;
            Id = id;
            FileTypeName = fileTypeName;
            ByteSize = byteSize;
        }
    }
}