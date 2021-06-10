using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
//using .IntDoublePair; 

namespace NorthOBD.ReaderOSU
{
   
    
     //Класс читает битмапу из потока
    class Beatmap
    {

        public long PositionByte;
        //Куча параметров. Смотерть https://osu.ppy.sh/wiki/ru/osu!_File_Formats/Db_(file_format)
        public string ArtistName;
        public string ArtistNameUC;
        public string SongName;
        public string SongNameUC;
        public string CreaterName;
        public string Difficulty;
        public string NameOfAudio;
        public string MD5Hash;
        public string NameOfOsuFile;
        public byte RankStatus;
        public ushort NumberOfHitcircles;
        public ushort NumberOfSliders;
        public ushort NumberOfSpinners;
        public ulong LastModification;
        public float ApproachRate;
        public float CircleSize;
        public float DrainHP;
        public float OverallDifficulty;
        public double SliderVelocity;
        public IntDouble[] StandardOsuStar;
        public IntDouble[] TaikoOsuStar;
        public IntDouble[] StbOsuStar;
        public IntDouble[] ManiaOsuStar;
        public uint DrainTime;
        public uint TotalTime;
        public uint PreviewTime;
        public TimePoint[] TimePoints;
        public uint BeatmapID;
        public uint BeatmapSetID;
        public uint ThreadID;
        public byte GradeAchievedStd;
        public byte GradeAchievedTaiko;
        public byte GradeAchievedCTB;
        public byte GradeAchievedMania;
        public ushort Offset;
        public float StackLeniency;
        public byte GameplayMode;
        public string SongSource;
        public string SongTags;
        public ushort OnlineOffset;
        public string FontTitle;
        public bool IsUnplayed;
        public ulong LastTimePlayed;
        public bool Osz2;
        public string SongFolder;
        public ulong LastChecked;
        public bool IgnoreSound;
        public bool IgnoreSkin;
        public bool DisableStoryboard;
        public bool DisableVideo;
        public bool VisualOverride;
        public uint ModificationTime;
        public byte ManiaScrollSpeed;


        public Beatmap()
        {
        }

        //Читаем след. битмапу с переносом указателя
        public void ReadBeatmap(ref BinaryReader readerDb)
        {
            PositionByte = readerDb.BaseStream.Position;
            ArtistName = ReadString(ref readerDb);
            ArtistNameUC = ReadString(ref readerDb);
            SongName = ReadString(ref readerDb);
            SongNameUC = ReadString(ref readerDb);
            CreaterName = ReadString(ref readerDb);
            Difficulty = ReadString(ref readerDb);
            NameOfAudio = ReadString(ref readerDb);
            MD5Hash = ReadString(ref readerDb);
            NameOfOsuFile = ReadString(ref readerDb);
            RankStatus = readerDb.ReadByte();
            NumberOfHitcircles = readerDb.ReadUInt16();
            NumberOfSliders = readerDb.ReadUInt16();
            NumberOfSpinners = readerDb.ReadUInt16();
            LastModification = readerDb.ReadUInt64();
            ApproachRate = readerDb.ReadSingle();
            CircleSize = readerDb.ReadSingle();
            DrainHP = readerDb.ReadSingle();
            OverallDifficulty = readerDb.ReadSingle();
            SliderVelocity = readerDb.ReadDouble();
            StandardOsuStar = new IntDoublePair(ref readerDb).GetIntDouble();
            TaikoOsuStar = new IntDoublePair(ref readerDb).GetIntDouble();
            StbOsuStar = new IntDoublePair(ref readerDb).GetIntDouble();
            ManiaOsuStar = new IntDoublePair(ref readerDb).GetIntDouble();
            DrainTime = readerDb.ReadUInt32();
            TotalTime = readerDb.ReadUInt32();
            PreviewTime = readerDb.ReadUInt32();
            TimePoints = new TimePointPlus(ref readerDb).GetTimePoints();
            BeatmapID = readerDb.ReadUInt32();
            BeatmapSetID = readerDb.ReadUInt32();
            ThreadID = readerDb.ReadUInt32();
            GradeAchievedStd = readerDb.ReadByte();
            GradeAchievedTaiko = readerDb.ReadByte();
            GradeAchievedCTB = readerDb.ReadByte();
            GradeAchievedMania = readerDb.ReadByte();
            Offset = readerDb.ReadUInt16();
            StackLeniency = readerDb.ReadSingle();
            GameplayMode = readerDb.ReadByte();
            SongSource = ReadString(ref readerDb);
            SongTags = ReadString(ref readerDb);
            OnlineOffset = readerDb.ReadUInt16();
            FontTitle = ReadString(ref readerDb);
            IsUnplayed = readerDb.ReadBoolean();
            LastTimePlayed = readerDb.ReadUInt64();
            Osz2 = readerDb.ReadBoolean();
            SongFolder = ReadString(ref readerDb);
            LastChecked = readerDb.ReadUInt64();
            IgnoreSound = readerDb.ReadBoolean();
            IgnoreSkin = readerDb.ReadBoolean();
            DisableStoryboard = readerDb.ReadBoolean();
            DisableVideo = readerDb.ReadBoolean();
            VisualOverride = readerDb.ReadBoolean();
            ModificationTime = readerDb.ReadUInt32();
            ManiaScrollSpeed = readerDb.ReadByte();
        }


