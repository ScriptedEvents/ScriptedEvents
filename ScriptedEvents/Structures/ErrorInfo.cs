namespace ScriptedEvents.Structures
{
    public struct ErrorInfo
    {
        public ErrorInfo(int id, string info, string longInfo)
        {
            Id = id;
            Info = info;
            LongDescription = longInfo;
        }

        public int Id { get; }

        public string Info { get; }

        public string LongDescription { get; }

        public override string ToString()
        {
            return $"{Info} [Error Code: SE-{Id}] [Run 'shelp SE-{Id}' in server console for more details]";
        }
    }
}
