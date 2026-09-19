namespace AwCon
{
    public interface INavigatorSensor
    {
        ref readonly NavigationContext GetCurrentContext();
    }
}