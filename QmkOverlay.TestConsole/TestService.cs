namespace QmkOverlay.TestConsole;

public interface ITestService
{
    Task Run(CancellationToken cancelationToken);
}

public class TestService : ITestService
{
    private readonly ILogger<TestService> _logger;

    public TestService(ILogger<TestService> logger)
    {
        _logger = logger;
    }

    public async Task Run(CancellationToken cancelationToken)
    {
        var devices = HidManager.GetDevices();
        _logger.LogInformation($"Found {devices.Count} devices.");

        var keyboard = devices.FirstOrDefault(d => d is QmkKeyboard) as QmkKeyboard;
        if (keyboard == null)
        {
            _logger.LogInformation("No keyboard found!");
            return;
        }
        _logger.LogInformation(keyboard.ToString());

        keyboard.OpenConnection();
        var keyboardId = await keyboard.GetKeyboardId();
        _logger.LogInformation($"Detected keyboardId {keyboardId}");
        
        while (!cancelationToken.IsCancellationRequested)
        {
            var isUnlocked = await keyboard.GetUnlockStatus();
            var matrixState = await keyboard.GetMatrixState();
            if (matrixState != null)
                KeyboardPrinter.Print(keyboard, keyboardId, isUnlocked, matrixState);

            await Task.Delay(10);
        }
    }
}
