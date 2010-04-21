using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Net;
using System.IO;
using NUnit.Framework;

namespace Braintree.Tests
{
    public class TestHelper
    {
        public static String QueryStringForTR(Request trParams, Request req, String postURL)
        {
            String trData = TrUtil.BuildTrData(trParams, "http://example.com");
            String postData = "tr_data=" + HttpUtility.UrlEncode(trData, Encoding.UTF8) + "&";
            postData += req.ToQueryString();

            var request = WebRequest.Create(postURL) as HttpWebRequest;
            request.Headers.Add("X-ApiVersion", "1");
            request.UserAgent = "Braintree .NET Tests";

            request.Method = "POST";
            request.KeepAlive = false;

            byte[] buffer = Encoding.UTF8.GetBytes(postData);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = buffer.Length;
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(buffer, 0, buffer.Length);
            requestStream.Close();

            var response = request.GetResponse() as HttpWebResponse;
            String query = response.ResponseUri.Query;

            response.Close();

            return query;
        }

        public static void AreDatesEqual(DateTime expected, DateTime actual)
        {
            Assert.AreEqual(expected.Day, actual.Day);
            Assert.AreEqual(expected.Month, actual.Month);
            Assert.AreEqual(expected.Year, actual.Year);
        }

        public static void AssertIncludes(String expected, String all)
        {
            Assert.IsTrue(all.IndexOf(expected) >= 0, "Expected:\n" + all + "\nto include:\n" + expected);
        }

        public static Boolean IncludesSubscription(PagedCollection<Subscription> collection, Subscription subscription)
        {
            foreach (Subscription item in collection)
            {
                if (item.Id.Equals(subscription.Id)) {
                    return true;
                }
            }
            return false;
        }

        public static PagedCollection<T> MockPagedCollection<T>(int totalItems) where T : class
        {
            return new PagedCollection<T>(new List<T>(), totalItems, delegate() {
                return MockPagedCollection<T>(totalItems);
            });
        }
    }
}