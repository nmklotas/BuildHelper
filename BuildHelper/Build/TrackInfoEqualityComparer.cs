using System;
using System.Collections.Generic;

namespace BuildHelper.Build
{
    internal class TrackInfoEqualityComparer : EqualityComparer<TrackInfo>
    {
        public override bool Equals(TrackInfo x, TrackInfo y)
        {
            if (x == null && y == null)
                return true;
            else if (x == null || y == null)
                return false;

            return x.Id.Equals(y.Id, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode(TrackInfo obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
