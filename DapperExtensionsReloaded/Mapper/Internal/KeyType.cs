namespace DapperExtensionsReloaded.Mapper.Internal
{
    /// <summary>
    /// Used by ClassMapper to determine which entity property represents the key.
    /// </summary>
    internal enum KeyType
    {
        /// <summary>
        /// The property is not a key and is not automatically managed.
        /// </summary>
        NotAKey,

        /// <summary>
        /// The property is an integery-based identity generated from the database.
        /// </summary>
        Identity,
        
        /// <summary>
        /// The property is a key that is not automatically managed.
        /// </summary>
        Assigned
    }
}