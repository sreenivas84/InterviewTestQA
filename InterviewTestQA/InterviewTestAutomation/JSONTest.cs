using Newtonsoft.Json;

namespace InterviewTestQA
{
    public class JSONTest
    {
        [Fact]
        public void Test()
        {
            string Json_FilePath = File.ReadAllText(ProjectDirectory() + @"\InterviewTestAutomation\Data\Cost Analysis.json");

            List<CostAnalysisItem> Cost_Items = JsonConvert.DeserializeObject<List<CostAnalysisItem>>(Json_FilePath);

            Assert.Equal(53, Cost_Items.Count);

            var top_Item = Cost_Items.OrderByDescending(item => item.Cost).FirstOrDefault();
            Assert.Equal("0", top_Item.CountryId);

            var total_Cost_2016 = Cost_Items.Where(item => item.YearId == 2016).Sum(item => item.Cost);
            Assert.Equal((decimal)77911.3744561, total_Cost_2016);
        }

        public class CostAnalysisItem
        {
            [JsonProperty("CountryId")]
            public string CountryId { get; set; }

            [JsonProperty("YearId")]
            public int YearId { get; set; }

            [JsonProperty("Cost")]
            public decimal Cost { get; set; }
        }

        static string ProjectDirectory()
        {
            string proj_loc = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string proj_Dir = Path.GetDirectoryName(proj_loc);
            while (!Directory.GetFiles(proj_Dir, "*.csproj").Any() && !Directory.GetFiles(proj_Dir, "*.sln").Any())
            {
                proj_Dir = Directory.GetParent(proj_Dir).FullName;
            }
            return proj_Dir;
        }
    }
}