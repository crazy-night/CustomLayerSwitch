using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioCustomLayerSwitch
{
    internal class StudioLayerResolveInfo
    {
        

        [Key("dicKey")]
        public int dicKey { get; set; }

        [Key("layers")]
        public List<int> layers { get; set; }

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
