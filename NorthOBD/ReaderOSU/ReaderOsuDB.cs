using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NorthOBD.ReaderOSU
{
    

    class ReaderOsuDB
    {
        public uint VersionOsu;
        public uint NumberOfFolder;
        public bool AccountStatus;
        public DateTime TimeBanned;
        public string AccountName;
        public uint NumberOfBM;
        public Beatmap[] Beatmaps;
        public uint UserPermissions;

        public ReaderOsuDB(ref BinaryReader reader)
        {
            ReadAllBd(ref reader);
        }

        private void ReadAllBd(ref BinaryReader reader)
        {
            VersionOsu = reader.ReadUInt32();
            NumberOfFolder = reader.ReadUInt32();
            AccountStatus = reader.ReadBoolean();
            TimeBanned = new DateTime(reader.ReadInt64());
            AccountName = ReadString(ref reader);
            NumberOfBM = reader.ReadUInt32();
            Beatmaps = new Beatmap[NumberOfBM];
            for(uint index = 0; index < NumberOfBM; index++)
            {
                Beatmaps[index] = new Beatmap();
                Beatmaps[index].ReadBeatmap(ref reader);
            }

            //Beatmaps[0] = new Beatmap();
            //Beatmaps[0].ReadBeatmap(ref reader);
            //Beatmaps[1] = new Beatmap();
            //Beatmaps[1].ReadBeatmap(ref reader);
            UserPermissions = reader.ReadUInt32();

        }

        //Чтение string из потока
        public string ReadString(ref BinaryReader reader)
        {
            byte ex = reader.ReadByte();
            string stringU = "";
            if (ex == 11)
            {
                uint len = DecodingUleb128(ref reader);
                long PosLenStart = reader.BaseStream.Position;
                for (uint i = 0; i < len; i++)
                {
                    char ch = reader.ReadChar();
                    stringU += ch;
                    if (reader.BaseStream.Position == PosLenStart + len)
                    {
                        break;
                    }
                }
            }
            return stringU;
        }

        //Преобразования Uleb128 в нормальный int. А зачем вообще использовали Uleb128? 0_о
        private uint DecodingUleb128(ref BinaryReader reader)
        {
            uint byteLeb = (uint)reader.ReadByte();
            uint firstbyte = byteLeb;
            uint bit8 = firstbyte >> 7;
            if (bit8 == 0)
            {
                return (uint)byteLeb;
            }
            bool flag = true;
            uint res = 0;
            int count = 0;
            while (flag)
            {
                bit8 = firstbyte >> 7;
                if (bit8 == 0)
                {
                    flag = false;
                }
                firstbyte &= 127;
                firstbyte <<= 8 * count;
                res <<= 1;
                res ^= firstbyte;
                if (count != 0)
                {
                    res >>= 1;
                }
                if (flag)
                {
                    firstbyte = reader.ReadByte();
                }
                count++;
            }
            return res;
        }


    }
}
