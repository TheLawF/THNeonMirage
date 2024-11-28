using System;
using JetBrains.Annotations;

namespace THNeonMirage.Data
{
    public record FieldData(string Name,[CanBeNull] string Description, int FirstBid, int SecondBid, int ThirdBid, int Tolls, Type FieldType);
}