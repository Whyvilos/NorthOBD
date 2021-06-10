using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NorthOBD.ReaderCollection
{
    class ReaderCollectionDB
    {

        public uint Version;
        public uint NumberOfCollections;
        public Collection[] Collections;

        public ReaderCollectionDB(ref BinaryReader reader)
        {
            ReadAllBd(ref reader);
        }

        private void ReadAllBd(ref BinaryReader reader)
        {
            Version = reader.ReadUInt32();
            NumberOfCollections = reader.ReadUInt32();
            Collections = new Collection[NumberOfCollections];
            for(uint index=0;index<NumberOfCollections; index++)
            {
                Collections[index] = new Collection();
                Collections[index].ReadCollection(ref reader);
            }

        }
    }
}
