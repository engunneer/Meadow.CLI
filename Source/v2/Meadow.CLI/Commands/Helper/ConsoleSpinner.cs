
using CliFx.Infrastructure;

namespace Meadow.CLI
{
    public class ConsoleSpinner : IDisposable
    {
        private int counter = 0;
        private readonly char[] sequence = { '|', '/', '-', '\\' };
        private readonly IConsole? console;

        private bool isDisposed;

        public ConsoleSpinner(IConsole? console)
        {
            this.console = console;
        }

        public static ConsoleSpinner Create(IConsole? console)
        {
            var spinner = new ConsoleSpinner(console);
            _ = spinner.Turn();
            return spinner;
        }

        public async Task Turn(int delay = 100, CancellationToken cancellationToken = default)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (isDisposed) break;

                counter++;
                console?.Output.WriteAsync($"{sequence[counter % 4]}         \r");
                await Task.Delay(delay, CancellationToken.None); // Not propogating the token intentionally.
            }
            console?.Output.WriteAsync($"          {Environment.NewLine}");
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                isDisposed = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}