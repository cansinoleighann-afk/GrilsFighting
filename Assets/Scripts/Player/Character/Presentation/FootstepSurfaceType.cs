namespace AwCon
{
    /// <summary>Terrain/material categories used by the footstep audio resolver.</summary>
    public enum FootstepSurfaceType
    {
        Concrete = 0,
        Dirt = 1,
        Grass = 2,
        Gravel = 3,
        Ice = 4,
        Metal = 5,
        Mud = 6,
        Sand = 7,
        Snow = 8,
        Water = 9,
        Wood = 10,
    }

    public enum FootstepActionType
    {
        Walk = 0,
        Run = 1,
        Land = 2,
        Slide = 3,
    }
}
