using System;
using System.Threading.Tasks;

namespace SharpTorrent.Commands.Base;
internal abstract class AsyncCommand : Command
{
    public override void Execute(object parameter)
    {
        try { ExucuteTask(parameter); }
        catch (Exception) { }
    }

    public abstract Task ExucuteTask(object parameter);
}
