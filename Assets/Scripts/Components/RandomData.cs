using Unity.Entities;
using Random = Unity.Mathematics.Random;

public partial struct RandomData : IComponentData
{
    public Random Value;
}