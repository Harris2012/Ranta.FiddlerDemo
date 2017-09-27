using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fiddler;
using System.Threading;

namespace Ranta.FiddlerDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            //设置别名
            Fiddler.FiddlerApplication.SetAppDisplayName("CanosFiddlerApp");

            //定义http代理端口
            int iPort = 8877;

            //启动代理程序，开始监听http请求
            Fiddler.FiddlerApplication.Startup(iPort, true, false, true);

            Fiddler.FiddlerApplication.BeforeRequest += FiddlerApplication_BeforeRequest;
            Fiddler.FiddlerApplication.BeforeResponse += FiddlerApplication_BeforeResponse;
            Fiddler.FiddlerApplication.OnNotification += FiddlerApplication_OnNotification;

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();

            Fiddler.FiddlerApplication.Shutdown();

            Console.WriteLine("Thank you.");

            Thread.Sleep(1000);
        }

        //通知
        private static void FiddlerApplication_OnNotification(object sender, NotificationEventArgs e)
        {
            Console.WriteLine(e.ToString());
        }

        //预处理
        private static void FiddlerApplication_BeforeRequest(Session oSession)
        {
            try
            {
                if (shouldIntercept(oSession))
                {
                    switch (oSession.fullUrl)
                    {
                        case "http://localhost:7496/api/database/dblist":
                            {

                                oSession.bBufferResponse = true;
                            }
                            break;

                        default:
                            {
                                Console.WriteLine("Before request for : \t" + oSession.fullUrl);
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Fiddler.FiddlerApplication.ReportException(ex, nameof(FiddlerApplication_BeforeRequest));
            }
        }

        //后处理
        private static void FiddlerApplication_BeforeResponse(Session oSession)
        {
            try
            {
                if (shouldIntercept(oSession))
                {
                    switch (oSession.fullUrl)
                    {
                        case "http://localhost:7496/api/database/dblist":
                            {
                                Console.WriteLine("OKOK: " + oSession.fullUrl);

                                oSession.oResponse.headers.HTTPResponseCode = 200;
                                oSession.oResponse.headers.HTTPResponseStatus = "200 OK";
                                oSession.oResponse.headers["Content-Type"] = "application/json";
                                oSession.utilSetResponseBody("{\"name\":\"tom\", \"age\": 12}");
                            }
                            break;

                        default:
                            {
                                //Console.WriteLine("Before request for : \t" + oSession.fullUrl);
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Fiddler.FiddlerApplication.ReportException(ex, nameof(FiddlerApplication_BeforeResponse));
            }
        }

        private static bool shouldIntercept(Session oSession)
        {
            return oSession.fullUrl != null && oSession.fullUrl.StartsWith("http://localhost:7496");
        }
    }
}

