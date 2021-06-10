using System;
using System.Collections.Generic;
using System.Text;

namespace NorthOBD.NorthOsuFileCreator
{
    
    


    class NorthOsuFileCreator
    {

        public string NameCreatorfFile;
        public uint NumberOfCollections = 0;
        public List<NorthCollection> Colletions;

        public NorthOsuFileCreator()
        {
        }

        public NorthOsuFileCreator(string Name)
        {
            NameCreatorfFile = Name;
            Colletions = new List<NorthCollection>();
        }

        public void AddColletion(NorthCollection collection)
        {
            //Array.Resize(ref Colletions, Colletions.Length + 1);
            Colletions.Add(collection);
            NumberOfCollections = (uint)Colletions.Count;

//            Colletions[Colletions.Length - 1].NameCollection; 
        }


        public void DeleteColletion(int index)
        {
            Colletions.RemoveAt(index);
            NumberOfCollections = (uint)Colletions.Count;
        }

    }
}
