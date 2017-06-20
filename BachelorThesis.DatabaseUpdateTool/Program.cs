using Topshelf;

namespace BachelorThesis.DatabaseUpdateTool
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<UpdateWorker>(s =>
                {
                    s.ConstructUsing(name => new UpdateWorker());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });

                x.RunAsLocalSystem();

                x.SetDescription("Bachelor Thesis chatbot");
                x.SetDisplayName("BachelorThesis");
                x.SetServiceName("BachelorThesis");
            });
        }
    }
}
