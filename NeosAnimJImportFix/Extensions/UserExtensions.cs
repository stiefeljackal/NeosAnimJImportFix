using FrooxEngine;

namespace JworkzNeosMod.Extensions
{
    internal static class UserExtensions
    {
        public static User GetAllocatingUser(this IWorldElement worldEl)
        {
            ulong position;
            byte userInBytes;
            worldEl.ReferenceID.ExtractIDs(out position, out userInBytes);
            return worldEl.World.GetUserByAllocationID(userInBytes);
        }
    }
}
