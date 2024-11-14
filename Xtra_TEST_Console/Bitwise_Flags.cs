namespace AAA_TEST_Console
{
    internal class Bitwise_Flags
    {
        public enum StateFlag
        {
            New = 0x01,
            Modified = 0x02,
            Deleted = 0x04,
            InErrornousState = 0x08,
            Stored = 0x10,
            NotInitialized = 0x20,
            ForgetOnSave = 0x40,
            FromEditingCache = 0x80,
            History = 0x100,
            Readonly = 0x200,
            ResolvingConflict = 0x400,
            ReCloned = 0x800,
            Locked = 0x1000
        }

        public StateFlag SetFlag(StateFlag flag1, StateFlag flag2)
        {
            return (flag1 &= ~flag2);
        }
    }
}
