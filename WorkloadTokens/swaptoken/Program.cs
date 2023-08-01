using Azure.Core;
using System.IO.Pipes;
using System.Threading.Channels;

internal class Program
{    
    private static async Task Main(string[] args)
	{
        var nameOfPipe = Environment.GetEnvironmentVariable("SWAPTOKEN_PIPE") ?? "swaptoken";
              
		var complete = new TaskCompletionSource();

		Console.CancelKeyPress += (_, ea) => { ea.Cancel = true; complete.SetResult(); };

		Console.Write("Starting swaptoken localserver...");		

		PipeServer(nameOfPipe);
		
		Console.WriteLine("OK");

		await complete.Task;

		Console.WriteLine("Closing swaptoken local server down");		
	}
    static void PipeServer(string nameOfPipe)
    {
        var signalServerStarted = new ManualResetEvent(false);

        Task t = Task.Factory.StartNew(() =>
        {
            try
            {                
                while (true)
                {
                    Console.WriteLine($"Listening on {nameOfPipe}");
                    var server = new NamedPipeServerStream(nameOfPipe, PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances);

                    signalServerStarted.Set();

                    server.WaitForConnection();

                    Console.WriteLine($"Processing connection on {Thread.CurrentThread.ManagedThreadId}");

                    StreamReader reader = new StreamReader(server);
                    StreamWriter writer = new StreamWriter(server);

                    Console.WriteLine($"Connected on thread {Thread.CurrentThread.ManagedThreadId}");
                                        
                    Console.Write("Reading from client...");
                    var line = reader.ReadLine();

                    string tokenString = string.Empty;

                    try{

                    var clientCredentials = new MyClientAssertionCredential();

                    TokenRequestContext context = new TokenRequestContext(scopes: new string[] { "https://database.windows.net/.default" });
                    CancellationToken cancel = new CancellationToken();

                    tokenString = clientCredentials.GetToken(context, cancel).Token.ToString();                                        
                    }
                    catch (Exception ex) {
                        tokenString = $"GetTokenError : {ex.Message}";
                    }

                    writer.WriteLine(tokenString);
                    writer.Flush();

                    reader.ReadToEnd();
                    Console.Write("Done...");
                }
            }
            catch (Exception ex) { 
                Console.Error.WriteLine(ex.ToString());
            }
        });

        if (!signalServerStarted.WaitOne(30000) || t.IsFaulted) {
            if (t.IsFaulted) 
                throw new Exception(message: "Listening to named pipe failed, see inner exception for details", innerException: t.Exception);
            else
                throw new Exception("It took more than 30 seconds to start listening, but no exception was thrown");
        }
    }
}