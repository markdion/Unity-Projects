﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.NPC
{
    public class AccessPointAvailableEventArgs : EventArgs
    {
        public bool removedNpc { get; set; }
    }
}