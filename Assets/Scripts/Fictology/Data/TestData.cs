using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using Fictology.Data.Serialization;

namespace Fictology.Data
{
    public class TestData
    {
        public int number;
        public string name;
        public List<float> scores;

        public TestData(int number, string name, List<float> scores)
        {
            this.number = number;
            this.name = name;
            this.scores = scores;
        }

        public CompoundData Serialize()
        {
            var data = new CompoundData();
            data.AddInt("number", number);
            return data;
        }

        public void Deserialize(CompoundData data)
        {
            number = data.GetInt("number");
        }

    }
}