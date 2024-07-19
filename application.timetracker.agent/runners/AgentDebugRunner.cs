using System;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using System.Collections.Specialized;
using System.ComponentModel;
using NLog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;


namespace application.timetracker.agent.runners
{
    using System.ComponentModel.DataAnnotations;
    using System.Text;

    using application.timetracker.agent.Models;
    using application.timetracker.agent.monitoring.configuration;
    using application.timetracker.agent.monitoring.statistic;
    using DbUtils;
    using DbUser = application.timetracker.agent.Models.User;

    public sealed class AgentDebugRunner : IAgentRunner
    {
        private readonly CancellationTokenSource _appCancel  = new CancellationTokenSource();

        private ILogger _log = LogManager.GetLogger("DebugRunner");


        public static AgentDebugRunner Create() 
        { 
            return new AgentDebugRunner(); 
        }
        
        private  AgentDebugRunner()
        {
            //
            // Subscribe to event "Proccess exit" 
            //
            //AppDomain.CurrentDomain.ProcessExit += (sender, args) => ExitHandler(args);

            //
            // Subscribe to event "Ctrl+C key pressed" 
            //
            // Note: Usualy, the Ctrl+C (or Ctrl+Break) keys mean that application should be terminated vs killed
            // So we need to handle this case and complete all activities of Agent
            Console.CancelKeyPress += (sender, args) => ExitHandler(args);
        }

        private void Handle(object t, NotifyCollectionChangedEventArgs args)
        {
            if (args.NewItems != null)
            {
                foreach (Application newItem in args.NewItems)
                {
                    Console.WriteLine($" +=> ID = {newItem.Id} Name = {newItem.Name}");
                }
            }

            if (args.OldItems != null)
            {
                foreach (Application newItem in args.OldItems)
                {
                    Console.WriteLine($" <=- ID = {newItem.Id} Name = {newItem.Name}");
                }
            }
        }

