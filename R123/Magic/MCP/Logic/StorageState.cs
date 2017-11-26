namespace MCP.Logic
{
    class StorageState
    {
        public StorageState(bool create,bool remove)
        {
            Create = create;
            Remove = remove;
        }

        public bool Create { get; set; }
        public bool Remove { get; set; }
    }
}
