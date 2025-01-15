using MessagePack;
using System.Collections.Generic;

namespace StudioCustomLayerSwitcher
{
    [MessagePackObject]
    public class StudioLayerResolveInfo
    {
        

        [Key("dicKey")]
        public int dicKey { get; set; }

        [Key("clothesLayers")]
        public List<int> clothesLayers;

        [Key("accessoryLayers")]
        public List<int> accessoryLayers;

        public StudioLayerResolveInfo() 
        {
            clothesLayers = new List<int>();
            accessoryLayers = new List<int>();
        }

        internal static StudioLayerResolveInfo Deserialize(byte[] data)
        {
            return MessagePackSerializer.Deserialize<StudioLayerResolveInfo>(data);
        }


        internal byte[] Serialize()
        {
            return MessagePackSerializer.Serialize<StudioLayerResolveInfo>(this);
        }
    }
}
