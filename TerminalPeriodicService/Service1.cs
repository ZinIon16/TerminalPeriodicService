using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.ServiceProcess;
using System.Timers;

namespace TerminalPeriodicService
{
    public partial class Service1 : ServiceBase
    {
        Timer timer = new Timer();
        SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"].ToString());
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Service1()
        {
            InitializeComponent();
        }
        protected override void OnStart(string[] args)
        {
            log.Info("Service Started");
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = 10000;
            timer.Enabled = true;
        }
        protected override void OnStop()
        {
            log.Info("Service is stopped at " + DateTime.Now);
        }
        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            log.Info("Service recalled at " + DateTime.Now);
            CheckSucharge();
        }
        //string TerminalID = "";
        //string Amount = "";
        public void CheckSucharge()
        {
            try
            {
                //TerminalID = "";
                //Amount = "";
                //string qry = "select * from [Controller] where timeFrom<'" + DateTime.Now.ToString("h:mm:ss") + "' and timeTo>'" + DateTime.Now.ToString("h:mm:ss") + "'";
                //SqlCommand cmd = new SqlCommand(qry, conn);
                //SqlDataReader sdr = cmd.ExecuteReader();
                using (var cmd = conn.CreateCommand())
                {

                    conn.Open();

                    cmd.CommandText = "GetAmount";

                    List<SurchargeAmount> surchargeAmountList = new List<SurchargeAmount>();

                    using (var sdr = cmd.ExecuteReader())
                    {
                        if (sdr.HasRows)
                        {
                            while (sdr.Read())
                            {
                                surchargeAmountList.Add(new SurchargeAmount(Convert.ToInt32(sdr["TerminalID"]), Convert.ToDouble(sdr["Amount"])));

                            }
                         
                        }
                        else
                        {
                            log.Info("Nope, found nothing");

                        }

                    }

                    for (int i = 0; i < surchargeAmountList.Count; i++)
                    {
                        //log.Info(surchargeAmountList[i].TerminalId);
                        //log.Info(surchargeAmountList[i].Amount);
                        using (var cmd2 = conn.CreateCommand())
                        {
                            cmd2.CommandText = "[UpdateSurchargeAmount] '" + surchargeAmountList[i].TerminalId + "','" + surchargeAmountList[i].Amount + "'";
                            using (var sdr2 = cmd2.ExecuteReader())
                            {

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Info(ex.Message + ex.StackTrace);
            }
            conn.Close();


            try
            {
                //conn.Open();
                //string[] Cvalues = TerminalID.Split(',');
                //string[] Avalues = Amount.Split(',');

                //log.Info("Loop length : " + ((Cvalues.Length) - 1) + " Hence,we will update " + ((Cvalues.Length) - 1) + " terminals");

                //for (int i = 0; i < Cvalues.Length - 1; i++)
                //{

                //    TerminalID = Cvalues[i].ToString();
                //    Amount = Avalues[i].ToString();
                //    string qry2 = "update [RealTime] set SurchargeAmount='" + Amount + "'" + " where TerminalID='" + TerminalID + "'";
                //    SqlCommand cmd2 = new SqlCommand(qry2, conn);
                //    cmd2.ExecuteNonQuery();
                //}


            }
            catch (Exception ex)
            {
                log.Info(ex.Message + ex.StackTrace);
            }
            //conn.Close();
        }


    }

    public class SurchargeAmount
    {
        public int TerminalId { get; set; }
        public double Amount { get; set; }

        public SurchargeAmount()
        {

        }

        public SurchargeAmount(int TID, double amt)
        {
            this.Amount = amt;
            this.TerminalId = TID;
        }
    }
}