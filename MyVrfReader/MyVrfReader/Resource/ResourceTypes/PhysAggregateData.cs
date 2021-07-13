using System;
using MyVrfReader.Blocks;

namespace MyVrfReader.ResourceTypes {
	public class PhysAggregateData : KeyValuesOrNTRO {
		public PhysAggregateData() {
		}

		public PhysAggregateData(BlockType type) : base(type, "VPhysXAggregateData_t") {
		}
	}
}
