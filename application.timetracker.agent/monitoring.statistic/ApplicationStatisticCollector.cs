using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace application.timetracker.agent.monitoring.statistic
{
    public class ApplicationStatisticCollector
    {
        private readonly Dictionary<string, ApplicationStatistic>  _appStatistic = new ();

        public IReadOnlyDictionary<string, ApplicationStatistic> ApplicationStatistics { get => _appStatistic; }



        public void OnUpdateStatistic(ApplicationStatisticRaw statistic)
        {
            /*
             * #1 Application started
             * #2 Application finished
             * #3 Application running
             * 

             * - Check if application already registered in our statistic collection 
             *   #1 Application started [No application registered]
             *    1.1 Do register new application
             *  
             *   #2 Application running [application registered]
             *    2.1 Do update application statistic
             *  
             * - Check if the running application is absent in raw statistic
             *      Do select all of active applications from our statistic collection
             *   #3 Application finished [application registered in our collection but absent in raw statistic]
             *    3.1 Do finish last application running
             */

            foreach(var statisticRaw in statistic.Applications)
            {
                ApplicationStatistic app = null;

                #region Study
                //// Case #1
                //_appStatistic.Keys.FirstOrDefault(app => app == statisticRaw.ApplicationName);

                //// Case #2
                //if(_appStatistic.ContainsKey(statisticRaw.ApplicationName))
                //{
                //    app = _appStatistic[statisticRaw.ApplicationName];
                //}

                //// Case #3
                //if (_appStatistic.TryGetValue(statisticRaw.ApplicationName, out app))
                //{
                //    // application exists
                //}

                //// #alt
                //if (app != null)
                //{
                //    // application exists
                //}

                //// Case #4
                //try
                //{
                //    app = _appStatistic[statisticRaw.ApplicationName];
                //}
                //catch
                //{
                //    // application not found
                //}
                //// application exists
                #endregion

                if (_appStatistic.TryGetValue(statisticRaw.ApplicationName, out app))
                {
                    // 2. Application running
                    UpdateApplication(statistic.StatisticTime, app, statisticRaw);
                }
                else
                {
                    // 1. Application started
                    RegisterNewApplication(statistic.StatisticTime, statisticRaw);
                }
            }

            // Do find the finished applications

            var runningApp = SelectActiveApplications();


            foreach (var activeApp in runningApp)
            {
                ApplicationInfo appN = statistic.Applications.FirstOrDefault(app => app.ApplicationName == activeApp.ApplicationName);

                if (appN == null)
                {
                    // Finish application for all users
                    foreach (var user in activeApp.Users)
                    {
                        FinishApplication(statistic.StatisticTime, user.Value.UserName, activeApp);
                    }
                }
                else
                {
                    /* How we keep statistic data
                     * Application_1
                     *       + User_Name1
                     *       + User_Name2
                     
                     * Application_2
                     *       + User_Name1
                     *       + User_Name2
                     *
                     *
                     * Input data
                     * 
                     *    1. AppName1 User1
                     *    2. AppName1 User2
                     *    3. AppName2 User1
                     *    4. AppName2 User2
                     *    
                     *    
                     *    
                     *    
                     *    ----------------------     ----------------------
                     *    | Table_Applications |     | Table_Users         |  
                     *    ======================     ====================== 
                     *    Application_Name           User_Name 
                     *    *Users                1---->n
                     * 
                     * 
                     *   
                     * 
                     * 
                     */
                    foreach (var user in activeApp.Users)
                    {
                        if (statistic
                            .Applications
                                .FirstOrDefault(
                                    statistic =>    statistic.ApplicationName == activeApp.ApplicationName
                                                 && statistic.UserName == user.Value.UserName
                                                 )
                            == null
                            )
                        {
                            FinishApplication(statistic.StatisticTime, user.Value.UserName, activeApp);
                        }
                    }
                }
            }
        }

        private void RegisterNewApplication(DateTime collectorTime, ApplicationInfo statistic)
        {
            // Create new application
            var app = new ApplicationStatistic(statistic.ApplicationName);

            // Fill application with statistic data
            app.Update(collectorTime, statistic);

            #region Alternative #1
            //UpdateApplication(app, statistic);
            #endregion

            #region Alternative #2
            //DoUpdateStatisticInternal(app, statistic);
            #endregion

            _appStatistic.TryAdd(app.ApplicationName, app);
        }

        private void UpdateApplication(DateTime collectorTime, ApplicationStatistic app, ApplicationInfo statistic)
        {
            app.Update(collectorTime, statistic);

            //DoUpdateStatisticInternal(app, statistic);
        }
        

        private void FinishApplication(DateTime collectorTime, string userName, ApplicationStatistic app)
        {
            app.Finish(collectorTime, userName);
        }

        private IEnumerable<ApplicationStatistic> SelectActiveApplications()
        {
            //List<ApplicationStatistic> active = new ();
            //foreach(var app in _appStatistic.Values)
            //{
            //    if(app.Users != null)
            //    {
            //        foreach(var users in app.Users)
            //        {
            //            if(users.Value.Times.Last().EndTime == DateTime.MinValue)
            //            {
            //                active.Add(app);

            //                break;
            //            }
            //        }
            //    }
            //}
            //return active;
            #region Alternative

            //var tt = _appStatistic
            //            .Values
            //                .Select<ApplicationStatistic, ApplicationUser>(t => t.Users.Values.First())



            return _appStatistic
                        .Values
                            .Where(app =>
                                app.Users.Values.FirstOrDefault(
                                    user => user.Times.Last().EndTime == DateTime.MinValue) != null
                                );


            #endregion
        }


        /// <summary>
        /// Bind to statistic provider and start listening new statistic data
        /// </summary>
        /// <param name="statistic"></param>
        public void BindStatisticProvider(ApplicationActivityTracker statistic)
        {
            statistic.ApplicationStatisticReady += OnUpdateStatistic;
        }

        private void DoUpdateStatisticInternal(ApplicationStatistic app, ApplicationInfo statistic)
        {
            //app.Update(statistic);
        }

        private List<string> GetApplicationNames()
        {
            return this._appStatistic.Values.Select(t => t.ApplicationName).ToList();
        }

        class AppIDS
        {
            public int AppId;
            public double AppUId;
        };

        private List<AppIDS> SelectApplicationIDs()
        {
            return this._appStatistic.Values.Select(t => new AppIDS { AppId = t.AppId, AppUId = t.AppUID }).ToList();
        }

        private List<dynamic> SelectApplicationUIDS()
        {
            return this._appStatistic.Values.Select(t => new {A = t.AppId, B = t.AppUID}).ToList<dynamic>();
        }

        private List<string> SelectAllUsers()
        {

            return
                this._appStatistic.Values
                    .SelectMany(t => t.Users)
                    .Select(r => r.Value.UserName)
                    .Distinct()
                    .ToList();
                    
        }
    }
}
