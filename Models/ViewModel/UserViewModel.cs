namespace task5.Models.ViewModel
{
    public class UserViewModel
    {
        public List<Region> regions {  get; set; }
        public List<User> users { get; set; }
        public string selectedRegion { get; set; } 
        public int seed { get; set; }
        public double errorCount { get; set; }
        public double errorCountVal { get; set; }
        public int page { get; set; } = 10;


        public UserViewModel() {
            var Addregions = new List<Region>
        {
            new Region { Name = "USA", Language = "english" },
            new Region { Name = "Polska", Language = "polski" },
            new Region { Name = "Россия", Language = "русский" }
        };
            regions = Addregions;
        }
    }
}
