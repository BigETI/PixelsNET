using System.Runtime.Serialization;

/// <summary>
/// Pixels .NET namespace
/// </summary>
namespace PixelsNET
{
    /// <summary>
    /// Pixels asset data contract
    /// </summary>
    [DataContract]
    public class PixelsAssetDataContract
    {
        /// <summary>
        /// Type of asset
        /// </summary>
        [DataMember]
        private string type;

        /// <summary>
        /// Entry name
        /// </summary>
        [DataMember]
        private string entry;

        /// <summary>
        /// Type of asset
        /// </summary>
        public string Type
        {
            get
            {
                if (type == null)
                {
                    type = "";
                }
                return type;
            }
        }

        /// <summary>
        /// Entry name
        /// </summary>
        public string Entry
        {
            get
            {
                if (entry == null)
                {
                    entry = "";
                }
                return entry;
            }
        }
    }
}
