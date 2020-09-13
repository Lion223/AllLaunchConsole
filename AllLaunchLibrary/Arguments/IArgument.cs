namespace AllLaunchLibrary.Arguments
{
    public interface IArgument
    {
        string Signature { get; }
        void Operate();
    }
}
