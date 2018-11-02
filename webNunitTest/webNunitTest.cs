using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Internal;
using NUnit.Framework.Interfaces;
using RestSharp.Authenticators;
using System.Net.Http;
using RestSharp;
using System.Net;
using System.Collections.Specialized;
using System.IO;
using System.Text.RegularExpressions;


namespace webNunitTest
{
    public class BaseTest
    {
        public static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        const string BaseUrl = "https://rozetka.com.ua";
        public bool _previousTestPassed = true;
        public string SearchProduct { get; set; }
        public string Rpattern = "([0-9]{6,10})";
        public string cookPatternt = "uid=(.[a-zA-z][0-9].+)\\D+;";
        //public string cookPatternt = "uid=(.[a-zA-z][0-9]+[a-zA-Z].+)";
        public string XcookPatternt = "sid=1; sid=(.[a-zA-Z]*\\d*\\S*.+)";
        //public string XcookPatternt = "sid=([a-zA-z][0-9]+[a-zA-Z]+.+);";
        public string _respString { get; set; }
        public string respString => _respString;
        public string response { get; set; }
        public string _response { get; set; }
        public string request { get; set; }
        public string searchKey = "macbook";
        public string _searchKey { get; set; }
        public string _resp_headers { get; set; }
        public string regexRequestTokenString { get; set; }
        public string _regexRequestTokenString { get; set; }
        public string XregexRequestTokenString { get; set; }
        public string _XregexRequestTokenString { get; set; }


        public CookieContainer _cookieJar = new CookieContainer();
       //public CookieContainer = new System.Net.CookieContainer();

        //public CookieContainer _cookieJar = new CookieContainer();

        /* public void SwitchCase(string searchKey)
          {
              Random rnd = new Random();
              int СsearchKey = rnd.Next(1, 2);
              switch (СsearchKey)
              {
                  case 1:
                      searchKey = "macbook";
                      break;
                  case 2:
                      searchKey = "acernotebook";
                      break;
              }

          } */
        public void regexresponseId()
        {
            var client = new RestClient();
            client.BaseUrl = new Uri(null);
            var request = new RestRequest();
            request.Resource = null;
            IRestResponse response = client.Execute(null);
            Regex regex = new Regex(Rpattern);
            var regexResult = regex.Match(null);
            string respString = regexResult.Groups[1].Value;
            respString = _respString;
        }





