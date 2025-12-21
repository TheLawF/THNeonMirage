using Fictology.Data.Serialization;

namespace THNeonMirage.Data
{
    public record NetPlayerViewer(NetPlayerViewer.Identity ID, int ViewId): ISerializable<CompoundData>
    {
        public enum Identity
        {
            Local,
            Remote,
            Other
        }

        public CompoundData Serialize()
        {
            var data = new CompoundData();
            var eData = EnumData.Of(ID);
            
            data.Add("identity", eData);

            return data;
        }

        public void Deserialize(CompoundData data)
        {
            throw new System.NotImplementedException();
        }
    }
}