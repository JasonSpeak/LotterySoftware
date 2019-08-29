using System.Collections.Generic;
using System.Xml;

namespace LotterySoftware.Model
{
    public class XmlHandle
    {
        public static List<Awards> GetAwards()
        {
            var xmlDocument = new XmlDocument();
            var awardsList = new List<Awards>();
            xmlDocument.Load("config.xml");
            var nameList = xmlDocument.GetElementsByTagName("Name");
            var priceList = xmlDocument.GetElementsByTagName("Prize");
            var numberList = xmlDocument.GetElementsByTagName("Number");
            for (var i = 0; i < numberList.Count; i++)
            {
                var award = new Awards
                {
                    AwardsName = nameList[i].InnerText,
                    AwardsPrize = priceList[i].InnerText,
                    IsLastAward = false,
                    AwardsNumber = int.TryParse(numberList[i].InnerText, out var number) ? number : 0
                };
                if (i == numberList.Count - 1)
                {
                    award.IsLastAward = true;
                }
                awardsList.Add(award);
            }
            return awardsList;
        }
    }
}