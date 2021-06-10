using System;
using System.Collections.Generic;
using System.Text;

namespace NorthOBD.NorthOsuFileCreator
{


    struct BMShortInfo
    {
        public string Difficulty;
        public string MD5Hash;
    }

    struct BMSetShortInfo
    {
        public string LinkSet;
        public string ArtistName;
        public string SongName;
        public string CreaterName;
        public uint NumberDif;
        public List<BMShortInfo> DifficultySet;
    }

    class NorthCollection //: NorthOBD.ReaderCollection.Collection
    {
        public string NameCollection;
        public uint NumberOfBMInCollection;
        public List<BMSetShortInfo> BMsCollection;

       
        public NorthCollection()
        {

        }
        


    }
}
