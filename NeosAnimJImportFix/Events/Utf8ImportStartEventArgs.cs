﻿using FrooxEngine;
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
        public World World { get; }

        public User User { get; }

        public string FileTypeName { get; }

        public FileId Id { get; }

        public long ByteSize { get; }

        public  Utf8ImportStartEventArgs(World world, User user, FileId id, string fileTypeName, long byteSize)
        {
            World = world;
            User = user;
            Id = id;
            FileTypeName = fileTypeName;
            ByteSize = byteSize;
        }
    }
}