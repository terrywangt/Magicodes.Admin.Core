using Xamarin.Forms.Internals;

namespace Magicodes.Admin.Behaviors
{
    [Preserve(AllMembers = true)]
    public interface IAction
    {
        bool Execute(object sender, object parameter);
    }
}