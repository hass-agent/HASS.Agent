using System.ServiceProcess;
using HASS.Agent.Shared.Enums;
using HASS.Agent.Shared.Managers;
using Microsoft.Extensions.Hosting.WindowsServices;
using Microsoft.Extensions.Options;
using Serilog;

namespace HASS.Agent.Satellite.Service.Managers
{
    /// <summary>
    /// Based on: https://stackoverflow.com/a/69135532
    /// </summary>
    public sealed class ServiceLifetimeManager : WindowsServiceLifetime, IHostLifetime
    {
        private readonly ManualResetEventSlim _applicationStarted;
        private readonly TaskCompletionSource<object> _delayStart = new();
        private IHostApplicationLifetime ApplicationLifetime { get; }

        public ServiceLifetimeManager(IHostEnvironment environment, IHostApplicationLifetime applicationLifetime, ILoggerFactory loggerFactory, IOptions<HostOptions> optionsAccessor) : base(environment, applicationLifetime, loggerFactory, optionsAccessor)
        {
            // prepare
            ApplicationLifetime = applicationLifetime;
            _applicationStarted = new ManualResetEventSlim();

            // set the servicebase to handle events 
            CanHandlePowerEvent = true;
            CanHandleSessionChangeEvent = true;

            // done
            Log.Information("[LIFETIMEMANAGER] Initialized");
        }
        
        public new Task WaitForStartAsync(CancellationToken cancellationToken)
        {
            // register started event
            ApplicationLifetime.ApplicationStarted.Register(() =>
            {
                _applicationStarted.Set();
            });

            // wait for startup to complete
            return Either(base.WaitForStartAsync(cancellationToken), _delayStart.Task);
            static async Task Either(Task a, Task b) => await Task.WhenAny(a, b);
        }

        protected override void OnStart(string[] args)
        {
            // allow host startup to proceed
            _delayStart.TrySetResult(null!);

            // wait for host startup to complete before returning to SCM
            _applicationStarted.Wait();

            // done, now base.OnStart will complete delayStart.Task
            base.OnStart(args);
        }

        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            if (Variables.ExtendedLogging) Log.Information("[LIFETIMEMANAGER] SessionChange: {desc}", changeDescription.Reason.ToString());

            switch (changeDescription.Reason)
            {
                case SessionChangeReason.SessionLock:
                    SharedSystemStateManager.SetSystemStateEvent(SystemStateEvent.SessionLock);
                    break;

                case SessionChangeReason.SessionLogoff:
                    SharedSystemStateManager.SetSystemStateEvent(SystemStateEvent.SessionLogoff);
                    break;

                case SessionChangeReason.SessionLogon:
                    SharedSystemStateManager.SetSystemStateEvent(SystemStateEvent.SessionLogon);
                    break;

                case SessionChangeReason.SessionUnlock:
                    SharedSystemStateManager.SetSystemStateEvent(SystemStateEvent.SessionUnlock);
                    break;

                case SessionChangeReason.ConsoleConnect:
                    SharedSystemStateManager.SetSystemStateEvent(SystemStateEvent.ConsoleConnect);
                    break;

                case SessionChangeReason.ConsoleDisconnect:
                    SharedSystemStateManager.SetSystemStateEvent(SystemStateEvent.ConsoleDisconnect);
                    break;

                case SessionChangeReason.RemoteConnect:
                    SharedSystemStateManager.SetSystemStateEvent(SystemStateEvent.RemoteConnect);
                    break;

                case SessionChangeReason.RemoteDisconnect:
                    SharedSystemStateManager.SetSystemStateEvent(SystemStateEvent.RemoteDisconnect);
                    break;

                case SessionChangeReason.SessionRemoteControl:
                    SharedSystemStateManager.SetSystemStateEvent(SystemStateEvent.SessionRemoteControl);
                    break;
            }
        }

        protected override bool OnPowerEvent(PowerBroadcastStatus powerStatus)
        {
            // not implemented yet

            return true;
        }
    }
}
