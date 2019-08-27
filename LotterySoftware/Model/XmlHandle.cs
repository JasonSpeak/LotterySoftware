using System.Collections.Generic;
using System.Xml;

namespace LotterySoftware.Model
{
    public class XmlHandle
    {
        public static List<Awards> InitializationXml()
        {
            var xmlDoc = new XmlDocument();
            var awardsList = new List<Awards>();
            xmlDoc.Load("config.xml");
            var nameList = xmlDoc.GetElementsByTagName("Name");
            var priceList = xmlDoc.GetElementsByTagName("Prize");
            var numberList = xmlDoc.GetElementsByTagName("Number");
            const string point = " ··· ";
            for (var i = 0; i < numberList.Count; i++)
            {
                if (nameList[i].InnerText == "") continue;
                var award = new Awards
                {
                    AwardsPrize = priceList[i].InnerText,
                    AwardsNumber = int.Parse(numberList[i].InnerText),
                };
                if (i < nameList.Count - 1)
                {
                    award.AwardsName = nameList[i].InnerText + point;
                }
                else
                {
                    award.AwardsName = nameList[i].InnerText;
                }
                awardsList.Add(award);
            }
            return awardsList;
        }
    }
}