        //Читаем битмапу из файла начинающегося с position (номер бита)
        public void ReadBeatmap(ref BinaryReader readerDb, long position)
        {
            long savePosition = readerDb.BaseStream.Position;
            readerDb.BaseStream.Position = position;
            ArtistName = ReadString(ref readerDb);
            ArtistNameUC = ReadString(ref readerDb);
            SongName = ReadString(ref readerDb);
            SongNameUC = ReadString(ref readerDb);
            CreaterName = ReadString(ref readerDb);
            Difficulty = ReadString(ref readerDb);
            NameOfAudio = ReadString(ref readerDb);
            MD5Hash = ReadString(ref readerDb);
            NameOfOsuFile = ReadString(ref readerDb);
            RankStatus = readerDb.ReadByte();
            NumberOfHitcircles = readerDb.ReadUInt16();
            NumberOfSliders = readerDb.ReadUInt16();
            NumberOfSpinners = readerDb.ReadUInt16();
            LastModification = readerDb.ReadUInt64();
            ApproachRate = readerDb.ReadSingle();
            CircleSize = readerDb.ReadSingle();
            DrainHP = readerDb.ReadSingle();
            OverallDifficulty = readerDb.ReadSingle();
            SliderVelocity = readerDb.ReadDouble();
            StandardOsuStar = new IntDoublePair(ref readerDb).GetIntDouble();
            TaikoOsuStar = new IntDoublePair(ref readerDb).GetIntDouble();
            StbOsuStar = new IntDoublePair(ref readerDb).GetIntDouble();
            ManiaOsuStar = new IntDoublePair(ref readerDb).GetIntDouble();
            DrainTime = readerDb.ReadUInt32();
            TotalTime = readerDb.ReadUInt32();
            PreviewTime = readerDb.ReadUInt32();
            TimePoints = new TimePointPlus(ref readerDb).GetTimePoints();
            BeatmapID = readerDb.ReadUInt32();
            BeatmapSetID = readerDb.ReadUInt32();
            ThreadID = readerDb.ReadUInt32();
            GradeAchievedStd = readerDb.ReadByte();
            GradeAchievedTaiko = readerDb.ReadByte();
            GradeAchievedCTB = readerDb.ReadByte();
            GradeAchievedMania = readerDb.ReadByte();
            Offset = readerDb.ReadUInt16();
            StackLeniency = readerDb.ReadSingle();
            GameplayMode = readerDb.ReadByte();
            SongSource = ReadString(ref readerDb);
            SongTags = ReadString(ref readerDb);
            OnlineOffset = readerDb.ReadUInt16();
            FontTitle = ReadString(ref readerDb);
            IsUnplayed = readerDb.ReadBoolean();
            LastTimePlayed = readerDb.ReadUInt64();
            Osz2 = readerDb.ReadBoolean();
            SongFolder = ReadString(ref readerDb);
            LastChecked = readerDb.ReadUInt64();
            IgnoreSound = readerDb.ReadBoolean();
            IgnoreSkin = readerDb.ReadBoolean();
            DisableStoryboard = readerDb.ReadBoolean();
            DisableVideo = readerDb.ReadBoolean();
            VisualOverride = readerDb.ReadBoolean();
            ModificationTime = readerDb.ReadUInt32();
            ManiaScrollSpeed = readerDb.ReadByte();
            readerDb.BaseStream.Position = savePosition;
        }

        //Быстрый скип битмапы
        public void SkipBeatmap(ref BinaryReader readerDb)
        {
            //скип битмапы, удобно, православно, но без реализации
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
