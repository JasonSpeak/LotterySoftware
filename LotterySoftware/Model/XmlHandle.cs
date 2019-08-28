using System.Collections.Generic;
using System.Xml;

namespace LotterySoftware.Model
{
    public class XmlHandle
    {
        public static List<Awards> GetAwards()
        {
            var xmlDoc = new XmlDocument();
            var awardsList = new List<Awards>();
            xmlDoc.Load("config.xml");
            var nameList = xmlDoc.GetElementsByTagName("Name");
            var priceList = xmlDoc.GetElementsByTagName("Prize");
            var numberList = xmlDoc.GetElementsByTagName("Number");
            for (var i = 0; i < numberList.Count; i++)
            {
                var award = new Awards
                {
                    AwardsName = nameList[i].InnerText,
                    AwardsPrize = priceList[i].InnerText,
                    IsLastAward = false
                };
                if (int.TryParse(numberList[i].InnerText, out var number))
                {
                    award.AwardsNumber = number;
                }
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