        public void Run() 
        {
            Console.WriteLine("********************************************");
            Console.WriteLine("* Welcome to Agent (v.0.1.)");
            Console.WriteLine("********************************************");

            _log.Info("********************************************");
            _log.Info("* Welcome to Agent (v.0.1.)");
            _log.Info("********************************************");


            Console.WriteLine("Starting ...");

            // Create configuration
            var trackerConfig = new ActivityTrackerConfiguration();


            var tracker = new ApplicationActivityTracker();
            var collector = new ApplicationStatisticCollector();

            // Subscribe to the new statistic data

            collector.BindStatisticProvider(tracker);

            var contextOptions =
                    new DbContextOptionsBuilder<timetrackingagentContext>()
                            .UseSqlServer(@"Data Source=.\SQLEXPRESS;Initial Catalog=timetracking.agent;Integrated Security=True;TrustServerCertificate=False")
                            .EnableSensitiveDataLogging()
                            /*.UseLazyLoadingProxies()*/
                            .LogTo(Console.WriteLine)
                            .Options;



            using (var DB = new timetrackingagentContext(contextOptions))
            {
                //
                // Use SQL query or stored procedure to get data directly from db and transform responce to Applications models
                //
                //DB.Applications.FromSqlRaw("SELECT * FROM applications").Load();


                //DB.Database.EnsureDeleted();
                //DB.Database.EnsureCreated();

                if (DB.Database.CanConnect())
                {
                    //
                    // Load all records from dbtable Users and copy it to list
                    //
                    //var users = DB.Users.ToList();


                    //
                    // Load all records from dbtable AppRunningTimes
                    //          + linked records from dbtable Applications
                    //          = copy it to list
                    //var times_app =
                    //    DB.AppRunningTimes
                    //      .Include(time => time.App)
                    //      .ToList();


                    //
                    // Load all records from dbtable AppRunningTimes
                    //          + linked records from dbtable Applications
                    //          + linked records from dbtable Users
                    //          = copy it to list
                    //var times_app_user =
                    //    DB.AppRunningTimes
                    //      .Include(time => time.App)
                    //      .Include(time => time.User)
                    //      .ToList();


                    //
                    // Select records (INNER JOIN)
                    //                  from dbtable Application
                    //                + related records from dbtable app_running_time
                    //                + exclude all non 'Opera' records from united SET
                    //                + order all records (filtered) by starting time
                    //
                    //DB.AppRunningTimes
                    //      .Include(time => time.App)
                    //      .Where(time => time.App.Name == "Opera")
                    //      .OrderBy(time => time.StartTime)
                    //      .Load();

                    DB.AppRunningTimes
                          .Include(time => time.App)
                          .Where(time => time.App.Name == "Opera")
                          .OrderBy(time => time.StartTime)
                          .Select(d => new { d.StartTime, d.App, Test = "Hello" }) /* To check*/
                          .Load();


                    //
                    //
                    // Load the first record from the dbtable app_running_time
                    //
                    var apptime = DB.AppRunningTimes.First();

                    
                    
                    //
                    // We handle the new|deleted items within the cache
                    //
                    //DB.Applications.Local.CollectionChanged += Handle;

                    //DB.Applications.Local.CollectionChanged += (sender, args) =>
                    //{
                    //    if (args.NewItems != null)
                    //    {
                    //        foreach (Application newItem in args.NewItems)
                    //        {
                    //            Console.WriteLine($" => ID = {newItem.Id} Name = {newItem.Name}");
                    //        }
                    //    }

                    //    if (args.OldItems != null)
                    //    {
                    //        foreach (Application newItem in args.OldItems)
                    //        {
                    //            Console.WriteLine($" <= ID = {newItem.Id} Name = {newItem.Name}");
                    //        }
                    //    }
                    //};
                    //DB.Applications.Local.CollectionChanged -= Handle;

                    //
                    //  Load all records from dbtable applications that contain 'Dota' string in the name of application
                    //
                    var items = DB
                                .Applications
                                    .Where(app => app.Name.Contains("Dota"))
                                    .ToList();

                    Console.WriteLine($" **** QUERY: Select 'Dota'");
                    foreach (Application appQ1 in items)
                    {
                        //
                        // Load implicitly the related data from dbtable app_running_time
                        //
                        DB.Entry(appQ1)
                            .Collection(d => d.AppRunningTimes)
                            .Query()
                            .Where(t => t.FinishTime == null)
                            .Load();

                        //
                        // Try to load data twice and check if new query will be send to DB
                        //
                        DB.Entry(appQ1)
                            .Collection(d => d.AppRunningTimes)
                            .Load();

                        DB.Database.ExecuteSqlRaw("SELECT TOP (1000) [id],[name] FROM [timetracking.agent].[dbo].[applications]");


                        //
                        // Check if related data (collection) is already loaded
                        //
                        if (!DB.Entry(appQ1).Collection(d => d.AppRunningTimes).IsLoaded)
                        {
                            DB.Entry(appQ1).Collection(d => d.AppRunningTimes).Load();
                        }

                        //
                        // Modify property FinishTime for the first items from AppRunningTime
                        //
                        //appQ1.AppRunningTimes.FirstOrDefault().FinishTime = DateTime.Now;

                        //
                        // Check if related collection marked as modified
                        //
                        if (DB.Entry(appQ1).Collection(d => d.AppRunningTimes).IsModified)
                        {
                            DB.Entry(appQ1).Collection(d => d.AppRunningTimes).Load();
                        }


                        // ==> Implement [ATT-2] here
                        try
                        {
                            //
                            // Select first app_running_time where finish time is null
                            //
                            var apptimeNotFinished = DB.AppRunningTimes.Where(app => app.FinishTime == null).First();


                            // 
                            // Make this application as finished but with wrond date
                            //
                            apptimeNotFinished.FinishTime = DateTime.Now.AddDays(-60);

                            //apptimeNotFinished.FinishTime = DateTime.Now - new TimeSpan(30, 5, 30);

                            DB.ValidateContext().SaveChanges();
                            //DB.SaveChanges();

                        }
                        // Handle the ModelValidationException here 
                        catch (Exception ex)
                        {
                            // DO print all failed entities
                            Console.WriteLine($"AppTimeTracking validation error. Error ({ex.Message})");
                        }


                        // END [ATT-2] implementation


                        //
                        // Remove first items from AppRunningTime
                        //
                        //var firstAppTime = appQ1.AppRunningTimes.FirstOrDefault();

                        //DB.AppRunningTimes.Remove(firstAppTime);

                        ////
                        //// Check if related collection marked as modified
                        ////
                        //if (DB.Entry(appQ1).Collection(d => d.AppRunningTimes).IsModified)
                        //{
                        //    DB.SaveChanges();

                        //// Do not need to call load as the all related records (applicationRunningTime) is already up to date      
                        //    DB.Entry(appQ1).Collection(d => d.AppRunningTimes).Load();
                        //}
                    }

                    //
                    // Do cache another bunch of Applications ... so Applications.Local contains applications with Dota + applications with Edge 
                    //


                    //
                    // Modify existing record in dbtable Application
                    //
                    //var appQuery2 = DB.Applications.Where(app => app.Name.Contains("Edge")).Single();


                    //var strNewApp = new StringBuilder(appQuery2.Name, 10).Append(" v.10");

                    //appQuery2.Name = strNewApp.ToString();



                    //
                    // Add new record in dbtable Application
                    //

                    var newApp =
                        DB.Applications.Add(
                            new Application
                            {
                                Name = "Some unknown browser"
                            }
                        );

                    DB.SaveChanges();

                    DB.Applications.Remove(newApp.Entity);
                    
                    var appDoDelete  = DB.Applications.Attach(new Application { Id = 1008 });

                    DB.Applications.Remove(appDoDelete.Entity);

                    DB.SaveChanges();

                    //foreach (Application appQ2 in appQuery2)
                    //{
                    //    Console.WriteLine($" ID = {appQ2.Id} Name = {appQ2.Name}");
                    //}


                    //
                    // Load all records from dbtable Applications
                    //
                    foreach (Application appQ3 in DB.Applications)
                    {
                        //Console.WriteLine($" ID = {appQ3.Id} Name = {appQ3.Name}");
                    }

                    //
                    // Find record by key
                    //
                    var dotaToDelete = DB.Applications.Find(9);

                    DB.Applications.Remove(dotaToDelete);



                    foreach (var application in DB.Applications.Local)
                    {
                        Console.WriteLine($"Application | Name: {application.Name} ID:{application.Id}");
                    }

                    foreach (var application in DB.Applications)
                    {
                        Console.WriteLine($"Application | Name: {application.Name} ID:{application.Id}");
                    }



                    foreach (var application in DB.Applications.Local)
                    {
                        Console.WriteLine($"Application | Name: {application.Name} ID:{application.Id}");
                    }

                    foreach (var application in DB.Applications)
                    {
                        Console.WriteLine($"Application | Name: {application.Name} ID:{application.Id}");
                    }


                    //var operaInstance = operaQuery.ToList();

                    var operaName = DB.Applications.Find(1).Name;
                    var edgeName = DB.Applications.Find(2).Name;
                    var appTime = DB.AppRunningTimes.Find(new DateTime(2022,04,25,14,27,14),1,1);

                    var appTime2 = DB.AppRunningTimes
                                        .Where(r =>
                                                       r.StartTime == new DateTime(2022, 04, 25, 14, 27, 14)
                                                    && r.UserId == 1
                                                    && r.AppId == 1
                                                    ).Single();


                    #region Update Application

                    // Find entity 'Edge'
                    var appEdge = DB.Applications.FirstOrDefault(t => t.Name.Contains("Edge"));


                    // Rename entity
                    appEdge.Name = "Microsoft Edge";

                    // do update entity
                    DB.Applications.Update(appEdge);
                    DB.SaveChanges();

                    #endregion


                    #region Register new Time


                    EntityEntry<Application> app = null;
                    EntityEntry<DbUser> user = null;
                    
                    if (DB.Applications.FirstOrDefault(t => t.Name == "Dota v5") == null)
                    {
                        app = DB.Applications.Add(new Application {Name = "Dota v5" });
                    }

                    if (DB.Users.FirstOrDefault(t => t.Name == "IDDQD") == null)
                    {
                        user = DB.Users.Add(new DbUser { Name = "IDDQD" });
                    }

                    //DB.SaveChanges();

                    //if (DB.Applications.FirstOrDefault(t => t.Name == "Cs") == null)
                    //{
                    //    DB.Applications.Add(new Application() { Id = 6, Name = "Cs" });
                    //}

                    //if (DB.Users.FirstOrDefault(t => t.Name == "Vasya") == null)
                    //{
                    //    DB.Users.Add(new DbUser { Id = 5, Name = "Vasya" });
                    //}

                    //if (DB.Users.FirstOrDefault(t => t.Name == "Kolya") == null)
                    //{
                    //    DB.Users.Add(new DbUser { Id = 6, Name = "Kolya" });
                    //}

                    var dtn = DateTime.Now;


                    DB.AppRunningTimes.AddRange(
                        /*new AppRunningTime { StartTime = dtn, FinishTime = DateTime.Now, User= user?.Entity, App = app?.Entity },*/
                        new AppRunningTime { StartTime = dtn, FinishTime = DateTime.Now,
                                                    User= DB.Users.Where(user=> user.Name == "Nikita").Single(),
                                                    App = DB.Applications.Where(t => t.Name == "Dota").Single()
                        }
                        );

                    DB.SaveChanges();



                    //DB.Applications.AddRange(app1, app2);
                    //DB.Users.AddRange(user1, user2);
                    //DB.AppRunningTimes.AddRange(time1, time2);
                    //DB.SaveChanges();

                    //var times = DB.AppRunningTimes
                    //           .Select(r => EF.Functions.DateDiffMinute(r.StartTime, r.FinishTime))
                    //           .ToList();
                    //foreach (var time in times)
                    //    Console.WriteLine($"{time.Value}");

                    #endregion


                    //var times = DB.AppRunningTimes
                    //           .Select(r => EF.Functions.DateDiffMinute(r.StartTime, r.FinishTime))
                    //           .ToList();
                    //foreach (var time in times)
                    //{
                    //    if(time.HasValue)
                    //        Console.WriteLine($"{time.Value}");
                    //}
                        

                }
            }


            // Run process tracker
            tracker.Start(trackerConfig);


            WaitForSignalFinish(_appCancel.Token);

            //
            // Do stop agent
            //

            // 1. Stop activity tracker
            var t = tracker.Stop();


            Task.WaitAll(new[] { t });

            //
            // Unsubscribe from application events
            //
            Console.CancelKeyPress -= (sender, args) => ExitHandler(args);

            Console.WriteLine("********************************************");
            Console.WriteLine("* Agent finished ");
            Console.WriteLine("********************************************");
        }

