using System;
using System.Runtime.Serialization;

/// <summary>
/// Pixels .NET namespace
/// </summary>
namespace PixelsNET
{
    /// <summary>
    /// Pixels data contract
    /// </summary>
    [DataContract]
    public class PixelsDataContract
    {
        /// <summary>
        /// UUID
        /// </summary>
        [DataMember]
        private string uuid;

        /// <summary>
        /// Name
        /// </summary>
        [DataMember]
        private string name;

        /// <summary>
        /// Description
        /// </summary>
        [DataMember]
        private string description;

        /// <summary>
        /// Author
        /// </summary>
        [DataMember]
        private string author;

        /// <summary>
        /// Tag
        /// </summary>
        [DataMember]
        private string tag;

        /// <summary>
        /// Assets
        /// </summary>
        [DataMember]
        private PixelsAssetDataContract[] assets;

        /// <summary>
        /// UUID
        /// </summary>
        public string UUID
        {
            get
            {
                if (uuid == null)
                {
                    uuid = Guid.NewGuid().ToString();
                }
                return uuid;
            }
        }

        /// <summary>
        /// Name
        /// </summary>
        public string Name
        {
            get
            {
                if (name == null)
                {
                    name = "";
                }
                return name;
            }
        }

        /// <summary>
        /// Description
        /// </summary>
        public string Description
        {
            get
            {
                if (description == null)
                {
                    description = "";
                }
                return description;
            }
        }

        /// <summary>
        /// Author
        /// </summary>
        public string Author
        {
            get
            {
                if (author == null)
                {
                    author = "";
                }
                return author;
            }
        }

        /// <summary>
        /// Tag
        /// </summary>
        public string Tag
        {
            get
            {
                if (tag == null)
                {
                    tag = "";
                }
                return tag;
            }
        }

        /// <summary>
        /// Assets
        /// </summary>
        public PixelsAssetDataContract[] Assets
        {
            get
            {
                if (assets == null)
                {
                    assets = new PixelsAssetDataContract[0];
                }
                return assets;
            }
        }
    }
}
