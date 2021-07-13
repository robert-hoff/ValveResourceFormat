using System;
using System.Collections.Generic;
using System.Linq;
using MyValveResourceFormat.Serialization;

namespace MyValveResourceFormat.ResourceTypes {
    public class World : KeyValuesOrNTRO {
        public IEnumerable<string> GetEntityLumpNames()
            => Data.GetArray<string>("m_entityLumps");

        public IEnumerable<string> GetWorldNodeNames()
            => Data.GetArray("m_worldNodes")
                .Select(nodeData => nodeData.GetProperty<string>("m_worldNodePrefix"))
                .ToList();
    }
}
