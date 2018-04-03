
namespace BridgeUI.Binding
{
    /// <summary>
    /// Action
    /// </summary>
    /// <param name="memberName"></param>
    public delegate void PropertyChanged(string memberName);

    /// <summary>
    /// Property Change observation
    /// </summary>
    public interface IPropertyChanged
    {
        event PropertyChanged OnPropertyChanged;
    }
}