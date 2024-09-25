
using System.Web;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.XPath;

var config = Configuration.Default.WithDefaultLoader();
var document = await BrowsingContext.New(config).OpenAsync("https://tw.stock.yahoo.com/class-quote?sectorId=26&exchange=TAI");

//使用 css selector 取得所有 etf 股票的 html node
var nodes = //document.QuerySelectorAll("li[class='List(n)']");
    document.All.Where(a => a.LocalName == "li" && a.ClassName== "List(n)");
    //document.QuerySelectorAll("li.List(n)");//這會造成錯誤

    Console.WriteLine(nodes.Count());
//
//將 html node 轉換成 Stock 物件
var stocks=nodes.Select(a => new Stock
{
    //以下使用 xpath 取得股票資訊
    Name = HttpUtility.HtmlDecode(a.SelectSingleNode("./div/div[1]/div[2]/div/div[1]").TextContent.Trim()),//將 html entity 轉換成字串
    Symbol = a.SelectSingleNode("./div/div[1]/div[2]/div/div[2]").TextContent.Trim(),
    Price =  a.SelectSingleNode("./div/div[2]").TextContent.Trim(),
    PriceChange = a.SelectSingleNode("./div/div[3]").TextContent.Trim(),
    Change= a.SelectSingleNode("./div/div[4]").TextContent.Trim(),
    Open = a.SelectSingleNode("./div/div[5]").TextContent.Trim(),
    LastClose = a.SelectSingleNode("./div/div[6]").TextContent.Trim(),
    High = a.SelectSingleNode("./div/div[7]").TextContent.Trim(),
    Low = a.SelectSingleNode("./div/div[8]").TextContent.Trim(),
    Turnover = a.SelectSingleNode("./div/div[9]").TextContent.Trim(),
    UpDown = UpDownCheck(((IElement)a.SelectSingleNode("./div/div[3]/span")).ClassName)
        //UpDownCheck(((IElement)a.SelectSingleNode("./div/div[3]/span")).GetAttribute("class")) //處理上漲或下跌的顯示
});

foreach(var stock in stocks) 
{
    Console.WriteLine($"股票名稱: {stock.Name.PadRight(12)}\t 股票代號: {stock.Symbol}\t 股價: {stock.Price.PadRight(5)}\t 漲跌: {stock.UpDown} {stock.PriceChange.PadRight(8)}\t 漲跌幅: {stock.UpDown} {stock.Change.PadRight(8)}\t 開盤: {stock.Open}\t 昨收: {stock.LastClose}\t 最高: {stock.High}\t 最低: {stock.Low}\t 成交量(張): {stock.Turnover}");
} 

string UpDownCheck(string value)
{
    if (value.Contains("up"))
    {
        return "上漲";
    }
    if (value.Contains("down"))
    {
        return "下跌";
    }
    return string.Empty;
}

class Stock
{
    public string Name { get; set; }
    public string Symbol { get; set; }
    public string Price { get; set; }
    public string Change { get; set; }
    public string PriceChange { get; set; }
    public string Open { get; set; }
    public string LastClose { get; set; }
    public string High { get; set; }
    public string Low { get; set; }
    public string Turnover { get; set; }
    public string UpDown { get; set; }
} 