using HtmlAgilityPack;
using System;

class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine(await Requests());
    }

    public static async Task<string> Requests()
    {
        var client = new HttpClient();
        string url = "http://web-16.challs.olicyber.it";
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
        HttpResponseMessage response = await client.SendAsync(request);
        string html = await response.Content.ReadAsStringAsync();

        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        var links = doc.DocumentNode.SelectNodes("//a");

        var externalResources = new List<string>();

        foreach (var link in links)
        {
            externalResources.Add(link.GetAttributeValue("href", ""));
        }


        bool flag = false;
        while (!flag)
        {
            foreach (var resource in externalResources.ToList())
            {
                if (!flag)
                {

                    HttpRequestMessage restRequest = new HttpRequestMessage(HttpMethod.Get, $"{url}{resource}");
                    HttpResponseMessage restResponse = await client.SendAsync(restRequest);
                    html = await restResponse.Content.ReadAsStringAsync();

                    var tempDoc = new HtmlDocument();

                    tempDoc.LoadHtml(html);
                    var titles = tempDoc.DocumentNode.SelectNodes("//h1[contains(., 'flag')]");
                    if (titles != null)
                    {
                        foreach (var node in titles)
                        {
                            flag = true;
                            Console.WriteLine(node.InnerText);
                            Console.WriteLine($"{url}{resource}");
                        }
                    }
                    var restLinks = tempDoc.DocumentNode.SelectNodes("//a");

                    foreach (var link in restLinks)
                    {
                        if (!externalResources.Contains(link.GetAttributeValue("href", "")))
                        {
                            externalResources.Add(link.GetAttributeValue("href", ""));
                        }
                    }
                }
            }
        }


        return "";

    }



}