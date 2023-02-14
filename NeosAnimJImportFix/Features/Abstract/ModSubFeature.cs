using NeosModLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JworkzNeosMod.Features.Abstract
{
    internal interface IModSubFeature
    {
        bool IsEnabled { get; }
        void Refresh(bool isMainModEnabled);
    }
}
