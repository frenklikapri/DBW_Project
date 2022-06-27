using FleSharing.Core.Interfaces;

namespace FileSharing.API.Workers
{
    public class InactiveFilesWorker : BackgroundService
    {
        private ILogger<InactiveFilesWorker> _logger;
        private IServiceScopeFactory _scopeFactory;
        private IInactiveFileRemoverService _inactiveFileRemoverService;

        public InactiveFilesWorker(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        private async Task SetServices()
        {
            using var scope = _scopeFactory.CreateScope();
            _logger = scope.ServiceProvider.GetRequiredService<ILogger<InactiveFilesWorker>>();
            _inactiveFileRemoverService = scope.ServiceProvider.GetRequiredService<IInactiveFileRemoverService>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await SetServices();

                await _inactiveFileRemoverService.DeleteInactiveFilesAsync();

                _logger.LogInformation("Remove inactive files executed!");

                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}
