using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpTorrent.MVVM.Model
{
    internal class Top10Listener : TraceListener
    {
        private readonly int capacity;
        private readonly LinkedList<string> traces;

        public Top10Listener(int capacity)
        {
            this.capacity = capacity;
            traces = new LinkedList<string>();
        }

        public override void Write(string? message)
        {

        }

        public override void WriteLine(string? message)
        {
            if (message is null) return;
            lock (traces)
            {
                if (traces.Count >= capacity)
                    traces.RemoveFirst();

                traces.AddLast(message);
            }
        }

        public void ExportTo(TextWriter output)
        {
            lock (traces)
                foreach (string s in this.traces)
                    output.WriteLine(s);
        }
    }
}