        private void WaitForSignalFinish(CancellationToken  token)
        {
            do
            {
                Task
                    .Delay(TimeSpan.FromSeconds(20), token)
                    .Wait(token);
            }
            while (!_appCancel.IsCancellationRequested);
        }

        private void ExitHandler(ConsoleCancelEventArgs args)
        {
            // You can add any arbitrary global clean up 
            Console.WriteLine("=> Exiting agent ...");

            //
            // Set the cancelation token flag
            //
            _appCancel.Cancel();

            //
            // Set the flag to prevent the agent from from immediately kill. 
            //
            // Note: we need to complete all the agent activities before agent will be killed
            args.Cancel = true;
        }

        #region Sample code

        public enum CAL_YEARS_RESULT
        {
            OK = 0,
            INVALID_YEAR_RANGE = 1,
            GENERAL_ERROR = 6666
        }

        public enum OP_RESULT
        {
            OK = 0,
            INVALID_PARAMETER = 1
        }

        public class YearsOutOfRangeException : ArgumentOutOfRangeException
        {
            public readonly int minValue = 1;
            public readonly int maxValue = DateTime.Now.Year;


            public YearsOutOfRangeException(string paramName, object actualValue, string message)
                : base(paramName, actualValue, message)
            {
            }

            public override string Message
            {
                get
                {
                    var strMsg = base.Message;

                    strMsg += $" Valid range of year is [from {minValue} to {maxValue} ]";

                    return strMsg;
                }
            }
        }

