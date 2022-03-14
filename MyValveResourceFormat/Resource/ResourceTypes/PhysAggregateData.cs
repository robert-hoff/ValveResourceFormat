using System;
using MyValveResourceFormat.Blocks;

namespace MyValveResourceFormat.ResourceTypes {
    public class PhysAggregateData : KeyValuesOrNTRO {
        public PhysAggregateData() {
        }

        public PhysAggregateData(BlockType type) : base(type, "VPhysXAggregateData_t") {
        }
    }
}