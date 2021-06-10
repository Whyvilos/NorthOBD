using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NorthOBD.ReaderCollection
{
    class Collection
    {

        public string NameCollection;
        public uint NumberOfBMInCollection;
        public string[] MD5Hash;


        public void ReadCollection(ref BinaryReader readerDb)
        {
            NameCollection = ReadString(ref readerDb);
            NumberOfBMInCollection = readerDb.ReadUInt32();
            MD5Hash = new string[NumberOfBMInCollection];
            for (uint index=0; index<NumberOfBMInCollection; index++) 
            {
                MD5Hash[index] = ReadString(ref readerDb);
            }
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