        private int CalcYears(int birthYear)
        {
            if (!(birthYear > 0 && birthYear <= DateTime.Now.Year))
            {
                //years = 0;

                //return CAL_YEARS_RESULT.INVALID_YEAR_RANGE;

                throw new YearsOutOfRangeException(nameof(birthYear), birthYear, "Year is out of range!");
            }

            return DateTime.Now.Year - birthYear;
        }


        private CAL_YEARS_RESULT CalcYears(int birthYear, out int years)
        {
            years = 0;

            try
            {
                if (!(birthYear > 0 && birthYear <= DateTime.Now.Year))
                {
                    return CAL_YEARS_RESULT.INVALID_YEAR_RANGE;
                }

                years = DateTime.Now.Year - birthYear;

                return CAL_YEARS_RESULT.OK;
            }
            catch
            {

            }

            return CAL_YEARS_RESULT.GENERAL_ERROR;
        }

        public struct Test : IDisposable
        {
            public void Dispose() => throw new NotImplementedException();
            
        }

        public class MyResource : IDisposable
        {
            // Pointer to an external unmanaged resource.
            private IntPtr handle;
            // Other managed resource this class uses.
            private Component component = new Component();
            // Track whether Dispose has been called.
            private bool disposed = false;

            // The class constructor.
            public MyResource(IntPtr handle)
            {
                this.handle = handle;
            }