        [SetUp]
        public void Setup()
        {
            Assume.That(_previousTestPassed, "Previous step is expected to be passed");
        }


    }
    public class TestMethod : BaseTest
    {
        [Test]
        public void MainPage()
        {
           
            log4net.Config.XmlConfigurator.Configure();
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
            Trace.WriteLine("MainPage+SearchWord");

            var client = new RestClient();
            client.BaseUrl = new Uri("http://rozetka.com.ua");
            client.CookieContainer = _cookieJar;
            var request = new RestRequest();
            request.Resource = "search/?text=" + searchKey;
            // CookieContainer _cookieJar = new CookieContainer();
            log.Info("request :  " + request.Resource);
            log.Info("SearchKey:  " + searchKey);
            IRestResponse response = client.Execute(request);
            
            /* getting ID of the search Item   */
            var resp_headers = response.ResponseUri.ToString(); //Location Header Uri
            Regex regex = new Regex(Rpattern);
            var regexResultFromSearch = regex.Match(response.Content);
            string respString = regexResultFromSearch.Groups[1].Value;  // regex group value ID
            log.Info("response URI/ID/loCATION:  " + resp_headers + "  Cookie from main page:  " + _cookieJar.GetCookieHeader(response.ResponseUri));
           
            _respString = respString;
            _resp_headers = resp_headers;
            /* parsing UID from cookie header*/
            string regExtrToken  = _cookieJar.GetCookieHeader(response.ResponseUri);
            Regex regexCook = new Regex(cookPatternt);
            var regexRequestToken = regexCook.Match(regExtrToken);
            string regexRequestTokenString = regexRequestToken.Groups[1].Value; //Getting uid= value from ---- ho_sid=1; sid=40ef9bd7236761626b4f5bafabc8ab70; uid=rB4eB1vcXXoHnFzdOA+xAg==  
            _regexRequestTokenString = regexRequestTokenString;
            log.Info("Token >>>> " + regexRequestTokenString + "; overriden token >>>> " + _regexRequestTokenString);
            /* parsing SID from cookie header*/
            Regex XregexCook = new Regex(XcookPatternt);
            var XregexRequestToken = XregexCook.Match(regExtrToken);
            string XregexRequestTokenString = XregexRequestToken.Groups[1].Value; //Getting sid= value from ---- ho_sid=1; sid=40ef9bd7236761626b4f5bafabc8ab70; uid=rB4eB1vcXXoHnFzdOA+xAg== 
            _XregexRequestTokenString = XregexRequestTokenString;
            log.Info("SID FROM COOKIE:  " + XregexRequestTokenString);

        }
        [Test]
        public void SearchItem_and_add_to_Cart()
        {
            log4net.Config.XmlConfigurator.Configure();
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
            Trace.WriteLine("Add item with extracted id to cart");
            var client = new RestClient();
            client.BaseUrl = new Uri("https://rozetka.com.ua");
            client.CookieContainer = _cookieJar;
            var request = new RestRequest("cgi-bin/form.php" , RestSharp.Method.POST);
            // var request = new RestRequest(RestSharp.Method.POST);
            //request.Resource = "cgi-bin/form.php";
            request.AddHeader("Accept", "text/javascript, text/html, application/xml, text/xml, */*");
            request.AddHeader("Accept-Encoding", "gzip, deflate, br");
            request.AddHeader("Accept-Language", "ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7,uk;q=0.6");
            request.AddHeader("ajaxAction", "https://my.rozetka.com.ua/cart/#add");
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("Referer", _resp_headers);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded; charset=utf-8");
            request.AddHeader("X-Requested-With", "XMLHttpRequest");
            //request.AddHeader("X-Rozetka-Header", "true");
            //request.AddHeader("x-request-token", _XregexRequestTokenString);
            //request.AddHeader("Pragma", "no-cache");
            //request.AddHeader("Cache-Control", "no-cache");
            request.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 6.3; WOW64; rv:26.0) Gecko/20100101 Firefox/26.0");
            request.AddParameter("goods_id", _respString);
            request.AddParameter("is_buy_from_catalog", "true");
            request.AddParameter("request_token", _regexRequestTokenString);

            IRestResponse response = client.Execute(request);
            log.Info("request url >>>> " + response.ResponseUri + " ; request item ID >>>> " + _respString + " ; Method >>>> " + request.Method) ;
            log.Info("cookie search/add >>>> " + _cookieJar.GetCookieHeader(response.ResponseUri));
            log.Info("POST Request Status Code >>>> " + response.StatusCode);
        }
        [Test]
        public void Cart_check()
        {
            log4net.Config.XmlConfigurator.Configure();
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
            Trace.WriteLine("Cart check");

            var client = new RestClient();
            client.BaseUrl = new Uri("https://my.rozetka.com.ua");
            client.CookieContainer = _cookieJar;
            var request_cart = new RestRequest("cart", RestSharp.Method.GET);
            //request_cart.Resource = "cart/";
            IRestResponse response = client.Execute(request_cart);
            string body_resp = response.Content; // var for getting out the response body into log file
            //  log.Info("Body - >>>>>>>>>>>> " + body_resp);

            Regex regex = new Regex(Rpattern);
            var regexResultFromCart = regex.Match(body_resp);
            string respString_cart = regexResultFromCart.Groups[1].Value;
            log.Info("Extracted id_cart:  " + respString_cart);
            log.Info("cookie cart check: " + _cookieJar.GetCookieHeader(response.ResponseUri));
        }


        [TearDown]
        public void TearDown()
        {
            _previousTestPassed = TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Passed;
        }

        public void TestOutput(string str)
        {
            TestContext.Out.WriteLine(str);
        }
    }
}
