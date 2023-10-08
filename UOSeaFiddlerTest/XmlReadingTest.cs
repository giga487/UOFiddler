// /***************************************************************************
//  *
//  * $Author: Turley
//  * 
//  * "THE BEER-WARE LICENSE"
//  * As long as you retain this notice you can do whatever you want with 
//  * this stuff. If we meet some day, and you think this stuff is worth it,
//  * you can buy me a beer in return.
//  *
//  ***************************************************************************/

using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace UOSeaFiddlerTest
{
    public class Tests
    {
        string pathFile { get; set; }

        [SetUp]
        public void Setup()
        {
            pathFile = @"C:\Users\giga4\AppData\Roaming\UoFiddler\plugins\multieditor.xml";

        }

        [Test]
        public void XMLMultiLoading()
        {
            TileGroups groups;
            XmlSerializer serializer = new XmlSerializer(typeof(TileGroups));

            using (Stream reader = new FileStream(pathFile, FileMode.Open))
            {
                // Call the Deserialize method to restore the object's state.
                groups = (TileGroups)serializer.Deserialize(reader);
            }

            foreach(var grp in groups.group)
            {
                Console.WriteLine(grp.name);

                foreach (var subgroup in grp.subgroup)
                {
                    Console.WriteLine($"\t{subgroup.name}");
                }
            }

            Assert.Pass();
        }

        [Test]
        public void XMLAddingAfterLoading()
        {
            TileGroups groups;
            XmlSerializer serializer = new XmlSerializer(typeof(TileGroups));

            using (Stream reader = new FileStream(pathFile, FileMode.Open))
            {
                // Call the Deserialize method to restore the object's state.
                groups = (TileGroups)serializer.Deserialize(reader);
            }

            string name = groups.group[0].name;

            int size = 15;
            TileGroupsGroupSubgroupEntry[] arrayToAdd = new TileGroupsGroupSubgroupEntry[size];

            for (ushort i = 0; i < size; i++)
            {

                arrayToAdd[i] = new TileGroupsGroupSubgroupEntry()
                {
                    index = i,
                };
            }

            TileGroupsGroupSubgroup seaHatsSubGroup = new TileGroupsGroupSubgroup()
            {
                name = "Merry",
                entry = arrayToAdd
            };

            TileGroupsGroup SeaHatsCustomGroup = new TileGroupsGroup()
            {
                name = "SEA HATS",
                subgroup = new TileGroupsGroupSubgroup[] { seaHatsSubGroup }
            };

            groups.group = groups.group.Prepend(SeaHatsCustomGroup).ToArray();

            foreach (var grp in groups.group)
            {
                Console.WriteLine(grp.name);

                foreach (var subgroup in grp.subgroup)
                {
                    Console.WriteLine($"\t{subgroup.name}");

                    foreach (var entry in subgroup.entry)
                    {
                        Console.WriteLine($"\t\t{entry.index}");
                    }
                }
            }

            StreamWriter myWriter = new StreamWriter(@"C:\Users\giga4\AppData\Roaming\UoFiddler\plugins\multieditor2.xml");
            serializer.Serialize(myWriter, groups);
            myWriter.Close();

            Assert.Pass();
        }
    }



    
}