            // Implement IDisposable.
            // Do not make this method virtual.
            // A derived class should not be able to override this method.
            public void Dispose()
            {
                Dispose(true);
                // This object will be cleaned up by the Dispose method.
                // Therefore, you should call GC.SupressFinalize to
                // take this object off the finalization queue
                // and prevent finalization code for this object
                // from executing a second time.
                GC.SuppressFinalize(this);
            }

            // Dispose(bool disposing) executes in two distinct scenarios.
            // If disposing equals true, the method has been called directly
            // or indirectly by a user's code. Managed and unmanaged resources
            // can be disposed.
            // If disposing equals false, the method has been called by the
            // runtime from inside the finalizer and you should not reference
            // other objects. Only unmanaged resources can be disposed.
            private void Dispose(bool disposing)
            {
                // Check to see if Dispose has already been called.
                if (!this.disposed)
                {
                    // If disposing equals true, dispose all managed
                    // and unmanaged resources.
                    if (disposing)
                    {
                        // Dispose managed resources.
                        component.Dispose();
                    }

                    // Call the appropriate methods to clean up
                    // unmanaged resources here.
                    // If disposing is false,
                    // only the following code is executed.
                    CloseHandle(handle);
                    handle = IntPtr.Zero;

                    // Note disposing has been done.
                    disposed = true;

                }
            }

            // Use interop to call the method necessary
            // to clean up the unmanaged resource.
            [System.Runtime.InteropServices.DllImport("Kernel32")]
            private extern static Boolean CloseHandle(IntPtr handle);

            // Use C# destructor syntax for finalization code.
            // This destructor will run only if the Dispose method
            // does not get called.
            // It gives your base class the opportunity to finalize.
            // Do not provide destructors in types derived from this class.
            ~MyResource()
            {
                // Do not re-create Dispose clean-up code here.
                // Calling Dispose(false) is optimal in terms of
                // readability and maintainability.
                Dispose(false);
            }
        }
        #endregion
    }
}
