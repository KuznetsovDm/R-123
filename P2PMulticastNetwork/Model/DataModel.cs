using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace P2PMulticastNetwork.Model
{
    [Serializable]
    public class DataModel
    {
        public byte[] RawAudioSample { get; set; }

        public Guid Guid { get; set; }
    }

    public class DataModelConverter : IDataAsByteConverter<DataModel>
    {
        private BinaryFormatter _formatter;

        public DataModelConverter()
        {
            _formatter = new BinaryFormatter();
        }

        public DataModel ConvertFrom(byte[] data)
        {
            DataModel model = null;
            using(var ms = new MemoryStream(data))
            {
                model = (DataModel)_formatter.Deserialize(ms);
            }
            return model;
        }

        public byte[] ConvertToBytes(DataModel data)
        {
            byte[] bytes = null;
            using(var ms = new MemoryStream())
            {
                _formatter.Serialize(ms, data);
                bytes = ms.ToArray();
            }
            return bytes;
        }
    }
}
